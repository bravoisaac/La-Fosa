using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace LaFosa
{
    // Opt-in integration check: never runs during a normal game.
    public sealed partial class SmokeCheck : MonoBehaviour
    {
        string output;
        bool failed;
        int checks;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void StartIfRequested()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--smoke-test") >= 0)
                new GameObject("Verificación de integración").AddComponent<SmokeCheck>();
        }
        void Awake()
        {
            output = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Verification"));
            Directory.CreateDirectory(output); File.WriteAllText(Path.Combine(output, "errors.txt"), ""); Application.logMessageReceived += OnLog;
        }
        void OnDestroy() { Application.logMessageReceived -= OnLog; }
        void OnLog(string message, string trace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert) return;
            failed = true; File.AppendAllText(Path.Combine(output, "errors.txt"), message + "\n" + trace + "\n"); Application.Quit(2);
        }
        void Check(bool condition, string message)
        {
            checks++;
            if (!condition) { failed = true; Debug.LogError("SMOKE_FAILED: " + message); }
            else Debug.Log("SMOKE_OK: " + message);
        }
        IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(1);
            var bootstrap = FindFirstObjectByType<GameBootstrap>();
            Check(bootstrap != null && bootstrap.Session != null, "Arranque de escena");
            if (bootstrap == null || bootstrap.Session == null) yield break;
            var session = bootstrap.Session; var diver = session.Diver;
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--water-preview") >= 0)
            {
                session.Begin(); yield return CaptureWaterViews(session);
                Debug.Log("LA_FOSA_WATER_PREVIEW_OK"); Application.Quit(failed ? 2 : 0); yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--save-restart-write") >= 0 || Array.IndexOf(Environment.GetCommandLineArgs(), "--save-restart-read") >= 0)
            { TestPersistenceRestart(session); yield break; }
            Check(session.Intro && Time.timeScale == 0, "Introducción pausa el mundo");
            Capture(diver.View, "01-inicio.png");
            yield return new WaitForSecondsRealtime(.5f);
            session.Begin();
            yield return new WaitForSecondsRealtime(.3f);
            Capture(diver.View, "02-playa.png");
            yield return TestClosedRoutes(session);
            Check(FindObjectsByType<TreasureChest>(FindObjectsSortMode.None).Length == session.Config.zones.Length, "Santuarios generados");
            CheckFauna(session);
            var pickup = FindFirstObjectByType<Pickup>(); pickup.Interact(diver);
            Check(diver.Inventory.Stacks.Count == 1, "Recoger concha");
            Teleport(diver, new Vector3(0, -6, 32));
            float oxygen = diver.Oxygen;
            yield return new WaitForSecondsRealtime(.5f);
            Check(diver.Underwater && diver.Oxygen < oxygen, "Detección de agua y consumo de aire");
            session.ToggleInventory(); oxygen = diver.Oxygen;
            yield return new WaitForSecondsRealtime(.3f);
            Check(Mathf.Approximately(diver.Oxygen, oxygen) && Time.timeScale == 0, "Inventario pausa oxígeno");
            session.ToggleInventory();
            FishAgent fish = null;
            foreach (var candidate in FindObjectsByType<FishAgent>(FindObjectsSortMode.None))
                if (candidate.species.id == "baunco") { fish = candidate; break; }
            fish.enabled = false;
            fish.transform.position = new Vector3(130, -5, 33);
            Teleport(diver, fish.transform.position - Vector3.forward * 4 - Vector3.up * 1.55f);
            diver.LookAt(fish.transform.position);
            yield return null;
            Physics.SyncTransforms();
            var gun = diver.GetComponent<HarpoonGun>();
            Check(gun.SelectedTool == 0 && gun.ToolName == "MANOS" && gun.toolModels[0].activeSelf && !gun.toolModels[1].activeSelf && !gun.toolModels[2].activeSelf && !gun.toolModels[3].activeSelf, "Inicio con manos y sin armas visibles");
            gun.Fire();
            Check(diver.Inventory.Stacks.Count == 1 && fish.Available, "Manos no capturan peces a distancia de arpón");
            Teleport(diver, fish.transform.position - Vector3.forward * 1.5f - Vector3.up * 1.55f);
            diver.LookAt(fish.transform.position);
            yield return new WaitForSeconds(.9f);
            Physics.SyncTransforms(); gun.Fire();
            Check(diver.Inventory.Stacks.Count == 2 && !fish.Available, "Manos capturan pez cercano y agregan inventario");
            gun.Fire(); Check(diver.Inventory.Stacks[1].count == 1, "Recarga evita disparo duplicado");
            Teleport(diver, new Vector3(0, -40, 75));
            float health = diver.Health;
            yield return new WaitForSecondsRealtime(.4f);
            Check(diver.Health < health, "Daño por presión");
            yield return TestCommerce(session, diver, gun);
            Check(Array.TrueForAll(session.GetComponentsInChildren<DiveGate>(), g => !g.IsOpen), "Comprar todo el equipo no abre las compuertas ni resuelve las piedras");
            yield return TestFishBehavior(session);
            var chests = FindObjectsByType<TreasureChest>(FindObjectsSortMode.None);
            foreach (var chest in chests)
                if (chest.puzzle.zone == session.Config.zones[1]) { chest.Interact(diver); Check(session.Treasures == 0, "No se puede saltar la progresión"); }
            for (int i = 0; i < session.Config.zones.Length; i++)
            {
                var zone = session.Config.zones[i];
                TreasureChest chest = null;
                foreach (var candidate in chests) if (candidate.puzzle.zone == zone) chest = candidate;
                Teleport(diver, zone.stationPosition + new Vector3(0, 1.1f, -5.8f));
                diver.LookAt(zone.stationPosition + new Vector3(0, 2.5f, .7f));
                yield return new WaitForSecondsRealtime(.3f);
                Capture(diver.View, "0" + (i + 3) + "-nivel.png");
                chest.Interact(diver); Check(!chest.Claimed, "Cofre bloqueado antes de resolver");
                yield return TestStonePuzzle(session, diver, chest, i);
                chest.Interact(diver);
                Check(chest.Claimed && session.Treasures == i + 1 && diver.SafeDepth == zone.unlockedDepth, "Tesoro mejora el equipo " + i);
                Check(Mathf.Approximately(diver.MaxOxygen, zone.unlockedOxygen + session.Config.shop.oxygenBonus), "Bombona se acumula con tesoro " + i);
                chest.Interact(diver); Check(session.Treasures == i + 1, "Recompensa única " + i);
                if (i < session.Config.zones.Length - 1) TestOpenedRoute(session, i);
            }
            Check(session.Complete, "Expedición completada");
            int stacks = diver.Inventory.Stacks.Count;
            diver.Rescue("Prueba de rescate");
            Check(Vector3.Distance(diver.transform.position, session.Config.spawn) < .1f && diver.Inventory.Stacks.Count == stacks && diver.Health == 100, "Rescate conserva progreso y repone salud");
            session.ToggleInventory();
            yield return new WaitForSecondsRealtime(.3f);
            Capture(diver.View, "06-regreso.png");
            TestSpeciesTrading(session);
            yield return CaptureFishGallery(session.Config);
            TestPersistence(session);
            TestRoutePersistence(session);
            yield return new WaitForSecondsRealtime(.6f);
            File.WriteAllText(Path.Combine(output, "result.txt"), (failed ? "FAILED" : "PASSED") + " · " + checks + " integration checks\n");
            Debug.Log("LA_FOSA_SMOKE_" + (failed ? "FAILED" : "OK") + " · " + checks);
            Application.Quit(failed ? 2 : 0);
        }
        IEnumerator TestCommerce(GameSession session, DiverController diver, HarpoonGun gun)
        {
            var economy = session.Economy; var catalog = session.Config.shop;
            Check(!session.Boat.gameObject.activeSelf && !session.JetSki.gameObject.activeSelf, "Vehículos no disponibles antes de comprar");
            Check(!gun.SelectTool(1) && !gun.SelectTool(2) && !gun.SelectTool(3) && gun.SelectedTool == 0, "Todas las herramientas bloqueadas antes de comprar");
            var counter = FindFirstObjectByType<ShopCounter>();
            Teleport(diver, new Vector3(-6, 3.2f, -10)); diver.LookAt(new Vector3(-13, 6, -23));
            yield return new WaitForSecondsRealtime(.2f);
            Capture(diver.View, "07-cabana.png");
            Teleport(diver, new Vector3(-12, 3.2f, -16.5f)); diver.LookAt(counter.transform.position + Vector3.up * .5f);
            yield return null;
            Check(diver.Target == counter, "Mostrador accesible mediante raycast de interacción");
            counter.Interact(diver);
            Check(session.ShopOpen && Time.timeScale == 0, "Tienda abierta y mundo en pausa");
            Check(!economy.Buy(catalog.offers[0]) && economy.Money == 0, "Compra rechazada sin saldo");
            int shells = diver.Inventory.Count(session.Config.shell);
            int fish = economy.FishCount;
            Check(economy.SellFish() && economy.Money == fish * catalog.fishValue, "Venta paga el valor exacto");
            Check(economy.FishCount == 0 && diver.Inventory.Count(session.Config.shell) == shells, "Venta retira solo peces");
            int balance = economy.Money;
            Check(!economy.SellFish() && economy.Money == balance, "No se puede vender dos veces la misma pesca");
            Check(diver.Inventory.TryAdd(session.Config.fish, 50) && economy.SellFish(), "Venta de varias pilas");
            balance = economy.Money;
            foreach (var offer in catalog.offers)
                if (offer.gear == GearKind.ReinforcedHarpoon || offer.gear == GearKind.Net)
                    Check(!economy.Buy(offer) && economy.Money == balance && !economy.Has(offer.gear), "Tridente requerido incluso con saldo: " + offer.title);
            Check(catalog.offers[0].gear == GearKind.Trident && catalog.offers[0].price == 2 * catalog.fishValue, "Tridente es la primera herramienta y cuesta dos peces");
            foreach (var offer in catalog.offers)
            {
                balance = economy.Money;
                Check(economy.Buy(offer) && economy.Has(offer.gear) && economy.Money == balance - offer.price, "Compra: " + offer.title);
                if (offer.gear == GearKind.Trident)
                    Check(gun.SelectedTool == 1 && gun.toolModels[1].activeSelf && !gun.toolModels[0].activeSelf, "Primer tridente se equipa automáticamente");
                balance = economy.Money;
                Check(!economy.Buy(offer) && economy.Money == balance, "Sin cobro duplicado: " + offer.title);
            }
            Check(Mathf.Approximately(diver.MaxOxygen, session.Config.initialOxygen + catalog.oxygenBonus), "Bombona aumenta reserva inicial");
            Check(gun.SelectTool(2) && diver.SwimSpeedMultiplier > 1 && gun.EffectiveRange == 40 && gun.EffectiveCooldown < 1, "Arpón adquirido se equipa con su alcance y recarga");
            Check(economy.FishCount == 0 && diver.Inventory.Stacks.Count == 1, "Equipo comprado no ocupa mochila");
            session.CloseShop();
            Check(!session.Blocked && Time.timeScale == 1 && !economy.SellFish(), "Cerrar tienda reanuda y bloquea venta remota");

            foreach (var vehicle in new[] { session.Boat, session.JetSki })
            {
                Check(vehicle.gameObject.activeInHierarchy, "Entrega: " + vehicle.DisplayName);
                Teleport(diver, vehicle.transform.position + Vector3.right * 2.5f + Vector3.down * .65f);
                Check(vehicle.TryBoard(diver) && diver.Riding == vehicle, "Embarque: " + vehicle.DisplayName);
                Vector3 start = vehicle.transform.position;
                vehicle.Drive(1, 0, .5f);
                Check(Vector3.Distance(start, vehicle.transform.position) > vehicle.speed * .3f, "Navegación: " + vehicle.DisplayName);
                session.ToggleInventory(); start = vehicle.transform.position; vehicle.Drive(1, 1, .5f);
                Check(vehicle.transform.position == start, "Pausa detiene embarcación"); session.ToggleInventory();
                Check(vehicle.Disembark() && diver.Riding == null && diver.GetComponent<CharacterController>().enabled, "Desembarque seguro");
            }
            Teleport(diver, new Vector3(12, 3, 28)); diver.LookAt(session.Boat.transform.position);
            yield return null; Capture(diver.View, "08-embarcaciones.png");

            var targets = new System.Collections.Generic.List<FishAgent>();
            foreach (var candidate in FindObjectsByType<FishAgent>(FindObjectsSortMode.None))
                if (candidate.Available && candidate.species.id == "baunco" && targets.Count < 3) { candidate.enabled = false; targets.Add(candidate); }
            // A clear patch outside ambient fish/rock spawning makes visibility deterministic.
            Teleport(diver, new Vector3(130, -6.55f, 26)); diver.LookAt(new Vector3(130, -5, 33));
            for (int i = 0; i < 2; i++) targets[i].transform.position = new Vector3(130 + (i == 0 ? -1.1f : 1.1f), -5, 32);
            targets[0].respawnSeconds = .25f;
            Physics.SyncTransforms();
            yield return new WaitForSeconds(1.2f);
            Check(gun.SelectTool(3) && gun.toolModels[3].activeSelf && !gun.toolModels[0].activeSelf, "Seleccionar red y modelo");
            int before = economy.FishCount; gun.Fire();
            Check(economy.FishCount >= before + 2 && !targets[0].Available && !targets[1].Available, "Red captura múltiples peces visibles");
            Check(!targets[0].Catch(diver), "Pez capturado no duplica recompensa");
            targets[0].enabled = true;
            yield return new WaitForSeconds(.35f);
            Check(targets[0].Available && targets[0].GetComponent<Collider>().enabled, "Reposición de peces permite economía continua");
            yield return new WaitForSeconds(4.8f);
            targets[2].transform.position = new Vector3(130, -5, 29);
            Physics.SyncTransforms();
            Check(gun.SelectTool(1) && gun.toolModels[1].activeSelf, "Seleccionar tridente");
            before = economy.FishCount; gun.Fire();
            Check(economy.FishCount == before + 1 && !targets[2].Available, "Tridente captura a corta distancia");
            gun.SelectTool(0);
            Teleport(diver, session.JetSki.transform.position + Vector3.right * 2.5f + Vector3.down * .65f);
            Check(session.JetSki.TryBoard(diver), "Reembarque tras abandonar moto");
            diver.Rescue("Rescate de prueba");
            Check(diver.Riding == null && session.JetSki.Passenger == null && economy.Has(GearKind.JetSki), "Rescate desde vehículo conserva propiedad");
        }
        static void Teleport(DiverController diver, Vector3 position)
        {
            var controller = diver.GetComponent<CharacterController>(); controller.enabled = false; diver.transform.position = position; controller.enabled = true;
            Physics.SyncTransforms();
        }
        void Capture(Camera camera, string name)
        {
            var previous = RenderTexture.active;
            var target = RenderTexture.GetTemporary(1600, 900, 24, RenderTextureFormat.ARGB32);
            var image = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            var originalTarget = camera.targetTexture;
            try
            {
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); image.Apply();
                File.WriteAllBytes(Path.Combine(output, name), image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = originalTarget; RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(target); Destroy(image);
            }
        }
    }
}
