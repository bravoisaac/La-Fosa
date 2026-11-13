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
        public string DisplayName => kind == GearKind.Boat ? "Lancha de caleta" : "Moto de agua";
        public bool IsMoored { get; private set; }
        public static Vector3 TrenchMooringPosition(float waterLevel) => new Vector3(6, waterLevel, 76);
        public bool NearMooring => Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(6, 76)) <= 8;
        public string MooringHint => IsMoored ? "G soltar amarre" : NearMooring ? "G amarrar en la fosa" : "Amarre: boya sobre la fosa";
        public override string Prompt => "Subir a " + DisplayName + " · E";
        public void Deliver() { gameObject.SetActive(true); }
        public override void Interact(DiverController diver) { TryBoard(diver); }
        public bool TryBoard(DiverController diver)
        {
            if (!gameObject.activeInHierarchy || !session.Economy.Has(kind) || session.Blocked || Passenger != null || diver.Riding != null) return false;
            if (Vector3.Distance(diver.View.transform.position, transform.position + Vector3.up) > 5.5f) return false;
            Passenger = diver; diver.Board(this);
            session.Notify(DisplayName + ": W/S avanzar, A/D girar, E desembarcar" + (kind == GearKind.Boat ? ", B bodega." : "."));
            return true;
        }
        public void Drive(float throttle, float steering, float deltaTime)
        {
            if (Passenger == null || session.Blocked || IsMoored) return;
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
        public bool ToggleMooring()
        {
            if (kind != GearKind.Boat || Passenger == null || session.Blocked || !session.Economy.Has(kind)) return false;
            if (IsMoored)
            {
                IsMoored = false; session.MarkSaveDirty(); session.Notify("Amarre suelto. WASD para navegar."); return true;
            }
            if (!NearMooring) { session.Notify("Acerca la lancha a la boya de amarre sobre la fosa."); return false; }
            Vector3 origin = transform.position, destination = TrenchMooringPosition(session.Config.waterLevel);
            // Check the complete short approach before moving; failed attempts leave the boat in place.
            int steps = Mathf.Max(1, Mathf.CeilToInt(Vector3.Distance(origin, destination) / .25f));
            steps = Mathf.Max(steps, Mathf.CeilToInt(Quaternion.Angle(transform.rotation, Quaternion.identity) / 5));
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                if (!Navigable(Vector3.Lerp(origin, destination, t), Quaternion.Slerp(transform.rotation, Quaternion.identity, t)))
                { session.Notify("Amarre ocupado o aproximación bloqueada. Despeja el paso."); return false; }
            }
            transform.SetPositionAndRotation(destination, Quaternion.identity);
            IsMoored = true; Physics.SyncTransforms(); session.MarkSaveDirty();
            session.Notify("Lancha amarrada. E para bucear; G para soltar el amarre."); return true;
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
                Passenger.LeaveVehicle(point); session.MarkSaveDirty(); session.Notify("En el agua. El vehículo queda aquí para volver a subir."); return true;
            }
            session.Notify("No hay espacio al costado. Aléjate del muelle o pulsa R."); return false;
        }
        public void ClearPassenger() { Passenger = null; }
        internal void Restore(bool owned, Vector3 position, float yaw, bool moored = false)
        {
            Passenger = null;
            IsMoored = owned && kind == GearKind.Boat && moored;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            gameObject.SetActive(owned);
        }
    }
}
