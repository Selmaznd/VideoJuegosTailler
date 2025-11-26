using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trampoline : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform targetObject;  // laisser vide = utiliser l’objet actuel
    public float targetY = 5.4f;
    public float targetScaleY = 1.0f;
    public float speed = 3f;   // vitesse d’interpolation

    private bool triggered = false;
    void Start()
    {
        if (targetObject == null)
            targetObject = transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            // --- Position ---
            Vector3 targetPos = targetObject.position;
            targetPos.y = targetY;

            targetObject.position = Vector3.Lerp(
                targetObject.position,
                targetPos,
                Time.deltaTime * speed
            );

            // --- Taille (scale) ---
            Vector3 targetScale = targetObject.localScale;
            targetScale.y = targetScaleY;

            targetObject.localScale = Vector3.Lerp(
                targetObject.localScale,
                targetScale,
                Time.deltaTime * speed
            );
        }
    }

    public void Trigger()
    {
        triggered = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = true;
        }
    }
}
