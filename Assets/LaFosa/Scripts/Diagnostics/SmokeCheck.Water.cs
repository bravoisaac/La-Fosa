using System.Collections;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        // Optional visual inspection, with the same isolated save folder as other diagnostics.
        IEnumerator CaptureWaterViews(GameSession session)
        {
            var diver = session.Diver;
            var points = new[]
            {
                new Vector3(0, 5, -12), new Vector3(0, 3, 1),
                new Vector3(0, 3, 32), new Vector3(0, 3, 70),
                new Vector3(0, -6, 32), new Vector3(0, -6, 68), new Vector3(0, -24, 67)
            };
            var targets = new[]
            {
                new Vector3(0, 0, 64), new Vector3(0, -3, 6),
                new Vector3(0, -6, 38), new Vector3(0, -32, 75),
                new Vector3(0, -6, 53), new Vector3(0, -6, 88), new Vector3(0, -32, 75)
            };
            var names = new[] { "agua-panorama-playa.png", "agua-orilla.png", "agua-costa-desde-arriba.png", "agua-fosa-desde-arriba.png", "agua-costa-sumergido.png", "agua-fosa-sumergido.png", "agua-fosa-profunda.png" };
            for (int i = 0; i < points.Length; i++)
            {
                Teleport(diver, points[i]); diver.LookAt(targets[i]);
                yield return new WaitForSeconds(.15f);
                Capture(diver.View, names[i]);
                Debug.Log("WATER_VIEW: " + names[i] + " fog=" + RenderSettings.fogDensity.ToString("F3"));
            }
            diver.Rescue("Fin de la revisión visual.");
        }
    }
}
