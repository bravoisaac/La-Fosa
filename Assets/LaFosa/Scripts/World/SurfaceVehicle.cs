using UnityEngine;

namespace LaFosa
{
    public sealed class SurfaceVehicle : Interactable
    {
        public GameSession session;
        public GearKind kind;
        public float speed = 12;
        public Vector3 hullSize = new Vector3(2.2f, .8f, 4.4f);
        public Vector3 SeatPosition => kind == GearKind.Boat ? new Vector3(0, .65f, -.55f) : new Vector3(0, .45f, -.3f);
        public DiverController Passenger { get; private set; }
        public string DisplayName => kind == GearKind.Boat ? "Bote de caleta" : "Moto de agua";
        public override string Prompt => "Subir a " + DisplayName + " · E";
        public void Deliver() { gameObject.SetActive(true); }
        public override void Interact(DiverController diver) { TryBoard(diver); }
        public bool TryBoard(DiverController diver)
        {
            if (!gameObject.activeInHierarchy || !session.Economy.Has(kind) || session.Blocked || Passenger != null || diver.Riding != null) return false;
            if (Vector3.Distance(diver.View.transform.position, transform.position + Vector3.up) > 5.5f) return false;
            Passenger = diver; diver.Board(this);
            session.Notify(DisplayName + ": W/S avanzar, A/D girar, E desembarcar, R regresar a la playa.");
            return true;
        }
        public void Drive(float throttle, float steering, float deltaTime)
        {
            if (Passenger == null || session.Blocked) return;
            throttle = Mathf.Clamp(throttle, -1, 1); steering = Mathf.Clamp(steering, -1, 1);
            // Substeps avoid crossing a dock or rock during a slow frame.
            int steps = Mathf.Clamp(Mathf.CeilToInt(deltaTime / .02f), 1, 30);
            float dt = Mathf.Min(deltaTime, .6f) / steps;
            for (int i = 0; i < steps; i++)
            {
                Quaternion rotation = transform.rotation * Quaternion.Euler(0, steering * 65 * dt, 0);
                Vector3 movement = rotation * Vector3.forward * throttle * speed * dt;
                Vector3 next = transform.position + movement; next.y = session.Config.waterLevel;
                if (!Navigable(next, rotation)) return;
                transform.SetPositionAndRotation(next, rotation);
            }
        }
        bool Navigable(Vector3 position, Quaternion rotation)
        {
            if (Mathf.Abs(position.x) > 100 || position.z < 10 || position.z > 175) return false;
            foreach (float x in new[] { -hullSize.x / 2, hullSize.x / 2 })
                foreach (float z in new[] { -hullSize.z / 2, hullSize.z / 2 })
                {
                    Vector3 point = position + rotation * new Vector3(x, 0, z);
                    if (WorldBuilder.FloorHeight(point.x, point.z) > session.Config.waterLevel - 1.3f) return false;
                }
            foreach (var obstacle in Physics.OverlapBox(position + Vector3.up, new Vector3(hullSize.x / 2, 1.2f, hullSize.z / 2), rotation, ~0, QueryTriggerInteraction.Ignore))
                if (!obstacle.transform.IsChildOf(transform)) return false;
            return true;
        }
        public bool Disembark()
        {
            if (Passenger == null) return false;
            foreach (float side in new[] { 1f, -1f })
            {
                Vector3 point = transform.position + transform.right * side * (hullSize.x / 2 + 1.2f);
                point.y = session.Config.waterLevel - .65f;
                if (WorldBuilder.FloorHeight(point.x, point.z) > point.y - .3f) continue;
                if (Physics.CheckCapsule(point + Vector3.up * .35f, point + Vector3.up * 1.45f, .34f, ~0, QueryTriggerInteraction.Ignore)) continue;
                Passenger.LeaveVehicle(point); session.Notify("En el agua. El vehículo queda aquí para volver a subir."); return true;
            }
            session.Notify("No hay espacio al costado. Aléjate del muelle o pulsa R."); return false;
        }
        public void ClearPassenger() { Passenger = null; }
    }
}
