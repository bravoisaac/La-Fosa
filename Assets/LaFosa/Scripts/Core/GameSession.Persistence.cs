using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class GameSession
    {
        readonly HashSet<string> claimedZones = new HashSet<string>();
        readonly HashSet<string> discoveredZones = new HashSet<string>();
        readonly Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>();
        StoneCircuitPuzzle[] puzzles;
        TreasureChest[] chests;
        Pickup[] pickups;
        SavedVehicle[] initialVehicles;
        SaveStore saveStore;
        bool saveDirty, restoring;
        bool wasUnderwater;
        float nextPeriodicSave;
        public bool CanContinue { get; private set; }
        public bool HasSaveFiles => saveStore != null && saveStore.HasFiles;
        public string SaveStatus { get; private set; }
        public bool SaveFailed { get; private set; }
        public int DiscoveredZoneCount => discoveredZones.Count;
        public bool IsTreasureClaimed(DiveZoneDefinition zone) => claimedZones.Contains(zone.id);
        internal SaveStore Store => saveStore;

        public void InitializePersistence()
        {
            puzzles = GetComponentsInChildren<StoneCircuitPuzzle>(true);
            chests = GetComponentsInChildren<TreasureChest>(true);
            pickups = GetComponentsInChildren<Pickup>(true);
            AddItem(Config.fish); AddItem(Config.shell);
            foreach (var fish in Config.fishSpecies) AddItem(fish.item);
            foreach (var zone in Config.zones) AddItem(zone.treasure);
            if (Config.zones.Any(z => string.IsNullOrWhiteSpace(z.id)) || Config.zones.Select(z => z.id).Distinct().Count() != Config.zones.Length)
                throw new InvalidOperationException("Cada zona necesita un ID estable y único.");
            if (puzzles.Length != Config.zones.Length || chests.Length != Config.zones.Length ||
                puzzles.Select(p => p.zone.id).Distinct().Count() != puzzles.Length ||
                pickups.Any(p => string.IsNullOrWhiteSpace(p.id)) || pickups.Select(p => p.id).Distinct().Count() != pickups.Length)
                throw new InvalidOperationException("IDs de mecanismos o recogibles incompletos o duplicados.");
            initialVehicles = CaptureVehicles();
            string path = Path.Combine(Application.persistentDataPath, "expedition.json");
            var args = Environment.GetCommandLineArgs();
            if (Array.IndexOf(args, "--smoke-test") >= 0)
            {
                string folder = Array.IndexOf(args, "--save-restart-write") >= 0 || Array.IndexOf(args, "--save-restart-read") >= 0 ? "RestartSave" : "SaveTests-" + Guid.NewGuid().ToString("N");
                path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Verification", folder, "expedition.json"));
            }
            saveStore = new SaveStore(path, ValidateSave);
            RefreshSaveAvailability();
            Diver.Inventory.Changed += MarkSaveDirty;
            foreach (var puzzle in puzzles) puzzle.Changed += MarkSaveDirty;
        }
        void AddItem(ItemDefinition item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.id)) throw new InvalidOperationException("Objeto sin ID de guardado.");
            if (items.TryGetValue(item.id, out var existing) && existing != item) throw new InvalidOperationException("ID de objeto duplicado: " + item.id);
            items[item.id] = item;
        }
        void RefreshSaveAvailability()
        {
            CanContinue = saveStore.TryLoad(out _, out var message); SaveStatus = message;
        }
        public bool NewGame(bool confirmOverwrite = false)
        {
            if (!Intro || saveStore == null) return false;
            if (HasSaveFiles && !confirmOverwrite) { SaveStatus = "Confirma Nueva partida para reemplazar la expedición y su respaldo."; return false; }
            var fresh = new SaveData
            {
                version = 1, worldSeed = Config.seed, inventory = new SavedStack[0], gear = new string[0],
                treasures = new string[0], discoveredZones = new string[0], collectedPickups = new string[0],
                puzzles = puzzles.Select(p => new SavedPuzzle { id = p.zone.id, turns = (int[])p.Definition.startingTurns.Clone() }).ToArray(),
                vehicles = initialVehicles
            };
            if (!saveStore.TrySave(fresh, out var message, true)) { SaveStatus = message; SaveFailed = true; return false; }
            ApplySave(fresh); Intro = false; CanContinue = true; SaveFailed = false; SaveStatus = message; ApplyPause();
            Notify("Nueva expedición. Empiezas con las manos; vende dos peces para comprar el tridente.");
            return true;
        }
        public bool ContinueGame()
        {
            if (!Intro || saveStore == null) return false;
            if (!saveStore.TryLoad(out var data, out var message)) { CanContinue = false; SaveStatus = message; return false; }
            ApplySave(data); Intro = false; SaveStatus = message; SaveFailed = false; ApplyPause(); Notify(message); return true;
        }
        public bool ReturnToMenu()
        {
            if (!SaveNow()) return false;
            Intro = true; Paused = false; InventoryOpen = false; ShopOpen = false;
            ApplyPause(); RefreshSaveAvailability(); return true;
        }
        internal void ApplySave(SaveData data)
        {
            if (!ValidateSave(data)) throw new ArgumentException("Partida inválida.");
            restoring = true;
            try
            {
                if (Diver.Riding != null) Diver.LeaveVehicle(Config.spawn);
                claimedZones.Clear(); claimedZones.UnionWith(data.treasures); Treasures = claimedZones.Count;
                discoveredZones.Clear(); discoveredZones.UnionWith(data.discoveredZones);
                Economy.Restore(data.money, data.gear);
                Diver.Inventory.Restore(data.inventory.Select(s => new InventoryStack(items[s.id], s.count)).ToArray());
                foreach (var puzzle in puzzles)
                {
                    var saved = data.puzzles.First(p => p.id == puzzle.zone.id);
                    puzzle.Restore(saved.turns, saved.moves, saved.solved);
                }
                foreach (var chest in chests) chest.Restore(IsTreasureClaimed(chest.puzzle.zone));
                foreach (var pickup in pickups) pickup.gameObject.SetActive(!data.collectedPickups.Contains(pickup.id));
                foreach (var vehicle in new[] { Boat, JetSki })
                {
                    var saved = data.vehicles.First(v => v.id == vehicle.kind.ToString());
                    vehicle.Restore(Economy.Has(vehicle.kind), saved.position, saved.yaw);
                }
                Diver.RestoreEquipment(); Diver.GetComponent<HarpoonGun>().SelectTool(data.selectedTool);
                ProgressChanged?.Invoke();
                Paused = false; InventoryOpen = false; ShopOpen = false;
                wasUnderwater = false; nextPeriodicSave = Time.unscaledTime + 20;
                Physics.SyncTransforms();
            }
            finally { restoring = false; saveDirty = false; }
        }
        SavedVehicle[] CaptureVehicles() => new[] { Boat, JetSki }.Select(v => new SavedVehicle
            { id = v.kind.ToString(), position = v.transform.position, yaw = v.transform.eulerAngles.y }).ToArray();
        internal SaveData CaptureSave() => new SaveData
        {
            version = 1, worldSeed = Config.seed, money = Economy.Money, selectedTool = Diver.GetComponent<HarpoonGun>().SelectedTool,
            inventory = Diver.Inventory.Stacks.Select(s => new SavedStack { id = s.item.id, count = s.count }).ToArray(),
            gear = Enum.GetValues(typeof(GearKind)).Cast<GearKind>().Where(Economy.Has).Select(g => g.ToString()).ToArray(),
            treasures = claimedZones.OrderBy(id => id).ToArray(), discoveredZones = discoveredZones.OrderBy(id => id).ToArray(),
            collectedPickups = pickups.Where(p => !p.gameObject.activeSelf).Select(p => p.id).ToArray(),
            puzzles = puzzles.Select(p => new SavedPuzzle { id = p.zone.id, turns = Enumerable.Range(0, p.Definition.Count).Select(p.Turns).ToArray(), moves = p.Moves, solved = p.Solved }).ToArray(),
            vehicles = CaptureVehicles()
        };
        public void MarkSaveDirty() { if (!restoring && !Intro) saveDirty = true; }
        public bool SaveNow()
        {
            if (Intro || restoring || saveStore == null) return false;
            bool saved = saveStore.TrySave(CaptureSave(), out var message);
            SaveStatus = message; SaveFailed = !saved; saveDirty = false; nextPeriodicSave = Time.unscaledTime + 20;
            if (saved) CanContinue = true;
            return saved;
        }
        void LateUpdate()
        {
            if (Intro || saveStore == null) return;
            if (!Blocked)
            {
                foreach (var zone in Config.zones)
                    if (Vector3.Distance(Diver.transform.position, zone.stationPosition) < 18 && discoveredZones.Add(zone.id)) saveDirty = true;
                if (wasUnderwater && !Diver.Underwater) saveDirty = true;
                wasUnderwater = Diver.Underwater;
            }
            // End-of-frame saving keeps inventory + reward/transaction in one snapshot.
            if (saveDirty || Time.unscaledTime >= nextPeriodicSave) SaveNow();
        }
        void OnApplicationQuit() { SaveNow(); }
        void OnApplicationPause(bool paused) { if (paused) SaveNow(); }
    }
}
