using UnityEngine;
using TMPro; // TextMeshPro

public class PickupObjects : MonoBehaviour {
    [Header("Pickup settings")]
    public float pickupDistance = 5f;
    public float sphereRadius = 0.4f;
    public Transform holdPoint; // Miejsce gdzie trzyma rzeczy
    public float throwForce = 5f;

    [Header("UI")]
    public TMP_Text actionText;

    private GameObject currentTarget;
    public GameObject heldObject;

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, pickupDistance)) {
            if (hit.collider.CompareTag("Pickup") || hit.collider.CompareTag("Package")) {
                currentTarget = hit.collider.gameObject;

                if (holdPoint.childCount == 0)
                    actionText.text = "[T] Take";
                else
                    actionText.text = "[Y] Throw";
            } else {
                ClearCurrentTarget();
            }
        } else {
            ClearCurrentTarget();
        }

        // Pickup
        if (currentTarget != null && Input.GetKeyDown(KeyCode.T)) {
            if (currentTarget.CompareTag("Pickup"))
                PickupClone();
            else if (currentTarget.CompareTag("Package"))
                PickupPackageOriginal();
        }

        // Throw
        if (holdPoint.childCount > 0 && Input.GetKeyDown(KeyCode.Y))
            Throw();
    }

    private void ClearCurrentTarget() {
        currentTarget = null;

        if (holdPoint.childCount == 0 && actionText != null)
            actionText.text = "";
    }

    void PickupClone() {
        if (currentTarget == null) return;
        if (holdPoint.childCount > 0) return;

        GameObject clone = Instantiate(currentTarget, holdPoint.position, holdPoint.rotation);
        clone.transform.SetParent(holdPoint);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;

        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb == null)
            rb = clone.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = clone.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        clone.tag = "Clone";

        currentTarget = null;
        actionText.text = "[Y] Throw";
    }

    void PickupPackageOriginal() {
        if (holdPoint.childCount > 0) return;

        currentTarget.transform.SetParent(holdPoint);
        currentTarget.transform.localPosition = Vector3.zero;
        currentTarget.transform.localRotation = Quaternion.identity;

        Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
        if (rb == null) rb = currentTarget.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = currentTarget.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        currentTarget = null;
        actionText.text = "[Y] Throw";
    }

    void Throw() {
        if (holdPoint.childCount == 0) return;

        Transform item = holdPoint.GetChild(0);
        item.SetParent(null);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Collider col = item.GetComponent<Collider>();
            if (col != null)
                col.isTrigger = false;

            if (Camera.main != null)
                rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }

        actionText.text = "";
    }

    public void ForceDrop() {
        if (holdPoint.childCount == 0) return;

        Transform item = holdPoint.GetChild(0);
        item.SetParent(null);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        actionText.text = "";
    }
}
