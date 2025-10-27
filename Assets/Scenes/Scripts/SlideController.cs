using UnityEngine;

public class SlideController : MonoBehaviour {
    public float slideForce = 10f;
    public float maxSlideSpeed = 12f;
    public bool isSliding = false;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Slide")) {
            isSliding = true;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.collider.CompareTag("Slide")) {
            isSliding = false;
        }
    }

    void FixedUpdate() {
        if (isSliding) {
            Vector3 slideDirection = Vector3.ProjectOnPlane(rb.velocity, Vector3.up).normalized;
            rb.AddForce(slideDirection * slideForce, ForceMode.Acceleration);

            if (rb.velocity.magnitude > maxSlideSpeed) {
                rb.velocity = rb.velocity.normalized * maxSlideSpeed;
            }
        }
    }
}
