using UnityEngine;

namespace LaFosa
{
    public sealed partial class FishAgent
    {
        readonly RaycastHit[] hits = new RaycastHit[32];
        static readonly float[] Angles = { 30, -30, 60, -60, 90, -90, 120, -120, 150, -150, 180 };
        float Clearance => species == null ? .25f : Mathf.Max(.14f, Mathf.Max(species.width, species.height) * .52f);
        bool IsObstacle(Collider collider) => collider != null && collider.GetComponentInParent<FishAgent>() == null && collider.GetComponentInParent<DiverController>() == null;
        bool CanSee(Vector3 point)
        {
            Vector3 delta = point - transform.position;
            int count = Physics.RaycastNonAlloc(transform.position, delta.normalized, hits, delta.magnitude, ~0, QueryTriggerInteraction.Ignore);
            if (count == hits.Length) return false;
            for (int i = 0; i < count; i++) if (IsObstacle(hits[i].collider)) return false;
            return true;
        }
        public bool WithinHabitat(Vector3 point)
        {
            if (Mathf.Abs(point.x) > 140 || point.z < 0 || point.z > 195) return false;
            float depth = waterLevel - point.y;
            if (species != null && (depth < species.playableDepth.x || depth > species.playableDepth.y)) return false;
            if (depth < .5f) return false;
            float r = Clearance;
            float floor = Mathf.Max(WorldBuilder.FloorHeight(point.x - r, point.z), WorldBuilder.FloorHeight(point.x + r, point.z), WorldBuilder.FloorHeight(point.x, point.z - r), WorldBuilder.FloorHeight(point.x, point.z + r));
            return point.y > floor + r + .15f;
        }
        bool CanMove(Vector3 direction, float distance)
        {
            Vector3 origin = transform.position;
            // Check the nose as well as the centre when approaching a trench wall.
            float ahead = Mathf.Max(distance, species != null ? species.length * .6f : .4f);
            if (!WithinHabitat(origin + direction * distance) || !WithinHabitat(origin + direction * ahead)) return false;
            int count = Physics.SphereCastNonAlloc(origin, Clearance, direction, hits, ahead, ~0, QueryTriggerInteraction.Ignore);
            if (count == hits.Length) return false;
            for (int i = 0; i < count; i++) if (IsObstacle(hits[i].collider)) return false;
            return true;
        }
        Vector3 SafeDirection(Vector3 wanted, float distance)
        {
            if (wanted.sqrMagnitude < .01f || distance <= 0) return Vector3.zero;
            if (CanMove(wanted, distance)) return wanted;
            Vector3 flat = Vector3.ProjectOnPlane(wanted, Vector3.up).normalized;
            if (flat.sqrMagnitude < .01f) flat = transform.forward;
            if (CanMove(flat, distance)) return flat;
            // Prefer sliding sideways around the obstruction, then turning back as a last resort.
            foreach (float angle in Angles)
            {
                Vector3 candidate = Quaternion.AngleAxis(angle, Vector3.up) * flat;
                if (CanMove(candidate, distance)) return candidate;
            }
            foreach (float vertical in new[] { .7f, -.7f })
            {
                Vector3 candidate = (flat + Vector3.up * vertical).normalized;
                if (CanMove(candidate, distance)) return candidate;
            }
            return Vector3.zero;
        }
    }
}
