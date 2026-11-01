using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        public static Mesh CreateDeepTrenchMesh(bool section = false)
        {
            var vertices = new List<Vector3>(); var colors = new List<Color>(); var triangles = new List<int>();
            void Face(Vector3 a, Vector3 b, Vector3 c, Vector3 normal, bool wall, bool clipped = false)
            {
                if (section && wall && !clipped)
                {
                    var input = new[] { a, b, c }; var polygon = new List<Vector3>();
                    for (int i = 0; i < 3; i++)
                    {
                        var from = input[i]; var to = input[(i + 1) % 3];
                        bool inside = from.z >= DeepTrenchProfile.Axis.y, nextInside = to.z >= DeepTrenchProfile.Axis.y;
                        if (inside) polygon.Add(from);
                        if (inside != nextInside) polygon.Add(Vector3.Lerp(from, to, (DeepTrenchProfile.Axis.y - from.z) / (to.z - from.z)));
                    }
                    for (int i = 1; i + 1 < polygon.Count; i++) Face(polygon[0], polygon[i], polygon[i + 1], normal, true, true);
                    return;
                }
                if (Vector3.Dot(Vector3.Cross(b - a, c - a), normal) < 0) { var swap = b; b = c; c = swap; }
                float depth = -(a.y + b.y + c.y) / 3;
                Color color = Color.Lerp(new Color(.22f, .44f, .45f), new Color(.2f, .15f, .32f), depth / DeepTrenchProfile.BottomDepth);
                if (section && !wall && depth < 600) color = new Color(.065f, .12f, .15f);
                color *= .9f + .1f * Mathf.Sin(a.x * .7f + a.y * .13f);
                int index = vertices.Count;
                vertices.Add(a); vertices.Add(b); vertices.Add(c);
                colors.Add(color); colors.Add(color); colors.Add(color);
                triangles.Add(index); triangles.Add(index + 1); triangles.Add(index + 2);
            }
            for (float depth = DeepTrenchProfile.EntranceDepth; depth < DeepTrenchProfile.BottomDepth; depth += 4)
            {
                float nextDepth = Mathf.Min(depth + 4, DeepTrenchProfile.BottomDepth);
                for (int side = 0; side < DeepTrenchProfile.Sides; side++)
                {
                    var a = DeepTrenchProfile.Wall(depth, side); var b = DeepTrenchProfile.Wall(depth, side + 1);
                    var c = DeepTrenchProfile.Wall(nextDepth, side); var d = DeepTrenchProfile.Wall(nextDepth, side + 1);
                    var center = DeepTrenchProfile.Center((depth + nextDepth) * .5f);
                    var inward = new Vector3(center.x - (a.x + b.x) * .5f, 0, center.y - (a.z + b.z) * .5f);
                    Face(a, c, b, inward, true); Face(b, c, d, inward, true);
                }
            }
            Vector2 bottom = DeepTrenchProfile.Center(DeepTrenchProfile.BottomDepth);
            for (int side = 0; side < DeepTrenchProfile.Sides; side++)
                Face(new Vector3(bottom.x, -DeepTrenchProfile.BottomDepth, bottom.y), DeepTrenchProfile.Wall(DeepTrenchProfile.BottomDepth, side),
                    DeepTrenchProfile.Wall(DeepTrenchProfile.BottomDepth, side + 1), Vector3.up, false);
            // Two tapered rock ribs split the broad cavity into routes which rejoin below.
            for (int pillar = 0; pillar < 2; pillar++)
            {
                float start = pillar == 0 ? 185 : 270, end = pillar == 0 ? 430 : 525;
                const int sides = 12;
                Vector3 Point(float depth, int side)
                {
                    float angle = side * Mathf.PI * 2 / sides;
                    return DeepTrenchProfile.PillarCenter(pillar, depth) + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * DeepTrenchProfile.PillarRadius(pillar, depth);
                }
                for (float depth = start; depth < end; depth += 5)
                    for (int side = 0; side < sides; side++)
                    {
                        float next = Mathf.Min(depth + 5, end);
                        var a = Point(depth, side); var b = Point(depth, side + 1); var c = Point(next, side); var d = Point(next, side + 1);
                        var outward = (a + b) * .5f - DeepTrenchProfile.PillarCenter(pillar, depth);
                        Face(a, b, c, outward, false); Face(b, d, c, outward, false);
                    }
                for (int side = 0; side < sides; side++)
                {
                    Face(DeepTrenchProfile.PillarCenter(pillar, start), Point(start, side), Point(start, side + 1), Vector3.up, false);
                    Face(DeepTrenchProfile.PillarCenter(pillar, end), Point(end, side), Point(end, side + 1), Vector3.down, false);
                }
            }
            var mesh = new Mesh { name = section ? "Fosa profunda · sección de editor" : "Fosa profunda · cavidad y ramales", indexFormat = IndexFormat.UInt32 };
            mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.SetColors(colors); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return mesh;
        }
        public void BuildDeepTrench()
        {
            var mesh = CreateDeepTrenchMesh();
            var cavity = new GameObject("Fosa profunda · 88 a 650 m"); cavity.transform.SetParent(root, false);
            cavity.AddComponent<MeshFilter>().sharedMesh = mesh;
            cavity.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("LaFosa/Faceted"));
            cavity.AddComponent<MeshCollider>().sharedMesh = mesh;
            var glow = Mat("Referencia profunda", new Color(.25f, .78f, .85f), true);
            for (float depth = 100; depth < DeepTrenchProfile.BottomDepth; depth += 20)
                Shape("Hilo de ascenso · sin colisión", PrimitiveType.Sphere, new Vector3(2, -depth, 77), Vector3.one * .25f, glow, false);
            foreach (var band in config.pressureBands)
            {
                var point = new Vector3(2, -band.depth, 76);
                Label("NIVEL " + config.PressureLevel(band.depth) + " · " + band.depth + " m\nPRESIÓN ↑ · AIRE ×" + band.airMultiplier.ToString("0.##") + "\nRESERVA AIRE PARA ASCENDER",
                    point, .18f, new Color(.65f, 1, .9f));
            }
        }
    }
}
