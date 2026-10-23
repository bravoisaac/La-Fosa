using System;
using System.IO;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class GameSession
    {
        ExpeditionReport expeditionReport;
        double reportClock, nextReportWrite;
        bool reportEngaged, reportActive, reportWriteFailed, reportReturnRescued;

        void StartExpeditionReport(string mode)
        {
            var args = Environment.GetCommandLineArgs();
            if (Array.IndexOf(args, "--expedition-report") < 0 || Array.IndexOf(args, "--smoke-test") >= 0) return;
            FinishExpeditionReport("replaced");
            expeditionReport = new ExpeditionReport
            {
                mode = mode, build = Application.buildGUID, unity = Application.unityVersion,
                width = Screen.width, height = Screen.height, worldSeed = Config.seed,
                initialTreasures = Treasures, initialFish = Diver.Inventory.FishCount, initialMoney = Economy.Money,
                initialTrident = Economy.Has(GearKind.Trident), initialSeal = Array.Find(puzzles, p => p.zone == Config.zones[0]).Solved
            };
            reportClock = Time.realtimeSinceStartupAsDouble;
            nextReportWrite = reportClock + 15;
            reportEngaged = !Intro && !Paused;
            reportActive = !Blocked;
            reportReturnRescued = false;
            reportWriteFailed = false;
            WriteExpeditionReport();
        }
        // Stored modes account for the interval BEFORE ApplyPause changes the clock mode.
        void SyncReportClock()
        {
            if (expeditionReport == null) return;
            double now = Time.realtimeSinceStartupAsDouble;
            expeditionReport.Advance(now - reportClock, reportEngaged, reportActive);
            reportClock = now;
        }
        void UpdateReportMode()
        {
            SyncReportClock();
            reportEngaged = !Intro && !Paused;
            reportActive = !Blocked;
        }
        void UpdateExpeditionReport()
        {
            if (expeditionReport == null) return;
            SyncReportClock();
            if (!Blocked && !expeditionReport.initialSeal &&
                Vector3.Distance(Diver.View.transform.position, Config.zones[0].stationPosition + Vector3.up * 3) <= 5)
                RecordExpeditionEvent("shrine", "first_zone_within_5m", true);
            if (reportClock >= nextReportWrite) WriteExpeditionReport();
        }
        void ReportPuzzleChange()
        {
            if (!restoring && expeditionReport != null && !expeditionReport.initialSeal && Array.Find(puzzles, p => p.zone == Config.zones[0]).Solved)
                RecordExpeditionEvent("seal", Config.zones[0].id, true);
        }
        internal void RecordCaptureAttempt(int tool)
        {
            if (expeditionReport == null || restoring || Intro) return;
            SyncReportClock();
            expeditionReport.Attempt(tool);
        }
        internal void RecordExpeditionEvent(string kind, string detail = "", bool once = false)
        {
            if (expeditionReport == null || restoring || Intro) return;
            SyncReportClock();
            if (expeditionReport.Record(kind, detail, Diver.Oxygen, Diver.Depth, once)) WriteExpeditionReport();
        }
        internal void RecordRescue(string reason)
        {
            if (restoring || Intro || expeditionReport == null) return;
            if (firstReturnPending) reportReturnRescued = true;
            RecordExpeditionEvent("rescue", reason);
        }
        void ReportExpeditionReturn()
        {
            RecordExpeditionEvent(reportReturnRescued ? "return_rescued" : "return", "first_treasure_to_beach", true);
        }
        void WriteExpeditionReport()
        {
            if (expeditionReport == null) return;
            SyncReportClock();
            string directory = Path.Combine(Application.persistentDataPath, "ExpeditionReports");
            bool saved = expeditionReport.TryWrite(directory, out string error);
            if (!saved && !reportWriteFailed) Debug.LogWarning("No se pudo escribir el informe de recorrido: " + error);
            reportWriteFailed = !saved;
            nextReportWrite = reportClock + 15;
        }
        void FinishExpeditionReport(string reason)
        {
            if (expeditionReport == null) return;
            SyncReportClock();
            expeditionReport.Finish(reason);
            WriteExpeditionReport();
            expeditionReport = null;
        }
    }
}
