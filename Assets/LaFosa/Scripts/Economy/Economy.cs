using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaFosa
{
    public sealed class Economy : MonoBehaviour
    {
        public int Money { get; private set; }
        public string Feedback { get; private set; } = "Trae tu pesca: cada especie tiene su propio valor.";
        readonly HashSet<GearKind> owned = new HashSet<GearKind>();
        GameSession session;
        public void Initialize(GameSession game) { session = game; }
        public bool Has(GearKind gear) => owned.Contains(gear);
        internal void Restore(int money, string[] gear)
        {
            Money = money; owned.Clear();
            foreach (string id in gear) owned.Add((GearKind)Enum.Parse(typeof(GearKind), id));
            Feedback = "Trae tu pesca: cada especie tiene su propio valor.";
        }
        public bool MeetsRequirement(GearKind gear) => (gear != GearKind.ReinforcedHarpoon && gear != GearKind.Net) || Has(GearKind.Trident);
        public int FishCount => session.Diver.Inventory.FishCount;
        public long SaleValue => session.Diver.Inventory.FishValue;

        public bool SellFish()
        {
            if (!session.ShopOpen) return false;
            int count = FishCount;
            long value = SaleValue;
            if (count == 0) return Reject("No tienes peces para vender. Acércate a un pez y atrápalo con las manos.");
            if (value <= 0 || value > int.MaxValue - Money) return Reject("No se puede completar esta venta.");
            if (!session.Diver.Inventory.TryRemoveAllFish(count, value)) return Reject("La pesca disponible cambió; inténtalo de nuevo.");
            Money += (int)value;
            session.RecordExpeditionEvent("sale", count + " fish / " + value);
            Feedback = "Vendiste " + count + " peces por $" + value + ". ¡Buena pesca!";
            session.SaveNow();
            return true;
        }
        public bool Buy(ShopOffer offer)
        {
            if (!session.ShopOpen) return false;
            if (offer == null || Array.IndexOf(session.Config.shop.offers, offer) < 0 || offer.price <= 0)
                return Reject("Artículo no disponible.");
            if (Has(offer.gear)) return Reject("Ya tienes este equipo. No necesitas comprarlo otra vez.");
            if (!MeetsRequirement(offer.gear)) return Reject("Primero compra el tridente: es tu siguiente nivel después de las manos.");
            if (Money < offer.price) return Reject("Te faltan $" + (offer.price - Money) + ". Vende más peces.");
            if ((offer.gear == GearKind.Boat && session.Boat == null) || (offer.gear == GearKind.JetSki && session.JetSki == null))
                return Reject("No hay un amarre disponible para esta embarcación.");
            Money -= offer.price;
            owned.Add(offer.gear);
            if (offer.gear == GearKind.Trident) session.RecordExpeditionEvent("trident", offer.price.ToString(), true);
            if (offer.gear == GearKind.Trident) session.Diver.GetComponent<HarpoonGun>()?.SelectTool(1);
            if (offer.gear == GearKind.OxygenTank) session.Diver.ApplyOxygenTank(session.Config.shop.oxygenBonus);
            if (offer.gear == GearKind.Boat) session.Boat.Deliver();
            if (offer.gear == GearKind.JetSki) session.JetSki.Deliver();
            session.RefreshRoutes();
            Feedback = "Compraste " + offer.title + ". " + (offer.gear == GearKind.Boat || offer.gear == GearKind.JetSki ? "Te espera junto al muelle." : "Disponible en tu equipo.");
            session.SaveNow();
            return true;
        }
        bool Reject(string message) { Feedback = message; return false; }
    }
}
