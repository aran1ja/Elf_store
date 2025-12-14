using UnityEngine;

public class TaskListController : MonoBehaviour {
    public GameObject contentPanel;
    public KeyCode toggleKey = KeyCode.Q;

    private bool isExpanded = false;

    void Start() {
        if (contentPanel != null)
            contentPanel.SetActive(isExpanded);
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            isExpanded = !isExpanded;
            if (contentPanel != null)
                contentPanel.SetActive(isExpanded);
        }
    }
}
