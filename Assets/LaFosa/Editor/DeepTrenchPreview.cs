using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class DeepTrenchPreview
    {
        public static void BuildAndExport() { ProjectSetup.BuildWindows(); Export(); }
        [MenuItem("La Fosa/Exportar sección profunda (sin jugar)")]
        public static void Export()
        {
            const string folder = "Docs/Mapa";
            Directory.CreateDirectory(folder);
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/LaFosa/Data/Expedicion.asset");
            var preview = new PreviewRenderUtility();
            var materials = new HashSet<Material>();
            Mesh mesh = null;
            try
            {
                var root = new GameObject("Sección de geometría · editor"); preview.AddSingleGO(root);
                mesh = WorldBuilder.CreateDeepTrenchMesh(true);
                var shell = new GameObject("Cavidad con frente retirado"); shell.transform.SetParent(root.transform, false);
                shell.AddComponent<MeshFilter>().sharedMesh = mesh;
                var material = new Material(Shader.Find("LaFosa/Faceted")); materials.Add(material);
                shell.AddComponent<MeshRenderer>().sharedMaterial = material;
                // Horizontal exaggeration is disclosed on the image; depth remains linear.
                shell.transform.localScale = new Vector3(1.6f, 1, 1);
                var lineMaterial = new Material(Shader.Find("Sprites/Default")); materials.Add(lineMaterial);
                void Line(Vector3 a, Vector3 b, Color color, float width = .35f)
                {
                    var go = new GameObject("Referencia"); go.transform.SetParent(root.transform, false);
                    var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = lineMaterial; line.positionCount = 2;
                    line.SetPositions(new[] { a, b }); line.startWidth = line.endWidth = width;
                    line.startColor = line.endColor = color;
                }
                void Label(string text, Vector3 position, float size, Color color)
                {
                    var go = new GameObject("Rótulo"); go.transform.SetParent(root.transform, false); go.transform.position = position;
                    var label = go.AddComponent<TextMesh>(); label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    label.text = text; label.fontSize = 64; label.characterSize = size; label.anchor = TextAnchor.MiddleLeft; label.color = color;
                    var fontMaterial = new Material(label.font.material); materials.Add(fontMaterial);
                    go.GetComponent<Renderer>().sharedMaterial = fontMaterial;
                }
                Color mint = new Color(.4f, .95f, .85f);
                Label("LA FOSA / 650 m", new Vector3(-130, 42, -40), 1.15f, Color.white);
                Label("Sección estática de la geometría · sin jugar", new Vector3(-130, 24, -40), .75f, mint);
                Label("Ancho visual ×1,6 / profundidad a escala", new Vector3(-130, 12, -40), .65f, Color.gray);
                // Existing upper approach: sample the terrain function along the cave axis.
                for (int side = -1; side <= 1; side += 2)
                    for (float depth = 9; depth < 88; depth += 1)
                    {
                        float Radius(float d)
                        {
                            d = Mathf.Min(d, 87.999f);
                            for (float x = 0; x <= 32; x += .05f) if (WorldBuilder.FloorHeight(x, 76) >= -d) return x;
                            return 32;
                        }
                        Line(new Vector3(side * Radius(depth) * 1.6f, -depth, 0), new Vector3(side * Radius(depth + 1) * 1.6f, -depth - 1, 0), mint, .55f);
                    }
                Line(new Vector3(-14.4f, -88, 0), new Vector3(-6.4f, -88, 0), mint);
                Line(new Vector3(6.4f, -88, 0), new Vector3(14.4f, -88, 0), mint);
                foreach (var band in config.pressureBands)
                {
                    Line(new Vector3(-130, -band.depth, -30), new Vector3(130, -band.depth, -30), new Color(.45f, .8f, .83f, .6f));
                    Label("N" + config.PressureLevel(band.depth) + "   " + band.depth + " m   /   aire ×" + band.airMultiplier.ToString("0.##"),
                        new Vector3(-130, -band.depth + 9, -40), .9f, Color.white);
                }
                Label("Líneas = presión. No hay suelos ni puertas entre niveles.", new Vector3(-130, -676, -40), .7f, mint);
                Label("Cámara inferior / 650 m · balance de aire provisional", new Vector3(-130, -690, -40), .7f, Color.white);
                preview.camera.orthographic = true; preview.camera.orthographicSize = 380;
                preview.camera.nearClipPlane = .1f; preview.camera.farClipPlane = 1400;
                preview.camera.transform.SetPositionAndRotation(new Vector3(0, -325, -900), Quaternion.identity);
                preview.camera.clearFlags = CameraClearFlags.SolidColor; preview.camera.backgroundColor = new Color(.025f, .045f, .07f);
                preview.ambientColor = new Color(.7f, .7f, .7f);
                preview.lights[0].intensity = 1.2f; preview.lights[0].transform.rotation = Quaternion.Euler(25, 150, 0);
                preview.lights[1].intensity = .7f; preview.lights[1].transform.rotation = Quaternion.Euler(-20, -40, 0);
                preview.BeginStaticPreview(new Rect(0, 0, 1000, 2400)); preview.Render();
                var image = preview.EndStaticPreview();
                File.WriteAllBytes(Path.Combine(folder, "fosa-seccion.png"), image.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(image);
                Debug.Log("LA_FOSA_DEEP_PREVIEW_OK · Sección de malla en editor, sin Play Mode.");
            }
            finally
            {
                preview.Cleanup();
                if (mesh != null) UnityEngine.Object.DestroyImmediate(mesh);
                foreach (var material in materials) UnityEngine.Object.DestroyImmediate(material);
            }
        }
    }
}
