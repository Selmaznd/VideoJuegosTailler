using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI bestScoreText;
    public Image img;
    public Sprite bronz_medal;
    public Sprite silver_medal;
    public Sprite gold_medal;

    void Start()
    {
        Debug.Log("=== WIN MANAGER DEBUG ===");

        if (img != null)
        {
            img.gameObject.SetActive(false);
        }

        string completedLevel = PlayerPrefs.GetString("LastCompletedLevel", "lvl1");
        float time = PlayerPrefs.GetFloat("FinalTime", 0f);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        finalTimeText.text = $"Final time : {minutes:00}:{seconds:00}";

        int current_medal = CalculateMedal(time);
        Debug.Log("Médaille calculée: " + current_medal);

        string medalKey = completedLevel + "_Medal";
        int previousBestMedal = PlayerPrefs.GetInt(medalKey, 0);
        Debug.Log("Ancienne meilleure médaille: " + previousBestMedal);

        // IMPORTANT : Sauvegarder la médaille si elle est meilleure, INDÉPENDAMMENT du temps
        if (current_medal > previousBestMedal)
        {
            Debug.Log("NOUVELLE MÉDAILLE ! " + current_medal + " > " + previousBestMedal);
            PlayerPrefs.SetInt(medalKey, current_medal);
            PlayerPrefs.Save();
        }

        string bestKey = completedLevel + "_BestTime";
        float current_best_score = PlayerPrefs.GetFloat(bestKey, 0f);
        int minutes_bs = Mathf.FloorToInt(current_best_score / 60f);
        int seconds_bs = Mathf.FloorToInt(current_best_score % 60f);

        string isbestKey = completedLevel + "_IsBestScore";
        bool isBest = PlayerPrefs.GetString(isbestKey, "false") == "true";
        Debug.Log("Est-ce un nouveau record ? " + isBest);

        if (isBest)
        {
            bestScoreText.text = "! New best time !";

            // Afficher la médaille si c'est une nouvelle médaille
            if (current_medal > previousBestMedal)
            {
                bestScoreText.text += "\nYou have a new reward!";

                if (img != null)
                {
                    img.gameObject.SetActive(true);

                    if (current_medal == 1 && bronz_medal != null)
                    {
                        img.sprite = bronz_medal;
                    }
                    else if (current_medal == 2 && silver_medal != null)
                    {
                        img.sprite = silver_medal;
                    }
                    else if (current_medal == 3 && gold_medal != null)
                    {
                        img.sprite = gold_medal;
                    }
                }
            }
        }
        else
        {
            bestScoreText.text = string.Format("Best time : {0:00}:{1:00}", minutes_bs, seconds_bs);
        }

        Debug.Log("=== FIN DEBUG ===");
    }

    private int CalculateMedal(float totalTime)
    {
        // Ajustez ces seuils selon la difficulté de vos niveaux
        // Exemple : médaille d'or si moins de 15 secondes, argent si moins de 25, bronze si moins de 40
        if (totalTime <= 15f)
        {
            return 3; // Or
        }
        else if (totalTime <= 25f)
        {
            return 2; // Argent
        }
        else if (totalTime <= 40f)
        {
            return 1; // Bronze
        }
        return 0; // Pas de médaille
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ChooseLevel()
    {
        SceneManager.LoadScene("ChooseLevel");
    }
}