using TMPro;
using UnityEditor;
using UnityEngine;

public class ToyPackingStation : MonoBehaviour {
    [Header("Packing Settings")]
    public float packTime = 3f;
    public Transform packPositionBig;

    [Header("UI")]
    public GameObject progressBarRoot;
    public RectTransform progressBarFill;
    public TMP_Text actionText;

    [Header("Player")]
    public Transform player;
    public float layDistance = 1f;

    [Header("Player Hold Settings")]
    public Transform holdPoint;
    public GameObject packagePrefab;
    public float throwForce = 5f;
    public Vector3 holdOffset = new Vector3(0f, 0f, 0.5f);

    private GameObject currentToy;
    private GameObject heldPackage;
    private bool isPlaced;
    private float packProgress;
    private float fullWidth;

    void Start() {
        fullWidth = progressBarFill.sizeDelta.x;
        progressBarRoot.SetActive(false);
        if (actionText != null)
            actionText.text = "";
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 5f)) {
            GameObject target = hit.collider.gameObject;
            float distanceToTable = Vector3.Distance(player.position, packPositionBig.position);

            // If player is looking at the placed toy, show packing texr
            if (target == currentToy && isPlaced) {
                if (!Input.GetKey(KeyCode.P)) {
                    actionText.text = "[P] Packing";
                } else {
                    actionText.text = "";
                }
            }

            // If toy is not placed and player is near table, show lay toy text
            else if (!isPlaced && distanceToTable <= layDistance) {
                actionText.text = "[L] Lay toy";
            } else {
                actionText.text = "";
            }

        } else {
            if (!Input.GetKey(KeyCode.P))
                actionText.text = "";
        }

        // Packing procesas
        if (isPlaced && Input.GetKey(KeyCode.P)) {
            progressBarRoot.SetActive(true);
            packProgress += Time.deltaTime;
            float percent = Mathf.Clamp01(packProgress / packTime);
            progressBarFill.sizeDelta = new Vector2(fullWidth * percent, progressBarFill.sizeDelta.y);

            if (packProgress >= packTime) {
                PackToy(); // Package in place of toy
            }
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            ResetProgress();
        }

        // Package position and throwing
        if (heldPackage != null)
        {
            Rigidbody rb = heldPackage.GetComponent<Rigidbody>();
            if (rb != null) {
                Vector3 targetPos = holdPoint.position + holdPoint.TransformDirection(holdOffset);
                Quaternion targetRot = Quaternion.Euler(-90f, 0f, 0f);
                rb.isKinematic = true;
                rb.MovePosition(targetPos);
                rb.MoveRotation(targetRot);
            }

            if (Input.GetKeyDown(KeyCode.Y)) {
                ThrowHeldObject(heldPackage);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Clone")) return;
        currentToy = other.gameObject;

        // Allows laying the toy if player is not holding a package
        if (!isPlaced && heldPackage == null) {
            float distanceToPlayer = Vector3.Distance(player.position, packPositionBig.position);

            if (distanceToPlayer <= layDistance) {
                actionText.text = "[L] Lay toy";

                if (Input.GetKeyDown(KeyCode.L)) {
                    PlaceToy();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == currentToy) {
            ResetProgress();
            currentToy = null;
            isPlaced = false;

            if (actionText != null)
                actionText.text = "";
        }
    }

    void ResetProgress() {
        packProgress = 0f;
        progressBarFill.sizeDelta = new Vector2(0, progressBarFill.sizeDelta.y);
        progressBarRoot.SetActive(false);
    }

    void PlaceToy() {
        // Toy goes from player to table
        currentToy.transform.SetParent(null);
        currentToy.transform.position = packPositionBig.position;
        currentToy.transform.rotation = packPositionBig.rotation;

        Rigidbody rb = currentToy.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        isPlaced = true;
    }

    void PackToy() {
        if (packagePrefab != null) {
            GameObject package = Instantiate(packagePrefab, packPositionBig.position, Quaternion.Euler(-90f, 0f, 0f));

            Rigidbody rb = package.GetComponent<Rigidbody>();
            if (rb == null) rb = package.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            package.tag = "Package";
        }

        Destroy(currentToy);
        ResetProgress();
        currentToy = null;
        isPlaced = false;

        if (actionText != null)
            actionText.text = "";
    }

    void ThrowHeldObject(GameObject held) {
        held.transform.SetParent(null);
        Rigidbody rb = held.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (Camera.main != null)
                rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }

        heldPackage = null;

        if (actionText != null)
            actionText.text = "";
    }
}
