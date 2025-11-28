using UnityEngine;

public class BouleController : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (rb != null)
            rb.useGravity = false; // gravité désactivée au début
    }

    public void ActivateGravity()
    {
        if (rb != null)
            rb.useGravity = true;
    }

    public void ResetBoule()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
