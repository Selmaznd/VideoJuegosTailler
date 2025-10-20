using UnityEngine;

public class ZoneRotationTrigger : MonoBehaviour
{
    public Transform objetARotater; // l'objet à faire tourner
    public float angleDepart = -86f;
    public float angleCible = 86f;
    public float vitesseRotation = 60f; // degrés par seconde

    private bool enRotation = false;
    private float angleActuel;

    private void Start()
    {
        if (objetARotater != null)
        {
            // On initialise la rotation de départ
            angleActuel = angleDepart;
            objetARotater.localRotation = Quaternion.Euler(0f, 0f, angleActuel);
        }
    }

    private void Update()
    {
        if (enRotation && objetARotater != null)
        {
            // On fait une interpolation progressive de l'angle
            angleActuel = Mathf.MoveTowards(angleActuel, angleCible, vitesseRotation * Time.deltaTime);
            objetARotater.localRotation = Quaternion.Euler(0f, 0f, angleActuel);

            // Stop quand on atteint la cible
            if (Mathf.Approximately(angleActuel, angleCible))
            {
                enRotation = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si le joueur entre dans la zone
        if (other.CompareTag("Player"))
        {
            enRotation = true;
        }
    }
}
