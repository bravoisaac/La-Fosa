using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class MooringChecks
    {
        // Reuse the developer check's isolated session/store, never a personal save.
        public static void Validate(GameSession session, SaveStore store)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var capture = typeof(GameSession).GetMethod("CaptureSave", flags);
            var apply = typeof(GameSession).GetMethod("ApplySave", flags);
            var validate = typeof(GameSession).GetMethod("ValidateSave", flags);
            var original = (SaveData)capture.Invoke(session, null);
            var boat = session.Boat; var diver = session.Diver;
            BoxCollider hull = null; GameObject obstacle = null;
            int checks = 0;
            void Check(bool condition, string message)
            { if (!condition) throw new Exception("LA_FOSA_MOORING_FAILED: " + message); checks++; }
            try
            {
                var ownedGear = original.gear.Concat(new[] { nameof(GearKind.Boat) }).Distinct().ToArray();
                typeof(Economy).GetMethod("Restore", flags).Invoke(session.Economy, new object[] { original.money, ownedGear });
                boat.Deliver(); hull = boat.gameObject.AddComponent<BoxCollider>();
                hull.center = Vector3.up * .25f; hull.size = boat.hullSize;
                Check(!boat.ToggleMooring(), "No se amarra sin pasajero");
                diver.LeaveVehicle(boat.transform.position + Vector3.up);
                Check(boat.TryBoard(diver), "Embarcar en lancha comprada");
                Check(!boat.ToggleMooring() && !boat.IsMoored, "Rechazar amarre lejos de la fosa");
                Vector3 target = SurfaceVehicle.TrenchMooringPosition(session.Config.waterLevel);
                boat.transform.position = target + Vector3.back * 6;
                obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.transform.position = target + Vector3.back * 3 + Vector3.up;
                Physics.SyncTransforms();
                Vector3 before = boat.transform.position;
                Check(!boat.ToggleMooring() && boat.transform.position == before, "Obstáculo intermedio impide acercamiento sin mover la lancha");
                boat.transform.SetPositionAndRotation(target, Quaternion.Euler(0, 180, 0));
                obstacle.transform.position = target + Vector3.right * 1.8f + Vector3.up;
                obstacle.transform.localScale = Vector3.one * .5f;
                Physics.SyncTransforms();
                Check(!boat.ToggleMooring() && boat.transform.position == target && Mathf.Abs(Mathf.DeltaAngle(boat.transform.eulerAngles.y, 180)) < .01f,
                    "Obstáculo durante giro en el mismo punto impide amarre");
                UnityEngine.Object.DestroyImmediate(obstacle); obstacle = null;
                boat.transform.SetPositionAndRotation(target + Vector3.back * 6, Quaternion.identity); Physics.SyncTransforms();
                Check(boat.ToggleMooring() && boat.IsMoored && Vector3.Distance(boat.transform.position, target) < .01f, "Amarre libre al lado del eje de descenso");
                boat.Drive(1, 1, 1);
                Check(boat.transform.position == target && Mathf.Abs(Mathf.DeltaAngle(boat.transform.eulerAngles.y, 0)) < .01f, "Amarre bloquea avance y giro");
                Check(boat.Disembark() && diver.Riding == null && boat.IsMoored, "Desembarcar para bucear mantiene amarre");
                Check(session.SaveNow() && store.TryLoad(out _, out _), "Guardar y leer lancha amarrada");
                store.TryLoad(out var saved, out _);
                Check(saved.vehicles.First(v => v.id == nameof(GearKind.Boat)).moored, "Archivo incluye estado de amarre");
                boat.transform.position += Vector3.forward * 20;
                apply.Invoke(session, new object[] { saved });
                Check(boat.IsMoored && boat.transform.position == target && diver.Riding == null, "Carga restaura amarre, posición y jugador en playa");
                diver.LeaveVehicle(target + Vector3.right * 2.4f); Physics.SyncTransforms();
                Check(boat.TryBoard(diver) && boat.ToggleMooring() && !boat.IsMoored, "Reembarcar y soltar amarre");
                boat.Drive(1, 0, .1f);
                Check(boat.transform.position.z > target.z, "Navegación vuelve tras soltar");
                diver.LeaveVehicle(session.Config.spawn);
                var legacy = JsonUtility.FromJson<SavedVehicle>("{\"id\":\"Boat\",\"position\":{\"x\":6,\"y\":0,\"z\":76},\"yaw\":0}");
                Check(!legacy.moored, "Vehículo v1 sin campo moored carga desamarrado");
                saved.vehicles.First(v => v.id == nameof(GearKind.Boat)).position += Vector3.right;
                Check(!(bool)validate.Invoke(session, new object[] { saved }), "Rechazar amarre guardado fuera de ubicación");
                saved.vehicles.First(v => v.id == nameof(GearKind.Boat)).position = target;
                saved.vehicles.First(v => v.id == nameof(GearKind.JetSki)).moored = true;
                Check(!(bool)validate.Invoke(session, new object[] { saved }), "Rechazar amarre de moto de agua");
                saved.vehicles.First(v => v.id == nameof(GearKind.JetSki)).moored = false;
                saved.gear = saved.gear.Where(g => g != nameof(GearKind.Boat)).ToArray();
                Check(!(bool)validate.Invoke(session, new object[] { saved }), "Rechazar lancha amarrada sin propiedad");
                Check(!session.JetSki.ToggleMooring(), "Moto de agua sin opción de amarre");
                Debug.Log("LA_FOSA_MOORING_TESTS_OK · " + checks + " comprobaciones de amarre, colisiones y guardado, sin Play Mode.");
            }
            finally
            {
                if (obstacle != null) UnityEngine.Object.DestroyImmediate(obstacle);
                if (hull != null) UnityEngine.Object.DestroyImmediate(hull);
                apply.Invoke(session, new object[] { original });
                session.SaveNow();
            }
        }
    }
}
