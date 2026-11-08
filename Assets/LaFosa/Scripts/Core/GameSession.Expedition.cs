using System.Linq;
using UnityEngine;

namespace LaFosa
{
    // Earlier values retained for compatibility with diagnostic tooling; puzzle steps are no longer used.
    public enum ExpeditionStep { Fishing, Sell, BuyTrident, Puzzle, ActivateSeal, ClaimTreasure, Return, Descend, Complete, Surface, FreeSpace, BuyPick, Mine, Craft }
    public readonly struct ExpeditionObjective
    {
        public readonly ExpeditionStep Step;
        public readonly string Title, Hint, Marker;
        public readonly Vector3 Destination;
        public ExpeditionObjective(ExpeditionStep step, string title, string hint, string marker, Vector3 destination)
        { Step = step; Title = title; Hint = hint; Marker = marker; Destination = destination; }
    }
    public sealed partial class GameSession
    {
        public static readonly Vector3 CabinWaypoint = new Vector3(-12, 4.3f, -18.8f);
        public Vector3 FishingWaypoint => new Vector3(0, Config.waterLevel - 1.8f, 14);
        bool firstReturnPending;
        FishAgent[] guideFish;
        public int TridentPrice => Config.shop.offers.First(o => o.gear == GearKind.Trident).price;
        public int MiningPickPrice => Config.shop.offers.First(o => o.gear == GearKind.MiningPick).price;
        internal void MarkMineralTrip() { firstReturnPending = true; }
        void UpdateExpeditionReturn()
        {
            if (firstReturnPending && !Diver.Underwater && Vector3.Distance(Diver.transform.position, Config.spawn) < 12)
            {
                ReportExpeditionReturn(); firstReturnPending = false;
                Notify("Minerales recuperados. Revisa FABRICAR en el taller de la caleta.");
            }
        }
        public ExpeditionObjective ExpeditionObjective()
        {
            if (Diver.NeedsAirReturn)
                return new ExpeditionObjective(ExpeditionStep.Surface, "Recupera aire", "Busca el conducto central y asciende con ESPACIO. Reserva aire para volver; R te rescata conservando la mochila.", "", Vector3.zero);
            if (Complete)
                return new ExpeditionObjective(ExpeditionStep.Complete, "Explora la fosa profunda", "Equipo abisal fabricado. Explora hasta 650 m, pesca y recoge cristal del fondo. Vigila presión y reserva de regreso.", "", Vector3.zero);
            bool hasPick = Economy.Has(GearKind.MiningPick);
            var recipe = Config.recipes[CraftedUpgrades];
            int price = hasPick ? recipe.price : MiningPickPrice;
            if (hasPick && !Diver.Inventory.HasIngredients(recipe.ingredients))
            {
                if (Diver.Inventory.Stacks.Count >= Diver.Inventory.capacity)
                    return new ExpeditionObjective(ExpeditionStep.FreeSpace, "Prepara espacio para mineral", "Vende pesca en la caleta o descarta una pila desde TAB. El pico extrae tres unidades por depósito.", "", CabinWaypoint);
                string materials = string.Join(" · ", recipe.ingredients.Select(i => i.item.displayName + " " + Diver.Inventory.Count(i.item) + "/" + i.amount));
                return new ExpeditionObjective(ExpeditionStep.Mine, "Minerales para " + recipe.title,
                    materials + ". Cobre a 24 m, hierro a 52 m y cristal a 88 m. Acércate a las vetas brillantes y pulsa E. Después fabrica en la caleta ($" + recipe.price + ").", "", recipe.upgrade.stationPosition);
            }
            if (Economy.Money >= price)
                return new ExpeditionObjective(hasPick ? ExpeditionStep.Craft : ExpeditionStep.BuyPick,
                    hasPick ? "Fabrica tu siguiente equipo" : "Compra el pico submarino",
                    hasPick ? "Tienes dinero y minerales. E en Don Lucho → FABRICAR → " + recipe.title + ". La mejora se equipa automáticamente."
                    : "E en Don Lucho → TIENDA → Pico submarino ($" + MiningPickPrice + "). El tridente es opcional para pescar a mayor distancia.", "", CabinWaypoint);
            if (Economy.FishCount > 0 && (Economy.Money + Economy.SaleValue >= price || Diver.Inventory.Stacks.Count >= Diver.Inventory.capacity))
                return new ExpeditionObjective(ExpeditionStep.Sell, "Vende tu pesca", "Llevas $" + Economy.SaleValue + " en peces. E en Don Lucho y VENDER. Los minerales se reservan para fabricar.", "", CabinWaypoint);
            if (Diver.Inventory.Stacks.Count >= Diver.Inventory.capacity)
                return new ExpeditionObjective(ExpeditionStep.FreeSpace, "Haz espacio para pescar", "TAB abre la mochila. Conserva minerales para la receta; vende peces o descarta una pila.", "", CabinWaypoint);
            return new ExpeditionObjective(ExpeditionStep.Fishing, "Pesca para financiar tu equipo",
                "CLIC junto a peces de la orilla: manos a 1,6 m. Faltan $" + Mathf.Max(0, price - Economy.Money - (int)Economy.SaleValue) + " en pesca para " + (hasPick ? recipe.title : "el pico") + ". Vende en la caleta; ESPACIO sube a respirar.", "", FishingWaypoint);
        }
    }
}