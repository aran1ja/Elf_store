using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour {
    [Header("Timer Settings")]
    public float startTime = 120f;
    private float timeNow;

    [Header("UI")]
    public TMP_Text timerText;

    private bool timerRunning = false;

    void Start() {
        timeNow = startTime;
        UpdateTimerUI();
    }

    void Update() {
        if (!timerRunning) return;

        timeNow -= Time.deltaTime;
        if (timeNow <= 0f) {
            timeNow = 0f;
            timerRunning = false;
            TimerFinished();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI() {
        if (timerText == null) return;

        int minuty = Mathf.FloorToInt(timeNow / 60f);
        int sekundy = Mathf.FloorToInt(timeNow % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minuty, sekundy);
    }

    void TimerFinished() {
        Debug.Log("Time is over!");
        GameOverController goController = FindObjectOfType<GameOverController>();
        if (goController != null)
            goController.ShowGameOver();
    }

    public void StartTimer() {
        timerRunning = true;
    }

    public void ResetTimer() {
        timeNow = startTime;
        timerRunning = false;
        UpdateTimerUI();
    }
}
