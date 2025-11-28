using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnOnTrigger : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    private BouleController[] boules;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();

        boules = FindObjectsOfType<BouleController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Si on touche un plane mortel
        if (other.CompareTag("RespawnPlane"))
        {
            Respawn();
        }

        if (other.CompareTag("Win"))
        {
            SceneManager.LoadScene("win");
        }

        if (other.CompareTag("triggerZoneBoule"))
        {
            foreach (var b in boules)
                b.ActivateGravity();
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

        if (boules != null)
        {
            foreach (var b in boules)
                b.ResetBoule();
        }
    }
}
