using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        GameObject BuildHands(Transform camera)
        {
            var hands = new GameObject("Manos · equipo inicial");
            hands.transform.SetParent(camera, false);
            var pose = hands.AddComponent<DiverHands>();
            var skin = Mat("Manos del buzo", new Color(.64f, .42f, .29f));
            var sleeve = Mat("Mangas del traje", new Color(.06f, .12f, .15f));
            var cuff = Mat("Puños de neopreno", new Color(.035f, .075f, .09f));
            var seam = Mat("Costura del traje", new Color(.19f, .39f, .38f));
            foreach (float side in new[] { -1f, 1f })
            {
                var wrist = new GameObject(side < 0 ? "Mano izquierda" : "Mano derecha").transform;
                wrist.SetParent(hands.transform, false);
                wrist.localPosition = new Vector3(side * .235f, -.255f, .45f);
                wrist.localRotation = Quaternion.Euler(-7, -side * 10, side * 9);
                // Rings store half-width, half-thickness and longitudinal position.
                HandSection("Manga entallada", wrist, sleeve, pose,
                    new Vector3(.053f, .047f, -.27f), new Vector3(.044f, .037f, -.12f), new Vector3(.033f, .026f, -.025f));
                HandSection("Puño", wrist, cuff, pose,
                    new Vector3(.035f, .029f, -.045f), new Vector3(.035f, .029f, -.018f));
                HandSection("Costura", wrist, seam, pose,
                    new Vector3(.0355f, .0295f, -.039f), new Vector3(.0355f, .0295f, -.035f));
                HandSection("Muñeca y palma", wrist, skin, pose,
                    new Vector3(.028f, .021f, -.02f), new Vector3(.029f, .022f, .015f),
                    new Vector3(.044f, .024f, .065f), new Vector3(.046f, .021f, .095f),
                    new Vector3(.042f, .017f, .111f));

                float[] lengths = { .077f, .086f, .079f, .061f };
                string[] names = { "Índice", "Medio", "Anular", "Meñique" };
                for (int finger = 0; finger < 4; finger++)
                {
                    float width = finger == 3 ? .008f : .0095f;
                    var joint = new GameObject(names[finger]).transform;
                    joint.SetParent(wrist, false);
                    joint.localPosition = new Vector3(side * (-.032f + finger * .021f), 0, finger == 3 ? .098f : .108f);
                    joint.localRotation = Quaternion.Euler(12 + finger * 3, side * (finger - 1.5f) * 4, 0);
                    BuildFinger(joint, lengths[finger], width, skin, pose, false);
                }
                var thumb = new GameObject("Pulgar opuesto").transform;
                thumb.SetParent(wrist, false);
                thumb.localPosition = new Vector3(-side * .035f, -.009f, .039f);
                thumb.localRotation = Quaternion.Euler(18, -side * 43, -side * 28);
                BuildFinger(thumb, .067f, .012f, skin, pose, true);
            }
            return hands;
        }

        void BuildFinger(Transform rootJoint, float length, float width, Material skin, DiverHands pose, bool thumb)
        {
            var joint = rootJoint;
            float[] proportions = thumb ? new[] { .55f, .45f } : new[] { .45f, .32f, .23f };
            for (int part = 0; part < proportions.Length; part++)
            {
                float segment = length * proportions[part];
                pose.RegisterJoint(joint, thumb ? 25 : part == 1 ? 62 : 46);
                float taper = 1 - part * .12f;
                HandSection("Falange " + (part + 1), joint, skin, pose,
                    new Vector3(width * taper, width * .88f * taper, -.003f),
                    new Vector3(width * taper, width * .88f * taper, segment * .72f),
                    new Vector3(width * taper * .72f, width * .67f * taper, segment));
                if (part == proportions.Length - 1) break;
                var next = new GameObject("Articulación " + (part + 2)).transform;
                next.SetParent(joint, false);
                next.localPosition = Vector3.forward * (segment - .001f);
                next.localRotation = Quaternion.Euler(thumb ? 12 : 16, 0, 0);
                joint = next;
            }
        }

        static void HandSection(string name, Transform parent, Material material, DiverHands owner, params Vector3[] rings)
        {
            // Beveled octagonal sections give a soft silhouette with flat, deliberate facets.
            var outline = new[] { new Vector2(-.65f, -1), new Vector2(.65f, -1), new Vector2(1, -.55f), new Vector2(1, .55f),
                new Vector2(.65f, 1), new Vector2(-.65f, 1), new Vector2(-1, .55f), new Vector2(-1, -.55f) };
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            Vector3 Point(int ring, int corner) => new Vector3(outline[corner].x * rings[ring].x, outline[corner].y * rings[ring].y, rings[ring].z);
            void Triangle(Vector3 a, Vector3 b, Vector3 c)
            {
                int index = vertices.Count;
                vertices.Add(a); vertices.Add(b); vertices.Add(c);
                triangles.Add(index); triangles.Add(index + 1); triangles.Add(index + 2);
            }
            for (int ring = 0; ring < rings.Length - 1; ring++)
                for (int corner = 0; corner < 8; corner++)
                {
                    int next = (corner + 1) % 8;
                    Triangle(Point(ring, corner), Point(ring, next), Point(ring + 1, next));
                    Triangle(Point(ring, corner), Point(ring + 1, next), Point(ring + 1, corner));
                }
            for (int corner = 0; corner < 8; corner++)
            {
                int next = (corner + 1) % 8;
                Triangle(new Vector3(0, 0, rings[0].z), Point(0, next), Point(0, corner));
                Triangle(new Vector3(0, 0, rings[rings.Length - 1].z), Point(rings.Length - 1, corner), Point(rings.Length - 1, next));
            }
            var mesh = new Mesh { name = name + " · facetado" };
            mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            owner.OwnMesh(mesh);
            var obj = new GameObject(name); obj.transform.SetParent(parent, false);
            obj.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = obj.AddComponent<MeshRenderer>(); renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }
}
