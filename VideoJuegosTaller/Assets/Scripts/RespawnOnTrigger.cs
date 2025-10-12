using UnityEngine;

public class RespawnOnTrigger : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Si on touche un plane mortel
        if (other.CompareTag("RespawnPlane"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;          // stop tout mouvement
            rb.angularVelocity = Vector3.zero;   // stop rotation
        }

        transform.position = startPosition;      // remet à la position de départ
        transform.rotation = startRotation;      // remet la rotation de départ
    }
}
