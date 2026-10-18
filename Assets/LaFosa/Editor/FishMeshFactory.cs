using System.Collections.Generic;
using UnityEngine;

namespace LaFosa.Editor
{
    // Mesh authoring only: all generated models are saved as independent native Unity assets.
    public sealed class FishMeshFactory
    {
        readonly List<Vector3> vertices = new List<Vector3>();
        readonly List<Color> colors = new List<Color>();
        readonly List<int> triangles = new List<int>();
        FishSpeciesDefinition s;
        float L => s.length;
        float W => s.width;
        float H => s.height;
        bool Tapered => s.form == FishForm.Eel || s.form == FishForm.Grenadier || s.form == FishForm.Snailfish;

        public Mesh Build(FishSpeciesDefinition species)
        {
            s = species;
            Body(); Fins(); Eyes(); Features();
            var mesh = new Mesh { name = s.id + " · low poly" };
            mesh.SetVertices(vertices); mesh.SetColors(colors); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return mesh;
        }
        float Profile(float t)
        {
            float curve = Mathf.Max(0, Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI));
            if (Tapered) return Mathf.Max(.025f, Mathf.Pow(t, s.form == FishForm.Grenadier ? 1.9f : 1.15f) * Mathf.Pow(curve, .45f) * 1.8f);
            if (s.form == FishForm.Clingfish) return Mathf.Max(.03f, Mathf.Pow(t, .6f) * Mathf.Pow(curve, .5f) * 1.4f);
            if (s.form == FishForm.Ray) return Mathf.Max(.025f, Mathf.Pow(curve, 1.3f));
            return Mathf.Max(.025f, Mathf.Pow(curve, s.form == FishForm.Reef ? .7f : .9f));
        }
        Vector3 Surface(float t, float angle)
        {
            float profile = Profile(t);
            float y = Mathf.Sin(angle) * H * .5f * profile;
            if (s.form == FishForm.Hatchet) y -= profile * H * .2f;
            return new Vector3(Mathf.Cos(angle) * W * .5f * profile, y, (t - .5f) * L);
        }
        void Body()
        {
            const int rings = 10, sides = 10;
            for (int i = 0; i < rings; i++)
                for (int j = 0; j < sides; j++)
                {
                    float t = i / (float)rings, next = (i + 1f) / rings;
                    float a = j * Mathf.PI * 2 / sides, b = (j + 1f) * Mathf.PI * 2 / sides;
                    Vector3 p = Surface(t, a), q = Surface(t, b), r = Surface(next, a), u = Surface(next, b);
                    Color c = BodyColor((t + next) / 2, (a + b) / 2, i, j);
                    Tri(p, q, r, c); Tri(q, u, r, c * .97f);
                }
            for (int j = 0; j < sides; j++)
            {
                float a = j * Mathf.PI * 2 / sides, b = (j + 1f) * Mathf.PI * 2 / sides;
                Tri(new Vector3(0, 0, -.5f * L), Surface(0, b), Surface(0, a), s.backColor);
                Tri(new Vector3(0, 0, .5f * L), Surface(1, a), Surface(1, b), s.backColor);
            }
        }
        Color BodyColor(float t, float angle, int ring, int side)
        {
            float vertical = Mathf.Sin(angle);
            Color c = Color.Lerp(s.bellyColor, s.backColor, Mathf.InverseLerp(-.75f, .4f, vertical));
            bool marked = false;
            switch (s.pattern)
            {
                case FishPattern.Bands: marked = ring % 3 == 1 && vertical > -.55f; break;
                case FishPattern.Spots: marked = Mathf.Sin(ring * 13.7f + side * 7.1f) > .6f && vertical > -.55f; break;
                case FishPattern.Stripe: marked = Mathf.Abs(vertical) < .33f; break;
                case FishPattern.Waves: marked = Mathf.Sin(t * 44 + angle * 4) > .1f && vertical > .1f; break;
                case FishPattern.Mottled: marked = Mathf.Sin(ring * 5 + side * 2.8f) > .1f && vertical > -.5f; break;
            }
            if (marked) c = Color.Lerp(c, s.accentColor, .82f);
            c *= .94f + .09f * Mathf.Abs(Mathf.Sin(ring * 2.3f + side)); c.a = 0; return c;
        }
        void Fins()
        {
            var fin = Color.Lerp(s.backColor, s.accentColor, .4f); fin.a = 0;
            if (s.form == FishForm.Ray)
            {
                foreach (float side in new[] { -1f, 1f })
                    Fin(new[] { new Vector3(side * W * .15f, 0, L * .36f), new Vector3(side * W * .68f, -.04f, 0), new Vector3(side * W * .18f, 0, -L * .35f) }, fin);
                Cone(new Vector3(0, 0, -L * .25f), new Vector3(0, -.03f, -L * 1.12f), W * .035f, fin);
            }
            else if (s.form == FishForm.Flatfish)
            {
                foreach (float side in new[] { -1f, 1f })
                    Fin(new[] { new Vector3(side * W * .2f, 0, L * .4f), new Vector3(side * W * .65f, -.01f, L * .08f), new Vector3(side * W * .55f, -.01f, -L * .2f), new Vector3(side * W * .1f, 0, -L * .43f) }, fin);
            }
            else
            {
                for (int i = 0; i < s.dorsalCount; i++)
                {
                    float center = s.dorsalCount == 1 ? -.02f : .22f - i * .32f;
                    float half = s.dorsalCount == 1 ? .32f : .12f;
                    var points = new List<Vector3>();
                    points.Add(new Vector3(0, H * .2f, (center - half) * L));
                    for (int k = 0; k < 7; k++)
                    {
                        float z = Mathf.Lerp(center - half, center + half, k / 6f);
                        float baseY = Profile(Mathf.Clamp01(z + .5f)) * H * .5f;
                        float crest = s.dorsalHeight * (.6f + .4f * Mathf.Sin(k / 6f * Mathf.PI));
                        if (s.form == FishForm.Shark || s.form == FishForm.Swordfish) crest *= k == 4 ? 1.5f : .5f;
                        points.Add(new Vector3(0, baseY + crest * (k % 2 == 0 ? 1 : .8f), z * L));
                    }
                    points.Add(new Vector3(0, H * .2f, (center + half) * L));
                    Fin(points.ToArray(), fin);
                }
                foreach (float side in new[] { -1f, 1f })
                {
                    float wing = s.form == FishForm.Chimera ? .38f : s.form == FishForm.Shark ? .3f : .16f;
                    Fin(new[] { new Vector3(side * W * .42f, -H * .08f, L * .19f), new Vector3(side * (W * .35f + L * wing), -H * .3f, -L * .12f), new Vector3(side * W * .3f, -H * .25f, -L * .14f) }, fin);
                }
                Fin(new[] { new Vector3(0, -H * .25f, -L * .02f), new Vector3(0, -H * .5f - s.dorsalHeight * .6f, -L * .22f), new Vector3(0, -H * .2f, -L * .4f) }, fin);
            }
            if (s.form != FishForm.Ray && !Tapered)
            {
                float spread = L * (s.form == FishForm.Blenny || s.form == FishForm.Clingfish ? .12f : .23f);
                float upper = s.form == FishForm.Shark ? 1.55f : 1;
                Fin(new[] { new Vector3(0, H * .06f, -L * .44f), new Vector3(0, spread * upper, -L * .76f), new Vector3(0, 0, -L * .64f), new Vector3(0, -spread, -L * .76f), new Vector3(0, -H * .06f, -L * .44f) }, fin);
            }
            if (Tapered)
                Fin(new[] { new Vector3(0, H * .14f, -L * .25f), new Vector3(0, H * .12f, -L * .63f), new Vector3(0, -H * .1f, -L * .58f), new Vector3(0, -H * .22f, -L * .12f) }, fin);
        }
        void Eyes()
        {
            bool top = s.form == FishForm.Flatfish || s.form == FishForm.Ray || s.form == FishForm.Clingfish;
            Color white = new Color(.83f, .86f, .75f, 0), black = new Color(.018f, .028f, .035f, 0);
            foreach (float side in new[] { -1f, 1f })
            {
                Vector3 p = top ? new Vector3(side * W * .16f, H * .5f, L * (.2f + (side > 0 && s.form == FishForm.Flatfish ? .1f : 0))) : new Vector3(side * W * .5f * Profile(.8f), H * .12f, L * .3f);
                Vector3 shape = top ? new Vector3(s.eyeSize, s.eyeSize * .5f, s.eyeSize) : new Vector3(s.eyeSize * .5f, s.eyeSize, s.eyeSize);
                Ellipsoid(p, shape, white);
                Ellipsoid(p + (top ? Vector3.up : Vector3.right * side) * s.eyeSize * .35f, shape * .69f, black);
            }
        }
        void Features()
        {
            Color mouth = new Color(.09f, .045f, .05f, 0);
            Ellipsoid(new Vector3(0, -H * .075f, L * .465f), new Vector3(W * .09f, H * .055f, L * .012f), mouth);
            if (s.form == FishForm.Swordfish) Cone(new Vector3(0, 0, L * .42f), new Vector3(0, .025f, L * 1.02f), W * .075f, s.backColor);
            if (s.form == FishForm.Chimera)
            {
                Cone(new Vector3(0, -H * .05f, L * .37f), new Vector3(0, -H * .45f, L * .65f), W * .14f, s.bellyColor);
                Ellipsoid(new Vector3(0, -H * .45f, L * .65f), new Vector3(W * .18f, H * .06f, L * .065f), s.backColor);
            }
            if (s.form == FishForm.Clingfish)
            {
                Ellipsoid(new Vector3(0, -H * .53f, L * .14f), new Vector3(W * .28f, H * .15f, L * .15f), s.bellyColor);
                Ellipsoid(new Vector3(0, -H * .66f, L * .14f), new Vector3(W * .2f, H * .045f, L * .105f), s.accentColor);
            }
            if (s.form == FishForm.Wrasse)
            {
                Ellipsoid(new Vector3(0, H * .28f, L * .23f), new Vector3(W * .31f, H * .22f, L * .16f), s.backColor);
                foreach (float side in new[] { -1f, 1f }) Cone(new Vector3(side * W * .07f, -H * .06f, L * .46f), new Vector3(side * W * .07f, -H * .17f, L * .52f), .016f, s.bellyColor);
            }
            if (s.form == FishForm.Blenny)
                foreach (float side in new[] { -1f, 1f }) Cone(new Vector3(side * W * .22f, H * .37f, L * .27f), new Vector3(side * W * .3f, H * .7f, L * .25f), .012f, s.accentColor);
            if (s.form == FishForm.Viper)
            {
                Fin(new[] { new Vector3(-W * .12f, -H * .1f, L * .29f), new Vector3(W * .12f, -H * .1f, L * .29f), new Vector3(W * .1f, -H * .4f, L * .49f), new Vector3(-W * .1f, -H * .4f, L * .49f) }, mouth);
                for (int i = 0; i < 5; i++) foreach (float side in new[] { -1f, 1f })
                    Cone(new Vector3(side * W * .1f, -H * .34f, L * (.29f + i * .043f)), new Vector3(side * W * .08f, H * .02f, L * (.3f + i * .043f)), .012f, s.bellyColor);
            }
            if (s.form == FishForm.Shark)
                for (int i = 0; i < 5; i++) foreach (float side in new[] { -1f, 1f })
                    Fin(new[] { new Vector3(side * W * .44f, H * .12f, L * (.14f - i * .023f)), new Vector3(side * W * .46f, -H * .2f, L * (.12f - i * .023f)), new Vector3(side * W * .46f, -H * .2f, L * (.13f - i * .023f)) }, s.accentColor);
            if (s.pattern == FishPattern.Photophores)
                for (int i = 0; i < 8; i++) foreach (float side in new[] { -1f, 1f })
                {
                    Color glow = s.accentColor; glow.a = 1;
                    float t = .25f + i * .065f;
                    Ellipsoid(new Vector3(side * W * .38f * Profile(t), -H * .3f * Profile(t), (t - .5f) * L), Vector3.one * L * .014f, glow);
                }
        }
        void Ellipsoid(Vector3 center, Vector3 scale, Color color)
        {
            const int rings = 5, sides = 8;
            for (int i = 0; i < rings; i++) for (int j = 0; j < sides; j++)
            {
                float a = Mathf.PI * i / rings, b = Mathf.PI * (i + 1) / rings;
                float u = Mathf.PI * 2 * j / sides, v = Mathf.PI * 2 * (j + 1) / sides;
                Vector3 p = EllipsoidPoint(center, scale, a, u), q = EllipsoidPoint(center, scale, a, v), r = EllipsoidPoint(center, scale, b, u), t = EllipsoidPoint(center, scale, b, v);
                if (i != 0) Tri(p, q, r, color);
                if (i != rings - 1) Tri(q, t, r, color);
            }
        }
        static Vector3 EllipsoidPoint(Vector3 c, Vector3 s, float a, float b) => c + Vector3.Scale(s, new Vector3(Mathf.Sin(a) * Mathf.Cos(b), Mathf.Cos(a), Mathf.Sin(a) * Mathf.Sin(b)));
        void Cone(Vector3 start, Vector3 end, float radius, Color color)
        {
            Vector3 direction = (end - start).normalized;
            Vector3 side = Vector3.Cross(direction, Mathf.Abs(direction.y) > .9f ? Vector3.right : Vector3.up).normalized;
            Vector3 up = Vector3.Cross(direction, side);
            for (int i = 0; i < 6; i++)
            {
                float a = i * Mathf.PI / 3, b = (i + 1) * Mathf.PI / 3;
                Vector3 p = start + (side * Mathf.Cos(a) + up * Mathf.Sin(a)) * radius;
                Vector3 q = start + (side * Mathf.Cos(b) + up * Mathf.Sin(b)) * radius;
                Tri(p, q, end, color); Tri(start, q, p, color);
            }
        }
        void Fin(Vector3[] points, Color color)
        {
            Vector3 center = Vector3.zero; foreach (var p in points) center += p; center /= points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 a = points[i], b = points[(i + 1) % points.Length];
                Color c = color * (i % 2 == 0 ? .87f : 1.07f); c.a = color.a;
                Tri(center, a, b, c); Tri(center, b, a, c);
            }
        }
        void Tri(Vector3 a, Vector3 b, Vector3 c, Color color)
        {
            if (Vector3.Cross(b - a, c - a).sqrMagnitude < .00000000001f) return;
            int i = vertices.Count; vertices.Add(a); vertices.Add(b); vertices.Add(c);
            colors.Add(color); colors.Add(color); colors.Add(color);
            triangles.Add(i); triangles.Add(i + 1); triangles.Add(i + 2);
        }
    }
}
