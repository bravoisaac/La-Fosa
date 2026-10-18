using UnityEngine;

namespace LaFosa
{
    [CreateAssetMenu(menuName = "La Fosa/Objeto")]
    public sealed class ItemDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        [Min(1)] public int maxStack = 10;
        public Color color = Color.white;
        public bool isFish;
        [Min(1)] public int salePrice = 30;
    }
}
