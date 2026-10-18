using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class FishCatalogBuilder
    {
        const string Data = "Assets/LaFosa/Data/FishSpecies";
        const string Models = "Assets/LaFosa/Models/Fish";
        public static void Ensure(GameConfig config)
        {
            if (config.fishSpecies != null && config.fishSpecies.Length > 0)
            {
                if (config.fishSpecies.Any(s => s == null || s.prefab == null || s.item == null)) throw new Exception("Hay fichas de fauna sin modelo u objeto. Completa sus referencias antes de preparar la escena.");
                return;
            }
            Generate(config);
        }
        [MenuItem("La Fosa/Fauna/Regenerar los 49 modelos originales")]
        public static void Regenerate()
        {
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/LaFosa/Data/Expedicion.asset");
            if (config == null) { ProjectSetup.Setup(); return; }
            Generate(config);
        }
        public static void RebuildAndBuild() { Regenerate(); ProjectSetup.BuildWindows(); }
        static void Generate(GameConfig config)
        {
            foreach (var path in new[] { Data, Data + "/Items", Models, Models + "/Meshes", Models + "/Materials" }) Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
            var shader = Shader.Find("LaFosa/Fish");
            if (shader == null) throw new Exception("No se encontró el shader de fauna.");
            var species = new List<FishSpeciesDefinition>();
            foreach (var recipe in FishRecipes.Create())
            {
                ItemDefinition item;
                if (recipe.id == "jurel") { item = config.fish; recipe.item.name = "jurel"; EditorUtility.CopySerialized(recipe.item, item); EditorUtility.SetDirty(item); UnityEngine.Object.DestroyImmediate(recipe.item); }
                else item = Save(recipe.item, Data + "/Items/" + recipe.id + ".asset");
                recipe.item = item;
                var definition = Save(recipe, Data + "/" + recipe.id + ".asset");
                var mesh = Save(new FishMeshFactory().Build(definition), Models + "/Meshes/" + definition.id + ".asset");
                var material = new Material(shader) { name = definition.displayName };
                material.SetFloat("_Length", definition.length); material.SetFloat("_Swim", definition.form == FishForm.Eel ? .065f : .025f);
                material.SetFloat("_Speed", definition.form == FishForm.Pelagic ? 5 : 3);
                material = Save(material, Models + "/Materials/" + definition.id + ".mat");
                var root = new GameObject(definition.displayName);
                try
                {
                    root.AddComponent<MeshFilter>().sharedMesh = mesh;
                    root.AddComponent<MeshRenderer>().sharedMaterial = material;
                    var body = root.AddComponent<BoxCollider>();
                    body.size = new Vector3(Mathf.Max(.12f, definition.width), Mathf.Max(.12f, definition.height), definition.length);
                    var fish = root.AddComponent<FishAgent>(); fish.species = definition; fish.item = item;
                    fish.speed = definition.swimSpeed; fish.radius = definition.habitatBand == 0 ? .65f : .85f;
                    definition.prefab = PrefabUtility.SaveAsPrefabAsset(root, Models + "/" + definition.id + ".prefab");
                    EditorUtility.SetDirty(definition); species.Add(definition);
                }
                finally { UnityEngine.Object.DestroyImmediate(root); }
            }
            config.fishSpecies = species.ToArray(); EditorUtility.SetDirty(config); AssetDatabase.SaveAssets();
            Directory.CreateDirectory("Docs");
            File.WriteAllLines("Docs/catalogo-peces.tsv", new[] { "id\tnombre\ttaxon\tfranja\tprofundidad_documento\tprofundidad_juego\tvalor\ttriangulos" }.Concat(species.Select(s => s.id + "\t" + s.displayName + "\t" + s.scientificName + "\t" + s.habitatBand + "\t" + s.referenceDepth + "\t" + s.playableDepth.x + "–" + s.playableDepth.y + " m\t" + s.item.salePrice + "\t" + s.prefab.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3)));
            Validate(config);
            Debug.Log("LA_FOSA_FAUNA_OK · 49 modelos, materiales, prefabs y fichas independientes.");
        }
        static T Save<T>(T source, string path) where T : UnityEngine.Object
        {
            source.name = Path.GetFileNameWithoutExtension(path);
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing == null) { AssetDatabase.CreateAsset(source, path); return source; }
            EditorUtility.CopySerialized(source, existing); EditorUtility.SetDirty(existing); UnityEngine.Object.DestroyImmediate(source); return existing;
        }
        public static void Validate(GameConfig config)
        {
            var ids = new HashSet<string>(); var meshes = new HashSet<Mesh>(); var shapes = new HashSet<string>();
            if (config.fishSpecies == null || config.fishSpecies.Length == 0) throw new Exception("Catálogo vacío.");
            foreach (var s in config.fishSpecies)
            {
                if (s == null || !ids.Add(s.id) || s.item == null || !s.item.isFish || s.item.salePrice <= 0 || s.prefab == null) throw new Exception("Ficha de fauna inválida.");
                var mesh = s.prefab.GetComponent<MeshFilter>().sharedMesh;
                if (mesh == null || !meshes.Add(mesh) || mesh.vertexCount < 100 || !shapes.Add(string.Join(";", mesh.vertices.Select(v => v.ToString("F5"))))) throw new Exception("Modelo repetido o inválido: " + s.id);
                if (mesh.vertices.Any(v => float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsInfinity(v.sqrMagnitude))) throw new Exception("Vértices inválidos: " + s.id);
                if (s.prefab.GetComponent<FishAgent>().species != s || s.prefab.GetComponent<Collider>() == null) throw new Exception("Prefab sin conexión: " + s.id);
            }
        }
    }
}
