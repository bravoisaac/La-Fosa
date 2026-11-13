using UnityEngine;

namespace LaFosa
{
    public sealed class ShopCounter : Interactable
    {
        public ShopKind shop;
        public override string Prompt => CityShops.Name(shop) + " · E abrir comercio";
        public override void Interact(DiverController diver)
        {
            if (Vector3.Distance(diver.View.transform.position, transform.position) > 5) return;
            diver.Session.OpenShop(shop);
        }
    }
}
