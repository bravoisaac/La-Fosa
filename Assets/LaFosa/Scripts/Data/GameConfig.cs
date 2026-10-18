using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Configuración")]
    public sealed class GameConfig : ScriptableObject
    {
        public int seed = 2718;
        public Vector3 spawn = new Vector3(0, 4, -22);
        public float waterLevel;
        public float initialOxygen = 115;
        public float initialSafeDepth = 34;
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
