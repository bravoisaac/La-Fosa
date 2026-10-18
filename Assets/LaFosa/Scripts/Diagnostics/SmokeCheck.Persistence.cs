using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        void TestPersistence(GameSession session)
        {
            var diver = session.Diver;
            var saved = session.CaptureSave();
            Check(session.ValidateSave(saved) && session.SaveNow(), "Guardar una expedición completa coherente");
            Check(session.Store.TryLoad(out var disk, out _) && JsonUtility.ToJson(saved) == JsonUtility.ToJson(disk), "JSON restaura dinero, inventario, equipo, tesoros, puzles, descubrimientos y vehículos");
            Check(session.ReturnToMenu() && session.Intro && session.CanContinue && Time.timeScale == 0, "Volver al menú guarda y pausa");
            string original = File.ReadAllText(session.Store.FilePath);
            Check(!session.NewGame() && File.ReadAllText(session.Store.FilePath) == original, "Nueva partida requiere confirmación y no sobrescribe al solicitarla");
            diver.Inventory.Restore(new InventoryStack[0]);
            Check(session.ContinueGame() && !session.Intro && JsonUtility.ToJson(session.CaptureSave()) == JsonUtility.ToJson(saved), "Continuar restaura el estado completo sin acumular compras ni mejoras");
            Check(Vector3.Distance(diver.transform.position, session.Config.spawn) < .01f && diver.Health == 100 && diver.Oxygen == diver.MaxOxygen, "Continuar sitúa al buzo a salvo en la playa");
            foreach (var chest in session.GetComponentsInChildren<TreasureChest>()) chest.Interact(diver);
            Check(session.Treasures == saved.treasures.Length && JsonUtility.ToJson(session.CaptureSave()) == JsonUtility.ToJson(saved), "Cofres cargados no duplican recompensas");
            session.OpenShop(); int balance = session.Economy.Money;
            Check(session.Config.shop.offers.All(o => !session.Economy.Buy(o)) && session.Economy.Money == balance, "Equipo cargado no se vuelve a comprar");
            session.CloseShop();
            Check(session.GetComponentsInChildren<Pickup>(true).Where(p => saved.collectedPickups.Contains(p.id)).All(p => !p.gameObject.activeSelf), "Conchas recogidas no reaparecen al cargar");

            var testStore = new SaveStore(Path.Combine(output, "PersistenceCases", "save.json"), session.ValidateSave);
            Check(testStore.TrySave(saved, out _, true), "Crear archivo de prueba y respaldo");
            var changed = Clone(saved); changed.money++;
            Check(testStore.TrySave(changed, out _) && testStore.TryLoad(out var latest, out _) && latest.money == changed.money, "El principal usa la última transacción");
            File.WriteAllText(testStore.FilePath, "{ archivo truncado");
            Check(testStore.TryLoad(out var fallback, out var recovery) && fallback.money == saved.money && recovery.Contains("respaldo"), "Archivo corrupto recupera el respaldo anterior");
            string backup = File.ReadAllText(testStore.BackupPath);
            Check(testStore.TrySave(changed, out _) && File.ReadAllText(testStore.BackupPath) == backup, "Guardar tras recuperar no copia el principal corrupto al respaldo");
            File.Delete(testStore.FilePath);
            Check(testStore.TryLoad(out fallback, out _) && fallback.money == saved.money, "Principal ausente recupera respaldo");
            File.WriteAllText(testStore.BackupPath, "{}");
            Check(!testStore.TryLoad(out _, out var invalidMessage) && invalidMessage.Contains("dañado"), "Ambos archivos inválidos se rechazan con explicación");
            File.Delete(testStore.BackupPath);
            Check(!testStore.TryLoad(out _, out var absentMessage) && absentMessage.Contains("Sin partida"), "Archivo ausente se distingue de archivo dañado");
            Check(testStore.TrySave(saved, out _, true), "Nueva partida puede reemplazar archivos inválidos");

            var invalid = Clone(saved); invalid.version = 999;
            Check(!session.ValidateSave(invalid) && !testStore.TrySave(invalid, out _), "Versión futura no se carga ni se guarda");
            invalid = Clone(saved); invalid.inventory[0].id = "desconocido";
            Check(!session.ValidateSave(invalid), "Objeto desconocido rechazado antes de modificar el inventario");
            invalid = Clone(saved); invalid.inventory[0].count = -1;
            Check(!session.ValidateSave(invalid), "Cantidad negativa rechazada");
            invalid = Clone(saved); invalid.gear = new[] { nameof(GearKind.Net) };
            Check(!session.ValidateSave(invalid), "Equipo incoherente sin tridente rechazado");
            invalid = Clone(saved); invalid.puzzles[0].turns[0] = 4;
            Check(!session.ValidateSave(invalid), "Giro fuera de rango rechazado");
            invalid = Clone(saved); invalid.puzzles[0].turns[0] = (invalid.puzzles[0].turns[0] + 1) % 4;
            Check(!session.ValidateSave(invalid), "Sello abierto con circuito roto rechazado");
            invalid = Clone(saved); invalid.treasures = new[] { session.Config.zones[1].id };
            Check(!session.ValidateSave(invalid), "Progreso que salta el primer tesoro rechazado");
            invalid = Clone(saved); invalid.vehicles[0].position.x = float.NaN;
            Check(!session.ValidateSave(invalid), "Posición no finita rechazada");
            invalid = Clone(saved); invalid.puzzles[1].id = invalid.puzzles[0].id;
            Check(!session.ValidateSave(invalid), "IDs de mecanismos duplicados rechazados");
            invalid = Clone(saved); invalid.discoveredZones = null;
            Check(!session.ValidateSave(invalid), "Campos obligatorios ausentes rechazados");
            string blocker = Path.Combine(output, "PersistenceCases", "not-a-directory"); File.WriteAllText(blocker, "test");
            var blockedStore = new SaveStore(Path.Combine(blocker, "save.json"), session.ValidateSave);
            Check(!blockedStore.TrySave(saved, out var failure) && failure.Contains("No se pudo guardar"), "Fallo de escritura devuelve un aviso recuperable");

            Check(session.ReturnToMenu() && session.NewGame(true), "Nueva partida confirmada reemplaza la expedición");
            Check(session.Treasures == 0 && session.Economy.Money == 0 && diver.Inventory.Stacks.Count == 0 &&
                diver.MaxOxygen == session.Config.initialOxygen && diver.SafeDepth == session.Config.initialSafeDepth &&
                !session.Boat.gameObject.activeSelf && !session.JetSki.gameObject.activeSelf && diver.GetComponent<HarpoonGun>().SelectedTool == 0,
                "Nueva partida reinicia dinero, mochila, equipo, tesoros y vehículos");
            Check(session.GetComponentsInChildren<Pickup>(true).All(p => p.gameObject.activeSelf) &&
                session.GetComponentsInChildren<TreasureChest>().All(c => !c.Claimed && !c.puzzle.Solved), "Nueva partida repone conchas, cofres y mecanismos");
            var firstPuzzle = session.GetComponentsInChildren<StoneCircuitPuzzle>().First(p => p.zone == session.Config.zones[0]);
            firstPuzzle.Rotate(0); int turn = firstPuzzle.Turns(0);
            Check(session.ReturnToMenu() && session.ContinueGame() && firstPuzzle.Turns(0) == turn && firstPuzzle.Moves == 1 && !firstPuzzle.Solved, "Un tablero parcialmente girado sobrevive a la carga");
            Check(session.ReturnToMenu() && session.NewGame(true), "Segundo reinicio confirmado");
            File.WriteAllText(session.Store.FilePath, "corrupto");
            Check(session.Store.TryLoad(out var freshBackup, out _) && freshBackup.treasures.Length == 0 && freshBackup.money == 0, "Respaldo de nueva partida no resucita la expedición anterior");
            session.ApplySave(saved); session.SaveNow(); session.Resume();
        }
        static SaveData Clone(SaveData data) => JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(data));

        // Run write and read in separate player processes to exercise real shutdown/restart.
        void TestPersistenceRestart(GameSession session)
        {
            bool write = Array.IndexOf(Environment.GetCommandLineArgs(), "--save-restart-write") >= 0;
            if (write)
            {
                Check(session.NewGame(true), "Nueva partida aislada para reinicio real");
                var diver = session.Diver;
                diver.Inventory.TryAdd(session.Config.fish, 30); session.OpenShop();
                Check(session.Economy.SellFish(), "Preparar saldo persistente");
                foreach (var kind in new[] { GearKind.Trident, GearKind.OxygenTank, GearKind.Boat })
                    Check(session.Economy.Buy(session.Config.shop.offers.First(o => o.gear == kind)), "Compra antes de cerrar: " + kind);
                session.CloseShop();
                var puzzle = session.GetComponentsInChildren<StoneCircuitPuzzle>().First(p => p.zone == session.Config.zones[0]);
                for (int i = 0; i < puzzle.Definition.Count; i++) while (puzzle.Turns(i) != 0) puzzle.Rotate(i);
                puzzle.Activate(); session.GetComponentsInChildren<TreasureChest>().First(c => c.puzzle == puzzle).Interact(diver);
                var next = session.GetComponentsInChildren<StoneCircuitPuzzle>().First(p => p.zone == session.Config.zones[1]); next.Rotate(0);
                session.GetComponentsInChildren<Pickup>().First().Interact(diver);
                session.Boat.transform.position = new Vector3(4.8f, session.Config.waterLevel, 23);
                Check(session.SaveNow(), "Guardar antes de cerrar el proceso");
                File.WriteAllText(Path.Combine(output, "restart-expected.json"), JsonUtility.ToJson(session.CaptureSave()));
            }
            else
            {
                Check(session.CanContinue && session.ContinueGame(), "Segundo proceso encuentra y carga la partida");
                string expected = File.ReadAllText(Path.Combine(output, "restart-expected.json"));
                Check(JsonUtility.ToJson(session.CaptureSave()) == expected, "Reinicio real restaura exactamente el progreso");
                var chest = session.GetComponentsInChildren<TreasureChest>().First(c => c.puzzle.zone == session.Config.zones[0]);
                chest.Interact(session.Diver);
                Check(JsonUtility.ToJson(session.CaptureSave()) == expected, "Reinicio real no duplica tesoro");
                Check(session.Diver.SafeDepth == session.Config.zones[0].unlockedDepth && session.Diver.MaxOxygen == session.Config.zones[0].unlockedOxygen + session.Config.shop.oxygenBonus,
                    "Reinicio real reaplica mejoras una sola vez");
            }
            string result = (failed ? "FAILED" : "PASSED") + " · " + checks;
            File.WriteAllText(Path.Combine(output, write ? "restart-write.txt" : "restart-read.txt"), result);
            Debug.Log("LA_FOSA_RESTART_" + (write ? "WRITE" : "READ") + "_" + result);
            Application.Quit(failed ? 2 : 0);
        }
    }
}
