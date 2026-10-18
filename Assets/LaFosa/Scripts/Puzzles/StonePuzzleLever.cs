using UnityEngine;

namespace LaFosa
{
    public sealed class StonePuzzleLever : Interactable
    {
        public StoneCircuitPuzzle puzzle;
        public bool reset;
        public override string Prompt => puzzle.Solved ? "Sello abierto · recoge el tesoro" : reset ? "Reiniciar estas piedras" : "Accionar palanca · abrir sello";
        public override void Interact(DiverController diver)
        {
            if (!diver.Session.IsZoneUnlocked(puzzle.zone)) { diver.Session.Notify("Recupera primero el tesoro del nivel anterior."); return; }
            if (puzzle.Solved) { diver.Session.Notify("El sello ya está abierto. Recoge el tesoro."); return; }
            if (reset) { puzzle.ResetBoard(); diver.Session.Notify("Piedras devueltas a su posición inicial."); return; }
            if (puzzle.Activate()) diver.Session.Notify("¡Sello abierto! La corriente ha liberado el tesoro.");
            else diver.Session.Notify("El mecanismo sigue cerrado: corriente en " + puzzle.PoweredCount + "/" + puzzle.Definition.Count + " piedras y " + puzzle.OpenConnections + " extremos sin unir. " + puzzle.Hint);
        }
    }
}
