using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoRunnerController : MonoBehaviour
{
    public float forwardSpeed = 5f;    // vitesse d’avancement automatique
    public float turnSpeed = 100f;     // vitesse de rotation (gauche/droite)
    public float gravity = -9.81f;     // gravité
    public float jumpHeight = 2f;      // hauteur du saut

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Vérifie si le joueur est au sol
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Déplacement automatique vers l’avant ---
        Vector3 move = transform.forward * forwardSpeed * Time.deltaTime;

        // --- Rotation gauche / droite ---
        float turn = Input.GetAxis("Horizontal"); // flèches gauche/droite
        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);

        // --- Saut ---
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // --- Gravité ---
        velocity.y += gravity * Time.deltaTime;

        // --- Application des mouvements ---
        controller.Move(move + velocity * Time.deltaTime);

    }
}

