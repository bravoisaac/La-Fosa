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
            var host = new GameObject("Pruebas de economía y Develop sin Play Mode");
            string directory = Path.Combine(Path.GetTempPath(), "LaFosa-Economy-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, "expedition.json");
            float timeScale = Time.timeScale; var cursorLock = Cursor.lockState; bool cursorVisible = Cursor.visible;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            int checks = 0;
            void Check(bool condition, string message)
            { if (!condition) throw new Exception("LA_FOSA_DEVELOPER_FAILED: " + message); checks++; }
            try
            {
                var session = host.AddComponent<GameSession>(); session.Initialize(config);
                var player = new GameObject("Buzo de prueba"); player.transform.SetParent(host.transform);
                var cameraObject = new GameObject("Cámara de prueba"); cameraObject.transform.SetParent(player.transform);
                cameraObject.transform.localPosition = Vector3.up * 1.55f;
                var diver = player.AddComponent<DiverController>(); diver.Initialize(session, cameraObject.AddComponent<Camera>()); session.Diver = diver;
                var controller = player.GetComponent<CharacterController>(); controller.height = 1.8f; controller.center = Vector3.up * .9f; controller.radius = .32f;
                player.AddComponent<HarpoonGun>().diver = diver;
                new WorldBuilder(host.transform, config).BuildMinerals();
                foreach (var kind in new[] { GearKind.Boat, GearKind.JetSki })
                {
                    var obj = new GameObject(kind.ToString()); obj.transform.SetParent(host.transform); obj.transform.position = new Vector3(0, config.waterLevel, 20);
                    var vehicle = obj.AddComponent<SurfaceVehicle>(); vehicle.kind = kind; vehicle.session = session;
                    if (kind == GearKind.Boat) session.Boat = vehicle; else session.JetSki = vehicle;
                }
                session.InitializePersistence();
                var validate = typeof(GameSession).GetMethod("ValidateSave", flags);
                var capture = typeof(GameSession).GetMethod("CaptureSave", flags);
                var store = new SaveStore(path, data => (bool)validate.Invoke(session, new object[] { data }));
                var storeField = typeof(GameSession).GetField("saveStore", flags); storeField.SetValue(session, store);
                Check(session.NewGame(), "Partida v2 creada en ruta temporal");
                EconomyProgressionChecks.Validate(session, store);
                CityProgressionChecks.Validate(session, store);
                var original = JsonUtility.ToJson(capture.Invoke(session, null));
                session.ToggleDeveloperMode();
                Check(session.DeveloperSession && session.DeveloperMode && session.Blocked, "Develop con copia protegida");
                byte[] baseline = File.ReadAllBytes(path), backup = File.ReadAllBytes(path + ".bak");
                session.DeveloperAddMoney(); session.DeveloperEquip(); session.DeveloperTravel(280);
                Check(diver.SafeDepth == 650 && Mathf.Abs(diver.Depth - 280) < .01f, "Equipo y traslado de prueba");
                session.ToggleDeveloperAir(); session.ToggleDeveloperSpeed(); session.ToggleDeveloperPanel();
                var vitals = typeof(DiverController).GetMethod("AdvanceVitals", flags);
                vitals.Invoke(diver, new object[] { 1f, false });
                Check(diver.Oxygen == diver.MaxOxygen, "Aire fijo en Develop");
                session.ToggleDeveloperMode(); float oxygen = diver.Oxygen;
                vitals.Invoke(diver, new object[] { 1f, false });
                Check(!session.DeveloperMode && diver.Oxygen < oxygen, "Jugador consume aire");
                int money = session.Economy.Money; session.DeveloperAddMoney(); session.DeveloperTravel(0);
                Check(session.Economy.Money == money && diver.Depth > 279, "Herramientas bloqueadas en Jugador");
                Check(!session.DeveloperPrepareScenario(DeveloperScenario.FirstMining), "Jugador no reinicia escenarios");
                session.ToggleDeveloperMode();
                Check(session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition) && session.CraftedUpgrades == 0 && session.Economy.Money == 0 && diver.Inventory.Stacks.Count == 0,
                    "Escenario inicial sin recursos ni mejoras");
                Check(!session.DeveloperInfiniteAir && !session.DeveloperFastMove, "Escenarios desactivan ventajas");
                Check(session.DeveloperPrepareScenario(DeveloperScenario.FirstMining) && session.Economy.Has(GearKind.MiningPick) && !session.Economy.Has(GearKind.Trident),
                    "Escenario minero prepara pico sin puzles");
                session.ToggleDeveloperMode();
                var deposit = host.GetComponentsInChildren<MineralDeposit>().First(p => p.mineral == config.minerals[0]);
                diver.LeaveVehicle(deposit.transform.position + Vector3.up); Physics.SyncTransforms();
                Check(deposit.TryMine(diver), "Minería dentro de prueba temporal");
                session.SaveNow(); typeof(GameSession).GetMethod("OnApplicationQuit", flags).Invoke(session, null);
                Check(File.ReadAllBytes(path).SequenceEqual(baseline) && File.ReadAllBytes(path + ".bak").SequenceEqual(backup), "Pruebas y cierre no escriben sobre partida/respaldo");
                session.EndDeveloperSession();
                Check(JsonUtility.ToJson(capture.Invoke(session, null)) == original && !session.DeveloperSession, "Restaurar progreso, minerales y enfriamiento anteriores");
                session.ToggleDeveloperMode(); session.DeveloperAddMoney();
                Check(session.ReturnToMenu() && !session.DeveloperSession && session.ContinueGame(), "Menú descarta pruebas y permite continuar");
                Check(JsonUtility.ToJson(capture.Invoke(session, null)) == original, "Carga posterior conserva partida anterior");
                MooringChecks.Validate(session, store);
                CargoChecks.Validate(session, store);
                storeField.SetValue(session, new SaveStore(path, _ => false)); session.ToggleDeveloperMode();
                Check(!session.DeveloperSession, "Fallo de guardado impide Develop");
                Debug.Log("LA_FOSA_DEVELOPER_TESTS_OK · " + checks + " comprobaciones de modos y guardado aislado, sin Play Mode.");
            }
            finally
            {
                var deposits = host.GetComponentsInChildren<MineralDeposit>(true);
                var meshes = deposits.Select(d => d.GetComponent<MeshFilter>().sharedMesh).Distinct().ToArray();
                var materials = deposits.Select(d => d.GetComponent<Renderer>().sharedMaterial).Distinct().ToArray();
                UnityEngine.Object.DestroyImmediate(host);
                foreach (var mesh in meshes) UnityEngine.Object.DestroyImmediate(mesh);
                foreach (var material in materials) UnityEngine.Object.DestroyImmediate(material);
                Time.timeScale = timeScale; Cursor.lockState = cursorLock; Cursor.visible = cursorVisible;
                foreach (string file in new[] { path, path + ".bak", path + ".tmp" }) if (File.Exists(file)) File.Delete(file);
                if (Directory.Exists(directory)) Directory.Delete(directory);
            }
        }
    }
}
