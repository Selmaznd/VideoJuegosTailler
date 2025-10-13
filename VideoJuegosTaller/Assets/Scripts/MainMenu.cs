using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject InfoUI;

    public void StartGame()
    {
        SceneManager.LoadScene("ChooseLevel");
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