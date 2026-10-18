using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace LaFosa
{
    [Serializable] public sealed class SavedStack { public string id; public int count; }
    [Serializable] public sealed class SavedPuzzle
    {
        public string id;
        public int[] turns;
        public int moves;
        public bool solved;
    }
    [Serializable] public sealed class SavedVehicle { public string id; public Vector3 position; public float yaw; }
    [Serializable] public sealed class SaveData
    {
        public int version;
        public int worldSeed;
        public int money;
        public int selectedTool;
        public SavedStack[] inventory;
        public string[] gear;
        public string[] treasures;
        public string[] discoveredZones;
        public string[] collectedPickups;
        public SavedPuzzle[] puzzles;
        public SavedVehicle[] vehicles;
    }

    // The envelope detects truncated/edited data before any live state is changed.
    // This checksum is corruption detection, not an anti-cheat mechanism.
    public sealed class SaveStore
    {
        [Serializable] sealed class Envelope { public string payload; public string checksum; }
        public string FilePath { get; }
        public string BackupPath => FilePath + ".bak";
        public bool HasFiles => File.Exists(FilePath) || File.Exists(BackupPath);
        readonly Func<SaveData, bool> validate;
        const int MaxBytes = 1024 * 1024;
        public SaveStore(string path, Func<SaveData, bool> validator) { FilePath = path; validate = validator; }
        static string Hash(string value)
        {
            using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
        public bool TryLoad(out SaveData data, out string message)
        {
            if (TryRead(FilePath, out data)) { message = "Partida disponible. Continuar te devuelve a la playa."; return true; }
            if (TryRead(BackupPath, out data)) { message = "Guardado principal no válido o ausente. Se recuperará la copia de respaldo."; return true; }
            message = HasFiles ? "No se pudo leer la partida ni su respaldo: archivo dañado o versión incompatible. Nueva partida requiere confirmación." : "Sin partida guardada. Empieza una nueva expedición.";
            return false;
        }
        bool TryRead(string path, out SaveData data)
        {
            data = null;
            try
            {
                if (!File.Exists(path) || new FileInfo(path).Length > MaxBytes) return false;
                var envelope = JsonUtility.FromJson<Envelope>(File.ReadAllText(path, Encoding.UTF8));
                if (envelope == null || string.IsNullOrEmpty(envelope.payload) || Hash(envelope.payload) != envelope.checksum) return false;
                var candidate = JsonUtility.FromJson<SaveData>(envelope.payload);
                if (candidate == null || !validate(candidate)) return false;
                data = candidate; return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is NotSupportedException)
            { return false; }
        }
        public bool TrySave(SaveData data, out string message, bool newGame = false)
        {
            message = "No se pudo guardar. El progreso sigue en memoria; vuelve a intentar guardar desde pausa.";
            try
            {
                if (!validate(data)) return false;
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                string payload = JsonUtility.ToJson(data);
                byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Envelope { payload = payload, checksum = Hash(payload) }, true));
                string temporary = FilePath + ".tmp";
                using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
                if (File.Exists(FilePath))
                {
                    // Never replace a good fallback with an invalid primary file.
                    bool validPrimary = TryRead(FilePath, out _);
                    File.Replace(temporary, FilePath, validPrimary && !newGame ? BackupPath : null);
                }
                else File.Move(temporary, FilePath);
                // A new expedition must not resurrect the previous expedition via fallback.
                if (newGame || !File.Exists(BackupPath)) File.Copy(FilePath, BackupPath, true);
                message = "Partida guardada. Al continuar volverás a la playa.";
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is NotSupportedException)
            { return false; }
        }
    }
}
