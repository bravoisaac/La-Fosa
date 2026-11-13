using System;

namespace LaFosa
{
    public sealed partial class GameSession
    {
        public const int BoatCargoSlots = 24;
        public Inventory BoatCargo { get; private set; }
        public bool CargoOpen { get; private set; }
        public bool CanAccessCargo => !Intro && Diver != null && Boat != null && Boat.gameObject.activeInHierarchy &&
            Economy.Has(GearKind.Boat) && Diver.Riding == Boat && Boat.Passenger == Diver;
        public bool IsCargoItem(ItemDefinition item) => item != null &&
            (item.isFish || Array.IndexOf(Config.minerals, item) >= 0);

        public bool OpenCargo()
        {
            if (Blocked || !CanAccessCargo) return false;
            CargoOpen = true; ApplyPause(); return true;
        }
        public void CloseCargo() { CargoOpen = false; ApplyPause(); }
        public bool CanTransferCargo(bool deposit, ItemDefinition item, int amount)
        {
            if (!CargoOpen || !CanAccessCargo || Paused || DeveloperPanelOpen || ShopOpen || InventoryOpen ||
                !IsCargoItem(item) || amount <= 0) return false;
            var source = deposit ? Diver.Inventory : BoatCargo;
            var destination = deposit ? BoatCargo : Diver.Inventory;
            return source.Count(item) >= amount && destination.CanReceive(item, amount);
        }
        public bool TransferCargo(bool deposit, ItemDefinition item, int amount)
        {
            if (!CanTransferCargo(deposit, item, amount))
            { Notify("Transferencia no disponible: revisa espacio, cantidad y acceso a la lancha."); return false; }
            var source = deposit ? Diver.Inventory : BoatCargo;
            var destination = deposit ? BoatCargo : Diver.Inventory;
            return source.TryTransferTo(destination, item, amount);
        }
    }
}
