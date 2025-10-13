using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public void ChooseLvl1()
    {
        SceneManager.LoadScene("lvl1");
    }

}