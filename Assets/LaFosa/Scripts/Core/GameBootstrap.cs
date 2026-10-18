using UnityEngine;

namespace LaFosa
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        public GameConfig config;
        public GameSession Session { get; private set; }
        public void Build()
        {
            if (Session != null) return;
            if (config == null || config.zones == null || config.zones.Length == 0)
            { Debug.LogError("La Fosa necesita una configuración y al menos una zona."); return; }
            Session = gameObject.AddComponent<GameSession>(); Session.Initialize(config);
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = .0035f; RenderSettings.fogColor = new Color(.64f, .79f, .81f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight = new Color(.7f, .77f, .78f);
            var sun = new GameObject("Sol del Pacífico"); sun.transform.SetParent(transform); sun.transform.rotation = Quaternion.Euler(42, -30, 0);
            var light = sun.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.1f; light.color = new Color(1, .91f, .75f);
            RenderSettings.sun = light;
            var world = new GameObject("Caleta Bruma · mundo procedural"); world.transform.SetParent(transform);
            var builder = new WorldBuilder(world.transform, config); builder.Build();
            builder.BuildVehicles(Session);
            var player = new GameObject("Buzo"); player.transform.SetParent(transform); player.transform.position = config.spawn;
            var controller = player.AddComponent<CharacterController>(); controller.height = 1.8f; controller.radius = .32f; controller.center = Vector3.up * .9f; controller.stepOffset = .35f;
            var cameraObject = new GameObject("Cámara del buzo"); cameraObject.tag = "MainCamera"; cameraObject.transform.SetParent(player.transform, false); cameraObject.transform.localPosition = Vector3.up * 1.55f;
            var camera = cameraObject.AddComponent<Camera>(); camera.nearClipPlane = .06f; camera.farClipPlane = 450; camera.fieldOfView = 76; camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.52f, .75f, .83f);
            cameraObject.AddComponent<AudioListener>();
            var diver = player.AddComponent<DiverController>(); diver.Initialize(Session, camera); Session.Diver = diver;
            foreach (var fish in world.GetComponentsInChildren<FishAgent>())
                fish.InitializeAI(diver, fish.species != null && fish.species.habitatBand == 0 ? config.coastalFishBehavior : config.offshoreFishBehavior);
            var gun = player.AddComponent<HarpoonGun>(); gun.diver = diver;
            gun.toolModels = builder.BuildFishingTools(camera.transform);
            var hud = gameObject.AddComponent<DiveHUD>(); hud.session = Session; hud.harpoon = gun;
        }
        void Awake() { Build(); }
    }
}
