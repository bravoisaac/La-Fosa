using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    // Renders only the hand model in an editor preview scene. Never enters Play Mode.
    public static class HandsPreview
    {
        [MenuItem("La Fosa/Exportar vistas de manos (sin jugar)")]
        public static void Export()
        {
            const string folder = "Docs/Manos";
            Directory.CreateDirectory(folder);
            var preview = new PreviewRenderUtility();
            var materials = new HashSet<Material>();
            try
            {
                var anchor = new GameObject("Estudio de manos");
                preview.AddSingleGO(anchor);
                var builder = new WorldBuilder(anchor.transform, null);
                var tools = builder.BuildFishingTools(anchor.transform);
                foreach (var renderer in anchor.GetComponentsInChildren<Renderer>(true))
                    foreach (var material in renderer.sharedMaterials) if (material != null) materials.Add(material);
                var hands = tools[0].GetComponent<DiverHands>();
                preview.camera.fieldOfView = 76;
                preview.camera.nearClipPlane = .06f;
                preview.camera.farClipPlane = 10;
                preview.camera.clearFlags = CameraClearFlags.SolidColor;
                preview.camera.backgroundColor = new Color(.055f, .085f, .105f);
                preview.camera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                preview.ambientColor = new Color(.48f, .48f, .48f);
                preview.lights[0].intensity = 1.1f;
                preview.lights[0].transform.rotation = Quaternion.Euler(35, -30, 0);
                preview.lights[1].intensity = .6f;
                preview.lights[1].transform.rotation = Quaternion.Euler(15, 150, 0);
                foreach (float grip in new[] { 0f, .5f, 1f })
                {
                    hands.SetGrip(grip);
                    preview.BeginStaticPreview(new Rect(0, 0, 1600, 900));
                    preview.Render();
                    var image = preview.EndStaticPreview();
                    File.WriteAllBytes(Path.Combine(folder, "manos-" + Mathf.RoundToInt(grip * 100) + ".png"), image.EncodeToPNG());
                    Object.DestroyImmediate(image);
                }
                Debug.Log("LA_FOSA_HANDS_PREVIEW_OK · Model only, no Play Mode.");
            }
            finally
            {
                preview.Cleanup();
                foreach (var material in materials) Object.DestroyImmediate(material);
            }
        }
    }
}
