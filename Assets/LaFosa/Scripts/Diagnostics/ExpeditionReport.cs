using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LaFosa
{
    // Separate from SaveData: observations never become progression or rewards.
    [Serializable]
    public sealed class ExpeditionReport
    {
        public int version = 1;
        public string id = Guid.NewGuid().ToString("N");
        public string startedUtc = DateTime.UtcNow.ToString("o");
        public string mode, build, unity, status = "recording";
        public int width, height, worldSeed, initialTreasures, initialFish, initialMoney;
        public bool initialTrident, initialSeal;
        public double elapsedSeconds, engagedSeconds, activeSeconds;
        public int[] attemptsByTool = new int[4];
        public int captures, shopVisits, sales, rescues;
        public List<ExpeditionObservation> events = new List<ExpeditionObservation>();

        public void Advance(double seconds, bool engaged, bool active)
        {
            if (status != "recording" || seconds <= 0 || double.IsNaN(seconds) || double.IsInfinity(seconds)) return;
            elapsedSeconds += seconds;
            if (engaged) engagedSeconds += seconds;
            if (engaged && active) activeSeconds += seconds;
        }
        public void Attempt(int tool)
        {
            if (status == "recording" && tool >= 0 && tool < attemptsByTool.Length) attemptsByTool[tool]++;
        }
        public bool Record(string kind, string detail, float oxygen, float depth, bool once = false)
        {
            if (status != "recording") return false;
            if (once && events.Exists(e => e.kind == kind)) return false;
            if (kind == "capture")
            {
                captures++;
                // Keep first-catch timing without growing the journal for every fish.
                if (captures > 1) return true;
            }
            if (kind == "shop") shopVisits++;
            if (kind == "sale") sales++;
            if (kind == "rescue") rescues++;
            events.Add(new ExpeditionObservation
            {
                kind = kind, detail = detail, oxygen = oxygen, depth = depth,
                elapsedSeconds = elapsedSeconds, engagedSeconds = engagedSeconds, activeSeconds = activeSeconds
            });
            return true;
        }
        public void Finish(string reason)
        {
            if (status == "recording") status = reason;
        }
        public bool TryWrite(string directory, out string error)
        {
            try
            {
                Directory.CreateDirectory(directory);
                string path = Path.Combine(directory, id + ".json");
                string temporary = path + ".tmp";
                File.WriteAllText(temporary, JsonUtility.ToJson(this, true));
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
                error = null;
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is ArgumentException || ex is NotSupportedException)
            {
                error = ex.Message;
                return false;
            }
        }
    }

    [Serializable]
    public sealed class ExpeditionObservation
    {
        public string kind, detail;
        public double elapsedSeconds, engagedSeconds, activeSeconds;
        public float oxygen, depth;
    }
}
