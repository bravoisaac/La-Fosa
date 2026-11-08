using System;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public enum DeveloperScenario { FirstExpedition, FirstSanctuary }

    public sealed partial class GameSession
    {
        SaveData developerSnapshot;
        public bool DeveloperSession => developerSnapshot != null;
        public bool DeveloperMode { get; private set; }
        public bool DeveloperPanelOpen { get; private set; }
        public bool DeveloperInfiniteAir { get; private set; }
        public bool DeveloperFastMove { get; private set; }

        public void ToggleDeveloperMode()
        {
            if (Intro || Diver == null) return;
            if (!DeveloperSession)
            {
                if (!SaveNow()) { Notify("No se pudo guardar tu progreso. No se iniciaron las pruebas."); return; }
                developerSnapshot = CaptureSave();
                FinishExpeditionReport("developer_session");
            }
            DeveloperMode = !DeveloperMode;
            DeveloperPanelOpen = DeveloperMode;
            Paused = false; InventoryOpen = false; ShopOpen = false;
            SaveStatus = "PRUEBA TEMPORAL · los cambios no se guardan.";
            ApplyPause();
            Notify(DeveloperMode ? "DEVELOP · F1 herramientas · F2 probar como jugador" : "JUGADOR DE PRUEBA · reglas normales · F2 volver a Develop");
        }
        public void ToggleDeveloperPanel()
        {
            if (!DeveloperSession || Intro) return;
            DeveloperPanelOpen = !DeveloperPanelOpen;
            Paused = false; InventoryOpen = false; ShopOpen = false;
            ApplyPause();
        }
        public void EndDeveloperSession()
        {
            if (!DeveloperSession) return;
            // Keep writes blocked until the entire original progress has been restored.
            ApplySave(developerSnapshot);
            developerSnapshot = null;
            DeveloperMode = false; DeveloperPanelOpen = false;
            DeveloperInfiniteAir = false; DeveloperFastMove = false;
            SaveStatus = "Pruebas descartadas. Progreso original recuperado en la playa.";
            ApplyPause(); StartExpeditionReport("after_developer"); Notify(SaveStatus);
        }
        public void ToggleDeveloperAir() { if (DeveloperMode) DeveloperInfiniteAir = !DeveloperInfiniteAir; }
        public void ToggleDeveloperSpeed() { if (DeveloperMode) DeveloperFastMove = !DeveloperFastMove; }
        public bool DeveloperPrepareScenario(DeveloperScenario scenario)
        {
            if (!DeveloperSession || !DeveloperMode || Intro) return false;
            if (scenario != DeveloperScenario.FirstExpedition && scenario != DeveloperScenario.FirstSanctuary) return false;
            var fresh = CreateFreshSave();
            bool sanctuary = scenario == DeveloperScenario.FirstSanctuary;
            if (sanctuary)
            {
                fresh.gear = new[] { nameof(GearKind.Trident) };
                fresh.selectedTool = 1;
            }
            // Apply only to the temporary session. Never replace developerSnapshot or save to disk.
            ApplySave(fresh);
            DeveloperInfiniteAir = false; DeveloperFastMove = false;
            if (sanctuary)
            {
                var station = Config.zones[0].stationPosition;
                Diver.LeaveVehicle(station + new Vector3(0, 2, 3));
                Diver.LookAt(station + Vector3.up * 2);
                Physics.SyncTransforms();
            }
            DeveloperPanelOpen = true;
            SaveStatus = "PRUEBA TEMPORAL · escenario reiniciado sin guardar.";
            ApplyPause();
            Notify(sanctuary ? "Primer santuario: tridente listo y sello sin resolver. F2 para probar como jugador."
                : "Inicio: manos, mochila vacía y sin dinero. F2 para probar como jugador.");
            return true;
        }
        public void DeveloperAddMoney()
        {
            if (!DeveloperMode) return;
            var state = CaptureSave();
            Economy.Restore((int)Math.Min(int.MaxValue, (long)Economy.Money + 1000), state.gear);
            Notify("+ $1000 de prueba. No se guardan.");
        }
        public void DeveloperEquip()
        {
            if (!DeveloperMode) return;
            var extra = new[] { GearKind.Trident, GearKind.ReinforcedHarpoon, GearKind.Net, GearKind.Fins, GearKind.OxygenTank };
            Economy.Restore(Economy.Money, CaptureSave().gear.Concat(extra.Select(g => g.ToString())).Distinct().ToArray());
            Diver.Upgrade(DeepTrenchProfile.BottomDepth, 1400);
            Diver.ApplyOxygenTank(Config.shop.oxygenBonus);
            RefreshRoutes(); Notify("Equipo de prueba preparado; los puzles conservan su progreso.");
        }
        public void DeveloperTravel(float depth)
        {
            if (!DeveloperMode) return;
            float viewOffset = Diver.View.transform.position.y - Diver.transform.position.y;
            var axis = DeepTrenchProfile.Axis;
            Vector3 target = depth <= 0 ? Config.spawn : new Vector3(axis.x, Config.waterLevel - Mathf.Clamp(depth, 1, 620) - viewOffset, axis.y);
            Diver.LeaveVehicle(target);
            Physics.SyncTransforms();
            Notify(depth <= 0 ? "Playa de prueba." : "Profundidad de prueba: " + depth.ToString("0") + " m.");
        }
    }
}
