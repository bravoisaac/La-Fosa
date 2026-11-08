using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        public void BuildMinerals()
        {
            var wreckWood = Mat("Madera sumergida", new Color(.2f, .28f, .25f));
            for (int beam = 0; beam < 5; beam++)
            {
                var rib = Shape("Cuaderna del naufragio", PrimitiveType.Cube, new Vector3(5.4f, -50.8f, 59 + beam * 1.1f), new Vector3(.3f, 2.4f, .25f), wreckWood);
                rib.transform.Rotate(0, 0, -24);
            }
            Shape("Quilla del naufragio", PrimitiveType.Cube, new Vector3(4.7f, -51.5f, 61), new Vector3(.5f, .3f, 6.5f), wreckWood);
            // Stable IDs and locations: loading never replenishes a vein before its saved cooldown.
            float[] rows = { 50, 60, 70 };
            for (int tier = 0; tier < config.minerals.Length; tier++)
            {
                var mineral = config.minerals[tier];
                var material = Mat("Veta de " + mineral.displayName, mineral.color, true);
                for (int i = 0; i < 10; i++)
                {
                    float x = (i % 5 - 2) * 3, z = rows[tier] + (i / 5);
                    if (tier == 1) x -= 3; // Keep iron clear of the retained wreck ribs.
                    CreateDeposit("mineral-" + mineral.id + "-" + i, mineral,
                        new Vector3(x, FloorHeight(x, z) + .6f, z), material);
                }
                Label(mineral.displayName.ToUpperInvariant() + "\n[ E ] EXTRAER CON PICO", new Vector3(-7, FloorHeight(-7, rows[tier]) + 2.5f, rows[tier]), .11f, mineral.color);
            }
            var crystal = config.minerals[2]; var deepMaterial = Mat("Cristal del fondo", crystal.color, true);
            for (int i = 0; i < 5; i++)
                CreateDeposit("mineral-fondo-" + i, crystal, new Vector3((i - 2) * 4, -DeepTrenchProfile.BottomDepth + .6f, 76), deepMaterial);
        }
        void CreateDeposit(string id, ItemDefinition mineral, Vector3 position, Material material)
        {
            var obj = Rock("Depósito de " + mineral.displayName, position, new Vector3(.65f, .6f, .65f), material, true);
            var deposit = obj.AddComponent<MineralDeposit>(); deposit.id = id; deposit.mineral = mineral; deposit.amount = 3;
            var light = obj.AddComponent<Light>(); light.type = LightType.Point; light.color = mineral.color; light.range = 3; light.intensity = .35f;
        }
    }
}
