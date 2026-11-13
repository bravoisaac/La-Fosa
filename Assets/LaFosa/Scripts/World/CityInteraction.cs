using UnityEngine;

namespace LaFosa
{
    public sealed class CityInteraction : Interactable
    {
        public GameSession session;
        public bool terminal, atBase;
        public int fishingIndex;
        public override string Prompt => terminal ? (session.InfrastructureLevel < 4 ? "Enlace inactivo · construye la base en el astillero" : "E · enlace ciudad / base / descensor")
            : "Pesca automática " + (fishingIndex + 1) + " · " + session.FishingStock(fishingIndex) + "/10 peces · E recoger · 1/min activo";
        public override void Interact(DiverController diver)
        {
            if (session.Blocked || diver.Riding != null || Vector3.Distance(diver.View.transform.position, transform.position) > 4.5f) return;
            if (terminal) session.OpenTransit(atBase);
            else session.CollectAutomaticFish(fishingIndex);
        }
    }
}
