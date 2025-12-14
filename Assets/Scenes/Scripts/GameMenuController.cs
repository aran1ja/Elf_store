using UnityEngine;

public class GameMenuController : MonoBehaviour {
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gamePanel;

    [Header("Timer")]
    public GameTimer gameTimer;

    private void Start() {

        ShowMenu();
    }

    public void StartButtonClicked() {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);

        // Start new game
        if (gameTimer != null) {
            gameTimer.ResetTimer();
            gameTimer.StartTimer();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowMenu() {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
