using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    public float rayDistance = 0.6f;
    public float knockbackDuration = 0.5f; // temps pendant lequel le joueur est propulsé

    private Rigidbody rb;
    private bool isGrounded;
    private float knockbackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return; 
        }

        // Avancer automatiquement
        Vector3 forwardMove = transform.forward * moveSpeed;
        rb.velocity = new Vector3(forwardMove.x, rb.velocity.y, forwardMove.z);

        // Rotation avec flèches
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotationInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotationInput = 1f;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotationInput * rotationSpeed * Time.fixedDeltaTime, 0f));
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance);

        // Saut
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && knockbackTimer <= 0f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            Debug.Log($"[AutoMove] Collision avec {collision.gameObject.name}");

            Transform hammer = collision.transform;
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 pivot = hammer.position;
            Vector3 radius = contactPoint - pivot;

            // Axe de rotation
            Vector3 angularVelocity = hammer.TransformDirection(Vector3.forward) * 15f;
            Vector3 hammerVelocity = Vector3.Cross(angularVelocity, radius);

            Debug.Log($"[AutoMove] Vitesse estimée du marteau : {hammerVelocity}");

            Vector3 launchDir = hammerVelocity.normalized;
            float impactStrength = hammerVelocity.magnitude * 1.5f;
            rb.AddForce(launchDir * impactStrength, ForceMode.Impulse);

            Debug.Log($"[AutoMove] Force appliquée : {launchDir * impactStrength}");

            // Bloque le mouvement automatique un court instant
            knockbackTimer = knockbackDuration;
        }
    }
}
