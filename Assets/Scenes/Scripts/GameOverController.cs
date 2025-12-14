using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour {
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    private void Start() {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowGameOver() {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        // Odblokowanie kursora
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMenu() {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (menuPanel != null)
            menuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SeeLeaderboard() {
        Debug.Log("Leaderboard button clicked");
    }
}
