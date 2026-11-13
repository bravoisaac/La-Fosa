using UnityEngine;

namespace LaFosa
{
    public sealed partial class GameSession : MonoBehaviour
    {
        public GameConfig Config { get; private set; }
        public DiverController Diver { get; set; }
        public bool Intro { get; private set; } = true;
        public bool InventoryOpen { get; private set; }
        public bool ShopOpen { get; private set; }
        public ShopKind CurrentShop { get; private set; }
        public Economy Economy { get; private set; }
        public SurfaceVehicle Boat { get; set; }
        public SurfaceVehicle JetSki { get; set; }
        public bool Paused { get; private set; }
        public bool Blocked => Intro || InventoryOpen || Paused || ShopOpen || CargoOpen || DeveloperPanelOpen;
        public int Treasures { get; private set; }
        public int CraftedUpgrades => Treasures;
        public event System.Action ProgressChanged;
        internal void RefreshRoutes() { ProgressChanged?.Invoke(); }
        public string Message { get; private set; }
        public float MessageUntil { get; private set; }
        public bool Complete => Treasures >= Config.zones.Length;
        public bool IsZoneUnlocked(DiveZoneDefinition zone)
        {
            // Equipment progression only. Swimming is limited by air consumption.
            int index = System.Array.IndexOf(Config.zones, zone);
            return index >= 0 && (index == 0 || IsTreasureClaimed(Config.zones[index - 1])) &&
                Diver != null && Diver.SafeDepth >= zone.requiredSafeDepth && Diver.MaxOxygen >= zone.requiredOxygen;
        }
        public string ZoneRequirement(DiveZoneDefinition zone)
        {
            int index = System.Array.IndexOf(Config.zones, zone);
            if (index < 0) return "Ruta no disponible.";
            if (index > 0 && !IsTreasureClaimed(Config.zones[index - 1]))
                return "Fabrica la mejora anterior en la caleta.";
            if (Diver.SafeDepth < zone.requiredSafeDepth || Diver.MaxOxygen < zone.requiredOxygen)
                return "Equipo necesario: " + zone.requiredSafeDepth + " m y " + zone.requiredOxygen + " u de aire.";
            return "Equipo preparado: " + zone.requiredSafeDepth + " m / " + zone.requiredOxygen + " u. " + zone.returnHint;
        }
        public Vector3 NextWaypoint(out string label)
        {
            int index = Mathf.Min(Treasures, Config.zones.Length - 1);
            var zone = Config.zones[index];
            label = "ZONA DE MINERALES";
            return zone.stationPosition + Vector3.up * 3;
        }
        public void Initialize(GameConfig config)
        {
            Config = config;
            Economy = gameObject.AddComponent<Economy>(); Economy.Initialize(this);
            BoatCargo = gameObject.AddComponent<Inventory>(); BoatCargo.capacity = BoatCargoSlots;
            ApplyPause();
        }
        public void OpenShop(ShopKind shop = ShopKind.Fishing)
        {
            if (Intro || Config.shop == null || Diver.Riding != null) return;
            if (!ShopOpen) RecordExpeditionEvent("shop");
            CurrentShop = shop;
            InventoryOpen = false; Paused = false; ShopOpen = true; ApplyPause();
        }
        public void CloseShop() { ShopOpen = false; ApplyPause(); }
        public void Begin() { NewGame(); }
        public void ToggleInventory() { if (ShopOpen || CargoOpen) return; InventoryOpen = !InventoryOpen; ApplyPause(); }
        public void Resume() { Paused = false; InventoryOpen = false; ShopOpen = false; CargoOpen = false; ApplyPause(); }
        public void Notify(string message) { Message = message; MessageUntil = Time.unscaledTime + 4; }
        public void Reward(DiveZoneDefinition zone)
        {
            if (!claimedZones.Add(zone.id)) return;
            discoveredZones.Add(zone.id);
            Treasures++;
            Diver.Upgrade(zone.unlockedDepth, zone.unlockedOxygen);
            ProgressChanged?.Invoke();
            Notify(Complete ? "Equipo abisal fabricado. Prepara la exploración de la fosa profunda." : "Equipo fabricado: ya puedes descender más.");
        }
        internal void CompleteCraft(DiveZoneDefinition zone) { Reward(zone); }
        void Update()
        {
            if (Intro) return;
            if (Input.GetKeyDown(KeyCode.F2)) { ToggleDeveloperMode(); return; }
            if (Input.GetKeyDown(KeyCode.F1) && DeveloperSession) { ToggleDeveloperPanel(); return; }
            if (DeveloperPanelOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) ToggleDeveloperPanel();
                return;
            }
            if (Input.GetKeyDown(KeyCode.B))
            { if (CargoOpen) CloseCargo(); else OpenCargo(); return; }
            if (CargoOpen && !CanAccessCargo) CloseCargo();
            if (!Blocked) UpdateExpeditionReturn();
            if (Input.GetKeyDown(KeyCode.Tab) && !Paused) ToggleInventory();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CargoOpen) CargoOpen = false;
                else if (ShopOpen) ShopOpen = false;
                else if (InventoryOpen) InventoryOpen = false;
                else Paused = !Paused;
                ApplyPause();
            }
        }
        void OnApplicationFocus(bool focused)
        {
            if (!focused && !Intro) { Paused = true; ApplyPause(); SaveNow(); WriteExpeditionReport(); }
        }
        void ApplyPause()
        {
            UpdateReportMode();
            Time.timeScale = Blocked ? 0 : 1;
            Cursor.lockState = Blocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Blocked;
        }
        void OnDestroy() { FinishExpeditionReport("destroyed"); Time.timeScale = 1; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }
}
