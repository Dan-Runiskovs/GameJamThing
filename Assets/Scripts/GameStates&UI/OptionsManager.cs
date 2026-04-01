using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
