using UnityEngine;

namespace LaFosa
{
    public sealed class Billboard : MonoBehaviour
    {
        Camera view;
        void LateUpdate()
        {
            if (view == null) view = Camera.main;
            if (view != null) transform.rotation = Quaternion.LookRotation(transform.position - view.transform.position);
        }
    }
}
