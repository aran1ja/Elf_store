using UnityEngine;

public class PickupObject : MonoBehaviour {
    public Transform holdPoint;
    public float throwForce = 5f;

    private GameObject currentTarget;
    private GameObject uiText;

    void Update() {
        // Pickup  T
        if (currentTarget != null && Input.GetKeyDown(KeyCode.T)) {
            Pickup();
        }

        // Throw  Y
        if (holdPoint.childCount > 0 && Input.GetKeyDown(KeyCode.Y)) {
            Throw();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Pickup")) {
            currentTarget = other.gameObject;

            uiText = currentTarget.GetComponentInChildren<Canvas>()?.gameObject;
            if (uiText != null)
                uiText.SetActive(true); //[T] Take
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Pickup")) {
            if (uiText != null)
                uiText.SetActive(false);

            currentTarget = null;
            uiText = null;
        }
    }

    void Pickup() {
        if (currentTarget == null) return;
        if (holdPoint.childCount > 0) return; // one toy
        if (currentTarget.CompareTag("Pickup")) {
            GameObject clone = Instantiate(currentTarget, holdPoint.position, Quaternion.identity);
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

            Canvas c = clone.GetComponentInChildren<Canvas>();
            if (c != null) c.gameObject.SetActive(false);

            clone.tag = "Clone";
        } else {
            // If it is a copy, we can't make a new copy from it
            currentTarget.transform.SetParent(holdPoint);
            currentTarget.transform.localPosition = Vector3.zero;
            currentTarget.transform.localRotation = Quaternion.identity;

            Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = currentTarget.GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;

            Canvas c = currentTarget.GetComponentInChildren<Canvas>();
            if (c != null) c.gameObject.SetActive(false);
        }
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
    }

}