using UnityEngine;

namespace LaFosa
{
    public sealed class HarpoonGun : MonoBehaviour
    {
        public DiverController diver;
        public float range = 40;
        public float reloadTime = .65f;
        public int SelectedTool { get; private set; }
        // Hotkeys: 1 hands, 2 trident, 3 harpoon, 4 net.
        public string ToolName => SelectedTool == 0 ? "MANOS" : SelectedTool == 1 ? "TRIDENTE" : SelectedTool == 2 ? "ARPÓN" : "RED";
        public float EffectiveRange => SelectedTool == 0 ? 1.6f : SelectedTool == 1 ? 4 : SelectedTool == 2 ? range : 6;
        public float EffectiveCooldown => SelectedTool == 0 ? .8f : SelectedTool == 1 ? .45f : SelectedTool == 2 ? reloadTime : 5;
        public GameObject[] toolModels;
        public float ReadyIn => Mathf.Max(0, nextShot - Time.time);
        float nextShot;
        LineRenderer line;
        float lineUntil;
        float reachUntil;
        float reachDuration = .25f;
        Vector3[] restingPositions;
        DiverHands handsPose;
        void Start()
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(.8f, 1, .9f); line.endColor = new Color(.25f, .8f, .8f);
            line.startWidth = .025f; line.endWidth = .008f; line.positionCount = 2; line.enabled = false;
            if (toolModels != null)
            {
                restingPositions = new Vector3[toolModels.Length];
                for (int i = 0; i < toolModels.Length; i++) restingPositions[i] = toolModels[i].transform.localPosition;
                if (toolModels.Length > 0) handsPose = toolModels[0].GetComponent<DiverHands>();
            }
            SelectTool(0);
        }
        void Update()
        {
            line.enabled = Time.time < lineUntil;
            if (handsPose != null)
            {
                float progress = 1 - Mathf.Clamp01((reachUntil - Time.time) / reachDuration);
                float grip = progress < .25f ? 0 : progress < .65f
                    ? Mathf.InverseLerp(.25f, .65f, progress) : 1 - Mathf.InverseLerp(.65f, 1, progress);
                handsPose.SetGrip(SelectedTool == 0 && Time.time < reachUntil ? grip : 0);
            }
            if (restingPositions != null)
                for (int i = 0; i < toolModels.Length; i++)
                    toolModels[i].transform.localPosition = restingPositions[i] + Vector3.forward * (i == SelectedTool && i < 2 && Time.time < reachUntil ? Mathf.Sin((reachUntil - Time.time) / reachDuration * Mathf.PI) * .18f : 0);
            if (diver.Session.Blocked) return;
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectTool(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectTool(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectTool(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectTool(3);
            if (Input.GetMouseButtonDown(0)) Fire();
        }
        public bool SelectTool(int tool)
        {
            if (tool < 0 || tool > 3) return false;
            if ((tool == 1 && !diver.Session.Economy.Has(GearKind.Trident)) || (tool == 2 && !diver.Session.Economy.Has(GearKind.ReinforcedHarpoon)) || (tool == 3 && !diver.Session.Economy.Has(GearKind.Net)))
            { diver.Session.Notify("Compra esta herramienta en la cabaña de Don Lucho."); return false; }
            if (SelectedTool != tool)
            {
                reachUntil = 0;
                if (handsPose != null) handsPose.SetGrip(0);
            }
            SelectedTool = tool;
            diver.Session.MarkSaveDirty();
            if (toolModels != null) for (int i = 0; i < toolModels.Length; i++) toolModels[i].SetActive(i == tool);
            return true;
        }
        public void Fire()
        {
            if (diver.Session.Blocked || ReadyIn > 0 || line == null) return;
            if (!diver.Underwater && !(SelectedTool == 3 && diver.Riding != null)) { diver.Session.Notify("Sumérgete para pescar. También puedes lanzar la red desde una embarcación."); return; }
            nextShot = Time.time + EffectiveCooldown;
            diver.Session.RecordCaptureAttempt(SelectedTool);
            Vector3 origin = diver.View.transform.position;
            Vector3 direction = diver.View.transform.forward;
            Vector3 end = origin + direction * EffectiveRange;
            reachDuration = SelectedTool == 0 ? .48f : .25f;
            reachUntil = Time.time + reachDuration;
            if (SelectedTool == 3)
            {
                if (diver.Riding != null)
                {
                    origin = diver.Riding.transform.position + diver.Riding.transform.forward * 3 + Vector3.down;
                    end = origin + diver.Riding.transform.forward * 3 + Vector3.down * 2;
                }
                if (Physics.Raycast(origin, (end - origin).normalized, out var obstruction, Vector3.Distance(origin, end), ~0, QueryTriggerInteraction.Ignore) && obstruction.collider.GetComponentInParent<FishAgent>() == null)
                    end = obstruction.point - (end - origin).normalized * .1f;
                int caught = CastNet(origin, end);
                diver.Session.Notify(caught > 0 ? "Red: " + caught + " peces capturados." : "Red vacía. Acércate a un banco de peces y comprueba tu mochila.");
            }
            else if (Physics.SphereCast(origin, SelectedTool == 0 ? .18f : SelectedTool == 1 ? .3f : .025f, direction, out var hit, EffectiveRange, ~0, QueryTriggerInteraction.Ignore))
            {
                end = hit.point;
                var fish = hit.collider.GetComponentInParent<FishAgent>();
                if (fish != null) fish.Catch(diver);
            }
            if (SelectedTool < 2) return; // Hands and trident reach forward; neither fires a projectile.
            line.SetPosition(0, diver.View.transform.TransformPoint(new Vector3(.26f, -.2f, .6f)));
            line.SetPosition(1, end); lineUntil = Time.time + .15f; line.enabled = true;
        }
        int CastNet(Vector3 origin, Vector3 center)
        {
            int caught = 0;
            var seen = new System.Collections.Generic.HashSet<FishAgent>();
            foreach (var collider in Physics.OverlapSphere(center, 4, ~0, QueryTriggerInteraction.Ignore))
            {
                var fish = collider.GetComponentInParent<FishAgent>();
                if (fish == null || !fish.Available || !seen.Add(fish)) continue;
                Vector3 delta = fish.transform.position - origin;
                if (Physics.Raycast(origin, delta.normalized, out var hit, delta.magnitude + .8f, ~0, QueryTriggerInteraction.Ignore) && hit.collider.GetComponentInParent<FishAgent>() == fish && fish.Catch(diver)) caught++;
                if (caught >= 5) break;
            }
            return caught;
        }
    }
}
