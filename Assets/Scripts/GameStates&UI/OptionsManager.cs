using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    // Old player selection stuff
    [SerializeField] private int totalPlayers = 4;
    [SerializeField] private string nextScene = "GameScene";

    private int readyPlayers = 0;
    //

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

    //Old player selection stuff
    public void PlayerReady()
    {
        readyPlayers++;

        Debug.Log("Players Ready: " + readyPlayers + "/" + totalPlayers);

        if (readyPlayers >= totalPlayers)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
