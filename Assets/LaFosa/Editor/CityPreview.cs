using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class CityPreview
    {
        public static void BuildAndExport() { ProjectSetup.BuildWindows(); Export(); }
        [MenuItem("La Fosa/Exportar ciudad y obras (sin jugar)")]
        public static void Export()
        {
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/LaFosa/Data/Expedicion.asset");
            var preview = new PreviewRenderUtility();
            var root = new GameObject("Vista estática de la ciudad");
            var session = root.AddComponent<GameSession>(); session.Initialize(config);
            var builder = new WorldBuilder(root.transform, config);
            Mesh[] meshes = null; Material[] materials = null;
            try
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                typeof(WorldBuilder).GetMethod("BuildTerrain", flags).Invoke(builder, null);
                typeof(WorldBuilder).GetMethod("BuildCoast", flags).Invoke(builder, null);
                // Overhead clouds would obscure the aerial architectural view, not the player route.
                foreach (Transform child in root.transform) if (child.name == "Nube low poly") child.gameObject.SetActive(false);
                builder.BuildInfrastructure(session);
                var infrastructure = root.GetComponentInChildren<InfrastructureView>();
                foreach (Transform stage in infrastructure.transform) stage.gameObject.SetActive(true);
                preview.AddSingleGO(root);
                preview.camera.orthographic = true; preview.camera.nearClipPlane = .1f; preview.camera.farClipPlane = 800;
                preview.camera.clearFlags = CameraClearFlags.SolidColor; preview.camera.backgroundColor = new Color(.52f, .75f, .83f);
                preview.ambientColor = new Color(.68f, .72f, .72f);
                preview.lights[0].intensity = 1.1f; preview.lights[0].transform.rotation = Quaternion.Euler(45, -30, 0);
                preview.lights[1].intensity = .5f; preview.lights[1].transform.rotation = Quaternion.Euler(25, 150, 0);
                string folder = Path.GetFullPath("Docs/Ciudad"); Directory.CreateDirectory(folder);
                void Render(string file, Vector3 position, Vector3 target, float size)
                {
                    preview.camera.transform.position = position; preview.camera.transform.LookAt(target); preview.camera.orthographicSize = size;
                    foreach (var text in root.GetComponentsInChildren<TextMesh>()) text.transform.rotation = preview.camera.transform.rotation;
                    preview.BeginStaticPreview(new Rect(0, 0, 1600, 1000)); preview.Render();
                    var image = preview.EndStaticPreview(); File.WriteAllBytes(Path.Combine(folder, file), image.EncodeToPNG()); Object.DestroyImmediate(image);
                }
                Render("ciudad.png", new Vector3(55, 48, 26), new Vector3(0, 4, -29), 29);
                Render("muelle-base.png", new Vector3(65, 65, 120), new Vector3(0, 1, 42), 53);
                Debug.Log("LA_FOSA_CITY_PREVIEW_OK · Dos vistas estáticas, sin Play Mode ni partida.");
            }
            finally
            {
                foreach (var view in root.GetComponentsInChildren<InfrastructureView>(true)) view.Release();
                meshes = root.GetComponentsInChildren<MeshFilter>(true).Select(m => m.sharedMesh).Where(m => m != null && !AssetDatabase.Contains(m) && (m.name == "Octaedro low poly" || m.name.Contains("Costa y fosa"))).Distinct().ToArray();
                materials = root.GetComponentsInChildren<Renderer>(true).Select(r => r.sharedMaterial).Where(m => m != null && !AssetDatabase.Contains(m)).Distinct().ToArray();
                preview.Cleanup();
                if (root != null) Object.DestroyImmediate(root);
                foreach (var mesh in meshes) Object.DestroyImmediate(mesh);
                foreach (var material in materials) Object.DestroyImmediate(material);
                Time.timeScale = 1;
            }
        }
    }
}
