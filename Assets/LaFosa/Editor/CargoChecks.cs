using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class CargoChecks
    {
        public static void Validate(GameSession session, SaveStore store)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var capture = typeof(GameSession).GetMethod("CaptureSave", flags);
            var apply = typeof(GameSession).GetMethod("ApplySave", flags);
            var validate = typeof(GameSession).GetMethod("ValidateSave", flags);
            SaveData Capture() => (SaveData)capture.Invoke(session, null);
            void Apply(SaveData data) => apply.Invoke(session, new object[] { data });
            bool Valid(SaveData data) => (bool)validate.Invoke(session, new object[] { data });
            SaveData Copy(SaveData data) => JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(data));
            var original = Capture(); var diver = session.Diver; var boat = session.Boat;
            var backpack = diver.Inventory; var cargo = session.BoatCargo;
            var fish = session.Config.fish; var mineral = session.Config.minerals[0];
            int checks = 0;
            void Check(bool condition, string message)
            { if (!condition) throw new Exception("LA_FOSA_CARGO_FAILED: " + message); checks++; }
            void BoardAndOpen()
            {
                diver.LeaveVehicle(boat.transform.position + Vector3.up);
                if (!boat.TryBoard(diver) || !session.OpenCargo()) throw new Exception("No se pudo preparar acceso a bodega");
            }
            try
            {
                var fresh = Copy(original);
                fresh.gear = fresh.gear.Where(g => g != nameof(GearKind.Boat)).ToArray();
                fresh.inventory = new SavedStack[0]; fresh.boatCargo = null;
                fresh.vehicles.First(v => v.id == nameof(GearKind.Boat)).moored = false;
                Apply(fresh);
                Check(!session.OpenCargo() && cargo.Stacks.Count == 0, "Sin propiedad no hay acceso; campo ausente inicia vacío");
                fresh.gear = fresh.gear.Concat(new[] { nameof(GearKind.Boat), nameof(GearKind.JetSki) }).Distinct().ToArray();
                Apply(fresh);
                Check(!session.OpenCargo(), "No abrir bodega remotamente desde playa");
                diver.LeaveVehicle(session.JetSki.transform.position + Vector3.up);
                Check(session.JetSki.TryBoard(diver) && !session.OpenCargo(), "Moto no tiene bodega");
                BoardAndOpen();
                Check(session.CargoOpen && session.Blocked && Time.timeScale == 0, "Bodega pausa simulación y conducción");
                session.ToggleInventory();
                Check(!session.InventoryOpen, "Tab no superpone mochila sobre bodega");
                backpack.TryAdd(fish, fish.maxStack + 2); backpack.TryAdd(mineral, 3); backpack.TryAdd(session.Config.shell);
                int expectedFish = backpack.Count(fish);
                bool coherent = true; int notifications = 0;
                Action observe = () =>
                {
                    notifications++;
                    coherent &= backpack.Count(fish) + cargo.Count(fish) == expectedFish && Valid(Capture());
                };
                backpack.Changed += observe; cargo.Changed += observe;
                try { Check(session.TransferCargo(true, fish, fish.maxStack + 1), "Transferir pesca repartida entre pilas"); }
                finally { backpack.Changed -= observe; cargo.Changed -= observe; }
                Check(coherent && notifications == 2 && backpack.Count(fish) == 1 && cargo.Stacks.Count == 2,
                    "Ambos observadores ven estado completo, conservado y válido para guardar");
                Check(session.TransferCargo(true, mineral, 3) && cargo.Count(mineral) == 3 && backpack.Count(mineral) == 0,
                    "Minerales se trasladan completos");
                string before = JsonUtility.ToJson(Capture());
                Check(!session.TransferCargo(true, session.Config.shell, 1) && !session.TransferCargo(true, fish, 0) &&
                    !session.TransferCargo(true, fish, -1) && !session.TransferCargo(true, fish, 2) &&
                    !backpack.TryTransferTo(backpack, fish, 1) && !backpack.TryTransferTo(null, fish, 1) &&
                    before == JsonUtility.ToJson(Capture()), "Rechazar objetos, cantidades y destinos inválidos sin cambios");
                Check(session.TransferCargo(false, fish, 1) && backpack.Count(fish) == 2 && cargo.Count(fish) == fish.maxStack,
                    "Retirar unidad fusiona pila en mochila");
                Check(session.TransferCargo(false, mineral, 3) && cargo.Count(mineral) == 0 && backpack.Count(mineral) == 3,
                    "Retirar pila completa libera espacio");
                session.CloseCargo(); before = JsonUtility.ToJson(Capture());
                Check(!session.TransferCargo(false, fish, 1) && before == JsonUtility.ToJson(Capture()) && !session.Blocked,
                    "Cerrar reanuda y bloquea transferencias ocultas");
                Check(session.SaveNow() && store.TryLoad(out _, out _), "Guardar carga en archivo temporal");
                store.TryLoad(out var loaded, out _);
                Check(loaded.boatCargo.Sum(s => s.count) == fish.maxStack, "Archivo contiene carga exacta");
                Apply(loaded);
                Check(cargo.Count(fish) == fish.maxStack && diver.Riding == null && !session.CargoOpen, "Carga restaura bodega y devuelve buzo a playa");
                Check(session.ReturnToMenu() && session.ContinueGame() && cargo.Count(fish) == fish.maxStack, "Continuar conserva bodega");
                session.OpenShop(); int cargoFish = cargo.Count(fish);
                Check(session.Economy.SellFish() && cargo.Count(fish) == cargoFish, "Comercio vende solo la pesca de mochila");
                session.CloseShop(); Apply(loaded);
                var invalid = Copy(loaded); invalid.boatCargo = new[] { new SavedStack { id = "inexistente", count = 1 } };
                Check(!Valid(invalid), "Rechazar ID desconocido");
                invalid.boatCargo = new SavedStack[] { null }; Check(!Valid(invalid), "Rechazar pila nula");
                invalid.boatCargo = new[] { new SavedStack { id = fish.id, count = 0 } }; Check(!Valid(invalid), "Rechazar cantidad cero");
                invalid.boatCargo[0].count = -1; Check(!Valid(invalid), "Rechazar cantidad negativa");
                invalid.boatCargo[0].count = fish.maxStack + 1; Check(!Valid(invalid), "Rechazar exceso por pila");
                invalid.boatCargo[0] = new SavedStack { id = session.Config.shell.id, count = 1 }; Check(!Valid(invalid), "Rechazar conchas en bodega guardada");
                invalid.boatCargo = Enumerable.Range(0, GameSession.BoatCargoSlots + 1).Select(_ => new SavedStack { id = fish.id, count = 1 }).ToArray();
                Check(!Valid(invalid), "Rechazar más de 24 espacios");
                invalid = Copy(loaded); invalid.gear = invalid.gear.Where(g => g != nameof(GearKind.Boat)).ToArray();
                Check(!Valid(invalid), "Rechazar carga sin propiedad");
                var full = Copy(loaded);
                full.boatCargo = Enumerable.Range(0, GameSession.BoatCargoSlots).Select(_ => new SavedStack { id = fish.id, count = fish.maxStack }).ToArray();
                Apply(full); BoardAndOpen(); before = JsonUtility.ToJson(Capture());
                Check(!session.TransferCargo(true, fish, 1) && before == JsonUtility.ToJson(Capture()), "Bodega llena no pierde pesca");
                session.CloseCargo(); full.boatCargo[0].count--;
                Apply(full); BoardAndOpen(); before = JsonUtility.ToJson(Capture());
                Check(!session.TransferCargo(true, fish, 2) && before == JsonUtility.ToJson(Capture()), "Hueco parcial no acepta una transferencia incompleta");
                Check(session.TransferCargo(true, fish, 1) && cargo.Count(fish) == GameSession.BoatCargoSlots * fish.maxStack,
                    "Con 24 espacios ocupados aún se completa una pila");
                session.CloseCargo();
                full.inventory = Enumerable.Range(0, backpack.capacity).Select(_ => new SavedStack { id = mineral.id, count = mineral.maxStack }).ToArray();
                Apply(full); BoardAndOpen(); before = JsonUtility.ToJson(Capture());
                Check(!session.TransferCargo(false, fish, 1) && before == JsonUtility.ToJson(Capture()), "Mochila llena no pierde ni duplica carga");
                session.CloseCargo(); Apply(loaded); BoardAndOpen();
                diver.Rescue("Prueba de rescate"); before = JsonUtility.ToJson(Capture());
                Check(!session.TransferCargo(false, fish, 1) && before == JsonUtility.ToJson(Capture()) && cargo.Count(fish) == fish.maxStack,
                    "Rescate conserva carga y corta acceso");
                session.CloseCargo(); Apply(loaded); session.SaveNow();
                session.ToggleDeveloperMode();
                byte[] main = File.ReadAllBytes(store.FilePath), backup = File.ReadAllBytes(store.BackupPath);
                session.ToggleDeveloperMode(); BoardAndOpen();
                Check(session.TransferCargo(false, fish, 1), "Transferir durante Jugador de prueba");
                session.SaveNow();
                Check(main.SequenceEqual(File.ReadAllBytes(store.FilePath)) && backup.SequenceEqual(File.ReadAllBytes(store.BackupPath)),
                    "Transferencias de prueba no sobrescriben principal ni respaldo");
                session.ToggleDeveloperMode();
                Check(!session.CargoOpen && session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition) && cargo.Stacks.Count == 0,
                    "Escenario Develop reinicia bodega y cierra panel");
                session.EndDeveloperSession();
                Check(JsonUtility.ToJson(Capture()) == JsonUtility.ToJson(loaded), "Salir de Develop restaura toda la carga original");
                var legacy = Copy(loaded); legacy.boatCargo = null;
                Check(Valid(legacy), "v2 anterior sin bodega sigue válido"); Apply(legacy);
                Check(cargo.Stacks.Count == 0, "Cargar archivo anterior vacía carga de sesión anterior");
                legacy.version = 1; legacy.treasures = legacy.equipmentUpgrades;
                legacy.discoveredZones = session.Config.zones.Select(z => z.id).ToArray();
                legacy.puzzles = session.Config.zones.Select(z => new SavedPuzzle { id = z.id, turns = new int[z.stonePuzzle.Count], solved = true }).ToArray();
                Check(Valid(legacy), "v1 sin bodega conserva compatibilidad"); Apply(legacy);
                Check(cargo.Stacks.Count == 0 && session.SaveNow(), "v1 migra con carga vacía");
                Apply(loaded); session.ReturnToMenu();
                Check(session.NewGame(true) && cargo.Stacks.Count == 0 && !session.Economy.Has(GearKind.Boat), "Nueva partida reinicia bodega y propiedad");
                Debug.Log("LA_FOSA_CARGO_TESTS_OK · " + checks + " comprobaciones de bodega, transferencias y persistencia, sin Play Mode.");
            }
            finally
            {
                if (session.DeveloperSession) session.EndDeveloperSession();
                session.CloseCargo(); Apply(original); session.SaveNow();
            }
        }
    }
}
