using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class DeveloperModeChecks
    {
        public static void Validate(GameConfig config)
        {
            var host = new GameObject("Prueba aislada de Develop · sin Play Mode");
            string directory = Path.Combine(Path.GetTempPath(), "LaFosa-Develop-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, "expedition.json");
            float timeScale = Time.timeScale;
            var cursorLock = Cursor.lockState; bool cursorVisible = Cursor.visible;
            int checks = 0;
            void Check(bool condition, string message)
            { if (!condition) throw new Exception("LA_FOSA_DEVELOPER_FAILED: " + message); checks++; }
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            try
            {
                var session = host.AddComponent<GameSession>(); session.Initialize(config);
                var player = new GameObject("Buzo de prueba"); player.transform.SetParent(host.transform);
                var cameraObject = new GameObject("Cámara de prueba"); cameraObject.transform.SetParent(player.transform);
                cameraObject.transform.localPosition = Vector3.up * 1.55f;
                var diver = player.AddComponent<DiverController>(); diver.Initialize(session, cameraObject.AddComponent<Camera>()); session.Diver = diver;
                player.AddComponent<HarpoonGun>().diver = diver;
                foreach (var zone in config.zones)
                {
                    var station = new GameObject(zone.id); station.transform.SetParent(host.transform);
                    var puzzle = station.AddComponent<StoneCircuitPuzzle>(); puzzle.Initialize(zone);
                    station.AddComponent<TreasureChest>().puzzle = puzzle;
                }
                foreach (var kind in new[] { GearKind.Boat, GearKind.JetSki })
                {
                    var vehicleObject = new GameObject(kind.ToString()); vehicleObject.transform.SetParent(host.transform);
                    vehicleObject.transform.position = new Vector3(0, config.waterLevel, 20);
                    var vehicle = vehicleObject.AddComponent<SurfaceVehicle>(); vehicle.kind = kind; vehicle.session = session;
                    if (kind == GearKind.Boat) session.Boat = vehicle; else session.JetSki = vehicle;
                }
                session.InitializePersistence();
                var validate = typeof(GameSession).GetMethod("ValidateSave", flags);
                var store = new SaveStore(path, data => (bool)validate.Invoke(session, new object[] { data }));
                var storeField = typeof(GameSession).GetField("saveStore", flags);
                storeField.SetValue(session, store); // All writes below target the unique temporary folder.
                Check(session.NewGame(), "Partida de prueba creada fuera del guardado personal");
                session.ToggleDeveloperMode();
                Check(session.DeveloperSession && session.DeveloperMode && session.Blocked, "Entrada Develop con panel y snapshot");
                byte[] baseline = File.ReadAllBytes(path), backup = File.ReadAllBytes(path + ".bak");
                session.DeveloperAddMoney(); session.DeveloperEquip(); session.DeveloperTravel(280);
                Check(session.Economy.Money == 1000 && diver.SafeDepth == 650 && Mathf.Abs(diver.Depth - 280) < .01f, "Preparar economía, equipo y profundidad");
                session.ToggleDeveloperAir(); session.ToggleDeveloperSpeed(); session.ToggleDeveloperPanel();
                var vitals = typeof(DiverController).GetMethod("AdvanceVitals", flags);
                vitals.Invoke(diver, new object[] { 1f, false });
                Check(diver.Oxygen == diver.MaxOxygen, "Develop conserva aire con opción activa");
                session.ToggleDeveloperMode();
                Check(!session.DeveloperMode && session.DeveloperSession && !session.Blocked && Mathf.Abs(diver.Depth - 280) < .01f, "Jugador conserva escenario de prueba sin privilegios");
                float oxygen = diver.Oxygen;
                vitals.Invoke(diver, new object[] { 1f, false });
                Check(diver.Oxygen < oxygen, "Jugador consume aire aunque Develop tuviera aire fijo");
                session.DeveloperAddMoney(); session.DeveloperTravel(0); diver.DeveloperRefill();
                Check(session.Economy.Money == 1000 && diver.Depth > 279 && diver.Oxygen < oxygen, "Herramientas denegadas en modo Jugador");
                session.OpenShop();
                Check(session.Economy.Buy(config.shop.offers.First(o => o.gear == GearKind.Boat)), "Compra real dentro de la prueba");
                var firstPuzzle = host.GetComponentsInChildren<StoneCircuitPuzzle>().First(p => p.zone == config.zones[0]);
                typeof(StoneCircuitPuzzle).GetMethod("Restore", flags).Invoke(firstPuzzle, new object[] { new int[firstPuzzle.Definition.Count], 0, false });
                Check(firstPuzzle.Activate() && diver.Inventory.TryAdd(config.zones[0].treasure), "Resolver sello y recoger tesoro en la prueba");
                session.Reward(config.zones[0]);
                Check(session.Treasures == 1 && session.IsTreasureClaimed(config.zones[0]), "Recompensa cambia progreso temporal");
                session.SaveNow();
                typeof(GameSession).GetMethod("OnApplicationQuit", flags).Invoke(session, null);
                Check(File.ReadAllBytes(path).SequenceEqual(baseline) && File.ReadAllBytes(path + ".bak").SequenceEqual(backup), "Compra, guardado y cierre no escriben prueba ni respaldo");
                session.EndDeveloperSession();
                Check(!session.DeveloperSession && !session.DeveloperMode && !session.DeveloperFastMove && !session.DeveloperInfiniteAir, "Salida limpia de todos los privilegios");
                Check(session.Economy.Money == 0 && !session.Economy.Has(GearKind.Boat) && !session.Economy.Has(GearKind.Trident) && diver.SafeDepth == config.initialSafeDepth, "Progreso y equipo originales restaurados");
                Check(session.Treasures == 0 && !firstPuzzle.Solved && diver.Inventory.Count(config.zones[0].treasure) == 0, "Tesoro, sello e inventario de prueba descartados");
                Check(Vector3.Distance(diver.transform.position, config.spawn) < .01f, "Regreso a playa al terminar pruebas");
                session.ToggleDeveloperMode(); session.DeveloperAddMoney();
                Check(session.ReturnToMenu() && session.Intro && !session.DeveloperSession, "Menú termina pruebas antes de guardar");
                Check(store.TryLoad(out var saved, out _) && saved.money == 0 && saved.gear.Length == 0, "Menú conserva el progreso normal en disco");
                Check(session.ContinueGame(), "Continúa la partida normal tras las pruebas");
                // Use a non-empty normal save so resetting a scenario cannot accidentally
                // replace the original snapshot with a fresh expedition unnoticed.
                typeof(Economy).GetMethod("Restore", flags).Invoke(session.Economy, new object[] { 137, new[] { nameof(GearKind.Trident) } });
                diver.Inventory.TryAdd(config.fish, 2); firstPuzzle.Rotate(0);
                var capture = typeof(GameSession).GetMethod("CaptureSave", flags);
                string originalProgress = JsonUtility.ToJson(capture.Invoke(session, null));
                Check(session.SaveNow(), "Guardar progreso no vacío antes de probar escenarios");
                session.ToggleDeveloperMode();
                byte[] scenarioBaseline = File.ReadAllBytes(path), scenarioBackup = File.ReadAllBytes(path + ".bak");
                session.DeveloperEquip(); session.DeveloperAddMoney();
                session.ToggleDeveloperAir(); session.ToggleDeveloperSpeed();
                Check(session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition), "Preparar primera expedición temporal");
                Check(session.Economy.Money == 0 && diver.Inventory.Stacks.Count == 0 && !session.Economy.Has(GearKind.Trident)
                    && diver.SafeDepth == config.initialSafeDepth && diver.GetComponent<HarpoonGun>().SelectedTool == 0,
                    "Inicio recupera manos, mochila vacía y equipo inicial");
                Check(session.Blocked && session.DeveloperPanelOpen && !session.DeveloperInfiniteAir && !session.DeveloperFastMove
                    && Vector3.Distance(diver.transform.position, config.spawn) < .01f, "Inicio en playa pausado y sin ventajas");
                Check(session.DeveloperPrepareScenario(DeveloperScenario.FirstSanctuary), "Preparar primer santuario temporal");
                Check(session.Economy.Has(GearKind.Trident) && !session.Economy.Has(GearKind.ReinforcedHarpoon)
                    && diver.GetComponent<HarpoonGun>().SelectedTool == 1 && !firstPuzzle.Solved && firstPuzzle.Moves == 0
                    && session.Treasures == 0, "Santuario con tridente, sin resolver ni regalar tesoros");
                Check(Vector3.Distance(diver.transform.position, config.zones[0].stationPosition) < 5
                    && diver.transform.position.y > WorldBuilder.FloorHeight(diver.transform.position.x, diver.transform.position.z),
                    "Llegada al santuario por encima del terreno");
                session.ToggleDeveloperMode();
                var scenarioPosition = diver.transform.position;
                Check(!session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition) && diver.transform.position == scenarioPosition,
                    "Jugador no puede reiniciar escenarios");
                session.ToggleDeveloperMode();
                Check(!session.DeveloperPrepareScenario((DeveloperScenario)99) && diver.transform.position == scenarioPosition,
                    "Escenario desconocido no cambia la prueba");
                session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition); session.SaveNow();
                Check(File.ReadAllBytes(path).SequenceEqual(scenarioBaseline) && File.ReadAllBytes(path + ".bak").SequenceEqual(scenarioBackup),
                    "Repetir escenarios no modifica partida ni respaldo");
                session.EndDeveloperSession();
                Check(JsonUtility.ToJson(capture.Invoke(session, null)) == originalProgress,
                    "Restaurar dinero, pesca, equipo y giros originales después de varios escenarios");
                Check(!session.DeveloperPrepareScenario(DeveloperScenario.FirstSanctuary), "Escenarios rechazados fuera de Develop");
                MooringChecks.Validate(session, store);
                storeField.SetValue(session, new SaveStore(path, _ => false));
                session.ToggleDeveloperMode();
                Check(!session.DeveloperSession && !session.DeveloperMode, "Fallo de guardado inicial impide entrar en Develop");
                Debug.Log("LA_FOSA_DEVELOPER_TESTS_OK · " + checks + " comprobaciones de modos y guardado aislado, sin Play Mode.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                Time.timeScale = timeScale; Cursor.lockState = cursorLock; Cursor.visible = cursorVisible;
                foreach (string file in new[] { path, path + ".bak", path + ".tmp" }) if (File.Exists(file)) File.Delete(file);
                if (Directory.Exists(directory)) Directory.Delete(directory);
            }
        }
    }
}
