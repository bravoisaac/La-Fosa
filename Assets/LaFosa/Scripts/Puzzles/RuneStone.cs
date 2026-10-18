using UnityEngine;

namespace LaFosa
{
    public sealed class RuneStone : Interactable
    {
        public StoneCircuitPuzzle puzzle;
        public int index;
        public Transform rotor;
        public Renderer[] channels;
        public Material dormant, flowing, complete;
        public override string Prompt => puzzle.Solved ? "Mecanismo abierto" : "Girar piedra " + (index + 1) + " · 90°";
        void Start() { puzzle.Changed += Refresh; Refresh(); }
        void OnDestroy() { if (puzzle != null) puzzle.Changed -= Refresh; }
        void Update()
        {
            if (rotor != null) rotor.localRotation = Quaternion.RotateTowards(rotor.localRotation, Quaternion.Euler(0, 0, -90 * puzzle.Turns(index)), 420 * Time.deltaTime);
        }
        void Refresh()
        {
            foreach (var channel in channels) channel.sharedMaterial = puzzle.Solved ? complete : puzzle.IsPowered(index) ? flowing : dormant;
        }
        public override void Interact(DiverController diver)
        {
            if (!diver.Session.IsZoneUnlocked(puzzle.zone)) { diver.Session.Notify(diver.Session.ZoneRequirement(puzzle.zone)); return; }
            if (!puzzle.Rotate(index)) return;
            diver.Session.Notify(puzzle.Ready ? "¡La corriente llega a todas las piedras! Acciona ABRIR SELLO." : "Corriente: " + puzzle.PoweredCount + "/" + puzzle.Definition.Count + " piedras. Une los canales sin dejar extremos abiertos.");
        }
    }
}
