using System;
using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Puzle de piedras")]
    public sealed class StonePuzzleDefinition : ScriptableObject
    {
        public string title = "La corriente de piedra";
        [TextArea] public string clue = "Gira las piedras. Lleva la luz desde ENTRADA hasta SELLO pasando por todas.";
        [Range(2, 6)] public int columns = 3;
        [Range(2, 6)] public int rows = 2;
        [Tooltip("Índices en la ruta de solución, por filas de arriba a abajo.")]
        public int[] route = { 0, 1, 2, 5, 4, 3 };
        [Tooltip("0 arriba, 1 derecha, 2 abajo, 3 izquierda.")]
        [Range(0, 3)] public int inletDirection = 3;
        [Range(0, 3)] public int outletDirection = 3;
        public int[] startingTurns = { 1, 1, 3, 1, 3, 2 };
        public int Count => columns * rows;
        public int Start => route[0];
        public int End => route[route.Length - 1];
        public int Neighbor(int index, int direction)
        {
            int x = index % columns, y = index / columns;
            switch (direction) { case 0: y--; break; case 1: x++; break; case 2: y++; break; case 3: x--; break; default: return -1; }
            return x < 0 || x >= columns || y < 0 || y >= rows ? -1 : y * columns + x;
        }
        public int[] SolutionMasks()
        {
            Validate();
            var masks = new int[Count];
            masks[Start] |= 1 << inletDirection; masks[End] |= 1 << outletDirection;
            for (int i = 1; i < route.Length; i++)
                for (int direction = 0; direction < 4; direction++)
                    if (Neighbor(route[i - 1], direction) == route[i])
                    { masks[route[i - 1]] |= 1 << direction; masks[route[i]] |= 1 << ((direction + 2) % 4); }
            return masks;
        }
        public void Validate()
        {
            if (columns < 2 || columns > 6 || rows < 2 || rows > 6 || route == null || route.Length != Count || startingTurns == null || startingTurns.Length != Count)
                throw new InvalidOperationException("Dimensiones, ruta o mezcla incompletas: " + name);
            var seen = new bool[Count];
            for (int i = 0; i < route.Length; i++)
            {
                int index = route[i];
                if (index < 0 || index >= Count || seen[index]) throw new InvalidOperationException("Ruta repetida o fuera del tablero: " + name);
                seen[index] = true;
                if (startingTurns[i] < 0 || startingTurns[i] > 3) throw new InvalidOperationException("Giro inválido: " + name);
                if (i == 0) continue;
                int previous = route[i - 1];
                if (Mathf.Abs(previous % columns - index % columns) + Mathf.Abs(previous / columns - index / columns) != 1)
                    throw new InvalidOperationException("La ruta salta una piedra: " + name);
            }
            if (inletDirection < 0 || inletDirection > 3 || outletDirection < 0 || outletDirection > 3 || Neighbor(Start, inletDirection) != -1 || Neighbor(End, outletDirection) != -1)
                throw new InvalidOperationException("Entrada y salida deben mirar fuera del tablero: " + name);
        }
    }
}
