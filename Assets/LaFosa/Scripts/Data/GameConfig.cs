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
        [Header("Umbrales de presión · no son barreras físicas")]
        public DepthPressureBand[] pressureBands =
        {
            new DepthPressureBand(60, 1.25f), new DepthPressureBand(150, 1.6f),
            new DepthPressureBand(280, 2.1f), new DepthPressureBand(420, 2.8f), new DepthPressureBand(560, 3.8f)
        };
        public int PressureLevel(float depth)
        {
            int level = 0;
            if (pressureBands != null)
                foreach (var band in pressureBands) if (depth >= band.depth) level++;
            return level;
        }
        public float OxygenCost(float depth, float safeDepth)
        {
            float cost = 1;
            if (pressureBands != null)
                foreach (var band in pressureBands) if (depth >= band.depth) cost = Mathf.Max(cost, band.airMultiplier);
            return cost * (depth > safeDepth ? Mathf.Max(1, pressureOxygenMultiplier) : 1);
        }
        public float AirConsumption(float depth, float safeDepth, bool sprint)
            => OxygenCost(depth, safeDepth) * (sprint ? 1.5f : 1);

        // Conservative planning reference for a direct, unsprinted ascent; excludes detours.
        public float ReturnAir(float depth, float safeDepth, float swimSpeed)
        {
            float air = 0;
            for (float top = 0; top < depth; top += 2)
            {
                float bottom = Mathf.Min(depth, top + 2);
                air += (bottom - top) / Mathf.Max(1, swimSpeed) * OxygenCost(bottom, safeDepth);
            }
            return air;
        }
        public int inventorySlots = 12;
        public ItemDefinition fish;
        public FishSpeciesDefinition[] fishSpecies;
        public FishBehaviorProfile coastalFishBehavior;
        public FishBehaviorProfile offshoreFishBehavior;
        public ItemDefinition shell;
        public ShopCatalog shop;
        public ItemDefinition[] minerals;
        public CraftingRecipe[] recipes;
        public DiveZoneDefinition[] zones;
        [HideInInspector] public Shader[] includedShaders;
    }

    [System.Serializable]
    public sealed class DepthPressureBand
    {
        [Min(1)] public float depth;
        [Min(1)] public float airMultiplier;
        public DepthPressureBand(float depth, float airMultiplier) { this.depth = depth; this.airMultiplier = airMultiplier; }
    }
}
