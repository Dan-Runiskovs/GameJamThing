using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject ResultsMenu;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private GameObject GameOverMenu;
    [SerializeField] private GameObject SettingsButton;

    private bool isActive = false;

    private void Update()
    {
        if (Gamepad.all.Count == 0) return;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                if (isActive)
                    TurnSettingsMenuOff();
                else
                    TurnSettingsMenuOn();
                break;
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadPlayerSelectionHub()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(2);
    }

    public void TurnSettingsMenuOn()
    {
        isActive = true;

        if (settingsMenu != null)
            settingsMenu.SetActive(true);
    }

    public void TurnSettingsMenuOff()
    {
        isActive = false;

        if (settingsMenu != null)
            settingsMenu.SetActive(false);
    }

    public void TurnWinMenuOn()
    {
        if (ResultsMenu != null)
        {
            ResultsMenu.SetActive(true);
            SettingsButton.SetActive(false);

            if (WinMenu != null)
            {
                WinMenu.SetActive(true);
            }
               
        }

    }
    public void TurnGameOverMenuOn()
    {
        if (ResultsMenu != null)
        {
            ResultsMenu.SetActive(true);
            SettingsButton.SetActive(false);

            if (GameOverMenu != null)
            {
                GameOverMenu.SetActive(true);
            }                
        }

    } 
    public void TurnWinMenuOff()
    {
        if (ResultsMenu != null)
        {         
            if (WinMenu != null)
            {
                WinMenu.SetActive(false);
            }               
        }

    }
    public void TurnGameOverMenuOff()
    {
        if (ResultsMenu != null)
        {           
            if (GameOverMenu != null)
            {
                GameOverMenu.SetActive(false);
            }                
        }
    }
}