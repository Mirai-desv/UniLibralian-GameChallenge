    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;

    public class GameStateController : MonoBehaviour
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
            CurrentState = GameState.Playing;
            ResetResultPanels();
        }

        private void ResetResultPanels()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            if (losePanel != null)
                losePanel.SetActive(false);
        }

        public void CheckGameState(List<Book> remainingBooks, BoxManager boxManager)
        {
            if (CurrentState != GameState.Playing)
                return;

            // Điều kiện thắng
            if (remainingBooks.Count == 0)
            {
                WinGame();
                return;
            }

            // Điều kiện thua
            if (!HasAnyValidMove(remainingBooks, boxManager))
            {
                LoseGame();
            }
        }

        private bool HasAnyValidMove(List<Book> books, BoxManager boxManager)
        {
            foreach (Book book in books)
            {
                if (book == null) continue;

                if (book.IsBlocked) continue;

                if (boxManager.TryGetTargetSpace(book, out _))
                    return true;
            }

            return false;
        }

        private void WinGame()
        {
            CurrentState = GameState.Win;

            Debug.Log("YOU WIN!");

            if (winPanel != null)
                winPanel.SetActive(true);

            if (losePanel != null)
                losePanel.SetActive(false);
        }

        private void LoseGame()
        {
            CurrentState = GameState.Lose;

            Debug.Log("GAME OVER!");

            if (losePanel != null)
                losePanel.SetActive(true);

            if (winPanel != null)
                winPanel.SetActive(false);
        }

    }