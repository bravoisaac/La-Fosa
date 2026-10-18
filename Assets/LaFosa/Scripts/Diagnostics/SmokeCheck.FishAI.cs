using System.Collections;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        FishAgent AIExample(GameSession session, string id, Vector3 point)
        {
            var species = session.Config.fishSpecies.First(s => s.id == id);
            var fish = Instantiate(species.prefab, point, Quaternion.identity).GetComponent<FishAgent>();
            fish.enabled = false; fish.waterLevel = session.Config.waterLevel;
            fish.InitializeAI(session.Diver, species.habitatBand == 0 ? session.Config.coastalFishBehavior : session.Config.offshoreFishBehavior);
            return fish;
        }
        static void ObserverAt(DiverController diver, Vector3 eyes) => Teleport(diver, eyes - Vector3.up * 1.55f);
        IEnumerator TestFishBehavior(GameSession session)
        {
            var diver = session.Diver; var gun = diver.GetComponent<HarpoonGun>();
            var coast = session.Config.coastalFishBehavior; var sea = session.Config.offshoreFishBehavior;
            Check(coast.escapeSpeed < 6 && sea.escapeSpeed > 9 * session.Config.shop.finSpeedMultiplier && sea.detectionRange > 4, "Orilla alcanzable y mar más rápido que sprint con aletas");
            var slow = AIExample(session, "baunco", new Vector3(125, -3, 35));
            var fast = AIExample(session, "jurel", new Vector3(125, -6, 70));
            yield return new WaitForSeconds(1);
            gun.SelectTool(0);
            ObserverAt(diver, slow.transform.position + Vector3.back * 3);
            float slowStart = Vector3.Distance(diver.View.transform.position, slow.transform.position);
            slow.Simulate(.05f);
            Check(slow.State == FishState.Fleeing && slow.CurrentSpeed > 0 && slow.CurrentSpeed <= coast.escapeSpeed + .01f && Vector3.Distance(diver.View.transform.position, slow.transform.position) > slowStart, "Pez de orilla detecta y se aleja despacio");
            for (int i = 0; i < 50 && Vector3.Distance(diver.View.transform.position, slow.transform.position) > 1.15f; i++)
            {
                ObserverAt(diver, Vector3.MoveTowards(diver.View.transform.position, slow.transform.position, 6 * .05f)); slow.Simulate(.05f);
            }
            diver.LookAt(slow.transform.position); Physics.SyncTransforms(); int count = diver.Inventory.FishCount;
            gun.Fire();
            Check(!slow.Available && diver.Inventory.FishCount == count + 1, "Persecución real a 6 m/s permite capturar con manos en la orilla");

            ObserverAt(diver, fast.transform.position + Vector3.back * 10);
            float nearest = 1000; bool habitat = true;
            for (int i = 0; i < 120; i++)
            {
                ObserverAt(diver, Vector3.MoveTowards(diver.View.transform.position, fast.transform.position, 9 * session.Config.shop.finSpeedMultiplier * .05f));
                fast.Simulate(.05f);
                nearest = Mathf.Min(nearest, Vector3.Distance(diver.View.transform.position, fast.transform.position));
                habitat &= fast.WithinHabitat(fast.transform.position);
            }
            Check(nearest > 5 && fast.Available && habitat, "Seis segundos de persecución con aletas no alcanzan al pez de mar (mínimo " + nearest.ToString("0.00") + " m)");
            yield return new WaitForSeconds(.9f);
            gun.SelectTool(1); diver.LookAt(fast.transform.position); Physics.SyncTransforms(); count = diver.Inventory.FishCount; gun.Fire();
            Check(fast.Available && diver.Inventory.FishCount == count, "Tridente no alcanza al pez que escapa en mar abierto");
            yield return new WaitForSeconds(.6f);
            fast.transform.position = new Vector3(125, -6, 80); fast.InitializeAI(diver, sea);
            ObserverAt(diver, new Vector3(125, -6, 70)); fast.Simulate(.05f);
            Check(fast.State == FishState.Fleeing && fast.CurrentSpeed > 12.15f, "Pez de mar entra en huida rápida");
            gun.SelectTool(2); diver.LookAt(fast.transform.position); Physics.SyncTransforms(); gun.Fire();
            Check(!fast.Available && diver.Inventory.FishCount == count + 1, "Arpón comprado captura al pez en huida desde fuera del alcance del tridente");
            Destroy(slow.gameObject); Destroy(fast.gameObject);

            var probe = AIExample(session, "jurel", new Vector3(125, -6, 100));
            ObserverAt(diver, new Vector3(125, -6, 94));
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Obstáculo de prueba IA"; wall.transform.position = new Vector3(125, -6, 97); wall.transform.localScale = new Vector3(8, 8, .5f);
            Physics.SyncTransforms(); probe.Simulate(.2f);
            Check(probe.State == FishState.Cruising, "Roca oculta al buzo y evita detección a través de paredes");
            wall.SetActive(false); Physics.SyncTransforms(); probe.Simulate(.2f);
            Check(probe.State == FishState.Fleeing, "Recuperar línea de visión provoca huida");
            Vector3 before = probe.transform.position; session.ToggleInventory(); probe.Simulate(.2f);
            Check(probe.transform.position == before, "Pausa detiene la IA y sus temporizadores"); session.ToggleInventory();

            probe.transform.position = new Vector3(125, -6, 100); probe.InitializeAI(diver, sea);
            wall.transform.position = new Vector3(125, -6, 102); wall.SetActive(true); Physics.SyncTransforms();
            bool outside = true;
            for (int i = 0; i < 60; i++) { probe.Simulate(.05f); outside &= !wall.GetComponent<Collider>().bounds.Contains(probe.transform.position) && probe.WithinHabitat(probe.transform.position); }
            Check(outside && Vector3.Distance(probe.transform.position, new Vector3(125, -6, 100)) > 3, "Huida rodea un obstáculo sin atravesarlo ni salir del hábitat");
            wall.SetActive(false); Destroy(wall);
            probe.transform.position = new Vector3(125, -6, 80); probe.InitializeAI(diver, sea);
            ObserverAt(diver, new Vector3(125, -6, 74));
            for (int i = 0; i < 20; i++) probe.Simulate(.05f);
            diver.Rescue("Prueba de calma de los peces");
            for (int i = 0; i < 40; i++) probe.Simulate(.05f);
            float away = Vector3.Distance(probe.Home, probe.transform.position); bool smooth = true;
            for (int i = 0; i < 40; i++) { before = probe.transform.position; probe.Simulate(.05f); smooth &= Vector3.Distance(before, probe.transform.position) <= sea.cruiseSpeed * .05f + .001f; }
            Check(probe.State == FishState.Returning && Vector3.Distance(probe.Home, probe.transform.position) < away && smooth, "Tras perder al buzo vuelve gradualmente, sin teletransporte");
            Destroy(probe.gameObject);

            var edge = AIExample(session, "baunco", new Vector3(125, -1.21f, 35));
            ObserverAt(diver, new Vector3(125, -3.5f, 33)); habitat = true;
            for (int i = 0; i < 60; i++) { edge.Simulate(.05f); habitat &= edge.WithinHabitat(edge.transform.position); }
            Check(habitat, "Fuga junto a superficie permanece sumergida y en su franja");
            Destroy(edge.gameObject); gun.SelectTool(0); diver.Rescue("Fin de prueba de fauna");
        }
    }
}
