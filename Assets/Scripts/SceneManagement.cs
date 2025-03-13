using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
        Time.timeScale = 1f;
    }

    public void OnPressStart()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1));
    }

}
