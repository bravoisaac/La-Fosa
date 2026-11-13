using System;

namespace LaFosa
{
    [Serializable]
    public sealed class InfrastructureOffer
    {
        public string title, description;
        public int price;
        public CraftIngredient[] ingredients;
    }

    public static class CityShops
    {
        public static string Name(ShopKind shop) => shop == ShopKind.Fishing ? "DON LUCHO · PESCA"
            : shop == ShopKind.Mining ? "LA VETA · MINERÍA" : shop == ShopKind.Diving ? "MAREA · BUCEO"
            : shop == ShopKind.Construction ? "ASTILLERO BRUMA" : "ENLACE DE LA FOSA";
    }
}
