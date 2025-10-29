using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BalancierRotation : MonoBehaviour
{
    [Header("Oscillation")]
    public float angleMax = 86f;
    public float angleMin = -86f;
    public float vitesse = 1f; // fr�quence de l'oscillation

    [Header("Force de frappe")]
    public float forceMultiplier = 0.05f; // ajuste la puissance globale
    public float forceMinimum = 1f;       // force minimale � appliquer
    public float forceMaximum = 50f;      // clamp pour �viter forces absurdes

    // champs internes
    private float currentAngle;
    private float previousAngle;
    private float lastAngularSpeed; // en degr�s / seconde

    private void Start()
    {
        // initialise l'angle de d�part
        currentAngle = angleMin;
        previousAngle = currentAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

        // s'assurer que le collider n'est pas en trigger (collision physique)
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }

    private void Update()
    {
        // calcule l'angle oscillant
        float t = Mathf.Sin(Time.time * vitesse);
        currentAngle = Mathf.Lerp(angleMin, angleMax, (t + 1f) / 2f);

        // calcule la "vitesse angulaire" en degr�s / seconde
        // (delta angle / delta time)
        lastAngularSpeed = (currentAngle - previousAngle) / Time.deltaTime;
        previousAngle = currentAngle;

        // applique la rotation
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    private void OnCollisionEnter(Collision collision)
    {

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;



        // coordonn�es du contact pour appliquer la force au bon endroit
        ContactPoint contact = collision.contacts.Length > 0 ? collision.contacts[0] : default;

        // direction de la force : vers l'ext�rieur � partir du point de contact
        // contact.normal pointe vers l'ext�rieur du collider que l'on touche,
        // on veut pousser le joueur dans le sens oppos� = -contact.normal
        Vector3 forceDir = -contact.normal.normalized;

        // calcule la magnitude de la force depuis la vitesse angulaire
        float magnitude = Mathf.Abs(lastAngularSpeed) * forceMultiplier;

        // garantir une force minimale et clamp pour la s�curit�
        magnitude = Mathf.Max(magnitude, forceMinimum);
        magnitude = Mathf.Clamp(magnitude, 0f, forceMaximum);

        Vector3 force = forceDir * magnitude;

        // applique l'impulsion au joueur au point de contact pour plus de r�alisme
        rb.AddForceAtPosition(force, contact.point, ForceMode.Impulse);

        // optionnel : debug
        // Debug.Log($"Impact: angSpeed={lastAngularSpeed:F1}�/s, force={magnitude:F2}");
    }
}
