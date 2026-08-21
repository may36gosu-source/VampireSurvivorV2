using UnityEngine;
using VampireSurvivors.Manager;

namespace VampireSurvivors.Helper {
    public static class GameStateHelper
    {
        public static bool IsPlaying()
        {
            return GameManager.Instance != null &&
                   GameManager.Instance.State == GameState.Playing;
        }

        public static bool IsReady()
        {
            return GameManager.Instance != null &&
                   GameManager.Instance.State == GameState.Ready;
        }

        public static void SetActiveWhenPlaying(GameObject obj)
        {
            if (obj == null)
                return;

            obj.SetActive(IsPlaying());
        }
    }
}