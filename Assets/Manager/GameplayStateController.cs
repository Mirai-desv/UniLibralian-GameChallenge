    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;

    public class GameplayStateController : MonoBehaviour
    {
        public enum GameState
        {
            Playing,
            Win,
            Lose
        }

        public GameState CurrentState { get; private set; } = GameState.Playing;

        [Header("Optional UI / Scene")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        private void Awake()
        {
            ResetResultPanels();
        }

        public void ResetGameState()
        {
            SetState(GameState.Playing);
        }

        public void SetState(GameState state)
        {
            CurrentState = state;

            if (winPanel != null)
                winPanel.SetActive(state == GameState.Win);

            if (losePanel != null)
                losePanel.SetActive(state == GameState.Lose);
        }

        private void ResetResultPanels()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            if (losePanel != null)
                losePanel.SetActive(false);
        }

    }