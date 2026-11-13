using UnityEngine;

namespace LaFosa
{
    public sealed class InfrastructureView : MonoBehaviour
    {
        GameSession session;
        GameObject[] stages;
        public void Initialize(GameSession game, GameObject[] objects)
        { session = game; stages = objects; session.ProgressChanged += Refresh; Refresh(); }
        void Refresh()
        {
            if (this == null || stages == null) return;
            for (int i = 0; i < stages.Length; i++) if (stages[i] != null) stages[i].SetActive(session.InfrastructureLevel >= i + 1);
        }
        public void Release() { if (session != null) session.ProgressChanged -= Refresh; }
        void OnDestroy() { Release(); }
    }
}
