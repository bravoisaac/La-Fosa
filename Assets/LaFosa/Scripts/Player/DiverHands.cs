using System.Collections.Generic;
using UnityEngine;

namespace LaFosa
{
    // Presentation only: fishing still uses HarpoonGun's original cast and cooldown.
    public sealed class DiverHands : MonoBehaviour
    {
        struct Joint
        {
            public Transform transform;
            public Quaternion restingRotation;
            public Vector3 curl;
        }
        struct Wrist
        {
            public Transform transform;
            public Vector3 position;
            public Quaternion rotation;
            public float side;
        }
        readonly List<Joint> joints = new List<Joint>();
        readonly List<Wrist> wrists = new List<Wrist>();
        readonly List<Mesh> meshes = new List<Mesh>();

        public void RegisterJoint(Transform joint, Vector3 curl)
        {
            joints.Add(new Joint { transform = joint, restingRotation = joint.localRotation, curl = curl });
        }
        public void RegisterWrist(Transform wrist, float side)
        {
            wrists.Add(new Wrist { transform = wrist, position = wrist.localPosition, rotation = wrist.localRotation, side = side });
        }
        public void OwnMesh(Mesh mesh) { meshes.Add(mesh); }

        public void SetGrip(float amount)
        {
            float grip = Mathf.SmoothStep(0, 1, Mathf.Clamp01(amount));
            foreach (var joint in joints)
                if (joint.transform != null) joint.transform.localRotation = joint.restingRotation * Quaternion.Euler(joint.curl * grip);
            foreach (var wrist in wrists)
                if (wrist.transform != null)
                {
                    wrist.transform.localPosition = wrist.position + new Vector3(-wrist.side * .035f, .015f, .045f) * grip;
                    wrist.transform.localRotation = wrist.rotation * Quaternion.Euler(-8 * grip, wrist.side * 14 * grip, -wrist.side * 8 * grip);
                }
        }
        void OnDisable() { SetGrip(0); }
        void OnDestroy()
        {
            foreach (var mesh in meshes)
                if (mesh != null)
                {
                    if (Application.isPlaying) Destroy(mesh);
                    else DestroyImmediate(mesh);
                }
        }
    }
}
