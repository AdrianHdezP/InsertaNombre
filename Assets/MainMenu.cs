using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Adri");
    }

    public void Exit()
    {
        Application.Quit();
    }
}