using UnityEngine;

namespace LaFosa
{
    public sealed class Pickup : Interactable
    {
        public ItemDefinition item;
        public override string Prompt => "Recoger " + item.displayName;
        public override void Interact(DiverController diver)
        {
            if (!diver.Inventory.TryAdd(item)) { diver.Session.Notify("Mochila llena. Libera una pila con TAB."); return; }
            diver.Session.Notify("+ " + item.displayName);
            Destroy(gameObject);
        }
    }
}
