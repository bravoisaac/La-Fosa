using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Zona de buceo")]
    public sealed class DiveZoneDefinition : ScriptableObject
    {
        public string displayName;
        public string subtitle;
        public Vector3 stationPosition;
        public float startsAtDepth;
        public float unlockedDepth = 65;
        public float unlockedOxygen = 150;
        public Color accent = Color.cyan;
        public int[] sequence = { 0, 1, 2 };
        public StonePuzzleDefinition stonePuzzle;
        public ItemDefinition treasure;
    }
}
