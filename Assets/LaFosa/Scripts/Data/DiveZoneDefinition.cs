using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Zona de buceo")]
    public sealed class DiveZoneDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public string subtitle;
        public Vector3 stationPosition;
        public float startsAtDepth;
        [Header("Acceso y preparación")]
        public bool hasPhysicalGate = true;
        public Vector3 entryPosition;
        [Min(0)] public float requiredSafeDepth;
        [Min(0)] public float requiredOxygen;
        [Min(12)] public float barrierWidth = 60;
        public string returnHint = "Asciende por la escotilla iluminada. R permite volver a la playa.";
        public float unlockedDepth = 65;
        public float unlockedOxygen = 150;
        public Color accent = Color.cyan;
        public int[] sequence = { 0, 1, 2 };
        public StonePuzzleDefinition stonePuzzle;
        public ItemDefinition treasure;
    }
}
