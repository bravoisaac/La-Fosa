using UnityEngine;

namespace LaFosa
{
    // The route is derived from the persisted puzzle/treasure state, never saved twice.
    public sealed class DiveGate : Interactable
    {
        public const float OpeningWidth = 8;
        public GameSession Session { get; private set; }
        public DiveZoneDefinition Destination { get; private set; }
        public StoneCircuitPuzzle Puzzle { get; private set; }
        public Transform panel;
        public TextMesh sign;
        public TextMesh returnSign;
        public Renderer signal;
        public Material lockedMaterial, openMaterial;
        public bool IsOpen { get; private set; }
        float nextNotice;
        public override string Prompt => IsOpen ? "Paso abierto · " + Destination.displayName : MissingRequirement;
        public string MissingRequirement => !Puzzle.Solved ? "Resuelve las piedras y acciona ABRIR SELLO en " + Puzzle.zone.displayName + "." : Session.ZoneRequirement(Destination);

        public void Initialize(GameSession session, DiveZoneDefinition destination, StoneCircuitPuzzle puzzle)
        {
            Session = session; Destination = destination; Puzzle = puzzle;
            Session.ProgressChanged += Refresh; Puzzle.Changed += Refresh; Refresh();
        }
        public void Refresh()
        {
            IsOpen = Puzzle.Solved && Session.IsZoneUnlocked(Destination);
            // Restoring a save applies collision and presentation together, even while paused.
            panel.localPosition = IsOpen ? Vector3.right * (OpeningWidth + .3f) : Vector3.zero;
            signal.sharedMaterial = IsOpen ? openMaterial : lockedMaterial;
            sign.text = Destination.displayName + "\n" + (IsOpen ? "PASO ABIERTO · BAJA POR EL CENTRO" : "PASO CERRADO · E PARA VER REQUISITOS") +
                "\nEQUIPO: " + Destination.requiredSafeDepth + " m / " + Destination.requiredOxygen + " s";
            sign.color = IsOpen ? new Color(.45f, 1, .8f) : new Color(1, .78f, .45f);
            UpdateSigns();
        }
        void LateUpdate() { UpdateSigns(); }
        void UpdateSigns()
        {
            if (Session == null || Session.Diver == null) return;
            bool above = Session.Diver.View.transform.position.y >= transform.position.y;
            sign.gameObject.SetActive(above);
            returnSign.gameObject.SetActive(!above);
        }
        public override void Interact(DiverController diver)
        {
            Refresh(); diver.Session.Notify(IsOpen ? Destination.returnHint : MissingRequirement);
        }
        public void NotifyBlocked(DiverController diver)
        {
            if (IsOpen || Time.unscaledTime < nextNotice) return;
            nextNotice = Time.unscaledTime + 4; Interact(diver);
        }
        void OnDestroy()
        {
            if (Session != null) Session.ProgressChanged -= Refresh;
            if (Puzzle != null) Puzzle.Changed -= Refresh;
        }
    }
}
