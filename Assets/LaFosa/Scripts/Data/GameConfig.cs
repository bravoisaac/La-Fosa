using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Configuración")]
    public sealed class GameConfig : ScriptableObject
    {
        public int seed = 2718;
        public Vector3 spawn = new Vector3(0, 4, -22);
        public float waterLevel;
        [Header("Bruma submarina")]
        [Range(.01f, .2f)] public float coastalFogDensity = .035f;
        [Range(.01f, .2f)] public float shallowFogDensity = .065f;
        [Range(.01f, .2f)] public float deepFogDensity = .115f;
        public float initialOxygen = 115;
        public float initialSafeDepth = 34;
        [Min(1)] public float pressureOxygenMultiplier = 8;
        public int inventorySlots = 12;
        public ItemDefinition fish;
        public FishSpeciesDefinition[] fishSpecies;
        public FishBehaviorProfile coastalFishBehavior;
        public FishBehaviorProfile offshoreFishBehavior;
        public ItemDefinition shell;
        public ShopCatalog shop;
        public DiveZoneDefinition[] zones;
        [HideInInspector] public Shader[] includedShaders;
    }
}
