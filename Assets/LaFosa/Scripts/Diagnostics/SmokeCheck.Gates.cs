using System.Collections;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        IEnumerator TestClosedRoutes(GameSession session)
        {
            var gates = session.GetComponentsInChildren<DiveGate>();
            var diver = session.Diver; Vector3 original = diver.transform.position;
            Check(gates.Length == session.Config.zones.Skip(1).Count(z => z.hasPhysicalGate) && gates.All(g => !g.IsOpen), "Solo las transiciones configuradas generan compuertas cerradas");
            var freeZone = session.Config.zones[1];
            Check(!freeZone.hasPhysicalGate && gates.All(g => g.Destination != freeZone), "Descenso al naufragio sin compuerta ni losa");
            Check(gates.Any(g => g.Destination == session.Config.zones[2] && !g.IsOpen), "Abismo conserva su compuerta cerrada");
            foreach (float x in new[] { -10f, 0f, 10f })
            {
                Vector3 start = freeZone.entryPosition + new Vector3(x, 3, 0);
                Teleport(diver, start);
                MoveSteps(diver, Vector3.down * 8);
                Check(diver.transform.position.y < freeZone.entryPosition.y - 4, "Primer descenso libre sin tesoro por centro/lateral: " + x);
                MoveSteps(diver, Vector3.up * 8);
                Check(Vector3.Distance(diver.transform.position, start) < .5f, "Retorno libre por centro/lateral: " + x);
            }
            Check(!session.IsZoneUnlocked(freeZone) && session.Treasures == 0, "Acceso físico libre no concede progreso ni recompensas");
            foreach (var gate in gates)
            {
                Check(gate.MissingRequirement.Contains("ABRIR SELLO"), "Paso cerrado explica el mecanismo requerido: " + gate.Destination.id);
                var colliders = gate.GetComponentsInChildren<Collider>();
                bool covered = true; int samples = 0;
                float half = gate.Destination.barrierWidth / 2 + 2;
                for (float x = -half; x <= half; x += .5f)
                    for (float z = -half; z <= half; z += .5f)
                    {
                        Vector3 point = gate.transform.position + new Vector3(x, 0, z);
                        if (WorldBuilder.FloorHeight(point.x, point.z) >= point.y - .6f) continue;
                        samples++;
                        var ray = new Ray(point + Vector3.up * 2, Vector3.down);
                        if (!colliders.Any(c => c.Raycast(ray, out _, 4))) covered = false;
                    }
                Check(covered && samples > 100, "Losa cubre toda la sección profunda sin huecos: " + gate.Destination.id + " (" + samples + " muestras)");
                Teleport(diver, gate.transform.position + Vector3.up * 3);
                MoveSteps(diver, Vector3.down * 6);
                Check(diver.transform.position.y > gate.transform.position.y + .3f, "CharacterController no atraviesa el centro cerrado: " + gate.Destination.id);
                Check(session.Message.Contains("ABRIR SELLO"), "Choque con compuerta muestra el requisito");
                Teleport(diver, gate.transform.position + new Vector3(7, 3, 0));
                MoveSteps(diver, Vector3.down * 6);
                Check(diver.transform.position.y > gate.transform.position.y + .3f, "No se puede rodear la escotilla por la losa lateral");
                Teleport(diver, gate.transform.position + new Vector3(0, 4, -7)); diver.LookAt(gate.transform.position);
                yield return null;
                Capture(diver.View, "compuerta-" + gate.Destination.id + "-cerrada.png");
            }
            var fish = session.GetComponentsInChildren<FishAgent>();
            Check(fish.All(f => gates.All(g => Mathf.Abs(f.transform.position.y - g.transform.position.y) > f.species.height + 1.4f ||
                Mathf.Abs(f.transform.position.x - g.transform.position.x) > g.Destination.barrierWidth / 2 ||
                Mathf.Abs(f.transform.position.z - g.transform.position.z) > g.Destination.barrierWidth / 2)), "Fauna generada fuera de las losas");
            Teleport(diver, original);
        }
        IEnumerator TestOpenedRoute(GameSession session, int previousIndex)
        {
            var destination = session.Config.zones[previousIndex + 1];
            if (!destination.hasPhysicalGate)
            {
                Check(session.IsZoneUnlocked(destination), "Primer tesoro prepara el equipo para el naufragio libre");
                session.NextWaypoint(out string marker);
                Check(marker == "SANTUARIO", "Descenso libre señala el santuario sin escotilla inexistente");
                yield break;
            }
            var gate = session.GetComponentsInChildren<DiveGate>().First(g => g.Puzzle.zone == session.Config.zones[previousIndex]);
            var diver = session.Diver; Vector3 original = diver.transform.position;
            Check(gate.IsOpen && session.IsZoneUnlocked(gate.Destination), "Recoger tesoro abre la compuerta físicamente: " + gate.Destination.id);
            Teleport(diver, gate.transform.position + Vector3.up * 3);
            session.NextWaypoint(out string label);
            Check(label == "ESCOTILLA DE DESCENSO", "Marcador guía hasta la escotilla antes del siguiente santuario");
            MoveSteps(diver, Vector3.down * 8);
            Check(diver.transform.position.y < gate.transform.position.y - 4, "Buzo cruza hacia abajo la escotilla abierta");
            session.NextWaypoint(out label);
            Check(label == "SANTUARIO", "Tras cruzar, marcador cambia al santuario");
            MoveSteps(diver, Vector3.up * 8);
            Check(diver.transform.position.y > gate.transform.position.y + 2, "Buzo puede regresar por la misma escotilla");
            Teleport(diver, gate.transform.position + new Vector3(0, 4, -7)); diver.LookAt(gate.transform.position);
            Capture(diver.View, "compuerta-" + gate.Destination.id + "-abierta.png");
            // Reach the sanctuary and return through the aperture using collision-aware movement.
            Teleport(diver, gate.transform.position + Vector3.up * 3);
            MoveSteps(diver, new Vector3(0, gate.Destination.stationPosition.y + 3 - diver.transform.position.y, 0));
            Vector3 approach = gate.Destination.stationPosition + new Vector3(0, 3, -1.8f);
            MoveSteps(diver, new Vector3(4.5f, 0, 0));
            MoveSteps(diver, new Vector3(0, 0, approach.z - diver.transform.position.z));
            MoveSteps(diver, approach - diver.transform.position);
            Check(Vector3.Distance(diver.transform.position, approach) < .5f, "Ruta desde la escotilla hasta el siguiente santuario transitable");
            Vector3 shaft = new Vector3(gate.transform.position.x, diver.transform.position.y, gate.transform.position.z);
            MoveSteps(diver, new Vector3(4.5f, 0, 0));
            MoveSteps(diver, new Vector3(0, 0, shaft.z - diver.transform.position.z));
            MoveSteps(diver, shaft - diver.transform.position);
            // Let fauna update while ascending: moving the entire shaft in one frame
            // can pin the controller beneath a fish that never gets its next Update.
            for (int step = 0; step < 1200 && diver.transform.position.y < 1; step++)
            {
                MoveSteps(diver, Vector3.up * .2f);
                yield return null;
            }
            Check(diver.transform.position.y > .5f, "Regreso nadando a superficie por las compuertas ya abiertas: " + diver.transform.position);
            Teleport(diver, original);
        }
        void TestRoutePersistence(GameSession session)
        {
            var saved = session.CaptureSave(); var gates = session.GetComponentsInChildren<DiveGate>();
            Check(session.ReturnToMenu() && session.ContinueGame() && gates.All(g => g.IsOpen), "Compuertas restauradas abiertas inmediatamente al continuar");
            foreach (var zone in session.Config.zones) session.Diver.Inventory.TryRemove(zone.treasure, 1);
            Check(gates.All(g => g.IsOpen && session.IsZoneUnlocked(g.Destination)), "Descartar objetos de tesoro no elimina mejoras ni bloquea rutas");
            Check(session.ReturnToMenu() && session.ContinueGame() && gates.All(g => g.IsOpen), "Rutas siguen abiertas tras cargar sin tesoros en la mochila");
            Check(session.ReturnToMenu() && session.NewGame(true) && gates.All(g => !g.IsOpen), "Nueva partida cierra las compuertas y restablece sus colisiones");
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
