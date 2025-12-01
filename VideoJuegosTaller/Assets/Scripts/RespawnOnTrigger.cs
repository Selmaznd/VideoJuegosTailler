using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RespawnOnTrigger : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    public TextMeshProUGUI finalTime;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();

        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
        {
            timer.StartTimer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si on touche un plane mortel
        if (other.CompareTag("RespawnPlane"))
        {
            Respawn();
        }

        // Si on touche la zone de victoire
        if (other.CompareTag("Win"))
        {
            GameTimer timer = FindObjectOfType<GameTimer>();
            float currentTime = 0f;

            if (timer != null)
            {
                timer.StopTimer();
                currentTime = timer.GetFinalTime();
            }

            string currentLevel = SceneManager.GetActiveScene().name;
            string bestKey = currentLevel + "_BestTime";
            float bestTime = PlayerPrefs.GetFloat(bestKey, float.MaxValue);
            string isbestKey = currentLevel + "_IsBestScore";

            // Sauvegarder le temps actuel
            PlayerPrefs.SetFloat("FinalTime", currentTime);

            // Vérifier si c'est un record UNE SEULE FOIS
            bool isNewRecord = (currentTime < bestTime && currentTime > 0);

            if (isNewRecord)
            {
                PlayerPrefs.SetFloat(bestKey, currentTime);
                PlayerPrefs.SetString(isbestKey, "true");
                Debug.Log("NOUVEAU RECORD ! " + currentTime + " < " + bestTime);
            }
            else
            {
                PlayerPrefs.SetString(isbestKey, "false");
                Debug.Log("Pas de record. Temps: " + currentTime + " vs Best: " + bestTime);
            }

            PlayerPrefs.SetString("LastCompletedLevel", currentLevel);

            // Sauvegarder UNE SEULE FOIS
            PlayerPrefs.Save();

            Debug.Log("PlayerPrefs sauvegardés - isbestKey = " + PlayerPrefs.GetString(isbestKey));

            LevelManager.UnlockNextLevel(GetCurrentLevelNumber());

            // Charger la scène win
            SceneManager.LoadScene("win");
        }
    }

    // IMPORTANT : Fonction Respawn() en DEHORS de OnTriggerEnter
    void Respawn()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    private int GetCurrentLevelNumber()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("lvl"))
        {
            string numberPart = sceneName.Substring(3);
            if (int.TryParse(numberPart, out int levelNumber))
            {
                return levelNumber;
            }
        }
        return 1;
    }
}