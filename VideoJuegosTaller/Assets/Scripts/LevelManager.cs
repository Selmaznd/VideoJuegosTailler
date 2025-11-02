using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;

    public Sprite completedSprite;
    public Sprite unlockedSprite;
    public Sprite lockedSprite;

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

    // Exemple : appeler cette méthode quand un joueur complète un niveau
    // pour déverrouiller le suivant.
    public void UnlockNextLevel(int completedLevelNumber)
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
