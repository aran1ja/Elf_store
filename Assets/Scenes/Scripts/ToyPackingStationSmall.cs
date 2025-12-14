using TMPro;
using UnityEditor;
using UnityEngine;

public class ToyPackingStationSmall : MonoBehaviour {
    [Header("Packing Settings")]
    public float packTime = 1f;
    public Transform packPositionSmall;

    [Header("UI")]
    public GameObject progressBarRoot;
    public RectTransform progressBarFill;
    public TMP_Text actionText;

    [Header("Player")]
    public Transform player;
    public float layDistance = 2f;

    [Header("Player Hold Settings")]
    public Transform holdPoint;
    public GameObject packagePrefab;
    public float throwForce = 5f;
    public Vector3 holdOffset = new Vector3(0f, 0f, 0.5f);

    private GameObject currentToy;
    private GameObject heldPackage;
    private GameObject tablePackage;
    private float packProgress;
    private float fullWidth;

    private enum TableState { Empty, ToyPlaced, PackageOnTable }
    private TableState tableState = TableState.Empty;


    void Start() {
        fullWidth = progressBarFill.sizeDelta.x;
        progressBarRoot.SetActive(false);
        if (actionText != null)
            actionText.text = "";
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Physics.Raycast(ray, out RaycastHit hit, 5f);
        GameObject target = hit.collider != null ? hit.collider.gameObject : null;
        float distanceToTable = Vector3.Distance(player.position, packPositionSmall.position);

        // Lay toy
        if (currentToy != null && tableState == TableState.Empty && heldPackage == null && distanceToTable <= layDistance) {

            // If toy is not placed and player is near table, show lay toy text
            actionText.text = "[L] Lay toy";
            if (Input.GetKeyDown(KeyCode.L))
                PlaceToy();

        } else if (tableState == TableState.Empty && distanceToTable <= layDistance) {
            actionText.text = "";
        }

          // Packing
          else if (tableState == TableState.ToyPlaced && target == currentToy && distanceToTable <= layDistance) {

            // If player is looking at the placed toy, show packing texr
            if (!Input.GetKey(KeyCode.P))
                actionText.text = "[P] Packing";
            else
                actionText.text = "";
        } else if (!Input.GetKey(KeyCode.P)) {
            actionText.text = "";
        }

        // Packing procesas
        if (tableState == TableState.ToyPlaced && Input.GetKey(KeyCode.P)) {
            progressBarRoot.SetActive(true);
            packProgress += Time.deltaTime;
            float percent = Mathf.Clamp01(packProgress / packTime);
            progressBarFill.sizeDelta = new Vector2(fullWidth * percent, progressBarFill.sizeDelta.y);

            if (packProgress >= packTime)
                PackToy();
        }

        if (Input.GetKeyUp(KeyCode.P))
            ResetProgress();

        // Package position and throwing
        if (heldPackage != null) {
            MoveHeldPackage();

            if (Input.GetKeyDown(KeyCode.Y))
                ThrowHeldObject(heldPackage);
        }

        if (tableState == TableState.PackageOnTable && tablePackage != null && distanceToTable <= layDistance && Input.GetKeyDown(KeyCode.T))
            TakePackageFromTable();
    }

    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Clone"))
            return;
        currentToy = other.gameObject;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == currentToy)
            currentToy = null;
    }

    void ResetProgress() {
        packProgress = 0f;
        progressBarFill.sizeDelta = new Vector2(0, progressBarFill.sizeDelta.y);
        progressBarRoot.SetActive(false);
    }

    void PlaceToy() {
        // Toy goes from player to table
        currentToy.transform.SetParent(null);
        currentToy.transform.position = packPositionSmall.position;
        currentToy.transform.rotation = packPositionSmall.rotation;

        Rigidbody rb = currentToy.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        tableState = TableState.ToyPlaced;
    }

    void PackToy() {
        if (packagePrefab != null) {
            tablePackage = Instantiate(packagePrefab, packPositionSmall.position, Quaternion.Euler(-90f, 0f, 0f));
            Rigidbody rb = tablePackage.GetComponent<Rigidbody>();
            if (rb == null) rb = tablePackage.AddComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            tableState = TableState.PackageOnTable;
        }

        Destroy(currentToy);
        currentToy = null;
        ResetProgress();
    }

    void MoveHeldPackage() {
        if (heldPackage == null) return;
        Rigidbody rb = heldPackage.GetComponent<Rigidbody>();
        if (rb != null) {
            Vector3 targetPos = holdPoint.position + holdPoint.TransformDirection(holdOffset);
            Quaternion targetRot = Quaternion.Euler(-90f, 0f, 0f);
            rb.isKinematic = true;
            rb.MovePosition(targetPos);
            rb.MoveRotation(targetRot);
        }
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

    void TakePackageFromTable() {
        if (tablePackage == null) return;

        heldPackage = tablePackage;
        tablePackage = null;

        heldPackage.transform.position = holdPoint.position + holdPoint.TransformDirection(holdOffset);
        heldPackage.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        Rigidbody rb = heldPackage.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        tableState = TableState.Empty;
        if (actionText != null)
            actionText.text = "[Y] Throw";
    }

}
