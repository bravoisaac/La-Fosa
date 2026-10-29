using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class ExpeditionReportChecks
    {
        [MenuItem("La Fosa/Validar informes (sin jugar)")]
        public static void Validate()
        {
            int checks = 0;
            void Check(bool condition, string message)
            {
                if (!condition) throw new Exception("LA_FOSA_REPORT_FAILED: " + message);
                checks++;
            }

            var report = new ExpeditionReport { mode = "new" };
            report.Advance(10, true, true); // Swimming.
            report.Advance(5, true, false); // Shop/inventory.
            report.Advance(20, false, false); // Pause/focus loss.
            Check(report.elapsedSeconds == 35 && report.engagedSeconds == 15 && report.activeSeconds == 10, "Separate clock modes");
            report.Advance(-5, true, true);
            report.Advance(double.NaN, true, true);
            report.Advance(double.PositiveInfinity, true, true);
            Check(report.elapsedSeconds == 35, "Invalid intervals cannot corrupt clocks");
            report.Attempt(0); report.Attempt(0); report.Attempt(1); report.Attempt(-1); report.Attempt(4);
            Check(report.attemptsByTool[0] == 2 && report.attemptsByTool[1] == 1, "Attempts separated by tool");
            report.Record("capture", "jurel", 80, 3);
            report.Advance(2, true, true);
            report.Record("capture", "jurel", 78, 3);
            Check(report.captures == 2 && report.events.Count == 1 && report.events[0].engagedSeconds == 15, "Preserve first-catch timing and total captures");
            Check(report.events[0].oxygen == 80 && report.events[0].depth == 3, "Observation retains pre-refill vitals");
            report.Record("shop", "", 100, 0);
            report.Record("shop", "", 100, 0);
            report.Record("sale", "2 fish / 20", 100, 0);
            report.Record("rescue", "oxygen_depleted", 0, 23);
            Check(report.shopVisits == 2 && report.sales == 1 && report.rescues == 1, "Separate visits, transactions and rescues");
            Check(report.Record("treasure", "huiros", 18, 22, true), "First milestone recorded");
            Check(!report.Record("treasure", "huiros", 155, 22, true), "Repeated milestone ignored");
            report.Record("return_rescued", "first_treasure_to_beach", 155, 0, true);
            var returned = report.events[report.events.Count - 1];
            report.Attempt(0); report.Record("capture", "jurel", 100, 2);
            Check(returned.attemptsByTool[0] == 2 && returned.captures == 2 && returned.rescues == 1, "Return snapshot unaffected by later excursions");
            Check(!report.events.Exists(e => e.kind == "return"), "Rescued return does not imply an unassisted return");

            string directory = Path.Combine(Path.GetTempPath(), "LaFosaReportChecks-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, report.id + ".json");
            try
            {
                Check(report.TryWrite(directory, out _), "First JSON write");
                string previous = File.ReadAllText(path);
                var decoded = JsonUtility.FromJson<ExpeditionReport>(previous);
                Check(decoded.events.Count == report.events.Count && decoded.events[0].attemptsByTool[0] == 2 && decoded.engagedSeconds == 17, "JSON round trip retains events, snapshots and clocks");
                report.Advance(1, true, true);
                using (var locked = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    Check(!report.TryWrite(directory, out string error) && !string.IsNullOrEmpty(error), "Locked destination reports failure without throwing");
                Check(File.ReadAllText(path) == previous, "Failed replacement preserves previous report");
                Check(report.TryWrite(directory, out _), "Write can retry after failure");
                Check(JsonUtility.FromJson<ExpeditionReport>(File.ReadAllText(path)).engagedSeconds == 18, "Replacement contains latest data");
                report.Finish("menu");
                double finalTime = report.elapsedSeconds;
                int finalAttempts = report.attemptsByTool[0];
                report.Advance(10, true, true); report.Attempt(0);
                Check(!report.Record("shop", "", 100, 0) && report.elapsedSeconds == finalTime && report.attemptsByTool[0] == finalAttempts, "Finished segment is frozen");
                report.Finish("quit");
                Check(report.status == "menu", "Finish is idempotent");
                Check(report.TryWrite(directory, out _) && JsonUtility.FromJson<ExpeditionReport>(File.ReadAllText(path)).status == "menu", "Final status persisted");
                var continued = new ExpeditionReport { mode = "continue", initialTrident = true, initialTreasures = 1 };
                Check(continued.id != report.id && continued.events.Count == 0 && continued.rescues == 0 && continued.engagedSeconds == 0, "Independent continuation segment");
            }
            finally
            {
                // Only these two files are owned by this check; no recursive deletion.
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(path + ".tmp")) File.Delete(path + ".tmp");
                if (Directory.Exists(directory)) Directory.Delete(directory);
            }
            Debug.Log("LA_FOSA_REPORT_TESTS_OK · " + checks + " comprobaciones de modelo/archivo en editor, sin Play Mode.");
        }
    }
}
