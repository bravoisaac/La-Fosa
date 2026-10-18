using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaFosa
{
    public sealed class StoneCircuitPuzzle : MonoBehaviour
    {
        public DiveZoneDefinition zone;
        public StonePuzzleDefinition Definition => zone.stonePuzzle;
        public bool Solved { get; private set; }
        public bool Ready { get; private set; }
        public int PoweredCount { get; private set; }
        public int OpenConnections { get; private set; }
        public int Moves { get; private set; }
        public string Hint => Definition.clue;
        public event Action Changed;
        int[] masks, turns;
        bool[] powered;
        public void Initialize(DiveZoneDefinition definition)
        {
            zone = definition;
            masks = Definition.SolutionMasks(); turns = new int[masks.Length]; powered = new bool[masks.Length];
            Solved = false; ResetBoard();
            if (Ready) throw new InvalidOperationException("La mezcla comienza resuelta: " + Definition.name);
        }
        public int Turns(int index) => turns[index];
        internal void Restore(int[] savedTurns, int moves, bool solved)
        {
            Array.Copy(savedTurns, turns, turns.Length); Moves = moves; Solved = solved; Evaluate();
        }
        public int BaseMask(int index) => masks[index];
        public bool IsPowered(int index) => powered[index];
        public int Mask(int index) => ((masks[index] << turns[index]) | (masks[index] >> (4 - turns[index]))) & 15;
        public bool Rotate(int index)
        {
            if (Solved || turns == null || index < 0 || index >= turns.Length) return false;
            turns[index] = (turns[index] + 1) % 4; Moves++; Evaluate(); return true;
        }
        public bool ResetBoard()
        {
            if (Solved || turns == null) return false;
            Array.Copy(Definition.startingTurns, turns, turns.Length); Moves = 0; Evaluate(); return true;
        }
        public bool Activate()
        {
            if (Solved || !Ready) return false;
            Solved = true; Changed?.Invoke(); return true;
        }
        void Evaluate()
        {
            Array.Clear(powered, 0, powered.Length); PoweredCount = 0; OpenConnections = 0;
            var d = Definition;
            for (int i = 0; i < turns.Length; i++)
                for (int side = 0; side < 4; side++)
                {
                    if ((Mask(i) & (1 << side)) == 0) continue;
                    int next = d.Neighbor(i, side);
                    if (next >= 0)
                    {
                        if ((Mask(next) & (1 << ((side + 2) % 4))) == 0) OpenConnections++;
                    }
                    else if (!(i == d.Start && side == d.inletDirection) && !(i == d.End && side == d.outletDirection)) OpenConnections++;
                }
            if ((Mask(d.Start) & (1 << d.inletDirection)) != 0)
            {
                var pending = new Queue<int>(); pending.Enqueue(d.Start); powered[d.Start] = true;
                while (pending.Count > 0)
                {
                    int tile = pending.Dequeue(); PoweredCount++;
                    for (int side = 0; side < 4; side++)
                    {
                        int next = d.Neighbor(tile, side);
                        if (next < 0 || powered[next] || (Mask(tile) & (1 << side)) == 0 || (Mask(next) & (1 << ((side + 2) % 4))) == 0) continue;
                        powered[next] = true; pending.Enqueue(next);
                    }
                }
            }
            Ready = PoweredCount == turns.Length && OpenConnections == 0 && (Mask(d.End) & (1 << d.outletDirection)) != 0;
            Changed?.Invoke();
        }
    }
}
