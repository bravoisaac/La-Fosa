using UnityEngine;

namespace LaFosa
{
    public sealed class StonePuzzleView : MonoBehaviour
    {
        public StoneCircuitPuzzle puzzle;
        public TextMesh status;
        public Transform seal, lever;
        public Renderer sealFace;
        public Material sleeping, ready, opened;
        void Start() { puzzle.Changed += Refresh; Refresh(); }
        void OnDestroy() { if (puzzle != null) puzzle.Changed -= Refresh; }
        void Update()
        {
            if (seal != null) seal.localRotation = Quaternion.RotateTowards(seal.localRotation, Quaternion.Euler(0, 0, puzzle.Solved ? 90 : 0), 90 * Time.deltaTime);
            if (lever != null) lever.localRotation = Quaternion.RotateTowards(lever.localRotation, Quaternion.Euler(puzzle.Solved ? -45 : 25, 0, 0), 90 * Time.deltaTime);
        }
        void Refresh()
        {
            status.text = puzzle.Solved ? "SELLO ABIERTO  /  RECOGE EL TESORO" : puzzle.Ready ? "CORRIENTE COMPLETA  /  ACCIONA LA PALANCA" : "CORRIENTE  " + puzzle.PoweredCount + " / " + puzzle.Definition.Count + "   ·   E PARA GIRAR";
            status.color = puzzle.Solved ? new Color(1, .8f, .4f) : puzzle.Ready ? new Color(.5f, 1, .75f) : new Color(.8f, .89f, .9f);
            sealFace.sharedMaterial = puzzle.Solved ? opened : puzzle.Ready ? ready : sleeping;
        }
    }
}
