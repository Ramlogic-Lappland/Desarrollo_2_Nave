using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        MusicManager.instance.PlayMusic("mainsong");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
