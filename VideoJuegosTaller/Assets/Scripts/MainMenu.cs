using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject InfoGame;
    public GameObject BGPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("ChooseLevel");
    }

    public void DisplayInfo()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.DeleteKey("FinalTime");
        PlayerPrefs.DeleteKey("LastCompletedLevel");

        // Réinitialiser tous les niveaux (ajustez le nombre selon vos niveaux)
        for (int i = 1; i <= 10; i++)
        {
            string levelName = "lvl" + i;
            PlayerPrefs.DeleteKey(levelName + "_BestTime");
            PlayerPrefs.DeleteKey(levelName + "_IsBestScore");
            PlayerPrefs.DeleteKey(levelName + "_Medal");
        }

        InfoGame.SetActive(true);
        BGPanel.SetActive(true);
    }

    public void CloseInfo()
    {
        InfoGame.SetActive(false);
        BGPanel.SetActive(false);
    }

    /**public void QuitGame()
    {
        Application.Quit();

    }

    public void GetHelp()
    {
        InfoUI.SetActive(true);
    }

    public void CloseHelp()
    {
        InfoUI.SetActive(false);
    }**/
}