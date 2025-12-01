using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;

    public Sprite completedSprite;
    public Sprite unlockedSprite;
    public Sprite lockedSprite;

    public Image rew_lvl1;
    public Image rew_lvl2;
    public Image rew_lvl3;
    public Sprite bronz_medal;
    public Sprite silver_medal;
    public Sprite gold_medal;

    private void Awake()
    {
        // level unlocked, 1 always
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            Button btn = buttons[i];
            if (btn == null) continue;

            int levelNumber = i + 1;
            btn.interactable = (levelNumber <= unlockedLevel);

            // change the sprite of the button
            Image img = btn.GetComponent<Image>();
            if (img == null) img = btn.GetComponentInChildren<Image>();

            if (levelNumber < unlockedLevel)
            {
                if (img != null && completedSprite != null) img.sprite = completedSprite;
            }
            else if (levelNumber == unlockedLevel)
            {
                if (img != null && unlockedSprite != null) img.sprite = unlockedSprite;
            }
            else
            {
                if (img != null && lockedSprite != null) img.sprite = lockedSprite;
            }
        }

        // Charger et afficher les médailles obtenues
        DisplayMedals();
    }

    private void DisplayMedals()
    {
        Debug.Log("=== LEVEL MANAGER - AFFICHAGE MÉDAILLES ===");
        Debug.Log("Sprites assignés - Bronze: " + (bronz_medal != null) +
                  ", Silver: " + (silver_medal != null) +
                  ", Gold: " + (gold_medal != null));

        if (rew_lvl1 != null)
        {
            int medal = PlayerPrefs.GetInt("lvl1_Medal", 0);
            Debug.Log("Médaille lvl1 lue depuis PlayerPrefs: " + medal);
            SetMedalSprite(rew_lvl1, medal);
        }
        else
        {
            Debug.LogWarning("rew_lvl1 non assigné !");
        }

        if (rew_lvl2 != null)
        {
            int medal = PlayerPrefs.GetInt("lvl2_Medal", 0);
            Debug.Log("Médaille lvl2: " + medal);
            SetMedalSprite(rew_lvl2, medal);
        }
        else
        {
            Debug.LogWarning("rew_lvl2 non assigné !");
        }

        if (rew_lvl3 != null)
        {
            int medal = PlayerPrefs.GetInt("lvl3_Medal", 0);
            Debug.Log("Médaille lvl3: " + medal);
            SetMedalSprite(rew_lvl3, medal);
        }
        else
        {
            Debug.LogWarning("rew_lvl3 non assigné !");
        }
    }

    private void SetMedalSprite(Image img, int medalLevel)
    {
        Debug.Log("SetMedalSprite appelé - niveau: " + medalLevel);

        if (medalLevel == 0)
        {
            img.gameObject.SetActive(false);
            Debug.Log("Pas de médaille - objet désactivé");
            return;
        }

        img.gameObject.SetActive(true);
        Debug.Log("Objet médaille activé");

        if (medalLevel == 1 && bronz_medal != null)
        {
            img.sprite = bronz_medal;
            Debug.Log("Sprite bronze assigné");
        }
        else if (medalLevel == 2 && silver_medal != null)
        {
            img.sprite = silver_medal;
            Debug.Log("Sprite argent assigné");
        }
        else if (medalLevel == 3 && gold_medal != null)
        {
            img.sprite = gold_medal;
            Debug.Log("Sprite or assigné");
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "lvl" + levelId.ToString();
        SceneManager.LoadScene(levelName);
    }

    public static void UnlockNextLevel(int completedLevelNumber)
    {
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int nextToUnlock = completedLevelNumber + 1;
        if (nextToUnlock > currentUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextToUnlock);
            PlayerPrefs.Save();
        }
    }
}