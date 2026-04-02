using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;

    private bool isActive = false;

    private void Update()
    {
        if (Gamepad.all.Count == 0) return;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                if(isActive)
                    TurnSettingsMenuOff();
                else
                    TurnSettingsMenuOn();
                break;
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadPlayerSelectionHub()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(0);
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
}