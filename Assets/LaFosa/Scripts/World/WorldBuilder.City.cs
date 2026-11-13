using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        public void BuildCity()
        {
            var paving = Mat("Adoquines de caleta", new Color(.36f, .4f, .4f));
            var trim = Mat("Bordes de plaza", new Color(.78f, .73f, .59f));
            var wood = Mat("Madera urbana", new Color(.43f, .27f, .16f));
            Shape("Plaza de Caleta Bruma", PrimitiveType.Cube, new Vector3(0, 3.02f, -30), new Vector3(12, .12f, 35), paving);
            foreach (float z in new[] { -17f, -31f, -45f })
                Shape("Calle de comercios", PrimitiveType.Cube, new Vector3(0, 3.02f, z), new Vector3(42, .12f, 3.4f), paving);
            BuildCityShop(ShopKind.Mining, new Vector3(13, 3, -25), new Color(.68f, .32f, .13f), "PICO / COMPRA DE MINERALES");
            BuildCityShop(ShopKind.Diving, new Vector3(-13, 3, -39), new Color(.12f, .39f, .65f), "ACCESORIOS / FABRICAR TRAJES");
            BuildCityShop(ShopKind.Construction, new Vector3(13, 3, -39), new Color(.54f, .43f, .17f), "OBRAS / EMBARCACIONES");
            foreach (float x in new[] { -28f, 28f })
                foreach (float z in new[] { -24f, -39f })
                {
                    var wall = Mat("Fachada " + x + z, x < 0 ? new Color(.57f, .68f, .62f) : new Color(.73f, .52f, .34f));
                    Shape("Casa de pescadores", PrimitiveType.Cube, new Vector3(x, 5, z), new Vector3(7, 4, 8), wall);
                    foreach (float side in new[] { -1f, 1f })
                    {
                        var roof = Shape("Tejado de barrio", PrimitiveType.Cube, new Vector3(x + side * 1.85f, 7.8f, z), new Vector3(4.2f, .22f, 9), wood);
                        roof.transform.rotation = Quaternion.Euler(0, 0, -side * 24);
                        Shape("Ventana de casa", PrimitiveType.Cube, new Vector3(x + side * 1.8f, 5.4f, z + 4.03f), new Vector3(1.2f, 1.25f, .08f), trim, false);
                    }
                    Shape("Puerta de casa", PrimitiveType.Cube, new Vector3(x, 4.25f, z + 4.05f), new Vector3(1.1f, 2.5f, .1f), wood, false);
                }
            foreach (float x in new[] { -5.2f, 5.2f })
                foreach (float z in new[] { -18f, -32f, -45f })
                {
                    Shape("Farol de plaza", PrimitiveType.Cylinder, new Vector3(x, 5.1f, z), new Vector3(.12f, 2, .12f), wood);
                    Shape("Luz de farol", PrimitiveType.Cube, new Vector3(x, 7.2f, z), new Vector3(.55f, .7f, .55f), Mat("Luz cálida", new Color(1, .79f, .38f), true), false);
                }
            foreach (float x in new[] { -4f, 4f })
            {
                Shape("Banco de plaza", PrimitiveType.Cube, new Vector3(x, 3.6f, -37), new Vector3(1, .18f, 3), wood);
                Shape("Respaldo de banco", PrimitiveType.Cube, new Vector3(x + Mathf.Sign(x) * .45f, 4.1f, -37), new Vector3(.12f, .8f, 3), wood);
            }
            Label("CALETA BRUMA\nPESCA ←  PLAZA  → MINERÍA\nBUCEO Y ASTILLERO AL FONDO", new Vector3(0, 6, -14), .14f, new Color(.98f, .83f, .48f));
        }
        void BuildCityShop(ShopKind shop, Vector3 position, Color color, string service)
        {
            var building = new GameObject(CityShops.Name(shop)); building.transform.SetParent(root); building.transform.position = position;
            var paint = Mat("Local " + shop, color); var wood = Mat("Madera urbana", new Color(.43f, .27f, .16f));
            var pale = Mat("Marcos urbanos", new Color(.9f, .83f, .65f));
            Shape("Corredor del comercio", PrimitiveType.Cube, new Vector3(0, .1f, .5f), new Vector3(8, .2f, 8), wood, true, building.transform);
            Shape("Fondo del local", PrimitiveType.Cube, new Vector3(0, 1.8f, -3), new Vector3(8, 3.6f, .25f), paint, true, building.transform);
            foreach (float x in new[] { -3.9f, 3.9f })
                Shape("Lateral del local", PrimitiveType.Cube, new Vector3(x, 1.8f, 0), new Vector3(.2f, 3.6f, 6), paint, true, building.transform);
            foreach (float side in new[] { -1f, 1f })
            {
                var roof = Shape("Zinc del comercio", PrimitiveType.Cube, new Vector3(side * 2, 4.3f, .5f), new Vector3(4.5f, .2f, 8.6f), paint, true, building.transform);
                roof.transform.localRotation = Quaternion.Euler(0, 0, -side * 23);
                Shape("Pilar frontal", PrimitiveType.Cylinder, new Vector3(side * 3.6f, 1.8f, 3.7f), new Vector3(.15f, 1.8f, .15f), pale, true, building.transform);
            }
            var counter = Shape("Mostrador " + shop, PrimitiveType.Cube, new Vector3(0, 1.1f, 3.25f), new Vector3(6, 1.8f, .8f), paint, true, building.transform);
            counter.AddComponent<ShopCounter>().shop = shop;
            Shape("Vendedor", PrimitiveType.Capsule, new Vector3(0, 1.4f, 1.8f), new Vector3(.65f, .85f, .65f), wood, false, building.transform);
            Shape("Rostro", PrimitiveType.Sphere, new Vector3(0, 2.4f, 1.8f), Vector3.one * .55f, pale, false, building.transform);
            Label(CityShops.Name(shop), position + new Vector3(0, 3.5f, 4), .17f, Color.white);
            Label("E · " + service, position + new Vector3(0, 2.55f, 4), .1f, new Color(1, .85f, .5f));
            for (int i = 0; i < 3; i++)
            {
                Vector3 display = new Vector3(-2.4f + i * 2.4f, 2.15f, 3.2f);
                if (shop == ShopKind.Mining) Rock("Mineral de muestra", display, Vector3.one * .38f, Mat("Muestra " + i, config.minerals[i].color, true), false, building.transform);
                else if (shop == ShopKind.Diving) Shape("Bombona de muestra", PrimitiveType.Cylinder, display + Vector3.up * .3f, new Vector3(.35f, .45f, .35f), pale, false, building.transform);
                else Shape("Maqueta del muelle", PrimitiveType.Cube, display, new Vector3(1.4f, .2f, .65f), wood, false, building.transform);
            }
        }
        public void BuildInfrastructure(GameSession session)
        {
            var holder = new GameObject("Infraestructura de Caleta Bruma"); holder.transform.SetParent(root);
            var stages = new GameObject[5];
            for (int i = 0; i < 5; i++) { stages[i] = new GameObject("Obra " + (i + 1)); stages[i].transform.SetParent(holder.transform); }
            var wood = Mat("Tablas del muelle nuevo", new Color(.47f, .32f, .19f));
            var metal = Mat("Estructura de base", new Color(.19f, .36f, .39f));
            var bright = Mat("Enlace turquesa", new Color(.25f, .9f, .8f), true);
            BuildPierSegment(stages[0].transform, 24, wood);
            BuildAutomaticFishing(session, stages[1].transform, 0, 26, wood);
            BuildPierSegment(stages[2].transform, 36, wood);
            BuildAutomaticFishing(session, stages[2].transform, 1, 38, wood);
            Transform platform = stages[3].transform;
            Shape("Base flotante · cubierta", PrimitiveType.Cube, new Vector3(-12, config.waterLevel + 2.2f, 76), new Vector3(16, .45f, 14), metal, true, platform);
            foreach (float x in new[] { -18f, -6f })
                Shape("Pontón flotante", PrimitiveType.Cube, new Vector3(x, config.waterLevel + .7f, 76), new Vector3(2, 1.6f, 14), wood, true, platform);
            var ramp = Shape("Rampa desde el agua", PrimitiveType.Cube, new Vector3(-12, config.waterLevel + .5f, 63.5f), new Vector3(3.5f, .2f, 11.5f), wood, true, platform);
            ramp.transform.localRotation = Quaternion.Euler(-18, 0, 0);
            foreach (float x in new[] { -19.7f, -4.3f })
                Shape("Baranda lateral", PrimitiveType.Cube, new Vector3(x, config.waterLevel + 3.1f, 76), new Vector3(.15f, 1.2f, 14), bright, true, platform);
            Shape("Refugio · techo", PrimitiveType.Cube, new Vector3(-14, config.waterLevel + 6, 80), new Vector3(8, .25f, 5), metal, true, platform);
            foreach (float x in new[] { -17.5f, -10.5f })
                Shape("Soporte del refugio", PrimitiveType.Cylinder, new Vector3(x, config.waterLevel + 4.2f, 81.5f), new Vector3(.2f, 1.8f, .2f), wood, true, platform);
            StageLabel("BASE DE LA FOSA\nENLACE A LA CIUDAD", new Vector3(-12, config.waterLevel + 7, 76), platform);
            BuildTerminal(session, new Vector3(-12, config.waterLevel + 3.3f, 71), true, platform, metal);
            BuildTerminal(session, new Vector3(3, 4.1f, -28), false, root, metal);
            var lift = stages[4].transform;
            Shape("Torre del descensor", PrimitiveType.Cube, new Vector3(-5.5f, config.waterLevel + 5, 80), new Vector3(.5f, 5.5f, .5f), metal, true, lift);
            Shape("Brazo del descensor", PrimitiveType.Cube, new Vector3(-2.75f, config.waterLevel + 7.5f, 76), new Vector3(6, .25f, .25f), metal, false, lift);
            Shape("Cable guía sin colisión", PrimitiveType.Cube, new Vector3(.8f, config.waterLevel - 307, 76), new Vector3(.035f, 630, .035f), bright, false, lift);
            StageLabel("DESCENSOR RÁPIDO\nUSA EL TERMINAL DE LA BASE", new Vector3(-5.5f, config.waterLevel + 8.5f, 78), lift);
            holder.AddComponent<InfrastructureView>().Initialize(session, stages);
        }
        void BuildPierSegment(Transform parent, float z, Material wood)
        {
            Shape("Extensión de muelle", PrimitiveType.Cube, new Vector3(9, 2.2f, z), new Vector3(4, .45f, 12), wood, true, parent);
            for (int i = 0; i < 3; i++)
                foreach (float x in new[] { 7.4f, 10.6f })
                    Shape("Pilote nuevo", PrimitiveType.Cylinder, new Vector3(x, -3, z - 5 + i * 5), new Vector3(.28f, 5.5f, .28f), wood, true, parent);
        }
        void BuildAutomaticFishing(GameSession session, Transform parent, int index, float z, Material wood)
        {
            Shape("Plataforma de pesca", PrimitiveType.Cube, new Vector3(12, 2.2f, z), new Vector3(4, .45f, 4), wood, true, parent);
            var box = Shape("Depósito de capturas", PrimitiveType.Cube, new Vector3(12.5f, 3, z), new Vector3(1.2f, 1.1f, 1.2f), Mat("Cajón de pesca", new Color(.22f, .52f, .54f)), true, parent);
            var interaction = box.AddComponent<CityInteraction>(); interaction.session = session; interaction.fishingIndex = index;
            var rope = Mat("Red automática", new Color(.75f, .74f, .49f));
            Shape("Pescante", PrimitiveType.Cube, new Vector3(13.5f, 4, z + 1.5f), new Vector3(.15f, 3.5f, .15f), wood, true, parent);
            Shape("Brazo de red", PrimitiveType.Cube, new Vector3(15, 5.6f, z + 1.5f), new Vector3(3, .15f, .15f), wood, false, parent);
            Shape("Cuerda de red", PrimitiveType.Cube, new Vector3(16, 2.2f, z + 1.5f), new Vector3(.04f, 6.6f, .04f), rope, false, parent);
            for (int i = 0; i < 6; i++)
            {
                Shape("Malla automática", PrimitiveType.Cube, new Vector3(15 + i * .4f, -.9f, z + 1.5f), new Vector3(.025f, .025f, 2), rope, false, parent);
                Shape("Malla transversal", PrimitiveType.Cube, new Vector3(16, -.9f, z + .5f + i * .4f), new Vector3(2, .025f, .025f), rope, false, parent);
            }
            StageLabel("PESCA AUTOMÁTICA " + (index + 1) + "\nE EN EL CAJÓN · 1 PEZ / MIN", new Vector3(12, 5.5f, z), parent);
        }
        void BuildTerminal(GameSession session, Vector3 position, bool atBase, Transform parent, Material material)
        {
            var terminal = Shape("Terminal " + (atBase ? "base" : "ciudad"), PrimitiveType.Cube, position, new Vector3(1.4f, 1.8f, .7f), material, true, parent);
            var interaction = terminal.AddComponent<CityInteraction>(); interaction.session = session; interaction.terminal = true; interaction.atBase = atBase;
            StageLabel("ENLACE · E\nCIUDAD / BASE", position + Vector3.up * 2, parent);
        }
        void StageLabel(string text, Vector3 position, Transform parent)
        { var label = Label(text, position, .12f, Color.white); label.transform.SetParent(parent, true); }
    }
}
