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