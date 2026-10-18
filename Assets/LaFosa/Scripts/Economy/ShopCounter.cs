using UnityEngine;

namespace LaFosa
{
    public sealed class ShopCounter : Interactable
    {
        public override string Prompt => "Tienda Don Lucho · vender pesca / comprar equipo";
        public override void Interact(DiverController diver)
        {
            if (Vector3.Distance(diver.View.transform.position, transform.position) > 5) return;
            diver.Session.OpenShop();
        }
    }
}
