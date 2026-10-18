using UnityEngine;

namespace LaFosa
{
    public sealed class TreasureChest : Interactable
    {
        public StoneCircuitPuzzle puzzle;
        public Transform lid;
        public bool Claimed { get; private set; }
        internal void Restore(bool claimed)
        {
            Claimed = claimed;
            if (lid != null) lid.localRotation = Quaternion.Euler(claimed ? -65 : 0, 0, 0);
        }
        public override string Prompt => Claimed ? "Tesoro recuperado" : puzzle.Solved ? "Abrir tesoro · mejorar equipo" : "Leer pista del mecanismo";
        public override void Interact(DiverController diver)
        {
            if (Claimed) return;
            if (!diver.Session.IsZoneUnlocked(puzzle.zone)) { diver.Session.Notify(diver.Session.ZoneRequirement(puzzle.zone)); return; }
            if (!puzzle.Solved) { diver.Session.Notify("Inscripción: " + puzzle.Hint); return; }
            if (!diver.Inventory.TryAdd(puzzle.zone.treasure)) { diver.Session.Notify("Libera espacio en tu mochila para el tesoro."); return; }
            Claimed = true;
            if (lid != null) lid.localRotation = Quaternion.Euler(-65, 0, 0);
            diver.Session.Reward(puzzle.zone);
        }
    }
}
