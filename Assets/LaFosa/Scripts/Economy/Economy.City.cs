using System;

namespace LaFosa
{
    public sealed partial class Economy
    {
        public string PurchaseRequirement(ShopOffer offer)
        {
            if (Has(offer.gear)) return "COMPRADO";
            if (!MeetsRequirement(offer.gear)) return "Tridente primero";
            if (offer.ingredients != null && offer.ingredients.Length > 0 && !session.Diver.Inventory.HasIngredients(offer.ingredients)) return "Faltan materiales";
            return Money < offer.price ? "Faltan $" + (offer.price - Money) : "";
        }
        public bool SellMineral(ItemDefinition item, int amount)
        {
            if (!session.ShopOpen || session.CurrentShop != ShopKind.Mining) return Reject("Vende minerales en La Veta.");
            if (item == null || Array.IndexOf(session.Config.minerals, item) < 0 || amount <= 0) return Reject("Venta no disponible.");
            long value = (long)item.salePrice * amount;
            if (value <= 0 || value > int.MaxValue - Money) return Reject("No se puede completar esta venta.");
            if (!session.Diver.Inventory.TryRemove(item, amount)) return Reject("No tienes esa cantidad de mineral.");
            Money += (int)value;
            Feedback = "Vendiste " + amount + " " + item.displayName + " por $" + value + ".";
            session.RecordExpeditionEvent("mineral_sale", item.id + " / " + amount); session.SaveNow(); return true;
        }
        public string ConstructionRequirement(int index)
        {
            if (index < 0 || index >= session.Config.infrastructure.Length) return "Obra no disponible";
            if (index < session.InfrastructureLevel) return "CONSTRUIDO";
            if (index != session.InfrastructureLevel) return "Completa la obra anterior";
            var offer = session.Config.infrastructure[index];
            if (offer.price <= 0 || !session.Diver.Inventory.HasIngredients(offer.ingredients)) return "Faltan materiales";
            return Money < offer.price ? "Faltan $" + (offer.price - Money) : "";
        }
        public bool Construct(int index)
        {
            if (!session.ShopOpen || session.CurrentShop != ShopKind.Construction) return Reject("Encarga las obras en el Astillero Bruma.");
            string requirement = ConstructionRequirement(index);
            if (requirement.Length > 0) return Reject(requirement);
            var offer = session.Config.infrastructure[index];
            if (!session.Diver.Inventory.TryConsume(offer.ingredients)) return Reject("Los materiales cambiaron. Revisa la mochila.");
            Money -= offer.price;
            session.CompleteConstruction();
            Feedback = "Construido: " + offer.title + ". Visita el muelle para usarlo.";
            session.RecordExpeditionEvent("construction", (index + 1).ToString()); session.SaveNow(); return true;
        }
    }
}
