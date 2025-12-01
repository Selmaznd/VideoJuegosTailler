using UnityEngine;

public class MoveAndRotate : MonoBehaviour
{
    public float zMin = -1.1f;     // Position Z minimale
    public float zMax = 1.7f;      // Position Z maximale
    public float moveSpeed = 2f;   // Vitesse du déplacement
    public float rotationSpeed = 90f; // Degrés par seconde

    private bool goingForward = true;

    void Update()
    {
        // --- ROTATION AUTOUR DE X ---
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);

        // --- DEPLACEMENT SUR Z ---
        float z = transform.localPosition.z;

        if (goingForward)
        {
            z += moveSpeed * Time.deltaTime;
            if (z >= zMax)
            {
                goingForward = false;
                rotationSpeed *= -1;
            }
                
        }
        else
        {
            z -= moveSpeed * Time.deltaTime;
            if (z <= zMin)
            {
                goingForward = true;
                rotationSpeed *= -1;
            }
                
        }

        Vector3 pos = transform.localPosition;
        pos.z = z;
        transform.localPosition = pos;
    }
}
