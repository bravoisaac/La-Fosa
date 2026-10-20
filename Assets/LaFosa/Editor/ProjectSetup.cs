using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class ProjectSetup
    {
        const string DataPath = "Assets/LaFosa/Data";
        const string ScenePath = "Assets/LaFosa/Scenes/LaFosa.unity";
        [MenuItem("La Fosa/Preparar escena inicial")]
        public static void Setup()
        {
            Directory.CreateDirectory(DataPath); Directory.CreateDirectory("Assets/LaFosa/Scenes"); AssetDatabase.Refresh();
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>(DataPath + "/Expedicion.asset");
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<GameConfig>();
                config.fish = Item("jurel", "Jurel", "Capturado en el Pacífico. Véndelo en la cabaña.", 10, new Color(.43f, .78f, .8f));
                config.shell = Item("concha", "Concha costera", "Un hallazgo en la orilla de Caleta Bruma.", 10, new Color(.95f, .71f, .48f));
                config.zones = new[]
                {
                    Zone("01_Huiros", "01 · Bosque de huiros", "La luz aún alcanza las ruinas.", new Vector3(0, -23.5f, 50), 12, 64, 155, new[]{0,1,2}, new Color(.38f,.91f,.74f), Item("perla", "Perla del Pacífico", "Primer tesoro. Mejora permanente de esta sesión: 64 m y 155 s de oxígeno.", 1, new Color(.8f,.96f,.88f))),
                    Zone("02_Naufragio", "02 · Ecos del naufragio", "El océano conserva sus secretos.", new Vector3(0, -51.5f, 58), 35, 100, 200, new[]{1,0,2,1}, new Color(.35f,.67f,1), Item("brujula", "Brújula sumergida", "Segundo tesoro. Mejora de equipo: 100 m y 200 s de oxígeno.", 1, new Color(.99f,.73f,.35f))),
                    Zone("03_Abismo", "03 · Corazón del abismo", "Un resplandor en la oscuridad.", new Vector3(0, -87.5f, 70), 67, 110, 240, new[]{2,1,0,2,0}, new Color(.74f,.49f,1), Item("corazon", "Corazón del abismo", "Último tesoro del prototipo. Expedición completada.", 1, new Color(.76f,.54f,1)))
                };
                AssetDatabase.CreateAsset(config, DataPath + "/Expedicion.asset");
            }
            FishCatalogBuilder.Ensure(config);
            FishBehaviorSetup.Ensure(config);
            StonePuzzleSetup.Ensure(config);
            config.includedShaders = new[] { Shader.Find("Standard"), Shader.Find("Sprites/Default"), Shader.Find("LaFosa/Faceted"), Shader.Find("LaFosa/Water"), Shader.Find("LaFosa/WorldText"), Shader.Find("LaFosa/Fish") };
            foreach (var shader in config.includedShaders) if (shader == null) throw new Exception("Falta un shader requerido.");
            if (config.shop == null)
            {
                var catalog = AssetDatabase.LoadAssetAtPath<ShopCatalog>(DataPath + "/Tienda.asset");
                if (catalog == null)
                {
                    catalog = ScriptableObject.CreateInstance<ShopCatalog>();
                    catalog.offers = new[]
                    {
                        new ShopOffer(GearKind.Trident, "Tridente", "Primera herramienta. Tecla 2: alcance de 4 m y recarga rápida.", 60),
                        new ShopOffer(GearKind.ReinforcedHarpoon, "Arpón", "Después del tridente. Tecla 3: alcance de 40 m y recarga de 0,65 s.", 150),
                        new ShopOffer(GearKind.OxygenTank, "Bombona de O₂ ampliada", "+60 s de reserva; se suma a las mejoras de los tesoros.", 90),
                        new ShopOffer(GearKind.Fins, "Aletas de buceo", "Nadas un 35% más rápido. Se equipan automáticamente.", 75),
                        new ShopOffer(GearKind.Net, "Red de pesca", "Después del tridente. Tecla 4: hasta 5 peces cercanos.", 120),
                        new ShopOffer(GearKind.Boat, "Bote de caleta", "Conducible a 12 m/s. Entrega al lado izquierdo del muelle.", 240),
                        new ShopOffer(GearKind.JetSki, "Moto de agua", "Conducible a 22 m/s. Entrega al lado derecho del muelle.", 360)
                    };
                    AssetDatabase.CreateAsset(catalog, DataPath + "/Tienda.asset");
                }
                config.shop = catalog;
            }
            EditorUtility.SetDirty(config);
            PlayerSettings.companyName = "Caleta Bruma"; PlayerSettings.productName = "La Fosa";
            PlayerSettings.defaultScreenWidth = 1600; PlayerSettings.defaultScreenHeight = 900;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.runInBackground = true; PlayerSettings.colorSpace = ColorSpace.Linear;
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var input = settings.FindProperty("activeInputHandler"); if (input != null) input.intValue = 0;
            settings.ApplyModifiedPropertiesWithoutUndo();
            EnsureMouseAxes();
            // Preserve the scene and user edits on subsequent runs.
            if (!File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                new GameObject("LA FOSA · Inicio").AddComponent<GameBootstrap>().config = config;
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("LA_FOSA_SETUP_OK · Escena y configuración listas.");
        }
        static ItemDefinition Item(string id, string title, string description, int stack, Color color)
        {
            string path = DataPath + "/" + id + ".asset";
            var item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
            if (item != null) return item;
            item = ScriptableObject.CreateInstance<ItemDefinition>(); item.id = id; item.displayName = title; item.description = description; item.maxStack = stack; item.color = color;
            AssetDatabase.CreateAsset(item, path); return item;
        }
        static DiveZoneDefinition Zone(string id, string title, string subtitle, Vector3 pos, float start, float maxDepth, float oxygen, int[] sequence, Color color, ItemDefinition treasure)
        {
            var zone = ScriptableObject.CreateInstance<DiveZoneDefinition>(); zone.id = id.Substring(3).ToLowerInvariant(); zone.displayName = title; zone.subtitle = subtitle; zone.stationPosition = pos; zone.startsAtDepth = start; zone.unlockedDepth = maxDepth; zone.unlockedOxygen = oxygen; zone.sequence = sequence; zone.accent = color; zone.treasure = treasure;
            zone.entryPosition = zone.id == "huiros" ? new Vector3(0, -2, 46) : new Vector3(0, zone.id == "naufragio" ? -32 : -64, 75);
            zone.requiredSafeDepth = zone.id == "huiros" ? 24 : zone.id == "naufragio" ? 52 : 88;
            zone.requiredOxygen = zone.id == "huiros" ? 115 : zone.id == "naufragio" ? 155 : 200;
            zone.barrierWidth = zone.id == "abismo" ? 40 : 60;
            zone.hasPhysicalGate = zone.id == "abismo";
            if (!zone.hasPhysicalGate) zone.returnHint = "Ascenso libre hacia la superficie. Fuera del límite del equipo el oxígeno se agota rápidamente. R permite volver a la playa.";
            AssetDatabase.CreateAsset(zone, DataPath + "/" + id + ".asset"); return zone;
        }
        static void EnsureMouseAxes()
        {
            var inputAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset");
            if (inputAsset.Length == 0) throw new Exception("InputManager.asset no disponible.");
            var serialized = new SerializedObject(inputAsset[0]); var axes = serialized.FindProperty("m_Axes");
            for (int mouseAxis = 0; mouseAxis < 2; mouseAxis++)
            {
                string name = mouseAxis == 0 ? "Mouse X" : "Mouse Y";
                SerializedProperty entry = null;
                for (int i = 0; i < axes.arraySize; i++)
                    if (axes.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == name) entry = axes.GetArrayElementAtIndex(i);
                if (entry == null) { int index = axes.arraySize; axes.InsertArrayElementAtIndex(index); entry = axes.GetArrayElementAtIndex(index); }
                entry.FindPropertyRelative("m_Name").stringValue = name;
                entry.FindPropertyRelative("type").intValue = 1; entry.FindPropertyRelative("axis").intValue = mouseAxis;
                entry.FindPropertyRelative("sensitivity").floatValue = .1f;
                entry.FindPropertyRelative("gravity").floatValue = 0; entry.FindPropertyRelative("dead").floatValue = 0;
                entry.FindPropertyRelative("invert").boolValue = false;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
        [MenuItem("La Fosa/Validar mecánicas")]
        public static void Validate()
        {
            Setup();
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>(DataPath + "/Expedicion.asset");
            var obj = new GameObject("Checks temporales");
            FishCatalogBuilder.Validate(config);
            Check(config.coastalFishBehavior.escapeSpeed > 0 && config.coastalFishBehavior.escapeSpeed < 6, "Fauna de orilla alcanzable nadando");
            Check(config.offshoreFishBehavior.escapeSpeed > 9 * config.shop.finSpeedMultiplier, "Fauna del mar más rápida que el buzo con aletas");
            try
            {
                var inventory = obj.AddComponent<Inventory>(); inventory.capacity = 2;
                Check(inventory.TryAdd(config.fish, 15), "Apilar peces");
                Check(inventory.Stacks.Count == 2 && inventory.Stacks[1].count == 5, "Distribución de pilas");
                Check(!inventory.TryAdd(config.fish, 6), "Rechazo por capacidad");
                Check(inventory.Stacks[1].count == 5, "Adición fallida atómica");
                Check(!inventory.TryAdd(null) && !inventory.TryAdd(config.fish, 0), "Entradas inválidas");
                inventory.RemoveAt(0); Check(inventory.TryAdd(config.zones[0].treasure), "Recoger tesoro tras liberar espacio");
                foreach (var zone in config.zones)
                {
                    StonePuzzleSetup.Validate(obj, zone);
                    Check(zone.stationPosition.y > WorldBuilder.FloorHeight(zone.stationPosition.x, zone.stationPosition.z), "Santuario sobre el terreno");
                }
                Check(-config.zones[0].stationPosition.y < config.initialSafeDepth, "Primera zona alcanzable");
                Check(config.pressureOxygenMultiplier > 1 && !float.IsInfinity(config.pressureOxygenMultiplier), "Consumo peligroso de oxígeno válido");
                for (int i = 1; i < config.zones.Length; i++) Check(-config.zones[i].stationPosition.y < config.zones[i - 1].unlockedDepth, "Progresión alcanzable");
                for (int i = 1; i < config.zones.Length; i++)
                {
                    var zone = config.zones[i]; var previous = config.zones[i - 1];
                    Check(zone.requiredSafeDepth <= previous.unlockedDepth && zone.requiredOxygen <= previous.unlockedOxygen, "Tesoro anterior permite preparar el descenso");
                    Check(zone.entryPosition.y < previous.stationPosition.y - 4 && zone.entryPosition.y > zone.stationPosition.y + 4, "Entrada entre los dos santuarios");
                    Check(zone.barrierWidth >= 12 && zone.requiredSafeDepth >= -zone.stationPosition.y, "Dimensiones y equipo de descenso válidos");
                    if (!zone.hasPhysicalGate) continue;
                    float half = zone.barrierWidth / 2;
                    for (float edge = -half; edge <= half; edge += 1)
                        foreach (float side in new[] { -half, half })
                            Check(WorldBuilder.FloorHeight(zone.entryPosition.x + edge, zone.entryPosition.z + side) > zone.entryPosition.y + 1 &&
                                WorldBuilder.FloorHeight(zone.entryPosition.x + side, zone.entryPosition.z + edge) > zone.entryPosition.y + 1, "Perímetro de compuerta enterrado sin atajos laterales");
                }
                Debug.Log("LA_FOSA_TESTS_OK · Inventario, capacidad atómica, puzles y progresión.");
            }
            finally { UnityEngine.Object.DestroyImmediate(obj); }
        }
        static void Check(bool condition, string message) { if (!condition) throw new Exception("LA_FOSA_TEST_FAILED: " + message); }
        [MenuItem("La Fosa/Compilar Windows")]
        public static void BuildWindows()
        {
            Validate(); Directory.CreateDirectory("Builds/Windows");
            // Allow an isolated preview while the player keeps the previous build open.
            string destination = "Builds/Windows/La Fosa.exe";
            var args = Environment.GetCommandLineArgs();
            if (Array.IndexOf(args, "--expedition-preview") >= 0) destination = "Builds/FirstExpedition/La Fosa.exe";
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { ScenePath }, locationPathName = destination, target = BuildTarget.StandaloneWindows64, options = BuildOptions.None });
            if (report.summary.result != BuildResult.Succeeded) throw new Exception("Falló la compilación: " + report.summary.result);
            Debug.Log("LA_FOSA_BUILD_OK · " + report.summary.totalSize + " bytes");
        }
    }
}
