using UnityEngine;

namespace LaFosa
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract string Prompt { get; }
        public abstract void Interact(DiverController diver);
    }
}
