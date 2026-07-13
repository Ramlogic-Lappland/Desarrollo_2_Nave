using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
    public void EnableGameOverScreen()
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Scenes/SolarSystem");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Scenes/Menu");
    }
}
