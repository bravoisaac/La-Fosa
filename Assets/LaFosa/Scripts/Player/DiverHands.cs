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
            public float curl;
        }
        readonly List<Joint> joints = new List<Joint>();
        readonly List<Mesh> meshes = new List<Mesh>();

        public void RegisterJoint(Transform joint, float curl)
        {
            joints.Add(new Joint { transform = joint, restingRotation = joint.localRotation, curl = curl });
        }
        public void OwnMesh(Mesh mesh) { meshes.Add(mesh); }

        public void SetGrip(float amount)
        {
            float grip = Mathf.SmoothStep(0, 1, Mathf.Clamp01(amount));
            foreach (var joint in joints)
                joint.transform.localRotation = joint.restingRotation * Quaternion.Euler(joint.curl * grip, 0, 0);
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
