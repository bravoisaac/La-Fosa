using UnityEngine;

namespace LaFosa
{
    public sealed class Pickup : Interactable
    {
        public ItemDefinition item;
        public string id;
        public override string Prompt => "Recoger " + item.displayName;
        public override void Interact(DiverController diver)
        {
            if (!gameObject.activeSelf) return;
            if (!diver.Inventory.TryAdd(item)) { diver.Session.Notify("Mochila llena. Libera una pila con TAB."); return; }
            diver.Session.Notify("+ " + item.displayName);
            gameObject.SetActive(false);
            diver.Session.MarkSaveDirty();
        }
    }
}
