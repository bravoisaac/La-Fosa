using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        public void BuildDescentGuides()
        {
            var mint = Mat("Guía de aire y ascenso", new Color(.2f, .95f, .73f), true);
            for (int i = 1; i < config.zones.Length; i++)
            {
                var zone = config.zones[i];
                var guide = new GameObject("Descenso libre · " + zone.id);
                guide.transform.SetParent(root, false);
                guide.transform.position = zone.entryPosition;
                // Signals have no colliders: equipment affects air, never passage.
                Shape("Baliza de profundidad", PrimitiveType.Cube, new Vector3(4, 1.2f, 0),
                    new Vector3(.2f, 1.2f, .2f), mint, false, guide.transform);
                var sign = Label(zone.displayName + "\nDESCENSO LIBRE · VIGILA TU AIRE\nFuera del límite del equipo, el aire se consume más rápido",
                    zone.entryPosition + new Vector3(4, 2.8f, 0), .13f, new Color(.65f, 1, .88f));
                sign.transform.SetParent(guide.transform, true);
                var returnSign = Label("↑ SUPERFICIE\nRESERVA AIRE PARA VOLVER", zone.entryPosition + new Vector3(4, -3.5f, 0),
                    .13f, new Color(.65f, 1, .88f));
                returnSign.transform.SetParent(guide.transform, true);
                for (float y = zone.stationPosition.y + 4; y < zone.entryPosition.y - 2; y += 5)
                    Shape("Guía de ascenso", PrimitiveType.Sphere, new Vector3(4, y - zone.entryPosition.y, 0),
                        Vector3.one * .2f, mint, false, guide.transform);
            }
        }
    }
}
