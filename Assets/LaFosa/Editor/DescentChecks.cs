using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class DescentChecks
    {
        public static void Validate(GameConfig config)
        {
            var host = new GameObject("Comprobación de descensos sin jugar");
            var materials = new HashSet<Material>();
            int checks = 0;
            void Check(bool condition, string message)
            {
                if (!condition) throw new Exception("LA_FOSA_DESCENT_FAILED: " + message);
                checks++;
            }
            try
            {
                new WorldBuilder(host.transform, config).BuildDescentGuides();
                Check(host.transform.childCount == config.zones.Length - 1, "Señales en cada transición");
                Check(host.GetComponentsInChildren<Collider>(true).Length == 0, "La construcción de descensos no crea colliders");
                Check(host.GetComponentsInChildren<Interactable>(true).Length == 0, "El paso no exige accionar un mecanismo");
                for (int i = 1; i < config.zones.Length; i++)
                {
                    var zone = config.zones[i];
                    Check(host.transform.GetChild(i - 1).position == zone.entryPosition, "Guía en la entrada de " + zone.id);
                    foreach (float x in new[] { -7f, 0f, 7f })
                    {
                        Vector3 point = zone.entryPosition + new Vector3(x, -5, 0);
                        Check(WorldBuilder.FloorHeight(point.x, point.z) < point.y - 2, "Terreno bajo el recorrido vertical: " + zone.id + " / " + x);
                    }
                }
                Debug.Log("LA_FOSA_DESCENT_TESTS_OK · " + checks + " comprobaciones de construcción/terreno en editor, sin Play Mode.");
            }
            finally
            {
                foreach (var renderer in host.GetComponentsInChildren<Renderer>(true))
                    foreach (var material in renderer.sharedMaterials)
                        if (material != null) materials.Add(material);
                UnityEngine.Object.DestroyImmediate(host);
                foreach (var material in materials) UnityEngine.Object.DestroyImmediate(material);
            }
        }
    }
}
