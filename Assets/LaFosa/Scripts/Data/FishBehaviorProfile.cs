using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Comportamiento de peces")]
    public sealed class FishBehaviorProfile : ScriptableObject
    {
        [Min(.1f)] public float cruiseSpeed = .5f;
        [Min(.1f)] public float escapeSpeed = 1.8f;
        [Min(.1f)] public float detectionRange = 3.5f;
        [Min(.1f)] public float releaseRange = 6;
        [Min(.1f)] public float alertMemory = 1.5f;
        [Min(.05f)] public float perceptionInterval = .12f;
        [Min(1)] public float turnSpeed = 600;
    }
}
