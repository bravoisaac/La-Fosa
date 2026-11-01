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
                ReportExpeditionReturn();
                firstReturnPending = false;
                Notify("Primera expedición recuperada. Tu equipo permite bajar más profundo; prepara otra salida en la cabaña.");
            }
        }

        Vector3 AscentWaypoint()
        {
            var position = Diver.transform.position;
            if (Diver.Depth > DeepTrenchProfile.EntranceDepth)
            {
                var axis = DeepTrenchProfile.Axis;
                if (Vector2.Distance(new Vector2(position.x, position.z), axis) > 2)
                    return new Vector3(axis.x, position.y, axis.y);
                return new Vector3(axis.x, Config.waterLevel + .5f, axis.y);
            }
            return new Vector3(position.x, Config.waterLevel + .5f, position.z);
        }

        public ExpeditionObjective ExpeditionObjective()
        {
            var position = Diver.transform.position;
            // Safety takes precedence without changing the underlying objective or progress.
            if (Diver.NeedsAirReturn)
            {
                string hint = Diver.Depth > Diver.SafeDepth
                    ? "Superaste el límite: gastas aire ×" + Diver.PressureOxygenMultiplier.ToString("0.#") + ". Busca el conducto central y asciende con ESPACIO. R te rescata."
                    : "Reserva de regreso: vuelve al conducto central y asciende con ESPACIO. R te rescata conservando tus hallazgos.";
                return new ExpeditionObjective(ExpeditionStep.Surface, "Primero, recupera aire", hint, "RUTA DE ASCENSO", AscentWaypoint());
            }
            if (firstReturnPending)
                return new ExpeditionObjective(ExpeditionStep.Return, "Vuelve con el hallazgo", "El tesoro amplió tu aire y profundidad segura. Sube a respirar y regresa a la cabaña antes de bajar al naufragio.", "REGRESO A LA CALETA", Diver.Underwater ? AscentWaypoint() : CabinWaypoint);
            if (Complete)
                return new ExpeditionObjective(ExpeditionStep.Complete, "Explora la fosa profunda", "Tres tesoros recuperados. Bajo el último santuario, la fosa continúa hasta 650 m. Cinco umbrales aumentan la presión y el gasto de aire. Reserva aire para volver.", "FOSA PROFUNDA", new Vector3(0, -92, 76));

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
                "Descenso libre: tu aire limita la salida. Mejora el equipo y reserva aire para volver. El arpón (tecla 3) alcanza 40 m.", label, destination);
        }
    }
}
