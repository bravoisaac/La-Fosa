using UnityEngine;

namespace LaFosa
{
    public enum FishState { Cruising, Fleeing, Returning }

    public sealed partial class FishAgent : MonoBehaviour
    {
        public ItemDefinition item;
        public FishSpeciesDefinition species;
        public float waterLevel;
        public float speed = .55f;
        public float radius = 3;
        public float respawnSeconds = 25;
        public bool Available { get; private set; } = true;
        public FishState State { get; private set; }
        public FishBehaviorProfile Behavior { get; private set; }
        public float CurrentSpeed { get; private set; }
        public Vector3 Home => anchor;
        float respawnAt;
        Renderer[] visuals;
        Collider body;
        Vector3 anchor;
        float phase;
        DiverController observer;
        float alert, perception, wanderTime;
        Vector3 lastThreat, destination;
        bool initialized;
        void Start()
        {
            visuals = GetComponentsInChildren<Renderer>(); body = GetComponent<Collider>();
            if (!initialized) { anchor = transform.position; destination = anchor; phase = Random.value * 6.28f; }
        }
        public void InitializeAI(DiverController diver, FishBehaviorProfile profile)
        {
            observer = diver; Behavior = profile; anchor = transform.position; destination = anchor;
            phase = Random.value * 6.28f; alert = perception = wanderTime = 0;
            State = FishState.Cruising; CurrentSpeed = 0; initialized = true;
        }
        void Update()
        {
            if (!Available)
            {
                if (Time.time < respawnAt) return;
                transform.position = anchor; alert = perception = wanderTime = 0;
                State = FishState.Cruising; SetAvailable(true);
            }
            Simulate(Time.deltaTime);
        }
        // Also used by deterministic integration checks; normal play calls this from Update.
        public void Simulate(float deltaTime)
        {
            if (!Available || Behavior == null || deltaTime <= 0 || (observer != null && observer.Session.Blocked)) { CurrentSpeed = 0; return; }
            // Substeps keep obstacle checks reliable during a slow frame.
            float remaining = Mathf.Min(deltaTime, .25f);
            while (remaining > .00001f)
            {
                float step = Mathf.Min(remaining, .025f); Step(step); remaining -= step;
            }
        }
        void Step(float dt)
        {
            alert = Mathf.Max(0, alert - dt); perception -= dt; wanderTime -= dt;
            if (perception <= 0)
            {
                perception = Behavior.perceptionInterval;
                if (observer != null && observer.Underwater)
                {
                    Vector3 threat = observer.View.transform.position;
                    float range = State == FishState.Fleeing ? Behavior.releaseRange : Behavior.detectionRange;
                    if ((threat - transform.position).sqrMagnitude <= range * range && CanSee(threat))
                    { lastThreat = threat; alert = Behavior.alertMemory; }
                }
            }
            Vector3 wanted;
            float travelSpeed;
            if (alert > 0)
            {
                State = FishState.Fleeing; wanted = transform.position - lastThreat;
                if (wanted.sqrMagnitude < .01f) wanted = transform.forward;
                travelSpeed = Behavior.escapeSpeed;
            }
            else
            {
                bool returning = Vector3.Distance(transform.position, anchor) > Mathf.Max(2, radius + 1);
                State = returning ? FishState.Returning : FishState.Cruising;
                if (returning) destination = anchor;
                else if (wanderTime <= 0 || Vector3.Distance(transform.position, destination) < .2f)
                {
                    phase += 1.7f; wanderTime = 2.5f;
                    destination = anchor + new Vector3(Mathf.Sin(phase) * radius, Mathf.Sin(phase * .7f) * .25f, Mathf.Cos(phase) * radius);
                }
                wanted = destination - transform.position;
                travelSpeed = Mathf.Min(Behavior.cruiseSpeed, wanted.magnitude / dt);
            }
            Vector3 direction = SafeDirection(wanted.normalized, travelSpeed * dt);
            Vector3 movement = direction * travelSpeed * dt;
            transform.position += movement; CurrentSpeed = movement.magnitude / dt;
            if (direction.sqrMagnitude > .01f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), Behavior.turnSpeed * dt);
        }
        public bool Catch(DiverController diver)
        {
            if (!Available) return false;
            if (!diver.Inventory.TryAdd(item)) { diver.Session.Notify("Mochila llena. Libera espacio antes de pescar."); return false; }
            SetAvailable(false); CurrentSpeed = 0; respawnAt = Time.time + respawnSeconds;
            diver.Session.RecordExpeditionEvent("capture", item.id);
            diver.Session.Notify("Captura: " + item.displayName + " · véndelo en la cabaña"); return true;
        }
        void SetAvailable(bool available)
        {
            Available = available;
            if (visuals == null) visuals = GetComponentsInChildren<Renderer>();
            if (body == null) body = GetComponent<Collider>();
            foreach (var visual in visuals) visual.enabled = available;
            if (body != null) body.enabled = available;
        }
    }
}
