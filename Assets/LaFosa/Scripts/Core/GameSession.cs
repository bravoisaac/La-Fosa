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
        public Economy Economy { get; private set; }
        public SurfaceVehicle Boat { get; set; }
        public SurfaceVehicle JetSki { get; set; }
        public bool Paused { get; private set; }
        public bool Blocked => Intro || InventoryOpen || Paused || ShopOpen;
        public int Treasures { get; private set; }
        public event System.Action ProgressChanged;
        internal void RefreshRoutes() { ProgressChanged?.Invoke(); }
        public string Message { get; private set; }
        public float MessageUntil { get; private set; }
        public bool Complete => Treasures >= Config.zones.Length;
        public bool IsZoneUnlocked(DiveZoneDefinition zone)
        {
            int index = System.Array.IndexOf(Config.zones, zone);
            return index >= 0 && (index == 0 || IsTreasureClaimed(Config.zones[index - 1])) &&
                Diver != null && Diver.SafeDepth >= zone.requiredSafeDepth && Diver.MaxOxygen >= zone.requiredOxygen;
        }
        public string ZoneRequirement(DiveZoneDefinition zone)
        {
            int index = System.Array.IndexOf(Config.zones, zone);
            if (index < 0) return "Ruta no disponible.";
            if (index > 0 && !IsTreasureClaimed(Config.zones[index - 1]))
                return "Abre el sello y recoge " + Config.zones[index - 1].treasure.displayName + ".";
            if (Diver.SafeDepth < zone.requiredSafeDepth || Diver.MaxOxygen < zone.requiredOxygen)
                return "Equipo necesario: " + zone.requiredSafeDepth + " m y " + zone.requiredOxygen + " s de aire.";
            return "Equipo preparado: " + zone.requiredSafeDepth + " m / " + zone.requiredOxygen + " s. " + zone.returnHint;
        }
        public Vector3 NextWaypoint(out string label)
        {
            int index = Mathf.Min(Treasures, Config.zones.Length - 1);
            var zone = Config.zones[index];
            bool approachGate = index > 0 && zone.hasPhysicalGate && Diver.transform.position.y > zone.entryPosition.y - 2;
            label = approachGate ? "ESCOTILLA DE DESCENSO" : "SANTUARIO";
            return approachGate ? zone.entryPosition + Vector3.up : zone.stationPosition + Vector3.up * 3;
        }
        public void Initialize(GameConfig config)
        {
            Config = config;
            Economy = gameObject.AddComponent<Economy>(); Economy.Initialize(this);
            ApplyPause();
        }
        public void OpenShop()
        {
            if (Intro || Config.shop == null || Diver.Riding != null) return;
            if (!ShopOpen) RecordExpeditionEvent("shop");
            InventoryOpen = false; Paused = false; ShopOpen = true; ApplyPause();
        }
        public void CloseShop() { ShopOpen = false; ApplyPause(); }
        public void Begin() { NewGame(); }
        public void ToggleInventory() { if (ShopOpen) return; InventoryOpen = !InventoryOpen; ApplyPause(); }
        public void Resume() { Paused = false; InventoryOpen = false; ShopOpen = false; ApplyPause(); }
        public void Notify(string message) { Message = message; MessageUntil = Time.unscaledTime + 4; }
        public void Reward(DiveZoneDefinition zone)
        {
            if (!claimedZones.Add(zone.id)) return;
            if (zone == Config.zones[0]) RecordExpeditionEvent("treasure", zone.id, true);
            discoveredZones.Add(zone.id);
            Treasures++;
            if (Treasures == 1) firstReturnPending = true;
            Diver.Upgrade(zone.unlockedDepth, zone.unlockedOxygen);
            ProgressChanged?.Invoke();
            Notify(Complete ? "¡Expedición completa! Has recuperado los tres tesoros del Pacífico." : "Tesoro recuperado. Equipo mejorado: ya puedes descender más.");
            SaveNow();
        }
        void Update()
        {
            if (Intro) return;
            if (!Blocked) UpdateExpeditionReturn();
            if (Input.GetKeyDown(KeyCode.Tab) && !Paused) ToggleInventory();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ShopOpen) ShopOpen = false;
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
