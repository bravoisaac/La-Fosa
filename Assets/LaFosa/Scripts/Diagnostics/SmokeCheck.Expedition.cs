using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        IEnumerator CaptureGuide(string name)
        {
            // Requires a rendered game window. Hidden/headless runs may output black images;
            // these files are not evidence of visual validation unless inspected.
            yield return null;
            ScreenCapture.CaptureScreenshot(Path.Combine(output, name));
            yield return new WaitForSecondsRealtime(.25f);
        }

        IEnumerator TestFirstExpedition(GameSession session)
        {
            var initial = session.CaptureSave();
            var diver = session.Diver;
            var inventory = diver.Inventory;
            var economy = session.Economy;
            var trident = session.Config.shop.offers.First(o => o.gear == GearKind.Trident);
            var zone = session.Config.zones[0];
            var chest = session.GetComponentsInChildren<TreasureChest>().First(c => c.puzzle.zone == zone);
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Fishing, "Guía inicial orienta a la pesca, no al santuario");
            Check(session.ExpeditionObjective().Destination == session.FishingWaypoint, "Inicio señala la zona costera");
            yield return CaptureGuide("guia-01-inicio.png");
            Teleport(diver, session.FishingWaypoint - Vector3.up * 1.55f);
            var coastal = session.GetComponentsInChildren<FishAgent>().Where(f => f.species.habitatBand == 0 && f.Available).ToArray();
            Check(coastal.Any(f => f.transform.position == session.ExpeditionObjective().Destination), "Bajo el agua el marcador busca un pez costero disponible");
            diver.LookAt(session.ExpeditionObjective().Destination);
            yield return CaptureGuide("guia-02-pesca.png");
            Check(coastal.Min(f => f.item.salePrice) > 0 && coastal.Any(f => f.item.salePrice < trident.price), "Orilla ofrece capturas con valor para el primer equipo");
            var cheapFish = coastal.OrderBy(f => f.item.salePrice).First().item;
            inventory.TryAdd(cheapFish);
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Fishing, "Una captura insuficiente mantiene el objetivo de reunir pesca");
            int oldPrice = trident.price;
            trident.price += 17;
            Check(session.TridentPrice == oldPrice + 17 && session.ExpeditionObjective().Hint.Contains((trident.price - economy.SaleValue).ToString()), "Guía usa precio editable y valor real, sin prometer dos peces");
            trident.price = oldPrice;
            while (economy.SaleValue < trident.price) Check(inventory.TryAdd(cheapFish), "Cabe la pesca necesaria en una sola salida");
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Sell && session.ExpeditionObjective().Destination == GameSession.CabinWaypoint, "Pesca suficiente guía a vender en la cabaña");
            Check(session.ReturnToMenu() && session.ContinueGame() && session.ExpeditionObjective().Step == ExpeditionStep.Sell, "Continuar conserva el objetivo de vender la pesca");
            Teleport(diver, new Vector3(-12, 3.2f, -16.5f)); diver.LookAt(GameSession.CabinWaypoint);
            yield return CaptureGuide("guia-03-venta.png");
            session.OpenShop();
            Check(economy.SellFish() && session.ExpeditionObjective().Step == ExpeditionStep.BuyTrident, "Vender permite comprar el tridente");
            Check(economy.Buy(trident) && diver.GetComponent<HarpoonGun>().SelectedTool == 1, "Compra equipa el tridente de la primera expedición");
            session.CloseShop();
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Puzzle, "Tras comprar, la guía presenta el primer mecanismo");
            Check(session.ReturnToMenu() && session.ContinueGame() && session.ExpeditionObjective().Step == ExpeditionStep.Puzzle, "Cargar compra no repite pesca ni venta");
            Teleport(diver, zone.stationPosition + new Vector3(0, 1.1f, -5.8f));
            diver.LookAt(zone.stationPosition + new Vector3(0, 2.5f, .7f));
            var puzzle = chest.puzzle;
            for (int i = 0; i < puzzle.Definition.Count; i++)
                while (puzzle.Turns(i) != 0) puzzle.Rotate(i);
            Check(puzzle.Ready && session.ExpeditionObjective().Step == ExpeditionStep.ActivateSeal, "Circuito listo solicita accionar la palanca");
            Check(puzzle.Activate() && session.ExpeditionObjective().Step == ExpeditionStep.ClaimTreasure, "Sello abierto solicita recoger el cofre");
            int oldCapacity = inventory.capacity;
            inventory.TryAdd(session.Config.shell); inventory.capacity = inventory.Stacks.Count;
            Check(session.ExpeditionObjective().Step == ExpeditionStep.FreeSpace, "Mochila llena explica cómo dejar espacio al tesoro");
            chest.Interact(diver);
            Check(!chest.Claimed && puzzle.Solved, "Sin espacio conserva el sello y la recompensa");
            inventory.capacity = oldCapacity;
            chest.Interact(diver);
            Check(chest.Claimed && session.ExpeditionObjective().Step == ExpeditionStep.Return, "Primer tesoro orienta a regresar");
            yield return CaptureGuide("guia-04-regreso.png");
            Check(session.IsZoneUnlocked(session.Config.zones[1]) && session.GetComponentsInChildren<DiveGate>().All(g => !g.IsOpen), "Primera recompensa prepara el naufragio sin abrir la compuerta del abismo");
            Teleport(diver, session.Config.zones[1].entryPosition + Vector3.down * 4);
            Check(session.ExpeditionObjective().Destination == new Vector3(diver.transform.position.x, session.Config.waterLevel + .5f, diver.transform.position.z), "Regreso desde naufragio apunta a superficie sin escotilla inexistente");
            Teleport(diver, new Vector3(0, -80, 75));
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Surface, "Presión peligrosa tiene prioridad sobre el regreso");
            Teleport(diver, session.Config.spawn);
            yield return null;
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Descend, "Llegar a la playa concluye el regreso y propone otra salida");
            Check(session.ReturnToMenu() && session.ContinueGame() && session.ExpeditionObjective().Step == ExpeditionStep.Descend && session.Treasures == 1, "Continuar la primera expedición conserva mejora y siguiente destino");
            // Also exercise a player who solved the puzzle without following the shopping guide.
            session.ApplySave(initial);
            for (int i = 0; i < puzzle.Definition.Count; i++) while (puzzle.Turns(i) != 0) puzzle.Rotate(i);
            puzzle.Activate();
            Check(session.ExpeditionObjective().Step == ExpeditionStep.ClaimTreasure, "Exploración libre no obliga a comprar para recoger el tesoro");
            session.ApplySave(initial);
            inventory.Restore(new[] { new InventoryStack(session.Config.shell, 1) });
            inventory.capacity = 1;
            Check(session.ExpeditionObjective().Step == ExpeditionStep.FreeSpace, "Mochila llena de objetos no vendibles ofrece recuperación");
            inventory.capacity = oldCapacity;
            session.ApplySave(initial); session.SaveNow(); session.Resume();
            Check(session.ExpeditionObjective().Step == ExpeditionStep.Fishing, "Partida inicial restaura también la guía sin banderas huérfanas");
        }

        void TestDepthOxygen(GameSession session)
        {
            var saved = session.CaptureSave();
            var diver = session.Diver;
            diver.Rescue("Preparar prueba de aire");
            Teleport(diver, new Vector3(0, -6, 75));
            float oxygen = diver.Oxygen;
            diver.AdvanceVitals(2, false);
            Check(Mathf.Approximately(oxygen - diver.Oxygen, 2), "Profundidad segura consume un segundo de aire por segundo");
            Teleport(diver, new Vector3(0, -40, 75));
            oxygen = diver.Oxygen; float health = diver.Health;
            float multiplier = session.Config.pressureOxygenMultiplier;
            Check(Mathf.Approximately(multiplier, 8) && diver.PressureOxygenMultiplier == multiplier, "Profundidad excesiva activa consumo ×8");
            diver.AdvanceVitals(2, false);
            Check(Mathf.Approximately(oxygen - diver.Oxygen, 2 * multiplier) && diver.Health == health, "Presión gasta aire sin daño directo mientras quede oxígeno");
            oxygen = diver.Oxygen;
            diver.AdvanceVitals(1, true);
            Check(Mathf.Approximately(oxygen - diver.Oxygen, 1.5f * multiplier), "Acelerar combina el coste de nado y profundidad");
            session.ToggleInventory(); oxygen = diver.Oxygen;
            diver.AdvanceVitals(2, false);
            Check(diver.Oxygen == oxygen && diver.Health == health, "Pausa detiene el consumo peligroso y la salud");
            session.ToggleInventory();
            Teleport(diver, new Vector3(0, -6, 75));
            oxygen = diver.Oxygen; diver.AdvanceVitals(1, false);
            Check(diver.PressureOxygenMultiplier == 1 && Mathf.Approximately(oxygen - diver.Oxygen, 1), "Ascender al límite seguro recupera consumo normal");
            Teleport(diver, session.Config.spawn);
            oxygen = diver.Oxygen; diver.AdvanceVitals(.5f, false);
            Check(Mathf.Approximately(diver.Oxygen - oxygen, 11), "Superficie repone aire tras exposición profunda");
            Teleport(diver, new Vector3(0, -40, 75));
            while (diver.Oxygen > 0) diver.AdvanceVitals(.1f, false);
            health = diver.Health; diver.AdvanceVitals(1, false);
            Check(diver.Oxygen == 0 && Mathf.Approximately(health - diver.Health, 16), "Aire agotado causa el daño por asfixia existente");
            session.Diver.Inventory.TryAdd(session.Config.shell);
            int shells = diver.Inventory.Count(session.Config.shell);
            diver.AdvanceVitals(10, false);
            Check(Vector3.Distance(diver.transform.position, session.Config.spawn) < .1f && diver.Health == 100 && diver.Oxygen == diver.MaxOxygen, "Agotamiento rescata y restaura salud y aire");
            Check(diver.Inventory.Count(session.Config.shell) == shells, "Rescate por asfixia conserva hallazgos");
            session.ApplySave(saved); session.SaveNow(); session.Resume();
        }
    }
}
