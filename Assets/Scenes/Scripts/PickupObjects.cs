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
                uiText.SetActive(true); //„[T] Take”
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

        GameObject clone = Instantiate(currentTarget, holdPoint.position, Quaternion.identity);
        clone.transform.SetParent(holdPoint);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;

        Canvas c = clone.GetComponentInChildren<Canvas>();
        if (c != null) c.gameObject.SetActive(false);

        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb == null)
            rb = clone.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;

        MeshRenderer mr = clone.GetComponent<MeshRenderer>();
        Collider col = clone.GetComponent<Collider>();

        Debug.Log("Clone MeshRenderer: " + (mr != null));
        Debug.Log("Clone Collider: " + (col != null));
    }


    void Throw() {
        if (holdPoint.childCount == 0) return;

        Transform item = holdPoint.GetChild(0);
        item.SetParent(null);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }
    }
}
