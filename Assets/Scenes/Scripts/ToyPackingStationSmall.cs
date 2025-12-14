using UnityEngine;
using TMPro;

public class ToyPackingStationSmall : MonoBehaviour {
    public float packTime = 1f;
    public Transform packPosition;

    public GameObject progressBarRoot;
    public RectTransform progressBarFill;
    public TMP_Text actionText;

    public Transform player;
    public float interactDistance = 2f;

    public Transform holdPoint;
    public GameObject packagePrefab;
    public float throwForce = 5f;

    GameObject currentToy;
    GameObject tablePackage;
    GameObject heldPackage;

    float packProgress;
    float fullWidth;

    enum State { Empty, ToyPlaced, PackageReady }
    State state = State.Empty;

    void Start() {
        fullWidth = progressBarFill.sizeDelta.x;
        progressBarRoot.SetActive(false);
        actionText.text = "";
    }

    void Update() {
        float dist = Vector3.Distance(player.position, packPosition.position);

        if (state == State.Empty && currentToy != null && dist <= interactDistance) {
            actionText.text = "[L] Lay toy";
            if (Input.GetKeyDown(KeyCode.L))
                PlaceToy();
            return;
        }

        if (state == State.ToyPlaced && dist <= interactDistance) {
            actionText.text = "[P] Pack";
            if (Input.GetKey(KeyCode.P))
                PackProgress();
            if (Input.GetKeyUp(KeyCode.P))
                ResetProgress();
            return;
        }

        if (state == State.PackageReady && tablePackage != null && dist <= interactDistance) {
            actionText.text = "[T] Take package";
            if (Input.GetKeyDown(KeyCode.T))
                TakePackage();
            return;
        }

        actionText.text = "";

        if (heldPackage != null) {
            HoldPackage();
            if (Input.GetKeyDown(KeyCode.Y))
                ThrowPackage();
        }
    }

    void PackProgress() {
        progressBarRoot.SetActive(true);
        packProgress += Time.deltaTime;

        float p = Mathf.Clamp01(packProgress / packTime);
        progressBarFill.sizeDelta = new Vector2(fullWidth * p, progressBarFill.sizeDelta.y);

        if (packProgress >= packTime)
            FinishPacking();
    }

    void ResetProgress() {
        packProgress = 0;
        progressBarFill.sizeDelta = new Vector2(0, progressBarFill.sizeDelta.y);
        progressBarRoot.SetActive(false);
    }

    void PlaceToy() {
        currentToy.transform.SetParent(null);
        currentToy.transform.position = packPosition.position;
        currentToy.transform.rotation = packPosition.rotation;

        Rigidbody rb = currentToy.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        state = State.ToyPlaced;
    }

    void FinishPacking() {
        Destroy(currentToy);
        currentToy = null;

        tablePackage = Instantiate(
            packagePrefab,
            packPosition.position,
            Quaternion.Euler(-90f, 0f, 0f)
        );

        Rigidbody rb = tablePackage.GetComponent<Rigidbody>();
        if (rb == null) rb = tablePackage.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = tablePackage.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        ResetProgress();
        state = State.PackageReady;
    }

    void TakePackage() {
        heldPackage = tablePackage;
        tablePackage = null;

        Rigidbody rb = heldPackage.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = heldPackage.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        state = State.Empty;
    }

    void HoldPackage() {
        heldPackage.transform.position = holdPoint.position;
        heldPackage.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    void ThrowPackage() {
        Rigidbody rb = heldPackage.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        Collider col = heldPackage.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        heldPackage = null;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Clone"))
            currentToy = other.gameObject;
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject == currentToy)
            currentToy = null;
    }
}
