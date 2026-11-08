using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class EconomyProgressionSetup
    {
        public static void Ensure(GameConfig config)
        {
            if (config.minerals == null || config.minerals.Length == 0)
                config.minerals = new[] { Mineral("cobre", "Cobre", new Color(.87f, .47f, .2f)),
                    Mineral("hierro", "Hierro", new Color(.55f, .67f, .73f)), Mineral("cristal", "Cristal abisal", new Color(.65f, .35f, 1)) };
            if (config.recipes == null || config.recipes.Length == 0)
                config.recipes = new[]
                {
                    new CraftingRecipe { title = "Traje reforzado", upgrade = config.zones[0], price = 120,
                        ingredients = new[] { new CraftIngredient(config.minerals[0], 8) } },
                    new CraftingRecipe { title = "Equipo presurizado", upgrade = config.zones[1], price = 300,
                        ingredients = new[] { new CraftIngredient(config.minerals[0], 6), new CraftIngredient(config.minerals[1], 12) } },
                    new CraftingRecipe { title = "Traje abisal", upgrade = config.zones[2], price = 650,
                        ingredients = new[] { new CraftIngredient(config.minerals[1], 8), new CraftIngredient(config.minerals[2], 16) } }
                };
            if (!config.shop.offers.Any(o => o.gear == GearKind.MiningPick))
                config.shop.offers = config.shop.offers.Concat(new[] { new ShopOffer(GearKind.MiningPick, "Pico submarino",
                    "E junto a un depósito: extrae minerales para fabricar equipo.", 45) }).ToArray();
            EditorUtility.SetDirty(config.shop); EditorUtility.SetDirty(config);
        }
        static ItemDefinition Mineral(string id, string title, Color color)
        {
            string path = "Assets/LaFosa/Data/" + id + ".asset";
            var item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
            if (item != null) return item;
            item = ScriptableObject.CreateInstance<ItemDefinition>(); item.id = id; item.displayName = title;
            item.description = "Mineral para fabricar equipo en la caleta."; item.maxStack = 30; item.color = color;
            AssetDatabase.CreateAsset(item, path); return item;
        }
    }
}
