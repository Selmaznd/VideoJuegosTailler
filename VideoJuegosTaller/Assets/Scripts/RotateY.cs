using UnityEngine;

public class RotateY : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // degrés par seconde
    public bool clockwise = true;     // sens de rotation

    void Update()
    {
        float direction = clockwise ? 1f : -1f;
        transform.Rotate(0f, rotationSpeed * direction * Time.deltaTime, 0f, Space.Self);
    }
}
