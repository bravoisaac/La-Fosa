using System;
using UnityEngine;

namespace LaFosa
{
    public sealed class MineralDeposit : Interactable
    {
        public string id;
        public ItemDefinition mineral;
        public int amount = 3;
        public const int RegrowSeconds = 180;
        public long ReadyAtUtcTicks { get; private set; }
        bool? shownAvailable;
        MaterialPropertyBlock appearance;
        public bool Available => DateTime.UtcNow.Ticks >= ReadyAtUtcTicks;
        public override string Prompt => Available ? "Extraer " + mineral.displayName + " ×" + amount + " · requiere pico"
            : mineral.displayName + " agotado · vuelve en " + Mathf.CeilToInt((float)TimeSpan.FromTicks(Math.Max(0, ReadyAtUtcTicks - DateTime.UtcNow.Ticks)).TotalSeconds) + " s";
        public override void Interact(DiverController diver) { TryMine(diver); }
        public bool TryMine(DiverController diver)
        {
            if (diver == null || diver.Session.Blocked || !diver.Underwater || !Available) return false;
            if (Vector3.Distance(diver.View.transform.position, transform.position) > 4.5f) return false;
            if (Physics.Linecast(diver.View.transform.position, transform.position, out var hit, ~0, QueryTriggerInteraction.Ignore)
                && hit.collider.GetComponentInParent<MineralDeposit>() != this) return false;
            if (!diver.Session.Economy.Has(GearKind.MiningPick)) { diver.Session.Notify("Compra un pico submarino en la caleta para extraer minerales."); return false; }
            if (!diver.Inventory.TryAdd(mineral, amount)) { diver.Session.Notify("Mochila llena. Libera espacio para extraer el mineral."); return false; }
            ReadyAtUtcTicks = DateTime.UtcNow.AddSeconds(RegrowSeconds).Ticks;
            UpdateAppearance();
            diver.Session.MarkMineralTrip();
            diver.Session.RecordExpeditionEvent("mineral", mineral.id + " / " + amount);
            diver.Session.Notify("+" + amount + " " + mineral.displayName + ". Úsalo en FABRICAR, en la caleta.");
            diver.Session.SaveNow(); return true;
        }
        internal void Restore(long readyAtUtcTicks) { ReadyAtUtcTicks = readyAtUtcTicks; UpdateAppearance(); }
        void Update() { UpdateAppearance(); }
        void UpdateAppearance()
        {
            if (mineral == null || shownAvailable == Available) return;
            shownAvailable = Available;
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                if (appearance == null) appearance = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(appearance);
                appearance.SetColor("_Color", Available ? mineral.color : new Color(.16f, .19f, .2f));
                appearance.SetColor("_EmissionColor", Available ? mineral.color * .35f : Color.black);
                renderer.SetPropertyBlock(appearance);
            }
            var glow = GetComponent<Light>(); if (glow != null) glow.enabled = Available;
        }
    }
}
