using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        readonly Transform root;
        readonly GameConfig config;
        readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();
        public WorldBuilder(Transform parent, GameConfig settings) { root = parent; config = settings; }
        public static float FloorHeight(float x, float z)
        {
            float shelf = Mathf.Lerp(3, -9, Mathf.InverseLerp(-12, 24, z));
            float r = Vector2.Distance(new Vector2(x, z), new Vector2(0, 75));
            if (r > 31) return shelf;
            if (r > 27) return Mathf.Lerp(-24, -9, (r - 27) / 4);
            if (r > 23) return -24;
            if (r > 19) return Mathf.Lerp(-52, -24, (r - 19) / 4);
            if (r > 13) return -52;
            if (r > 9) return Mathf.Lerp(-88, -52, (r - 9) / 4);
            return -88;
        }
        Material Mat(string key, Color color, bool glow = false)
        {
            if (materials.TryGetValue(key, out var found)) return found;
            var material = new Material(Shader.Find("Standard")) { color = color, name = key };
            material.SetFloat("_Glossiness", .12f);
            if (glow) { material.EnableKeyword("_EMISSION"); material.SetColor("_EmissionColor", color * .35f); }
            materials.Add(key, material); return material;
        }
        public GameObject Shape(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material material, bool solid = true, Transform parent = null)
        {
            var obj = GameObject.CreatePrimitive(type); obj.name = name; obj.transform.SetParent(parent != null ? parent : root, false);
            obj.transform.localPosition = pos; obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            if (!solid) RemoveCollider(obj);
            return obj;
        }
        static void RemoveCollider(GameObject obj)
        {
            var collider = obj.GetComponent<Collider>();
            if (collider == null) return;
            collider.enabled = false;
            if (Application.isPlaying) Object.Destroy(collider);
            else Object.DestroyImmediate(collider);
        }
        public void Build()
        {
            Random.State previous = Random.state; Random.InitState(config.seed);
            BuildTerrain(); BuildCoast(); BuildSeaLife(); BuildStations();
            Random.state = previous;
        }
        void BuildTerrain()
        {
            var vertices = new List<Vector3>(); var colors = new List<Color>(); var indices = new List<int>();
            const int step = 2;
            for (int x = -150; x < 150; x += step)
                for (int z = -90; z < 210; z += step)
                {
                    Vector3 a = new Vector3(x, FloorHeight(x, z), z);
                    Vector3 b = new Vector3(x, FloorHeight(x, z + step), z + step);
                    Vector3 c = new Vector3(x + step, FloorHeight(x + step, z), z);
                    Vector3 d = new Vector3(x + step, FloorHeight(x + step, z + step), z + step);
                    Triangle(a, b, c, vertices, colors, indices); Triangle(c, b, d, vertices, colors, indices);
                }
            var mesh = new Mesh { name = "Costa y fosa · caras planas", indexFormat = IndexFormat.UInt32 };
            mesh.SetVertices(vertices); mesh.SetColors(colors); mesh.SetTriangles(indices, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            var terrain = new GameObject("Terreno · playa / plataforma / fosa"); terrain.transform.SetParent(root);
            terrain.AddComponent<MeshFilter>().sharedMesh = mesh;
            terrain.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("LaFosa/Faceted"));
            terrain.AddComponent<MeshCollider>().sharedMesh = mesh;
            var sea = Shape("Océano Pacífico", PrimitiveType.Plane, new Vector3(0, config.waterLevel, 104), new Vector3(30, 1, 22), Mat("temporary water", Color.cyan), false);
            sea.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("LaFosa/Water"));
        }
        void Triangle(Vector3 a, Vector3 b, Vector3 c, List<Vector3> vertices, List<Color> colors, List<int> indices)
        {
            float height = (a.y + b.y + c.y) / 3;
            Color color = height > -.4f ? new Color(.73f, .64f, .43f) : height > -12 ? new Color(.28f, .48f, .42f) : height > -35 ? new Color(.23f, .39f, .39f) : new Color(.13f, .24f, .31f);
            color *= Random.Range(.86f, 1.12f); color.a = 1;
            int start = vertices.Count; vertices.Add(a); vertices.Add(b); vertices.Add(c);
            colors.Add(color); colors.Add(color); colors.Add(color);
            indices.Add(start); indices.Add(start + 1); indices.Add(start + 2);
        }
        GameObject Rock(string name, Vector3 position, Vector3 scale, Material material, bool solid = true, Transform parent = null)
        {
            var obj = new GameObject(name); obj.transform.SetParent(parent != null ? parent : root, false); obj.transform.localPosition = position; obj.transform.localScale = scale;
            Vector3[] points = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            int[] triangles = { 0,4,3, 0,3,5, 0,5,2, 0,2,4, 1,3,4, 1,5,3, 1,2,5, 1,4,2 };
            var verts = new Vector3[24]; var ids = new int[24];
            for (int i = 0; i < 24; i++) { verts[i] = points[triangles[i]]; ids[i] = i; }
            var mesh = new Mesh { name = "Octaedro low poly", vertices = verts, triangles = ids }; mesh.RecalculateNormals();
            obj.AddComponent<MeshFilter>().sharedMesh = mesh; obj.AddComponent<MeshRenderer>().sharedMaterial = material;
            if (solid) obj.AddComponent<BoxCollider>().size = new Vector3(1.4f, 1.4f, 1.4f);
            return obj;
        }
        void BuildCoast()
        {
            var stone = Mat("Granito costero", new Color(.32f, .36f, .36f));
            var wood = Mat("Madera", new Color(.31f, .2f, .13f));
            var pale = Mat("Marfil", new Color(.87f, .88f, .75f));
            var red = Mat("Rojo señal", new Color(.85f, .23f, .13f));
            for (int i = 0; i < 65; i++)
            {
                float x = Random.Range(-110, 110), z = Random.Range(-30, 160);
                if (Mathf.Abs(x) < 10) continue;
                if (x > -21 && x < -6 && z > -31 && z < -15) continue;
                if (x > 0 && x < 19 && z > 8 && z < 30) continue;
                float h = FloorHeight(x, z); float s = Random.Range(1, 4);
                Rock("Roca", new Vector3(x, h + s * .25f, z), new Vector3(s, s * .8f, s), stone).transform.Rotate(0, Random.Range(0, 360), 0);
            }
            for (int i = 0; i < 11; i++)
                Rock("Cordillera costera", new Vector3(-90 + i * 18, 4, -66), new Vector3(17, Random.Range(15, 28), 16), stone);
            var clouds = Mat("Nubes del Pacífico", new Color(.88f, .93f, .91f));
            for (int i = 0; i < 8; i++)
            {
                float x = -130 + i * 36, z = Random.Range(100, 190), y = Random.Range(28, 48);
                for (int puff = 0; puff < 3; puff++)
                    Rock("Nube low poly", new Vector3(x + puff * 8, y + puff % 2 * 2, z), new Vector3(12, 3.5f, 7), clouds, false);
            }
            Rock("Islote lejano oeste", new Vector3(-85, 0, 170), new Vector3(29, 17, 18), stone);
            Rock("Islote lejano este", new Vector3(98, 0, 185), new Vector3(34, 13, 22), stone);
            Shape("Muelle", PrimitiveType.Cube, new Vector3(9, 2.2f, 3), new Vector3(4, .45f, 30), wood);
            for (int i = 0; i < 6; i++)
                foreach (float x in new[] { 7.4f, 10.6f }) Shape("Pilote", PrimitiveType.Cylinder, new Vector3(x, -1, -10 + i * 5), new Vector3(.28f, 3.5f, .28f), wood);
            BuildCabin();
            Shape("Poste bandera", PrimitiveType.Cylinder, new Vector3(-7, 6.5f, -23), new Vector3(.09f, 3.5f, .09f), pale);
            Shape("Bandera roja", PrimitiveType.Cube, new Vector3(-5.9f, 8.25f, -23), new Vector3(2.2f, .65f, .06f), red, false);
            Shape("Bandera blanca", PrimitiveType.Cube, new Vector3(-5.5f, 8.9f, -23), new Vector3(1.4f, .65f, .06f), pale, false);
            Shape("Bandera azul", PrimitiveType.Cube, new Vector3(-6.6f, 8.9f, -23), new Vector3(.8f, .65f, .06f), Mat("Azul bandera", new Color(.05f, .18f, .48f)), false);
            Label("★", new Vector3(-6.6f, 8.92f, -23.08f), .13f, Color.white);
            Label("LA FOSA\n↓  SIGUE LAS BOYAS", new Vector3(2, 4, -9), .2f, new Color(.1f, .25f, .27f));
            for (int i = 0; i < 5; i++)
            {
                Vector3 position = new Vector3(0, .15f, 10 + i * 9);
                Shape("Boya de ruta", PrimitiveType.Cylinder, position, new Vector3(.8f, .4f, .8f), red, false);
                Shape("Mástil de boya", PrimitiveType.Cylinder, position + Vector3.up * .8f, new Vector3(.08f, .65f, .08f), pale, false);
            }
            Label("PUNTO DE INMERSIÓN\n01 / BOSQUE DE HUIROS", new Vector3(0, 3.5f, 46), .18f, Color.white);
            for (int i = 0; i < 9; i++)
            {
                var shell = Rock("Concha recolectable", new Vector3(Random.Range(-5, 5), 3.3f, Random.Range(-20, -12)), new Vector3(.32f, .18f, .4f), Mat("Concha", new Color(.95f, .71f, .48f)));
                shell.AddComponent<Pickup>().item = config.shell;
            }
        }
        void BuildSeaLife()
        {
            var kelp = Mat("Huiro verde", new Color(.23f, .4f, .17f));
            for (int i = 0; i < 95; i++)
            {
                float x = Random.Range(-40, 40), z = Random.Range(20, 120);
                if (Mathf.Abs(x) < 5 && z < 85) continue;
                float floor = FloorHeight(x, z); if (floor < -35) continue;
                float h = Random.Range(2, 7);
                Shape("Tallo de huiro", PrimitiveType.Cylinder, new Vector3(x, floor + h / 2, z), new Vector3(.1f, h / 2, .1f), kelp, false);
                for (int j = 1; j <= 4; j++)
                {
                    var leaf = Rock("Hoja de huiro", new Vector3(x + (j % 2 == 0 ? .45f : -.45f), floor + h * j / 5, z), new Vector3(.8f, .18f, .35f), kelp, false);
                    leaf.transform.Rotate(0, j * 35, j % 2 == 0 ? 35 : -35);
                }
            }
            BuildFishPopulation();
            for (int i = 0; i < 55; i++)
            {
                float x = Random.Range(-22, 22), z = Random.Range(53, 97);
                if (Mathf.Abs(x) < 4) continue;
                float floor = FloorHeight(x, z);
                var color = i % 2 == 0 ? new Color(.2f, .85f, .77f) : new Color(.61f, .41f, .86f);
                Rock("Cristal bioluminiscente", new Vector3(x, floor + .6f, z), new Vector3(.35f, Random.Range(.6f, 1.7f), .4f), Mat("Cristal" + i % 2, color, true), false);
            }
        }
        void BuildStations()
        {
            var wreckWood = Mat("Madera sumergida", new Color(.2f, .28f, .25f));
            for (int beam = 0; beam < 5; beam++)
            {
                var rib = Shape("Cuaderna del naufragio", PrimitiveType.Cube, new Vector3(5.4f, -50.8f, 59 + beam * 1.1f), new Vector3(.3f, 2.4f, .25f), wreckWood);
                rib.transform.Rotate(0, 0, -24);
            }
            Shape("Quilla del naufragio", PrimitiveType.Cube, new Vector3(4.7f, -51.5f, 61), new Vector3(.5f, .3f, 6.5f), wreckWood);
            foreach (var zone in config.zones)
            {
                var station = new GameObject(zone.displayName); station.transform.SetParent(root); station.transform.position = zone.stationPosition;
                var puzzle = station.AddComponent<StoneCircuitPuzzle>(); puzzle.Initialize(zone);
                var platform = Mat("Basalto", new Color(.16f, .23f, .26f));
                Shape("Santuario", PrimitiveType.Cylinder, new Vector3(0, -.3f, 0), new Vector3(6.8f, .3f, 4.2f), platform, true, station.transform);
                var chest = Shape("Tesoro sellado", PrimitiveType.Cube, new Vector3(0, .55f, 0), new Vector3(1.5f, 1, 1), Mat("Cofre", new Color(.31f, .29f, .2f)), true, station.transform);
                var lid = Shape("Tapa", PrimitiveType.Cube, new Vector3(0, .55f, 0), new Vector3(1.05f, .2f, 1.1f), Mat("Oro", new Color(.9f, .66f, .26f)), false, chest.transform);
                var treasure = chest.AddComponent<TreasureChest>(); treasure.puzzle = puzzle; treasure.lid = lid.transform;
                BuildStonePuzzle(puzzle);
                var beacon = new GameObject("Luz del santuario"); beacon.transform.SetParent(station.transform, false); beacon.transform.localPosition = Vector3.up * 3;
                var light = beacon.AddComponent<Light>(); light.type = LightType.Point; light.color = zone.accent; light.intensity = 1.3f; light.range = 14;
            }
        }
        public GameObject BuildHarpoon(Transform camera)
        {
            var metal = Mat("Acero arpón", new Color(.23f, .29f, .32f));
            var gun = new GameObject("Arpón · modelo de primera persona"); gun.transform.SetParent(camera, false); gun.transform.localPosition = new Vector3(.28f, -.25f, .55f);
            gun.transform.localScale = Vector3.one * .65f;
            Shape("Cuerpo arpón", PrimitiveType.Cube, Vector3.zero, new Vector3(.09f, .1f, .65f), metal, false, gun.transform);
            Shape("Empuñadura", PrimitiveType.Cube, new Vector3(0, -.08f, -.17f), new Vector3(.08f, .17f, .1f), Mat("Corcho", new Color(.57f, .37f, .2f)), false, gun.transform);
            Shape("Varilla", PrimitiveType.Cube, new Vector3(0, .055f, .25f), new Vector3(.015f, .015f, .8f), Mat("Varilla clara", new Color(.8f, .86f, .82f)), false, gun.transform);
            Shape("Goma", PrimitiveType.Cube, new Vector3(0, .07f, -.08f), new Vector3(.17f, .025f, .025f), Mat("Goma ámbar", new Color(.94f, .57f, .2f)), false, gun.transform);
            return gun;
        }
        GameObject Label(string text, Vector3 position, float size, Color color)
        {
            var obj = new GameObject(text); obj.transform.SetParent(root); obj.transform.position = position;
            var mesh = obj.AddComponent<TextMesh>(); mesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); mesh.text = text; mesh.fontSize = 48; mesh.characterSize = size * .28f; mesh.anchor = TextAnchor.MiddleCenter; mesh.alignment = TextAlignment.Center; mesh.color = color;
            var textMaterial = new Material(Shader.Find("LaFosa/WorldText")); textMaterial.mainTexture = mesh.font.material.mainTexture;
            obj.GetComponent<Renderer>().sharedMaterial = textMaterial;
            obj.AddComponent<Billboard>();
            return obj;
        }
    }
}
