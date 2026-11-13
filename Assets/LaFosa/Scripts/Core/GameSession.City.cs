using UnityEngine;

namespace LaFosa
{
    public sealed partial class GameSession
    {
        public const int FishingCapacity = 10;
        public const float FishingSeconds = 60;
        public static readonly Vector3 BaseArrival = new Vector3(-12, 2.55f, 76);
        public static readonly float[] DescentStops = { 24, 52, 88, 150, 280, 420, 620 };
        public int InfrastructureLevel { get; private set; }
        public float ExploredDepth { get; private set; }
        readonly int[] automaticFish = new int[2];
        readonly float[] fishingProgress = new float[2];
        bool transitFromBase;
        public int FishingStock(int index) => index >= 0 && index < 2 ? automaticFish[index] : 0;
        public bool FishingUnlocked(int index) => index >= 0 && index < 2 && InfrastructureLevel >= (index == 0 ? 2 : 3);
        internal void CompleteConstruction() { InfrastructureLevel++; RefreshRoutes(); }
        internal void AdvanceCity(float seconds)
        {
            if (Blocked || seconds <= 0 || !Finite(seconds)) return;
            if (Diver.Depth > ExploredDepth)
            {
                float previous = ExploredDepth;
                ExploredDepth = Mathf.Min(DeepTrenchProfile.BottomDepth, Diver.Depth);
                // Save unlocked destinations immediately; fine-grained depth uses the periodic save.
                foreach (float stop in DescentStops) if (previous < stop && ExploredDepth >= stop) { MarkSaveDirty(); break; }
            }
            for (int i = 0; i < 2; i++)
            {
                if (!FishingUnlocked(i) || automaticFish[i] >= FishingCapacity) continue;
                fishingProgress[i] += Mathf.Min(seconds, FishingSeconds * FishingCapacity);
                int caught = Mathf.Min(FishingCapacity - automaticFish[i], Mathf.FloorToInt(fishingProgress[i] / FishingSeconds));
                automaticFish[i] += caught; fishingProgress[i] -= caught * FishingSeconds;
                if (automaticFish[i] == FishingCapacity) fishingProgress[i] = 0;
                // Periodic saves retain the partial timer without a disk write each frame.
                if (caught > 0) MarkSaveDirty();
            }
        }
        public bool CollectAutomaticFish(int index)
        {
            if (Blocked || !FishingUnlocked(index) || automaticFish[index] == 0) return false;
            int amount = automaticFish[index];
            if (!Diver.Inventory.TryAdd(Config.fish, amount))
            { Notify("Libera espacio para " + amount + " peces. La captura sigue guardada en el puesto."); return false; }
            automaticFish[index] = 0;
            RecordExpeditionEvent("automatic_fish", amount.ToString());
            Notify("Recogiste " + amount + " peces. Véndelos en Don Lucho."); SaveNow(); return true;
        }
        public void OpenTransit(bool fromBase)
        {
            if (InfrastructureLevel < 4) { Notify("Encarga la base en el Astillero Bruma."); return; }
            transitFromBase = fromBase; OpenShop(ShopKind.Transit);
        }
        // -1 = city, 0 = base, positive = previously explored depth.
        public string TravelRequirement(float depth)
        {
            if (!ShopOpen || CurrentShop != ShopKind.Transit || InfrastructureLevel < 4) return "Usa un terminal de enlace";
            if (depth == -1 || depth == 0) return "";
            if (System.Array.IndexOf(DescentStops, depth) < 0) return "Destino no disponible";
            if (!transitFromBase || InfrastructureLevel < 5) return "Requiere descensor en la base";
            if (ExploredDepth + .1f < depth) return "Explora primero esta profundidad";
            if (Diver.SafeDepth < depth) return "Mejora tu equipo de buceo";
            if (Diver.Oxygen < Config.ReturnAir(depth, Diver.SafeDepth, 6 * Diver.SwimSpeedMultiplier) * 1.2f + 20)
                return "Recupera aire antes de bajar";
            return "";
        }
        public bool Travel(float depth)
        {
            string requirement = TravelRequirement(depth);
            if (requirement.Length > 0) { Notify(requirement); return false; }
            float offset = Diver.View.transform.position.y - Diver.transform.position.y;
            Vector3 target = depth < 0 ? Config.spawn : depth == 0 ? BaseArrival + Vector3.up * Config.waterLevel
                : new Vector3(DeepTrenchProfile.Axis.x, Config.waterLevel - depth - offset, DeepTrenchProfile.Axis.y);
            var controller = Diver.GetComponent<CharacterController>();
            Vector3 center = target + controller.center;
            float half = Mathf.Max(0, controller.height * .5f - controller.radius);
            foreach (var hit in Physics.OverlapCapsule(center - Vector3.up * half, center + Vector3.up * half, controller.radius, ~0, QueryTriggerInteraction.Ignore))
                if (!hit.transform.IsChildOf(Diver.transform)) { Notify("La llegada está obstruida. Inténtalo cuando quede libre."); return false; }
            CloseShop(); Diver.LeaveVehicle(target); Diver.LookAt(target + Vector3.forward * 5); Physics.SyncTransforms();
            RecordExpeditionEvent("travel", depth.ToString());
            Notify(depth < 0 ? "De vuelta en Caleta Bruma." : depth == 0 ? "Base de la fosa. Enlace y descensor junto al panel." : "Descenso a " + depth + " m. Conserva aire para regresar.");
            SaveNow(); return true;
        }
        void RestoreCity(SaveData data)
        {
            InfrastructureLevel = data.infrastructureLevel; ExploredDepth = data.exploredDepth;
            for (int i = 0; i < 2; i++)
            { automaticFish[i] = data.automaticFish == null ? 0 : data.automaticFish[i]; fishingProgress[i] = data.fishingProgress == null ? 0 : data.fishingProgress[i]; }
        }
        bool ValidateCity(SaveData data)
        {
            if (data.infrastructureLevel < 0 || data.infrastructureLevel > Config.infrastructure.Length || !Finite(data.exploredDepth) || data.exploredDepth < 0 || data.exploredDepth > DeepTrenchProfile.BottomDepth) return false;
            // Absent optional fields are older v1/v2 saves with no city progress.
            if (data.automaticFish == null && data.fishingProgress == null) return data.infrastructureLevel == 0;
            if (data.automaticFish == null || data.fishingProgress == null || data.automaticFish.Length != 2 || data.fishingProgress.Length != 2) return false;
            for (int i = 0; i < 2; i++)
            {
                if (data.automaticFish[i] < 0 || data.automaticFish[i] > FishingCapacity || !Finite(data.fishingProgress[i]) || data.fishingProgress[i] < 0 || data.fishingProgress[i] >= FishingSeconds) return false;
                if (data.infrastructureLevel < (i == 0 ? 2 : 3) && (data.automaticFish[i] != 0 || data.fishingProgress[i] != 0)) return false;
                if (data.automaticFish[i] == FishingCapacity && data.fishingProgress[i] != 0) return false;
            }
            return true;
        }
    }
}
