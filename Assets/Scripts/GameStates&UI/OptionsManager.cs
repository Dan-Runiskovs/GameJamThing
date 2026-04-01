using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    // Old player selection stuff
    [SerializeField] private int totalPlayers = 4;
    [SerializeField] private string nextScene = "GameScene";
    //[SerializeField] private string mainMenu = "MainMenu";

    private int readyPlayers = 0;
    //

    private bool isActive = false;

    [SerializeField] private GameObject settingsMenu;

    private void Update()
    {
        if (Gamepad.all.Count == 0) return;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                TurnSettingsMenuOn();
                break;
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(3);
    }

    public void TurnSettingsMenuOn()
    {
        isActive = true;
        settingsMenu.SetActive(isActive);
    }
    public void TurnSettingsMenuOff()
    {
        isActive = false;
        settingsMenu.SetActive(isActive);
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
