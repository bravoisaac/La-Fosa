using UnityEngine;

namespace LaFosa
{
    public sealed class SequencePuzzle : MonoBehaviour
    {
        public DiveZoneDefinition zone;
        public bool Solved { get; private set; }
        public int Progress { get; private set; }
        public static readonly string[] Symbols = { "OLA", "LUNA", "SOL" };
        public string Hint
        {
            get
            {
                if (zone.sequence == null || zone.sequence.Length == 0) return "Sin secuencia configurada";
                string hint = "";
                foreach (int symbol in zone.sequence) hint += (hint.Length > 0 ? "  →  " : "") + Symbols[Mathf.Clamp(symbol, 0, 2)];
                return hint;
            }
        }
        public bool Press(int symbol)
        {
            if (Solved || zone.sequence == null || zone.sequence.Length == 0) return false;
            if (symbol != zone.sequence[Progress]) { Progress = 0; return false; }
            Progress++;
            Solved = Progress == zone.sequence.Length;
            return true;
        }
    }
}
