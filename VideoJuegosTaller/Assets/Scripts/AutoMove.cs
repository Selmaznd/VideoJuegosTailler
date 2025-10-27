using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    public float rayDistance = 0.6f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // empêche la capsule de basculer
    }

    void FixedUpdate()
    {
        // Avancer
        Vector3 forwardMove = transform.forward * moveSpeed;
        rb.velocity = new Vector3(forwardMove.x, rb.velocity.y, forwardMove.z);

        // Rotation
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotationInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotationInput = 1f;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotationInput * rotationSpeed * Time.fixedDeltaTime, 0f));

    }

    void Update()
    {
        
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance);
        // Saut avec flèche du haut
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    //void OnCollisionStay(Collision collision)
    //{
    //    isGrounded = true;
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    isGrounded = false;
    //}
}
