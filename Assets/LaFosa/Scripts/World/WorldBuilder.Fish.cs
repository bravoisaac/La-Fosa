using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        void BuildFishPopulation()
        {
            if (config.fishSpecies == null) return;
            var school = new GameObject("Fauna chilena · 49 especies"); school.transform.SetParent(root);
            foreach (var species in config.fishSpecies)
            {
                if (species == null || species.prefab == null) continue;
                for (int i = 0; i < species.population; i++)
                {
                    Vector3 position = FishPosition(species);
                    var obj = Object.Instantiate(species.prefab, position, Quaternion.identity, school.transform);
                    obj.name = species.displayName + " · " + (i + 1);
                    obj.GetComponent<FishAgent>().waterLevel = config.waterLevel;
                }
            }
        }
        Vector3 FishPosition(FishSpeciesDefinition species)
        {
            // Sample the entire orbit plus model length, so bodies do not cross trench walls.
            for (int attempt = 0; attempt < 1000; attempt++)
            {
                int band = species.habitatBand;
                float x, z;
                if (band == 0) { x = Random.Range(-12f, 12f); z = Random.Range(8f, 24f); }
                else if (band == 1) { x = Random.Range(-24f, 24f); z = Random.Range(29f, 65f); }
                else if (band == 2) { x = Random.Range(-18f, 18f); z = Random.Range(40f, 90f); }
                else if (band == 3) { x = Random.Range(-14f, 14f); z = Random.Range(61f, 89f); }
                else { x = Random.Range(-4.5f, 4.5f); z = Random.Range(71f, 79f); }
                float depth = Random.Range(species.playableDepth.x + .35f, species.playableDepth.y - .35f);
                float extent = species.length * .85f + 1.3f;
                bool clear = true;
                foreach (var zone in config.zones)
                    if (zone != config.zones[0] && Mathf.Abs(config.waterLevel - depth - zone.entryPosition.y) < species.height + 1.5f &&
                        Mathf.Abs(x - zone.entryPosition.x) < zone.barrierWidth / 2 + extent && Mathf.Abs(z - zone.entryPosition.z) < zone.barrierWidth / 2 + extent)
                    { clear = false; break; }
                for (int j = 0; j < 12; j++)
                {
                    float angle = j * Mathf.PI / 6;
                    if (FloorHeight(x + Mathf.Cos(angle) * extent, z + Mathf.Sin(angle) * extent) > config.waterLevel - depth - species.height - 1) { clear = false; break; }
                }
                if (clear) return new Vector3(x, config.waterLevel - depth, z);
            }
            throw new System.InvalidOperationException("No hay hábitat válido para " + species.displayName);
        }
    }
}
