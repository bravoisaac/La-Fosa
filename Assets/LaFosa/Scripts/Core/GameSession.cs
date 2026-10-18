using UnityEngine;

namespace LaFosa
{
    public sealed class GameSession : MonoBehaviour
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
        public string Message { get; private set; }
        public float MessageUntil { get; private set; }
        public bool Complete => Treasures >= Config.zones.Length;
        public bool IsZoneUnlocked(DiveZoneDefinition zone)
        {
            int index = System.Array.IndexOf(Config.zones, zone);
            return index >= 0 && index <= Treasures;
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
            InventoryOpen = false; Paused = false; ShopOpen = true; ApplyPause();
        }
        public void CloseShop() { ShopOpen = false; ApplyPause(); }
        public void Begin() { Intro = false; ApplyPause(); }
        public void ToggleInventory() { if (ShopOpen) return; InventoryOpen = !InventoryOpen; ApplyPause(); }
        public void Resume() { Paused = false; InventoryOpen = false; ShopOpen = false; ApplyPause(); }
        public void Notify(string message) { Message = message; MessageUntil = Time.unscaledTime + 4; }
        public void Reward(DiveZoneDefinition zone)
        {
            Treasures++;
            Diver.Upgrade(zone.unlockedDepth, zone.unlockedOxygen);
            Notify(Complete ? "¡Expedición completa! Has recuperado los tres tesoros del Pacífico." : "Tesoro recuperado. Equipo mejorado: ya puedes descender más.");
        }
        void Update()
        {
            if (Intro) return;
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
            if (!focused && !Intro) { Paused = true; ApplyPause(); }
        }
        void ApplyPause()
        {
            Time.timeScale = Blocked ? 0 : 1;
            Cursor.lockState = Blocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Blocked;
        }
        void OnDestroy() { Time.timeScale = 1; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }
}
