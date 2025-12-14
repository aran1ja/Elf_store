using UnityEngine;
using TMPro;

public class ConveyorBeltController : MonoBehaviour {
    [Header("Conveyor Settings")]
    public Transform packagePoint;
    public Vector3 moveDirection = Vector3.forward;
    public float beltSpeed = 2f;
    public float playerDistance = 1.5f;

    [Header("References")]
    public Transform player;
    public Transform playerHoldPoint;
    public TMP_Text actionText;

    public PickupObjects pickupObjects;
    private GameObject packageOnBelt;
    private bool packagePlaced = false;
    private bool isSending = false;

    void Update() {
        float dist = Vector3.Distance(player.position, packagePoint.position);

        if (!packagePlaced && HasPackageInHand() && dist <= playerDistance) {
            actionText.text = "[L] Lay package";
            if (Input.GetKeyDown(KeyCode.L))
                LayPackage();
        } else if (packagePlaced && !isSending && dist <= playerDistance) {
            actionText.text = "[Z] Send package";
            if (Input.GetKeyDown(KeyCode.Z))
                SendPackage();
        } else if (!isSending) {
            actionText.text = "";
        }

        if (isSending && packageOnBelt != null) {
            Rigidbody rb = packageOnBelt.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
                rb.velocity = moveDirection.normalized * beltSpeed;
        }
    }

    bool HasPackageInHand() {
        return playerHoldPoint.childCount > 0 && playerHoldPoint.GetChild(0).CompareTag("Package");
    }

    void LayPackage() {
        if (!HasPackageInHand()) return;

        packageOnBelt = playerHoldPoint.GetChild(0).gameObject;

        if (pickupObjects != null)
            pickupObjects.ForceDrop();

        packageOnBelt.transform.SetParent(null);
        packageOnBelt.transform.position = packagePoint.position;
        packageOnBelt.transform.rotation = packagePoint.rotation;

        Rigidbody rb = packageOnBelt.GetComponent<Rigidbody>();
        if (rb == null)
            rb = packageOnBelt.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;

        packagePlaced = true;
    }

    void SendPackage() {
        if (packageOnBelt == null) return;

        Rigidbody rb = packageOnBelt.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.velocity = moveDirection.normalized * beltSpeed;
        }

        isSending = true;
        actionText.text = "";
    }
}
