using System;
using UnityEngine;

namespace LaFosa
{
    [Serializable] public sealed class CraftIngredient
    {
        public ItemDefinition item;
        [Min(1)] public int amount;
        public CraftIngredient(ItemDefinition item, int amount) { this.item = item; this.amount = amount; }
    }
    [Serializable] public sealed class CraftingRecipe
    {
        public string title;
        public DiveZoneDefinition upgrade;
        [Min(1)] public int price;
        public CraftIngredient[] ingredients;
    }
}
