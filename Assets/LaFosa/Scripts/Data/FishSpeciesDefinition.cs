using UnityEngine;

namespace LaFosa
{
    public enum FishForm { Reef, Blenny, Clingfish, Wrasse, Flatfish, Pelagic, Eel, Ray, Shark, Chimera, Swordfish, Hake, Grenadier, Lantern, Viper, Hatchet, Snailfish }
    public enum FishPattern { Plain, Bands, Spots, Stripe, Waves, Mottled, Photophores }

    [CreateAssetMenu(menuName = "La Fosa/Especie de pez")]
    public sealed class FishSpeciesDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public string scientificName;
        [Range(0, 5)] public int habitatBand;
        public string referenceDepth;
        public Vector2 playableDepth;
        public FishForm form;
        public FishPattern pattern;
        public float length = 1;
        public float width = .25f;
        public float height = .4f;
        public Color backColor;
        public Color bellyColor;
        public Color accentColor;
        public float dorsalHeight = .2f;
        public int dorsalCount = 1;
        public float eyeSize = .04f;
        public float swimSpeed = .35f;
        public int population = 2;
        public ItemDefinition item;
        public GameObject prefab;
    }
}
