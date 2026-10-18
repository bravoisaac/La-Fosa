using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class FishBehaviorSetup
    {
        public static void Ensure(GameConfig config)
        {
            if (config.coastalFishBehavior == null) config.coastalFishBehavior = Profile("PecesOrilla", .45f, 1.8f, 3.5f, 6);
            if (config.offshoreFishBehavior == null) config.offshoreFishBehavior = Profile("PecesMar", 1.1f, 16, 12, 22);
            EditorUtility.SetDirty(config);
        }
        static FishBehaviorProfile Profile(string name, float cruise, float escape, float detect, float release)
        {
            string path = "Assets/LaFosa/Data/" + name + ".asset";
            var profile = AssetDatabase.LoadAssetAtPath<FishBehaviorProfile>(path);
            if (profile != null) return profile;
            profile = ScriptableObject.CreateInstance<FishBehaviorProfile>(); profile.name = name;
            profile.cruiseSpeed = cruise; profile.escapeSpeed = escape; profile.detectionRange = detect; profile.releaseRange = release;
            AssetDatabase.CreateAsset(profile, path); return profile;
        }
    }
}
