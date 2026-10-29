using System.Collections;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        IEnumerator TestFreeRoutes(GameSession session)
        {
            var diver = session.Diver;
            Vector3 original = diver.transform.position;
            var guides = session.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("Descenso libre · ")).ToArray();
            Check(guides.Length == session.Config.zones.Length - 1 && guides.All(g => g.GetComponentsInChildren<Collider>(true).Length == 0), "Descensos señalizados sin colliders de barrera");
            foreach (var zone in session.Config.zones.Skip(1))
            {
                foreach (float x in new[] { -7f, 0f, 7f })
                {
                    Vector3 start = zone.entryPosition + new Vector3(x, 3, 0);
                    Teleport(diver, start);
                    MoveSteps(diver, Vector3.down * 8);
                    Check(diver.transform.position.y < zone.entryPosition.y - 4, "Descenso sin tesoro por centro/lateral: " + zone.id + " / " + x);
                    MoveSteps(diver, Vector3.up * 8);
                    Check(Vector3.Distance(diver.transform.position, start) < .5f, "Ascenso libre por centro/lateral: " + zone.id + " / " + x);
                }
                Check(!session.IsZoneUnlocked(zone) && session.Treasures == 0, "Nadar libremente no concede el progreso del mecanismo: " + zone.id);
            }
            Teleport(diver, original);
            yield return null;
        }
        IEnumerator TestPreparedRoute(GameSession session, int previousIndex)
        {
            var destination = session.Config.zones[previousIndex + 1];
            var diver = session.Diver; Vector3 original = diver.transform.position;
            Check(session.IsZoneUnlocked(destination), "Tesoro anterior prepara el mecanismo y el aire: " + destination.id);
            session.NextWaypoint(out string label);
            Check(label == "SANTUARIO", "Marcador dirige al santuario sin puerta intermedia");
            Teleport(diver, destination.entryPosition + Vector3.up * 3);
            MoveSteps(diver, Vector3.down * 8);
            Check(diver.transform.position.y < destination.entryPosition.y - 4, "Descenso libre con equipo mejorado: " + destination.id);
            MoveSteps(diver, Vector3.up * 8);
            Check(diver.transform.position.y > destination.entryPosition.y + 2, "Regreso libre con equipo mejorado: " + destination.id);
            MoveSteps(diver, new Vector3(0, destination.stationPosition.y + 3 - diver.transform.position.y, 0));
            Vector3 approach = destination.stationPosition + new Vector3(0, 3, -1.8f);
            MoveSteps(diver, new Vector3(4.5f, 0, 0));
            MoveSteps(diver, new Vector3(0, 0, approach.z - diver.transform.position.z));
            MoveSteps(diver, approach - diver.transform.position);
            Check(Vector3.Distance(diver.transform.position, approach) < .5f, "Ruta hasta el santuario transitable: " + destination.id);
            Vector3 shaft = new Vector3(destination.entryPosition.x, diver.transform.position.y, destination.entryPosition.z);
            MoveSteps(diver, new Vector3(4.5f, 0, 0));
            MoveSteps(diver, new Vector3(0, 0, shaft.z - diver.transform.position.z));
            MoveSteps(diver, shaft - diver.transform.position);
            // Let fauna update during ascent so a stationary fish cannot pin the controller.
            for (int step = 0; step < 1200 && diver.transform.position.y < 1; step++)
            {
                MoveSteps(diver, Vector3.up * .2f);
                yield return null;
            }
            Check(diver.transform.position.y > .5f, "Regreso nadando a superficie sin compuertas: " + diver.transform.position);
            Teleport(diver, original);
        }
        void TestRoutePersistence(GameSession session)
        {
            var saved = session.CaptureSave();
            Check(session.ReturnToMenu() && session.ContinueGame() && session.Config.zones.All(session.IsZoneUnlocked), "Continuar conserva las mejoras y los mecanismos habilitados");
            foreach (var zone in session.Config.zones) session.Diver.Inventory.TryRemove(zone.treasure, 1);
            Check(session.Config.zones.All(session.IsZoneUnlocked), "Descartar objetos de tesoro no elimina las mejoras");
            Check(session.ReturnToMenu() && session.ContinueGame() && session.Config.zones.All(session.IsZoneUnlocked), "Mejoras persisten tras cargar sin objetos de tesoro");
            Check(session.ReturnToMenu() && session.NewGame(true) && session.Treasures == 0 && session.Diver.SafeDepth == session.Config.initialSafeDepth,
                "Nueva partida restablece equipo y progreso sin crear barreras");
            session.ApplySave(saved); session.SaveNow(); session.Resume();
        }
        static void MoveSteps(DiverController diver, Vector3 displacement)
        {
            int steps = Mathf.Max(1, Mathf.CeilToInt(displacement.magnitude / .15f));
            var controller = diver.GetComponent<CharacterController>();
            for (int i = 0; i < steps; i++) { controller.Move(displacement / steps); Physics.SyncTransforms(); }
        }
    }
}
