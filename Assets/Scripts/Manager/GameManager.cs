using UnityEngine;
using VampireSurvivors.Core;
namespace VampireSurvivors.Manager {
    public enum GameState
    {
        Ready,
        Playing,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; }

        private void Awake()
        {
            Instance = this;

            State = GameState.Ready;
        }

        public void StartGame()
        {
            if (State != GameState.Ready)
            return;

            State = GameState.Playing;

            GameEvents.GameStarted();
        }

        public void GameOver()
        {
            if (State != GameState.Playing)
                return;

            State = GameState.GameOver;

            GameEvents.GameOver();
        }
    }
}