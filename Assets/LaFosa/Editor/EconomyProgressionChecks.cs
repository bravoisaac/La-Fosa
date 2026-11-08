using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class EconomyProgressionChecks
    {
        public static void Validate(GameSession session, SaveStore store)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var capture = typeof(GameSession).GetMethod("CaptureSave", flags);
            var apply = typeof(GameSession).GetMethod("ApplySave", flags);
            var validate = typeof(GameSession).GetMethod("ValidateSave", flags);
            var restoreMoney = typeof(Economy).GetMethod("Restore", flags);
            var restoreDeposit = typeof(MineralDeposit).GetMethod("Restore", flags);
            var diver = session.Diver; var inventory = diver.Inventory; var config = session.Config;
            SaveData Capture() => (SaveData)capture.Invoke(session, null);
            void Fund(int money) => restoreMoney.Invoke(session.Economy, new object[] { money, Capture().gear });
            int checks = 0;
            void Check(bool condition, string message)
            { if (!condition) throw new Exception("LA_FOSA_ECONOMY_FAILED: " + message); checks++; }
            void Approach(MineralDeposit deposit) { diver.LeaveVehicle(deposit.transform.position + Vector3.up); Physics.SyncTransforms(); }
            var deposits = session.GetComponentsInChildren<MineralDeposit>();
            Check(session.GetComponentsInChildren<StoneCircuitPuzzle>().Length == 0 && session.GetComponentsInChildren<TreasureChest>().Length == 0,
                "No se generan puzles ni cofres de progreso");
            Check(deposits.Length == 35 && deposits.Select(p => p.id).Distinct().Count() == deposits.Length, "35 depósitos con IDs estables únicos");
            Check(config.recipes.Length == 3 && config.recipes.Select(r => r.upgrade).SequenceEqual(config.zones), "Tres recetas en orden de equipo");
            foreach (var recipe in config.recipes)
                Check(recipe.price > 0 && recipe.ingredients.Length > 0 && recipe.ingredients.All(i => i.item != null && i.amount > 0), "Receta con coste y materiales válidos");
            for (int tier = 0; tier < config.minerals.Length; tier++)
            {
                float safeDepth = tier == 0 ? config.initialSafeDepth : config.zones[tier - 1].unlockedDepth;
                Check(deposits.Count(d => d.mineral == config.minerals[tier] && -d.transform.position.y < safeDepth) >= 10,
                    "Mineral alcanzable antes de necesitarlo para fabricar");
            }
            var copper = deposits.First(d => d.mineral == config.minerals[0]); Approach(copper);
            Check(!copper.TryMine(diver) && copper.Available, "Pico necesario sin consumir depósito");
            Check(!session.Economy.Craft(config.recipes[0]), "Fabricación solo en taller");
            diver.Inventory.TryAdd(config.fish, 10); session.OpenShop();
            long value = session.Economy.SaleValue;
            Check(session.Economy.SellFish() && session.Economy.Money == value, "Pesca financia la economía");
            Check(session.Economy.Buy(config.shop.offers.First(o => o.gear == GearKind.MiningPick)), "Comprar pico con venta de peces");
            session.CloseShop(); Approach(copper);
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                wall.transform.position = Vector3.Lerp(diver.View.transform.position, copper.transform.position, .5f);
                wall.transform.localScale = Vector3.one * .5f; Physics.SyncTransforms();
                Check(!copper.TryMine(diver), "No extraer a través de obstáculos");
            }
            finally { UnityEngine.Object.DestroyImmediate(wall); Physics.SyncTransforms(); }
            Check(copper.TryMine(diver) && inventory.Count(copper.mineral) == 3 && !copper.Available, "Extraer tres minerales y agotar depósito");
            Check(!copper.TryMine(diver) && inventory.Count(copper.mineral) == 3, "Evitar extracción doble");
            Check(session.ReturnToMenu(), "Guardar minerales antes de cargar");
            restoreDeposit.Invoke(copper, new object[] { 0L }); // Destroy in-memory state to prove it comes back from disk.
            Check(session.ContinueGame(), "Cargar minerales desde disco");
            Approach(copper);
            Check(!copper.TryMine(diver) && inventory.Count(copper.mineral) == 3, "Cargar no reinicia regeneración");
            restoreDeposit.Invoke(copper, new object[] { DateTime.UtcNow.AddSeconds(-1).Ticks });
            Check(copper.TryMine(diver) && inventory.Count(copper.mineral) == 6, "Mineral vuelve tras su enfriamiento");
            var iron = deposits.First(d => d.mineral == config.minerals[1]); Approach(iron);
            int capacity = inventory.capacity; inventory.capacity = inventory.Stacks.Count;
            Check(!iron.TryMine(diver) && iron.Available, "Mochila llena no consume depósito"); inventory.capacity = capacity;
            diver.LeaveVehicle(iron.transform.position + Vector3.right * 12); Physics.SyncTransforms();
            Check(!iron.TryMine(diver), "Minería exige cercanía");
            session.OpenShop(); var first = config.recipes[0];
            int money = session.Economy.Money, ore = inventory.Count(copper.mineral);
            Check(!session.Economy.Craft(first) && session.Economy.Money == money && inventory.Count(copper.mineral) == ore, "Material insuficiente no cobra ni consume");
            inventory.TryAdd(copper.mineral, 3); Fund(0);
            Check(!session.Economy.Craft(first) && inventory.Count(copper.mineral) == 9, "Dinero insuficiente no consume minerales");
            Fund(1000);
            Check(!session.Economy.Craft(config.recipes[2]), "No saltar mejoras intermedias");
            Check(session.Economy.Craft(first) && session.CraftedUpgrades == 1 && inventory.Count(copper.mineral) == 1
                && session.Economy.Money == 1000 - first.price && diver.SafeDepth == first.upgrade.unlockedDepth, "Fabricar descuenta costes exactos y aplica equipo");
            money = session.Economy.Money;
            Check(!session.Economy.Craft(first) && session.Economy.Money == money, "Mejora única sin doble cobro");
            inventory.TryAdd(copper.mineral, 6); ore = inventory.Count(copper.mineral);
            Check(!session.Economy.Craft(config.recipes[1]) && inventory.Count(copper.mineral) == ore && session.Economy.Money == money,
                "Receta con varios minerales no consume el primero cuando falta el segundo");
            for (int i = 1; i < config.recipes.Length; i++)
            {
                var recipe = config.recipes[i]; Fund(2000);
                foreach (var ingredient in recipe.ingredients) inventory.TryAdd(ingredient.item, ingredient.amount);
                Check(session.Economy.Craft(recipe) && diver.SafeDepth == recipe.upgrade.unlockedDepth, "Fabricar mejora " + i);
            }
            Check(session.Complete && session.CraftedUpgrades == 3 && diver.SafeDepth >= 650, "Economía y fabricación permiten alcanzar fondo profundo");
            var saved = Capture();
            Check(session.ReturnToMenu() && session.ContinueGame() && JsonUtility.ToJson(Capture()) == JsonUtility.ToJson(saved), "Carga v2 conserva costes, materiales, mejoras y enfriamiento");
            var invalid = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(saved)); invalid.equipmentUpgrades = new[] { config.zones[2].id };
            Check(!(bool)validate.Invoke(session, new object[] { invalid }), "Rechazar salto de mejoras en archivo");
            invalid = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(saved)); invalid.minerals[0].readyAtUtcTicks = -1;
            Check(!(bool)validate.Invoke(session, new object[] { invalid }), "Rechazar fecha mineral inválida");
            invalid = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(saved)); invalid.minerals[0].id = "desconocido";
            Check(!(bool)validate.Invoke(session, new object[] { invalid }), "Rechazar depósito desconocido");
            var legacy = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(saved)); legacy.version = 1;
            legacy.equipmentUpgrades = null; legacy.minerals = null; legacy.treasures = config.zones.Select(z => z.id).ToArray();
            legacy.discoveredZones = legacy.treasures.ToArray();
            legacy.puzzles = config.zones.Select(z => new SavedPuzzle { id = z.id, turns = new int[z.stonePuzzle.Count], solved = true }).ToArray();
            Check(store.TrySave(legacy, out _), "Guardado v1 histórico válido");
            apply.Invoke(session, new object[] { legacy });
            Check(session.CraftedUpgrades == 3 && diver.SafeDepth == 650 && session.Economy.Money == legacy.money, "Migrar mejoras y dinero v1 sin exigir recetas otra vez");
            var partial = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(legacy));
            partial.treasures = new[] { config.zones[0].id };
            for (int i = 1; i < partial.puzzles.Length; i++) { partial.puzzles[i].solved = false; partial.puzzles[i].turns = (int[])config.zones[i].stonePuzzle.startingTurns.Clone(); }
            apply.Invoke(session, new object[] { partial });
            Check(session.CraftedUpgrades == 1 && diver.SafeDepth == config.zones[0].unlockedDepth && !session.HasUpgrade(config.zones[1]),
                "Migración parcial conserva solo las mejoras ganadas");
            apply.Invoke(session, new object[] { legacy });
            Check(session.SaveNow() && store.TryLoad(out var migrated, out _) && migrated.version == 2 && migrated.equipmentUpgrades.Length == 3
                && migrated.puzzles.Length == 0, "Migración escribe v2 sin progreso de puzles");
            Debug.Log("LA_FOSA_ECONOMY_TESTS_OK · " + checks + " comprobaciones de minería, fabricación y migración, sin Play Mode.");
        }
    }
}
