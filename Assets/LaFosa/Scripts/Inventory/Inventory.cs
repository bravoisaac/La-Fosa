using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaFosa
{
    [Serializable]
    public sealed class InventoryStack
    {
        public ItemDefinition item;
        public int count;
        public InventoryStack(ItemDefinition definition, int amount) { item = definition; count = amount; }
    }

    public sealed class Inventory : MonoBehaviour
    {
        public int capacity = 12;
        readonly List<InventoryStack> stacks = new List<InventoryStack>();
        public IReadOnlyList<InventoryStack> Stacks => stacks;
        public event Action Changed;

        public int FishCount { get { int n = 0; foreach (var s in stacks) if (s.item.isFish) n += s.count; return n; } }
        public long FishValue { get { long n = 0; foreach (var s in stacks) if (s.item.isFish) n += (long)s.count * s.item.salePrice; return n; } }
        public bool TryRemoveAllFish(int expectedCount, long expectedValue)
        {
            if (expectedCount <= 0 || FishCount != expectedCount || FishValue != expectedValue) return false;
            stacks.RemoveAll(s => s.item.isFish); Changed?.Invoke(); return true;
        }

        public int Count(ItemDefinition item)
        {
            int total = 0;
            foreach (var stack in stacks) if (stack.item == item) total += stack.count;
            return total;
        }
        public bool TryRemove(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0 || Count(item) < amount) return false;
            for (int i = stacks.Count - 1; i >= 0 && amount > 0; i--)
            {
                if (stacks[i].item != item) continue;
                int removed = Mathf.Min(amount, stacks[i].count);
                stacks[i].count -= removed; amount -= removed;
                if (stacks[i].count == 0) stacks.RemoveAt(i);
            }
            Changed?.Invoke();
            return true;
        }

        public bool TryAdd(ItemDefinition item, int amount = 1)
        {
            if (item == null || amount <= 0) return false;
            int limit = Mathf.Max(1, item.maxStack);
            long room = (long)Mathf.Max(0, capacity - stacks.Count) * limit;
            foreach (var stack in stacks)
                if (stack.item == item) room += Mathf.Max(0, limit - stack.count);
            if (room < amount) return false; // Atomic: never lose a partial reward.
            foreach (var stack in stacks)
            {
                if (stack.item != item) continue;
                int added = Mathf.Min(amount, limit - stack.count);
                stack.count += added;
                amount -= added;
            }
            while (amount > 0)
            {
                int added = Mathf.Min(amount, limit);
                stacks.Add(new InventoryStack(item, added));
                amount -= added;
            }
            Changed?.Invoke();
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= stacks.Count) return;
            stacks.RemoveAt(index);
            Changed?.Invoke();
        }
    }
}
