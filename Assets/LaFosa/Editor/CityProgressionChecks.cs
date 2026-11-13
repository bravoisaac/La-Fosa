using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class CityProgressionChecks
    {
        public static void Validate(GameSession session, SaveStore store)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var capture = typeof(GameSession).GetMethod("CaptureSave", flags);
            var apply = typeof(GameSession).GetMethod("ApplySave", flags);
            var validate = typeof(GameSession).GetMethod("ValidateSave", flags);
            var advance = typeof(GameSession).GetMethod("AdvanceCity", flags);
            SaveData Capture() => (SaveData)capture.Invoke(session, null);
            void Apply(SaveData data) => apply.Invoke(session, new object[] { data });
            void Tick(float seconds) => advance.Invoke(session, new object[] { seconds });
            bool Valid(SaveData data) => (bool)validate.Invoke(session, new object[] { data });
            var original = Capture(); var config = session.Config; var diver = session.Diver; var inventory = diver.Inventory;
            int checks = 0;
            void Check(bool condition, string message) { if (!condition) throw new Exception("LA_FOSA_CITY_FAILED: " + message); checks++; }
            var city = new GameObject("Ciudad de prueba en editor"); city.transform.SetParent(session.transform);
            try
            {
                var builder = new WorldBuilder(city.transform, config); builder.BuildCity(); builder.BuildInfrastructure(session);
                typeof(WorldBuilder).GetMethod("BuildCabin", flags).Invoke(builder, null);
                var fresh = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(original));
                fresh.infrastructureLevel = 0; fresh.automaticFish = null; fresh.fishingProgress = null; fresh.exploredDepth = 0;
                fresh.inventory = new SavedStack[0]; fresh.money = 10000;
                Check(Valid(fresh), "Partidas v2 anteriores sin campos de ciudad siguen válidas"); Apply(fresh);
                Check(city.GetComponentsInChildren<ShopCounter>().Select(s => s.shop).Distinct().Count() == 4, "Cuatro locales físicos distintos");
                Physics.SyncTransforms();
                foreach (var counter in city.GetComponentsInChildren<ShopCounter>())
                    Check(Physics.Raycast(counter.transform.position + Vector3.up * .6f + Vector3.forward * 3, Vector3.back, out var hit, 4.5f)
                        && hit.collider.GetComponentInParent<ShopCounter>() == counter, "Mostrador accesible desde la calle: " + counter.shop);
                Check(config.infrastructure.Length == 5 && config.infrastructure.All(o => o.price > 0 && o.ingredients.Length > 0), "Cinco obras con dinero y materiales");
                var view = city.GetComponentInChildren<InfrastructureView>();
                Check(view.transform.Cast<Transform>().All(t => !t.gameObject.activeSelf), "Ninguna obra visible antes de construir");
                inventory.TryAdd(config.fish, 2); inventory.TryAdd(config.minerals[0], 3);
                int balance = session.Economy.Money;
                session.OpenShop(ShopKind.Mining);
                Check(!session.Economy.SellFish() && inventory.FishCount == 2, "Minería no vende pesca");
                Check(session.Economy.SellMineral(config.minerals[0], 1) && inventory.Count(config.minerals[0]) == 2 && session.Economy.Money == balance + config.minerals[0].salePrice, "Venta mineral exacta por unidad");
                Check(!session.Economy.SellMineral(config.minerals[0], 3) && inventory.Count(config.minerals[0]) == 2, "Venta excesiva no consume");
                session.OpenShop(ShopKind.Fishing);
                Check(!session.Economy.SellMineral(config.minerals[0], 1), "Pesca no vende minerales");
                Check(session.Economy.SellFish() && inventory.Count(config.minerals[0]) == 2, "Pesca conserva materiales");
                var oxygen = config.shop.offers.First(o => o.gear == GearKind.OxygenTank);
                Check(!session.Economy.Buy(oxygen), "No comprar accesorios en pesca");
                session.OpenShop(ShopKind.Diving); balance = session.Economy.Money;
                Check(!session.Economy.Buy(oxygen) && session.Economy.Money == balance && inventory.Count(config.minerals[0]) == 2, "Compra sin materiales no cobra");
                inventory.TryAdd(config.minerals[0], 2);
                Check(session.Economy.Buy(oxygen) && inventory.Count(config.minerals[0]) == 0 && session.Economy.Money == balance - oxygen.price, "Accesorio consume dinero y cobre exactos");
                Check(!session.Economy.Construct(0), "Obras solo en astillero"); session.OpenShop(ShopKind.Construction);
                Check(!session.Economy.Construct(1), "No saltar etapas de construcción");
                balance = session.Economy.Money;
                Check(!session.Economy.Construct(0) && session.Economy.Money == balance, "Obra sin materiales no cobra");
                for (int i = 0; i < config.infrastructure.Length; i++)
                {
                    var offer = config.infrastructure[i];
                    foreach (var ingredient in offer.ingredients) Check(inventory.TryAdd(ingredient.item, ingredient.amount), "Materiales disponibles para obra " + i);
                    balance = session.Economy.Money;
                    Check(session.Economy.Construct(i) && session.InfrastructureLevel == i + 1 && session.Economy.Money == balance - offer.price, "Obra aplicada con precio exacto " + i);
                    Check(offer.ingredients.All(c => inventory.Count(c.item) == 0), "Costes completos de obra " + i);
                    balance = session.Economy.Money;
                    Check(!session.Economy.Construct(i) && session.Economy.Money == balance, "Obra única sin cobro repetido " + i);
                }
                Check(view.transform.Cast<Transform>().All(t => t.gameObject.activeSelf), "Todas las ampliaciones visibles tras comprar");
                Physics.SyncTransforms();
                foreach (float z in new[] { 18.1f, 24f, 29.9f, 30.1f, 36f, 41.9f })
                    Check(Physics.Raycast(new Vector3(9, 10, z), Vector3.down, out var deck, 10) && Mathf.Abs(deck.point.y - 2.425f) < .01f, "Cubierta transitable en z=" + z);
                Check(!Physics.Raycast(new Vector3(0, 10, 76), Vector3.down, 90), "Base y cable no tapan el eje de inmersión");
                Check(city.GetComponentsInChildren<CityInteraction>().Count(i => !i.terminal) == 2, "Dos zonas automáticas físicas");
                Tick(60); Check(session.FishingStock(0) == 0, "No producir en tienda/pausa"); session.CloseShop();
                Tick(30); Check(session.FishingStock(0) == 0, "Respeta intervalo de producción");
                SaveData half = null;
                Check(session.SaveNow() && store.TryLoad(out half, out _) && half.fishingProgress[0] == 30, "Guarda intervalo parcial");
                Tick(30); Check(session.FishingStock(0) == 1 && session.FishingStock(1) == 1, "Producción independiente en ambos puestos");
                Apply(half); Tick(30); Check(session.FishingStock(0) == 1, "Cargar conserva tiempo sin duplicar ingresos");
                Tick(600); Check(session.FishingStock(0) == 10 && session.FishingStock(1) == 10, "Depósito limitado a diez peces");
                int capacity = inventory.capacity; inventory.capacity = 0;
                Check(!session.CollectAutomaticFish(0) && session.FishingStock(0) == 10, "Mochila llena conserva toda la captura"); inventory.capacity = capacity;
                Check(session.CollectAutomaticFish(0) && inventory.FishCount == 10 && session.FishingStock(0) == 0, "Recogida atómica de pesca automática");
                Check(!session.CollectAutomaticFish(0) && inventory.FishCount == 10, "No duplicar recogida");
                session.OpenTransit(false);
                Check(session.TravelRequirement(24).Length > 0, "Descensor solo desde base");
                Check(session.Travel(0) && Vector3.Distance(diver.transform.position, GameSession.BaseArrival) < .01f, "Llegada libre a plataforma construida");
                session.OpenTransit(true); Check(!session.Travel(24), "No bajar a cota sin explorar"); session.CloseShop();
                float eye = diver.View.transform.position.y - diver.transform.position.y;
                diver.LeaveVehicle(new Vector3(0, -88 - eye, 76)); Tick(.1f);
                Check(session.ExploredDepth >= 88, "Registra profundidad explorada");
                session.OpenTransit(true); Check(session.Travel(52) && Mathf.Abs(diver.Depth - 52) < .01f, "Descenso a cota explorada con equipo");
                var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                try
                {
                    blocker.transform.position = GameSession.BaseArrival + Vector3.up; blocker.transform.localScale = Vector3.one * 2; Physics.SyncTransforms();
                    session.OpenTransit(true); Vector3 before = diver.transform.position;
                    Check(!session.Travel(0) && diver.transform.position == before, "Destino obstruido no desplaza al buzo");
                }
                finally { UnityEngine.Object.DestroyImmediate(blocker); Physics.SyncTransforms(); }
                Check(session.Travel(-1) && diver.transform.position == config.spawn, "Enlace de vuelta a ciudad");
                var completed = Capture();
                Check(session.ReturnToMenu() && session.ContinueGame() && JsonUtility.ToJson(completed) == JsonUtility.ToJson(Capture()), "Continuar conserva obras, stock, temporizadores y exploración");
                var bad = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(completed)); bad.automaticFish[0] = 11; Check(!Valid(bad), "Rechazar stock excesivo");
                bad.automaticFish[0] = 0; bad.fishingProgress[0] = float.NaN; Check(!Valid(bad), "Rechazar temporizador no finito");
                bad.fishingProgress[0] = 0; bad.infrastructureLevel = 0; Check(!Valid(bad), "Rechazar stock en puestos no construidos");
                bad = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(completed)); bad.infrastructureLevel = 6; Check(!Valid(bad), "Rechazar etapa inexistente");
                bad.infrastructureLevel = 5; bad.exploredDepth = 999; Check(!Valid(bad), "Rechazar exploración fuera del mapa");
                session.ToggleDeveloperMode(); session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition);
                Check(session.InfrastructureLevel == 0 && session.FishingStock(1) == 0, "Escenario Develop reinicia infraestructura temporal");
                session.DeveloperMaterials(); Check(inventory.Count(config.minerals[0]) == 10, "Materiales de prueba disponibles");
                session.ToggleDeveloperMode(); session.DeveloperMaterials(); Check(inventory.Count(config.minerals[0]) == 10, "Jugador no concede materiales");
                session.EndDeveloperSession();
                Check(JsonUtility.ToJson(Capture()) == JsonUtility.ToJson(completed), "Salir de Develop restaura toda la ciudad");
                Debug.Log("LA_FOSA_CITY_TESTS_OK · " + checks + " comprobaciones de comercios, obras, pesca, viajes y guardado, sin Play Mode.");
            }
            finally
            {
                if (session.DeveloperSession) session.EndDeveloperSession();
                Apply(original); session.SaveNow();
                foreach (var view in city.GetComponentsInChildren<InfrastructureView>(true)) view.Release();
                var meshes = city.GetComponentsInChildren<MeshFilter>(true).Select(m => m.sharedMesh).Where(m => m != null && m.name == "Octaedro low poly").Distinct().ToArray();
                var materials = city.GetComponentsInChildren<Renderer>(true).Select(r => r.sharedMaterial).Where(m => m != null).Distinct().ToArray();
                UnityEngine.Object.DestroyImmediate(city);
                foreach (var mesh in meshes) UnityEngine.Object.DestroyImmediate(mesh);
                foreach (var material in materials) if (!UnityEditor.AssetDatabase.Contains(material)) UnityEngine.Object.DestroyImmediate(material);
            }
        }
    }
}
