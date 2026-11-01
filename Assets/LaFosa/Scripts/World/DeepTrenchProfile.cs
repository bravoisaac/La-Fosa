using UnityEngine;

namespace LaFosa
{
    // A true 3D cavity below the existing shelf; FloorHeight describes only the upper terrain.
    public static class DeepTrenchProfile
    {
        public const float EntranceDepth = 88, BottomDepth = 650;
        public static readonly Vector2 Axis = new Vector2(0, 76);
        public const int Sides = 32;
        static readonly float[] Depths = { 88, 104, 140, 166, 215, 280, 340, 420, 480, 560, 620, 650 };
        static readonly Vector2[] Widths =
        {
            new Vector2(4, 4), new Vector2(9, 8), new Vector2(15, 12), new Vector2(10, 11),
            new Vector2(25, 19), new Vector2(23, 21), new Vector2(31, 25), new Vector2(24, 22),
            new Vector2(37, 30), new Vector2(44, 35), new Vector2(55, 43), new Vector2(52, 41)
        };
        public static Vector2 Center(float depth)
        {
            float t = Mathf.Clamp01((depth - EntranceDepth) / 55);
            return Axis + t * new Vector2(Mathf.Sin(depth * .018f) * 6, Mathf.Sin(depth * .012f) * 4);
        }
        public static Vector2 Width(float depth)
        {
            for (int i = 1; i < Depths.Length; i++)
                if (depth <= Depths[i]) return Vector2.Lerp(Widths[i - 1], Widths[i], Mathf.InverseLerp(Depths[i - 1], Depths[i], depth));
            return Widths[Widths.Length - 1];
        }
        public static Vector3 Wall(float depth, int side)
        {
            float angle = side * Mathf.PI * 2 / Sides;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 square = direction / Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y)) * 4;
            float shape = 1 + .11f * Mathf.Sin(angle * 3 + depth * .031f) + .06f * Mathf.Sin(angle * 7 - depth * .047f);
            // Side chambers and irregular ledges; the broad lower room remains connected.
            float alcove = Mathf.Pow(Mathf.Max(0, Mathf.Cos(angle - .4f)), 12) * Mathf.Exp(-Mathf.Pow((depth - 235) / 32, 2)) * 15;
            alcove += Mathf.Pow(Mathf.Max(0, -Mathf.Cos(angle)), 12) * Mathf.Exp(-Mathf.Pow((depth - 375) / 42, 2)) * 18;
            Vector2 ellipse = Vector2.Scale(direction, Width(depth)) * shape + direction * alcove;
            Vector2 point = Center(depth) + Vector2.Lerp(square, ellipse, Mathf.Clamp01((depth - EntranceDepth) / 16));
            return new Vector3(point.x, -depth, point.y);
        }
        public static bool IsOpeningCell(float x, float z, float step)
            => x >= -4 && x + step <= 4 && z >= 72 && z + step <= 80;

        public static Vector3 PillarCenter(int pillar, float depth)
        {
            float start = pillar == 0 ? 185 : 270, end = pillar == 0 ? 430 : 525;
            float t = Mathf.InverseLerp(start, end, depth);
            Vector3 wall = Wall(depth, pillar == 0 ? Sides / 2 : 0);
            Vector3 inner = new Vector3((pillar == 0 ? -11 : 13) + Mathf.Sin(depth * .025f + pillar) * 3,
                -depth, Axis.y + (pillar == 0 ? -1 : 3));
            return Vector3.Lerp(wall, inner, Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * Mathf.PI)), .45f));
        }
        public static float PillarRadius(int pillar, float depth)
        {
            float start = pillar == 0 ? 185 : 270, end = pillar == 0 ? 430 : 525;
            float t = Mathf.InverseLerp(start, end, depth);
            return Mathf.Max(.15f, Mathf.Sin(t * Mathf.PI) * (pillar == 0 ? 5 : 6));
        }
    }
}
