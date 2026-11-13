using UnityEditor;

namespace LaFosa.Editor
{
    public static class CityProgressionSetup
    {
        public static void Ensure(GameConfig config)
        {
            var copper = config.minerals[0]; var iron = config.minerals[1]; var crystal = config.minerals[2];
            foreach (var offer in config.shop.offers)
            {
                offer.shop = offer.gear == GearKind.MiningPick ? ShopKind.Mining
                    : offer.gear == GearKind.Boat || offer.gear == GearKind.JetSki ? ShopKind.Construction
                    : offer.gear == GearKind.OxygenTank || offer.gear == GearKind.Fins ? ShopKind.Diving : ShopKind.Fishing;
                // Initial tools stay attainable before the first mining trip.
                if (offer.ingredients == null || offer.ingredients.Length == 0)
                {
                    if (offer.gear == GearKind.ReinforcedHarpoon) offer.ingredients = new[] { new CraftIngredient(iron, 6) };
                    if (offer.gear == GearKind.Net) offer.ingredients = new[] { new CraftIngredient(copper, 6) };
                    if (offer.gear == GearKind.OxygenTank) offer.ingredients = new[] { new CraftIngredient(copper, 4) };
                    if (offer.gear == GearKind.Fins) offer.ingredients = new[] { new CraftIngredient(copper, 3) };
                    if (offer.gear == GearKind.Boat) offer.ingredients = new[] { new CraftIngredient(copper, 8), new CraftIngredient(iron, 6) };
                    if (offer.gear == GearKind.JetSki) offer.ingredients = new[] { new CraftIngredient(iron, 12), new CraftIngredient(crystal, 4) };
                }
            }
            if (config.infrastructure == null || config.infrastructure.Length == 0)
                config.infrastructure = new[]
                {
                    new InfrastructureOffer { title = "1 · Ampliar el muelle", price = 180,
                        description = "Prolonga el muelle 12 m y prepara el primer puesto de pesca.", ingredients = new[] { new CraftIngredient(copper, 12) } },
                    new InfrastructureOffer { title = "2 · Primera zona de pesca", price = 280,
                        description = "Red automática: 1 pez cada 60 s de juego activo. Guarda hasta 10; recoge con E.", ingredients = new[] { new CraftIngredient(copper, 10), new CraftIngredient(iron, 6) } },
                    new InfrastructureOffer { title = "3 · Muelle pesquero", price = 450,
                        description = "Otros 12 m de muelle y una segunda zona automática independiente.", ingredients = new[] { new CraftIngredient(copper, 8), new CraftIngredient(iron, 18) } },
                    new InfrastructureOffer { title = "4 · Base sobre la fosa", price = 900,
                        description = "Plataforma junto al conducto de descenso y enlace ciudad ↔ base.", ingredients = new[] { new CraftIngredient(iron, 24), new CraftIngredient(crystal, 12) } },
                    new InfrastructureOffer { title = "5 · Descensor rápido", price = 700,
                        description = "Desde la base: baja a cotas exploradas y seguras para tu equipo. Sin coste por viaje.", ingredients = new[] { new CraftIngredient(copper, 12), new CraftIngredient(crystal, 18) } }
                };
            int[] prices = { 8, 14, 26 };
            for (int i = 0; i < config.minerals.Length; i++)
            { config.minerals[i].salePrice = prices[i]; EditorUtility.SetDirty(config.minerals[i]); }
        }
    }
}
