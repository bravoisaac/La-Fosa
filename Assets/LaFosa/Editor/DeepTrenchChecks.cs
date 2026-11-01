using System;
using System.Linq;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class DeepTrenchChecks
    {
        public static void Validate(GameConfig config)
        {
            int checks = 0;
            void Check(bool condition, string message)
            {
                if (!condition) throw new Exception("LA_FOSA_DEEP_FAILED: " + message);
                checks++;
            }
            Check(config.pressureBands != null && config.pressureBands.Length == 5, "Cinco umbrales de presión");
            float previousDepth = 0, previousCost = 1;
            for (int i = 0; i < config.pressureBands.Length; i++)
            {
                var band = config.pressureBands[i];
                Check(band.depth > previousDepth && band.depth < DeepTrenchProfile.BottomDepth && band.airMultiplier > previousCost && !float.IsInfinity(band.airMultiplier), "Umbrales y coste crecientes");
                Check(config.PressureLevel(band.depth - .01f) == i && config.PressureLevel(band.depth) == i + 1, "Cruce exacto del umbral " + i);
                Check(Mathf.Approximately(config.OxygenCost(band.depth, 650), band.airMultiplier), "Consumo según franja con equipo preparado");
                Check(Mathf.Approximately(config.OxygenCost(band.depth, 34), band.airMultiplier * config.pressureOxygenMultiplier), "Coste adicional por exceder el equipo");
                previousDepth = band.depth; previousCost = band.airMultiplier;
            }
            Check(config.OxygenCost(20, 34) == 1 && config.PressureLevel(20) == 0, "Pesca costera conserva consumo normal");
            Check(config.ReturnAir(600, 650, 6) > config.ReturnAir(300, 650, 6) && config.ReturnAir(600, 650, 8.1f) < config.ReturnAir(600, 650, 6), "Reserva crece con recorrido y considera aletas");
            Check(config.zones.Last().unlockedDepth >= DeepTrenchProfile.BottomDepth &&
                config.zones.Last().unlockedOxygen > config.ReturnAir(DeepTrenchProfile.BottomDepth, 650, 6) * 2 * 1.2f, "Equipo final cubre ida y vuelta teórica con margen");
            int openingCells = 0;
            for (int x = -150; x < 150; x += 2)
                for (int z = -90; z < 210; z += 2)
                    if (DeepTrenchProfile.IsOpeningCell(x, z, 2)) openingCells++;
            Check(openingCells == 16, "Abertura superior de 8 por 8 m en la cuadrícula");
            for (int i = 0; i < DeepTrenchProfile.Sides; i++)
            {
                var point = DeepTrenchProfile.Wall(88, i);
                Check(Mathf.Abs(Mathf.Max(Mathf.Abs(point.x), Mathf.Abs(point.z - 76)) - 4) < .001f && point.y == -88, "Borde de cavidad coincide con abertura");
            }
            var host = new GameObject("Validación estática de cavidad");
            Mesh mesh = null, upperMesh = null;
            var randomState = UnityEngine.Random.state;
            bool backfaces = Physics.queriesHitBackfaces;
            try
            {
                mesh = WorldBuilder.CreateDeepTrenchMesh();
                Check(mesh.vertices.All(v => !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z)), "Vértices finitos");
                Check(Mathf.Abs(mesh.bounds.min.y + 650) < .001f && Mathf.Abs(mesh.bounds.max.y + 88) < .001f, "Malla alcanza 650 m");
                var collider = host.AddComponent<MeshCollider>(); collider.sharedMesh = mesh;
                upperMesh = WorldBuilder.CreateTerrainMesh();
                var upper = new GameObject("Terreno superior"); upper.transform.SetParent(host.transform, false);
                var upperCollider = upper.AddComponent<MeshCollider>(); upperCollider.sharedMesh = upperMesh;
                Physics.SyncTransforms(); Physics.queriesHitBackfaces = false;
                Check(!upperCollider.Raycast(new Ray(new Vector3(0, -86, 76), Vector3.down), out _, 600), "Terreno real abierto sobre la cavidad");
                Check(upperCollider.Raycast(new Ray(new Vector3(-6, -86, 76), Vector3.down), out var shelf, 4) && Mathf.Abs(shelf.point.y + 88) < .01f, "Repisa lateral conservada junto al acceso");
                Check(collider.Raycast(new Ray(new Vector3(0, -90, 76), Vector3.down), out var hit, 600) && Mathf.Abs(hit.point.y + 650) < .01f, "Eje central abierto hasta el fondo");
                foreach (var band in config.pressureBands.Where(b => b.depth > 88))
                {
                    var origin = new Vector3(0, -band.depth, 76);
                    Check(!collider.Raycast(new Ray(origin, Vector3.up), out _, band.depth - 87), "Umbral sin techo que cierre el ascenso central");
                    Check(collider.Raycast(new Ray(origin, Vector3.right), out _, 150) && collider.Raycast(new Ray(origin, Vector3.left), out _, 150), "Paredes orientadas hacia el agua y colisionables");
                }
                Check(!collider.Raycast(new Ray(new Vector3(0, -648, 76), Vector3.up), out _, 560), "Ascenso continuo del fondo a la abertura");
                Debug.Log("LA_FOSA_DEEP_TESTS_OK · " + checks + " comprobaciones de perfil, aire y MeshCollider en editor, sin Play Mode.");
            }
            finally
            {
                Physics.queriesHitBackfaces = backfaces;
                UnityEngine.Random.state = randomState;
                UnityEngine.Object.DestroyImmediate(host);
                if (mesh != null) UnityEngine.Object.DestroyImmediate(mesh);
                if (upperMesh != null) UnityEngine.Object.DestroyImmediate(upperMesh);
            }
        }
    }
}
