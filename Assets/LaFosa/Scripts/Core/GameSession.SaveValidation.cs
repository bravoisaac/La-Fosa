using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class GameSession
    {
        internal bool ValidateSave(SaveData data)
        {
            if (data == null || data.version != 1 || data.worldSeed != Config.seed || data.money < 0 ||
                data.inventory == null || data.inventory.Length > Config.inventorySlots ||
                data.puzzles == null || data.puzzles.Length != puzzles.Length ||
                data.vehicles == null || data.vehicles.Length != 2) return false;
            var zones = new HashSet<string>(Config.zones.Select(z => z.id));
            var gearIds = new HashSet<string>(Config.shop.offers.Select(o => o.gear.ToString()));
            if (!ValidIds(data.gear, gearIds) || !ValidIds(data.treasures, zones) || !ValidIds(data.discoveredZones, zones) ||
                !ValidIds(data.collectedPickups, new HashSet<string>(pickups.Select(p => p.id)))) return false;
            bool trident = data.gear.Contains(nameof(GearKind.Trident));
            if (!trident && (data.gear.Contains(nameof(GearKind.ReinforcedHarpoon)) || data.gear.Contains(nameof(GearKind.Net)))) return false;
            if (data.selectedTool < 0 || data.selectedTool > 3 ||
                (data.selectedTool == 1 && !trident) ||
                (data.selectedTool == 2 && !data.gear.Contains(nameof(GearKind.ReinforcedHarpoon))) ||
                (data.selectedTool == 3 && !data.gear.Contains(nameof(GearKind.Net)))) return false;
            foreach (var stack in data.inventory)
                if (stack == null || string.IsNullOrEmpty(stack.id) || !items.TryGetValue(stack.id, out var item) || stack.count < 1 || stack.count > Mathf.Max(1, item.maxStack)) return false;
            if (data.puzzles.Any(p => p == null) || !ValidIds(data.puzzles.Select(p => p.id).ToArray(), zones)) return false;
            for (int i = 0; i < Config.zones.Length; i++)
            {
                var zone = Config.zones[i];
                var puzzle = data.puzzles.FirstOrDefault(p => p.id == zone.id);
                if (puzzle == null || puzzle.turns == null || puzzle.turns.Length != zone.stonePuzzle.Count ||
                    puzzle.turns.Any(t => t < 0 || t > 3) || puzzle.moves < 0) return false;
                bool claimed = data.treasures.Contains(zone.id);
                if (claimed && (!puzzle.solved || !data.discoveredZones.Contains(zone.id))) return false;
                if ((claimed || puzzle.solved) && i > 0 && !data.treasures.Contains(Config.zones[i - 1].id)) return false;
                if (puzzle.solved && !CircuitComplete(zone.stonePuzzle, puzzle.turns)) return false;
                long treasureCount = data.inventory.Where(s => s.id == zone.treasure.id).Sum(s => (long)s.count);
                if (treasureCount > (claimed ? 1 : 0)) return false;
            }
            if (data.vehicles.Any(v => v == null) || !ValidIds(data.vehicles.Select(v => v.id).ToArray(), new HashSet<string> { nameof(GearKind.Boat), nameof(GearKind.JetSki) })) return false;
            foreach (var vehicle in data.vehicles)
                if (!Finite(vehicle.position.x) || !Finite(vehicle.position.y) || !Finite(vehicle.position.z) || !Finite(vehicle.yaw) ||
                    Mathf.Abs(vehicle.position.x) > 100 || vehicle.position.z < 10 || vehicle.position.z > 175 ||
                    Mathf.Abs(vehicle.position.y - Config.waterLevel) > .01f || vehicle.yaw < 0 || vehicle.yaw > 360) return false;
            return true;
        }
        static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        static bool ValidIds(string[] ids, HashSet<string> allowed) => ids != null && ids.Length <= allowed.Count &&
            ids.All(id => id != null && allowed.Contains(id)) && ids.Distinct().Count() == ids.Length;
        static bool CircuitComplete(StonePuzzleDefinition definition, int[] turns)
        {
            var masks = definition.SolutionMasks();
            for (int i = 0; i < masks.Length; i++) masks[i] = ((masks[i] << turns[i]) | (masks[i] >> (4 - turns[i]))) & 15;
            if ((masks[definition.Start] & (1 << definition.inletDirection)) == 0 || (masks[definition.End] & (1 << definition.outletDirection)) == 0) return false;
            var visited = new HashSet<int>(); var pending = new Queue<int>(); pending.Enqueue(definition.Start);
            while (pending.Count > 0)
            {
                int tile = pending.Dequeue(); if (!visited.Add(tile)) continue;
                for (int side = 0; side < 4; side++)
                {
                    if ((masks[tile] & (1 << side)) == 0) continue;
                    int next = definition.Neighbor(tile, side);
                    if (next < 0)
                    {
                        if (!(tile == definition.Start && side == definition.inletDirection) && !(tile == definition.End && side == definition.outletDirection)) return false;
                    }
                    else
                    {
                        if ((masks[next] & (1 << ((side + 2) % 4))) == 0) return false;
                        if (!visited.Contains(next)) pending.Enqueue(next);
                    }
                }
            }
            return visited.Count == masks.Length;
        }
    }
}
