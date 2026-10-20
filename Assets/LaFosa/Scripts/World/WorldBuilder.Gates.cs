using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        public void BuildDiveGates(GameSession session)
        {
            var puzzles = root.GetComponentsInChildren<StoneCircuitPuzzle>();
            var stone = Mat("Losa del descenso", new Color(.18f, .29f, .31f));
            var bronze = Mat("Escotilla antigua", new Color(.38f, .3f, .17f));
            var amber = Mat("Ruta cerrada", new Color(1, .6f, .17f), true);
            var mint = Mat("Ruta abierta", new Color(.2f, .95f, .73f), true);
            for (int i = 1; i < config.zones.Length; i++)
            {
                var zone = config.zones[i];
                if (!zone.hasPhysicalGate) continue;
                var obj = new GameObject("Compuerta · " + zone.id); obj.transform.SetParent(root, false); obj.transform.position = zone.entryPosition;
                var gate = obj.AddComponent<DiveGate>();
                float half = zone.barrierWidth / 2, hole = DiveGate.OpeningWidth / 2;
                // Four solid slabs enclose the only opening. Outer edges are buried in terrain.
                // Small overlaps seal the joints without relying on one-sided mesh collision.
                foreach (float side in new[] { -1f, 1f })
                {
                    Shape("Losa lateral", PrimitiveType.Cube, new Vector3(side * (half + hole) / 2, 0, 0), new Vector3(half - hole + .02f, 1, zone.barrierWidth), stone, true, obj.transform);
                    Shape("Losa frontal", PrimitiveType.Cube, new Vector3(0, 0, side * (half + hole) / 2), new Vector3(DiveGate.OpeningWidth + .02f, 1, half - hole + .02f), stone, true, obj.transform);
                    Shape("Borde luminoso", PrimitiveType.Cube, new Vector3(side * (hole + .2f), .56f, 0), new Vector3(.12f, .08f, DiveGate.OpeningWidth + .5f), mint, false, obj.transform);
                    Shape("Borde luminoso", PrimitiveType.Cube, new Vector3(0, .56f, side * (hole + .2f)), new Vector3(DiveGate.OpeningWidth + .5f, .08f, .12f), mint, false, obj.transform);
                }
                gate.panel = Shape("Escotilla deslizante", PrimitiveType.Cube, Vector3.zero, new Vector3(DiveGate.OpeningWidth + .02f, 1, DiveGate.OpeningWidth + .02f), bronze, true, obj.transform).transform;
                for (int bar = -2; bar <= 2; bar++)
                    Shape("Canal del cierre", PrimitiveType.Cube, new Vector3(bar * .16f, .53f, 0), new Vector3(.025f, .04f, .9f), amber, false, gate.panel);
                gate.signal = Shape("Baliza del paso", PrimitiveType.Cube, new Vector3(-hole - .8f, 1.2f, -hole), new Vector3(.35f, 1.5f, .35f), amber, true, obj.transform).GetComponent<Renderer>();
                gate.sign = Label("", zone.entryPosition + new Vector3(0, 2.6f, -hole - .6f), .14f, Color.white).GetComponent<TextMesh>();
                gate.sign.transform.SetParent(obj.transform, true);
                gate.returnSign = Label("↑ SUPERFICIE\nREGRESA POR LA ESCOTILLA", zone.entryPosition + new Vector3(0, -3.5f, 0), .13f, new Color(.5f, 1, .82f)).GetComponent<TextMesh>();
                gate.returnSign.transform.SetParent(obj.transform, true);
                gate.lockedMaterial = amber; gate.openMaterial = mint;
                gate.Initialize(session, zone, puzzles.First(p => p.zone == config.zones[i - 1]));
                // A luminous guide avoids leaving the lower room without a visible exit.
                for (float y = zone.stationPosition.y + 4; y < zone.entryPosition.y - 2; y += 5)
                    Rock("Guía de ascenso", new Vector3(zone.entryPosition.x + 3, y, zone.entryPosition.z), Vector3.one * .17f, mint, false);
            }
        }
    }
}
