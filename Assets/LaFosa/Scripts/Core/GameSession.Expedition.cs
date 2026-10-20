using UnityEngine;

namespace LaFosa
{
    public enum ExpeditionStep { Fishing, Sell, BuyTrident, Puzzle, ActivateSeal, ClaimTreasure, Return, Descend, Complete, Surface, FreeSpace }

    public readonly struct ExpeditionObjective
    {
        public readonly ExpeditionStep Step;
        public readonly string Title, Hint, Marker;
        public readonly Vector3 Destination;
        public ExpeditionObjective(ExpeditionStep step, string title, string hint, string marker, Vector3 destination)
        { Step = step; Title = title; Hint = hint; Marker = marker; Destination = destination; }
    }

    public sealed partial class GameSession
    {
        public static readonly Vector3 CabinWaypoint = new Vector3(-12, 4.3f, -18.8f);
        public Vector3 FishingWaypoint => new Vector3(0, Config.waterLevel - 1.8f, 14);
        bool firstReturnPending;
        FishAgent[] guideFish;

        public int TridentPrice
        {
            get
            {
                if (Config.shop != null && Config.shop.offers != null)
                    foreach (var offer in Config.shop.offers)
                        if (offer != null && offer.gear == GearKind.Trident) return offer.price;
                return 0;
            }
        }

        void UpdateExpeditionReturn()
        {
            if (firstReturnPending && !Diver.Underwater && Vector3.Distance(Diver.transform.position, Config.spawn) < 12)
            {
                firstReturnPending = false;
                Notify("Primera expedición recuperada. Tu equipo permite bajar más profundo; prepara otra salida en la cabaña.");
            }
        }

        Vector3 AscentWaypoint()
        {
            var position = Diver.transform.position;
            Vector3 ascent = new Vector3(position.x, Config.waterLevel + .5f, position.z);
            // Keep the target above the nearest overhead hatch until the whole diver clears it.
            foreach (var zone in Config.zones)
                if (zone != Config.zones[0] && zone.hasPhysicalGate && position.y < zone.entryPosition.y + 2 && IsZoneUnlocked(zone))
                    ascent = zone.entryPosition + Vector3.up * 2.5f;
            return ascent;
        }

        public ExpeditionObjective ExpeditionObjective()
        {
            var position = Diver.transform.position;
            // Safety takes precedence without changing the underlying objective or progress.
            if (Diver.Underwater && (Diver.Oxygen < 30 || Diver.Depth > Diver.SafeDepth))
            {
                string hint = Diver.Depth > Diver.SafeDepth
                    ? "Superaste el límite: gastas aire ×" + Diver.PressureOxygenMultiplier.ToString("0.#") + ". ESPACIO para ascender. R te rescata conservando tus hallazgos."
                    : "Queda poco aire. ESPACIO para ascender y respirar. R te rescata conservando tus hallazgos.";
                return new ExpeditionObjective(ExpeditionStep.Surface, "Primero, recupera aire", hint, "RUTA DE ASCENSO", AscentWaypoint());
            }
            if (firstReturnPending)
                return new ExpeditionObjective(ExpeditionStep.Return, "Vuelve con el hallazgo", "El tesoro amplió tu aire y profundidad segura. Sube a respirar y regresa a la cabaña antes de bajar al naufragio.", "REGRESO A LA CALETA", Diver.Underwater ? AscentWaypoint() : CabinWaypoint);
            if (Complete)
                return new ExpeditionObjective(ExpeditionStep.Complete, "Los secretos del abismo", "Expedición completada. Puedes seguir pescando y preparando equipo en la cabaña.", "CABAÑA", CabinWaypoint);

            // A player who found the treasure freely must never be sent back through the tutorial.
            if (Treasures == 0)
            {
                var first = Config.zones[0];
                StoneCircuitPuzzle puzzle = null;
                if (puzzles != null) foreach (var candidate in puzzles) if (candidate.zone == first) { puzzle = candidate; break; }
                if (puzzle != null && puzzle.Solved)
                {
                    if (Diver.Inventory.Stacks.Count >= Diver.Inventory.capacity && Diver.Inventory.Count(first.treasure) == 0)
                        return new ExpeditionObjective(ExpeditionStep.FreeSpace, "Haz espacio al tesoro", "TAB abre la mochila. Vende pesca o descarta una pila. El cofre espera y el sello permanece abierto.", "COFRE DEL SANTUARIO", first.stationPosition + Vector3.up * 3);
                    return new ExpeditionObjective(ExpeditionStep.ClaimTreasure, "Recoge tu primera mejora", "Apunta al cofre y pulsa E. Recupera aire y salud y mejora tu equipo para soportar la profundidad del naufragio.", "COFRE DEL SANTUARIO", first.stationPosition + Vector3.up * 3);
                }
                if (!Economy.Has(GearKind.Trident) && TridentPrice > 0)
                {
                    if (Economy.Money >= TridentPrice)
                        return new ExpeditionObjective(ExpeditionStep.BuyTrident, "Compra el tridente", "Ya tienes $" + TridentPrice + ". E en el mostrador. Tecla 2: alcanza peces a 4 m; las manos llegan a 1,6 m.", "CABAÑA DE DON LUCHO", CabinWaypoint);
                    long total = Economy.Money + Economy.SaleValue;
                    if (Economy.FishCount > 0 && (total >= TridentPrice || Diver.Inventory.Stacks.Count >= Diver.Inventory.capacity))
                        return new ExpeditionObjective(ExpeditionStep.Sell, "Vende tu primera pesca", "Llevas $" + Economy.SaleValue + " en peces. E en el mostrador y VENDER. Tridente: $" + TridentPrice + ".", "CABAÑA DE DON LUCHO", CabinWaypoint);
                    if (Diver.Inventory.Stacks.Count >= Diver.Inventory.capacity)
                        return new ExpeditionObjective(ExpeditionStep.FreeSpace, "Haz espacio para pescar", "TAB abre la mochila. Descarta una pila si solo llevas conchas o hallazgos; los peces se venden en la cabaña.", "PESCA DE ORILLA", FishingWaypoint);
                    Vector3 target = FishingWaypoint;
                    if (Diver.Underwater && Vector3.Distance(position, FishingWaypoint) < 22 && guideFish != null)
                    {
                        float closest = float.MaxValue;
                        foreach (var fish in guideFish)
                            if (fish != null && fish.Available && fish.species != null && fish.species.habitatBand == 0)
                            {
                                float distance = (fish.transform.position - position).sqrMagnitude;
                                if (distance < closest) { closest = distance; target = fish.transform.position; }
                            }
                    }
                    string hint = Diver.Underwater
                        ? "Apunta y haz CLIC a menos de 1,6 m. ESPACIO sube a respirar; C baja. Faltan $" + (TridentPrice - total) + " en pesca."
                        : "Ratón para mirar, WASD hasta la orilla. C baja; ESPACIO sube a respirar. Reúne $" + (TridentPrice - total) + " en peces lentos.";
                    return new ExpeditionObjective(ExpeditionStep.Fishing, "Tu primera pesca", hint, "PECES DE ORILLA", target);
                }
                bool ready = puzzle != null && puzzle.Ready;
                return new ExpeditionObjective(ready ? ExpeditionStep.ActivateSeal : ExpeditionStep.Puzzle,
                    ready ? "Abre el primer sello" : "Explora el primer santuario",
                    ready ? "Los canales están conectados. Apunta a ABRIR SELLO y pulsa E; después recoge el cofre."
                          : "Tridente: tecla 2, alcance 4 m. Sigue las boyas. E gira piedras; conecta ENTRADA con SELLO sin extremos abiertos.",
                    "SANTUARIO DE HUIROS", first.stationPosition + Vector3.up * 3);
            }
            var destination = NextWaypoint(out var label);
            return new ExpeditionObjective(ExpeditionStep.Descend, "Prepara el siguiente descenso",
                "Equipo mejorado. El arpón (tecla 3) alcanza 40 m para peces rápidos. Compra aire si lo necesitas y sigue el marcador.", label, destination);
        }
    }
}
