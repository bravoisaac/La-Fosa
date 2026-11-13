using System;
using UnityEngine;

namespace LaFosa
{
    public enum GearKind { ReinforcedHarpoon, OxygenTank, Fins, Net, Trident, Boat, JetSki, MiningPick }
    public enum ShopKind { Fishing, Mining, Diving, Construction, Transit }

    [Serializable]
    public sealed class ShopOffer
    {
        public GearKind gear;
        public string title;
        [TextArea] public string description;
        [Min(1)] public int price;
        public ShopKind shop;
        public CraftIngredient[] ingredients = new CraftIngredient[0];
        public ShopOffer(GearKind kind, string name, string detail, int cost)
        { gear = kind; title = name; description = detail; price = cost; }
    }

    [CreateAssetMenu(menuName = "La Fosa/Catálogo de tienda")]
    public sealed class ShopCatalog : ScriptableObject
    {
        [Min(1)] public int fishValue = 30;
        [Min(1)] public float oxygenBonus = 60;
        [Min(1)] public float finSpeedMultiplier = 1.35f;
        public ShopOffer[] offers;
    }
}
