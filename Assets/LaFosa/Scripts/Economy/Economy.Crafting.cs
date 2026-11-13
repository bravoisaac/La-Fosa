using System;

namespace LaFosa
{
    public sealed partial class Economy
    {
        public string CraftRequirement(CraftingRecipe recipe)
        {
            if (recipe == null || Array.IndexOf(session.Config.recipes, recipe) < 0 || recipe.price <= 0 || recipe.upgrade == null) return "Receta no disponible";
            if (session.HasUpgrade(recipe.upgrade)) return "FABRICADO";
            int index = Array.IndexOf(session.Config.recipes, recipe);
            if (index > 0 && !session.HasUpgrade(session.Config.recipes[index - 1].upgrade)) return "Fabrica el equipo anterior";
            if (!session.Diver.Inventory.HasIngredients(recipe.ingredients)) return "Faltan minerales";
            if (Money < recipe.price) return "Faltan $" + (recipe.price - Money);
            return "";
        }
        public bool Craft(CraftingRecipe recipe)
        {
            if (!session.ShopOpen || session.CurrentShop != ShopKind.Diving) return Reject("Fabrica tu equipo en Marea · Buceo.");
            string requirement = CraftRequirement(recipe);
            if (requirement.Length > 0) return Reject(requirement);
            // All preconditions are checked before consuming; a failed recipe spends nothing.
            if (!session.Diver.Inventory.TryConsume(recipe.ingredients)) return Reject("Los materiales cambiaron. Revisa la mochila.");
            Money -= recipe.price;
            session.CompleteCraft(recipe.upgrade);
            session.RecordExpeditionEvent("craft", recipe.upgrade.id);
            Feedback = "Fabricaste " + recipe.title + ". Equipo mejorado para " + recipe.upgrade.unlockedDepth + " m.";
            session.SaveNow(); return true;
        }
    }
}
