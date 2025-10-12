using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float moveSpeed = 5f;       // vitesse d'avance
    public float rotationSpeed = 100f; // vitesse de rotation
    public float jumpForce = 5f;       // force du saut

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // empêche la capsule de tomber en roulant
    }

    void FixedUpdate()
    {
        // Avancer automatiquement (dans l'axe local "forward")
        Vector3 forwardMove = transform.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove);

        // Rotation avec flèches gauche/droite
        float rotationInput = Input.GetAxis("Horizontal"); // -1 gauche, 1 droite
        Quaternion rotation = Quaternion.Euler(0f, rotationInput * rotationSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * rotation);
    }

    void Update()
    {
        // Saut avec flèche du haut
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
