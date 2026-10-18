using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        void CheckFauna(GameSession session)
        {
            var fish = FindObjectsByType<FishAgent>(FindObjectsSortMode.None);
            Check(session.Config.fishSpecies.Length == 49 && fish.Length == session.Config.fishSpecies.Sum(s => s.population), "49 especies y población completa");
            foreach (var species in session.Config.fishSpecies)
            {
                var group = fish.Where(f => f.species == species).ToArray();
                Check(group.Length == species.population && group.All(f => f.item == species.item && f.GetComponent<MeshFilter>().sharedMesh != null), "Modelo e ítem: " + species.id);
                Check(group.All(f => -f.transform.position.y >= species.playableDepth.x - .01f && -f.transform.position.y <= species.playableDepth.y + .01f && f.transform.position.y > WorldBuilder.FloorHeight(f.transform.position.x, f.transform.position.z) + species.height), "Hábitat: " + species.id);
            }
        }
        void TestSpeciesTrading(GameSession session)
        {
            session.OpenShop();
            var inventory = session.Diver.Inventory;
            if (session.Economy.FishCount > 0) session.Economy.SellFish();
            int treasures = inventory.Stacks.Count;
            foreach (var species in session.Config.fishSpecies)
            {
                var test = Instantiate(species.prefab, new Vector3(500, -5, 500), Quaternion.identity);
                var fish = test.GetComponent<FishAgent>(); fish.enabled = false;
                int money = session.Economy.Money;
                Check(fish.Catch(session.Diver) && inventory.Count(species.item) == 1 && !fish.Catch(session.Diver), "Captura única: " + species.id);
                Check(session.Economy.SellFish() && session.Economy.Money == money + species.item.salePrice && inventory.Count(species.item) == 0, "Venta por especie: " + species.id);
                Destroy(test);
            }
            var a = session.Config.fishSpecies[0].item;
            var b = session.Config.fishSpecies[45].item;
            inventory.TryAdd(a, 2); inventory.TryAdd(b, 3);
            int balance = session.Economy.Money;
            Check(!inventory.TryRemoveAllFish(6, 1) && inventory.FishCount == 5, "Venta inválida no retira peces");
            Check(session.Economy.SellFish() && session.Economy.Money == balance + 2 * a.salePrice + 3 * b.salePrice && inventory.Stacks.Count == treasures, "Venta mixta conserva conchas y tesoros");
        }
        IEnumerator CaptureFishGallery(GameConfig config)
        {
            Directory.CreateDirectory(Path.Combine(output, "FishModels"));
            bool fog = RenderSettings.fog; Color ambient = RenderSettings.ambientLight;
            float sun = RenderSettings.sun != null ? RenderSettings.sun.intensity : 0;
            RenderSettings.fog = false; RenderSettings.ambientLight = new Color(.6f, .67f, .72f);
            if (RenderSettings.sun != null) RenderSettings.sun.intensity = 1;
            var cameraObject = new GameObject("Cámara catálogo"); var camera = cameraObject.AddComponent<Camera>();
            camera.enabled = false; camera.orthographic = true; camera.cullingMask = 1 << 30;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.035f, .075f, .1f);
            camera.nearClipPlane = .01f; camera.farClipPlane = 30; camera.aspect = 1600f / 900;
            try
            {
                foreach (var species in config.fishSpecies)
                {
                    var model = Instantiate(species.prefab, new Vector3(500, 100, 0), Quaternion.identity);
                    model.layer = 30; model.GetComponent<FishAgent>().enabled = false;
                    var bounds = model.GetComponent<Renderer>().bounds;
                    Vector3 direction = new Vector3(1, species.form == FishForm.Flatfish || species.form == FishForm.Ray ? 1.35f : .38f, .12f).normalized;
                    camera.transform.position = bounds.center + direction * 10; camera.transform.LookAt(bounds.center);
                    float maxX = 0, maxY = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        var p = bounds.center + Vector3.Scale(bounds.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                        var local = camera.transform.InverseTransformPoint(p);
                        maxX = Mathf.Max(maxX, Mathf.Abs(local.x)); maxY = Mathf.Max(maxY, Mathf.Abs(local.y));
                    }
                    camera.orthographicSize = Mathf.Max(maxY, maxX / camera.aspect) * 1.23f;
                    Capture(camera, "FishModels/" + species.id + ".png");
                    Destroy(model); yield return null;
                }
            }
            finally
            {
                Destroy(cameraObject); RenderSettings.fog = fog; RenderSettings.ambientLight = ambient;
                if (RenderSettings.sun != null) RenderSettings.sun.intensity = sun;
            }
            Check(config.fishSpecies.All(s => File.Exists(Path.Combine(output, "FishModels/" + s.id + ".png"))), "49 vistas renderizadas desde los prefabs");
        }
    }
}
