using UnityEngine;

namespace LaFosa
{
    [RequireComponent(typeof(CharacterController), typeof(Inventory))]
    public sealed class DiverController : MonoBehaviour
    {
        public GameSession Session { get; private set; }
        public Inventory Inventory { get; private set; }
        public Camera View { get; private set; }
        public float Oxygen { get; private set; }
        public float MaxOxygen { get; private set; }
        public float SafeDepth { get; private set; }
        public float Health { get; private set; } = 100;
        public float Depth => Mathf.Max(0, Session.Config.waterLevel - View.transform.position.y);
        public bool Underwater => Depth > .15f;
        public float PressureOxygenMultiplier => Depth > SafeDepth ? Mathf.Max(1, Session.Config.pressureOxygenMultiplier) : 1;
        public Interactable Target { get; private set; }
        public FishAgent ObservedFish { get; private set; }
        public float sensitivity = 2;
        public SurfaceVehicle Riding { get; private set; }
        public float SwimSpeedMultiplier => Session.Economy.Has(GearKind.Fins) ? Session.Config.shop.finSpeedMultiplier : 1;
        CharacterController controller;
        float baseOxygen, oxygenBonus;
        float pitch, verticalSpeed;
        Light lamp;

        public void Initialize(GameSession session, Camera view)
        {
            Session = session; View = view;
            controller = GetComponent<CharacterController>();
            Inventory = GetComponent<Inventory>();
            Inventory.capacity = session.Config.inventorySlots;
            baseOxygen = session.Config.initialOxygen; MaxOxygen = baseOxygen; Oxygen = MaxOxygen;
            SafeDepth = session.Config.initialSafeDepth;
            lamp = view.gameObject.AddComponent<Light>();
            lamp.type = LightType.Spot; lamp.range = 45; lamp.spotAngle = 85; lamp.intensity = 3;
            lamp.color = new Color(.65f, .92f, 1);
        }

        void Update()
        {
            if (Session == null || Session.Blocked) return;
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, -85, 85);
            View.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            if (Input.GetKeyDown(KeyCode.F)) lamp.enabled = !lamp.enabled;
            Move(); UpdateVitals(); UpdateTarget(); UpdateAtmosphere();
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Riding != null) Riding.Disembark();
                else if (Target != null) Target.Interact(this);
            }
            if (Input.GetKeyDown(KeyCode.R)) Rescue("Regreso a la playa. Tu inventario se conserva.");
        }
        void Move()
        {
            float x = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
            float z = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
            if (Riding != null) { Riding.Drive(z, x, Time.deltaTime); return; }
            bool swimming = transform.position.y < Session.Config.waterLevel - .6f;
            Vector3 move;
            if (swimming)
            {
                float y = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C) ? 1 : 0);
                move = View.transform.forward * z + transform.right * x + Vector3.up * y;
                move = Vector3.ClampMagnitude(move, 1) * (Input.GetKey(KeyCode.LeftShift) ? 9 : 6) * SwimSpeedMultiplier;
                verticalSpeed = 0;
            }
            else
            {
                move = Vector3.ClampMagnitude(transform.forward * z + transform.right * x, 1) * (Input.GetKey(KeyCode.LeftShift) ? 8 : 5);
                if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
                if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space)) verticalSpeed = 6;
                verticalSpeed -= 18 * Time.deltaTime;
                move.y = verticalSpeed;
            }
            controller.Move(move * Time.deltaTime);
            if (transform.position.y < -120 || Mathf.Abs(transform.position.x) > 145 || transform.position.z > 200 || transform.position.z < -85)
                Rescue("La corriente te devuelve al campamento.");
        }
        void UpdateVitals()
        {
            AdvanceVitals(Time.deltaTime, Input.GetKey(KeyCode.LeftShift));
        }
        internal void AdvanceVitals(float deltaTime, bool sprint)
        {
            if (Session.Blocked || deltaTime <= 0) return;
            float consumption = (sprint ? 1.5f : 1) * PressureOxygenMultiplier;
            Oxygen = Mathf.Clamp(Oxygen + (Underwater ? -consumption : 22) * deltaTime, 0, MaxOxygen);
            float damage = Oxygen <= 0 ? 16 : 0;
            Health = Mathf.Clamp(Health + (damage > 0 ? -damage : !Underwater ? 14 : 0) * deltaTime, 0, 100);
            if (Health <= 0) Rescue("Rescate de emergencia. Recuperaste tu equipo y tus hallazgos.");
        }
        void UpdateTarget()
        {
            Target = null;
            ObservedFish = null;
            if (Riding != null) return;
            if (Physics.Raycast(View.transform.position, View.transform.forward, out var hit, 4.5f, ~0, QueryTriggerInteraction.Ignore))
            {
                Target = hit.collider.GetComponentInParent<Interactable>();
                ObservedFish = hit.collider.GetComponentInParent<FishAgent>();
            }
        }
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            hit.collider.GetComponentInParent<DiveGate>()?.NotifyBlocked(this);
        }
        void UpdateAtmosphere()
        {
            float t = Mathf.Clamp01(Depth / 95);
            float trench = WorldBuilder.TrenchBlend(View.transform.position);
            if (RenderSettings.sun != null) RenderSettings.sun.intensity = Underwater ? Mathf.Lerp(.65f, Mathf.Lerp(.48f, .07f, t), trench) : 1.1f;
            lamp.intensity = Underwater ? 1.4f : 0;
            Color water = Color.Lerp(new Color(.045f, .3f, .29f), Color.Lerp(new Color(.035f, .23f, .24f), new Color(.009f, .025f, .045f), t), trench);
            RenderSettings.fogColor = Underwater ? water : new Color(.64f, .79f, .81f);
            float shallowFog = Mathf.Clamp(Session.Config.shallowFogDensity, .01f, .2f);
            float deepFog = Mathf.Clamp(Session.Config.deepFogDensity, shallowFog, .2f);
            float coastalFog = Mathf.Clamp(Session.Config.coastalFogDensity, .01f, shallowFog);
            RenderSettings.fogDensity = Underwater ? Mathf.Lerp(coastalFog, Mathf.Lerp(shallowFog, deepFog, t), trench) : .0035f;
            View.backgroundColor = Underwater ? water : new Color(.52f, .75f, .83f);
            RenderSettings.ambientLight = Underwater ? Color.Lerp(new Color(.32f, .52f, .49f), Color.Lerp(new Color(.26f, .44f, .43f), new Color(.08f, .14f, .19f), t), trench) : new Color(.7f, .77f, .78f);
        }
        public void Upgrade(float depth, float oxygen)
        {
            SafeDepth = Mathf.Max(SafeDepth, depth); baseOxygen = Mathf.Max(baseOxygen, oxygen); MaxOxygen = baseOxygen + oxygenBonus;
            Oxygen = MaxOxygen; Health = 100;
        }
        internal void RestoreEquipment()
        {
            baseOxygen = Session.Config.initialOxygen; oxygenBonus = 0;
            SafeDepth = Session.Config.initialSafeDepth; MaxOxygen = baseOxygen;
            foreach (var zone in Session.Config.zones)
                if (Session.IsTreasureClaimed(zone)) Upgrade(zone.unlockedDepth, zone.unlockedOxygen);
            if (Session.Economy.Has(GearKind.OxygenTank)) ApplyOxygenTank(Session.Config.shop.oxygenBonus);
            Rescue("Expedición recuperada en la playa.");
        }
        public void ApplyOxygenTank(float extraSeconds)
        {
            oxygenBonus = Mathf.Max(oxygenBonus, extraSeconds);
            MaxOxygen = baseOxygen + oxygenBonus; Oxygen = MaxOxygen;
        }
        public void Board(SurfaceVehicle vehicle)
        {
            controller.enabled = false; Riding = vehicle; verticalSpeed = 0;
            transform.SetParent(vehicle.transform, false); transform.localPosition = vehicle.SeatPosition;
            transform.localRotation = Quaternion.identity; pitch = 0; View.transform.localRotation = Quaternion.identity;
            Target = null;
        }
        public void LeaveVehicle(Vector3 position)
        {
            if (Riding != null) Riding.ClearPassenger();
            Riding = null; transform.SetParent(Session.transform, true); transform.position = position;
            controller.enabled = true; verticalSpeed = 0;
        }
        public void LookAt(Vector3 point)
        {
            Vector3 direction = point - View.transform.position;
            if (direction.sqrMagnitude < .001f) return;
            transform.rotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0);
            pitch = -Mathf.Asin(direction.normalized.y) * Mathf.Rad2Deg;
            View.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
        public void Rescue(string message)
        {
            if (Riding != null) LeaveVehicle(Session.Config.spawn);
            controller.enabled = false; transform.position = Session.Config.spawn;
            transform.rotation = Quaternion.identity; pitch = 0; View.transform.localRotation = Quaternion.identity;
            controller.enabled = true; verticalSpeed = 0; Oxygen = MaxOxygen; Health = 100;
            Target = null; Session.Notify(message);
            Session.MarkSaveDirty();
        }
    }
}
