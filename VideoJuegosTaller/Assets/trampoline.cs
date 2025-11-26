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

    public float targetYDown = -0.43f;
    public float targetScaleYDown = 0.33f;
    public float speedDown = 3f;   // vitesse d’interpolation

    private bool triggered = false;
    private bool getDown = false;

    public float delayBeforeDown = 0.2f;
    private bool delayStarted = false;
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
            if (transform.position.y >= (targetY -0.01))
            {
                
                triggered = false;
                if (!delayStarted)
                {
                    delayStarted = true;
                    StartCoroutine(StartGoingDown());
                }
                
            }

        }
        if (getDown)
        {

            // --- Position ---
            Vector3 targetPos = targetObject.position;
            targetPos.y = targetYDown;

            if(transform.localPosition.y >= (targetYDown + 0.0001))
            {
                targetObject.position = Vector3.Lerp(
                    targetObject.position,
                    targetPos,
                    Time.deltaTime * speedDown
                );
            }
            

            // --- Taille (scale) ---
            Vector3 targetScale = targetObject.localScale;
            targetScale.y = targetScaleYDown;

            targetObject.localScale = Vector3.Lerp(
                targetObject.localScale,
                targetScale,
                Time.deltaTime * speedDown
            );

            if (transform.localPosition.y <= (targetYDown + 0.0001) && transform.localScale.y <= (targetScaleYDown + 0.01))
            {
                getDown = false;
            }
        }

    }

    IEnumerator StartGoingDown()
    {
        yield return new WaitForSeconds(delayBeforeDown);
        getDown = true;
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
