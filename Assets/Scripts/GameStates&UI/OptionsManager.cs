using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private string nextScene = "GameScene";
    [SerializeField] private GameObject settingsMenu;

    // Player slot stuff
    [SerializeField] private GameObject playerSlotPrefab;
    [SerializeField] private Transform playerSlotParent;

    private bool isActive = false;

    private int connectedPlayers;
    private List<bool> readyStates = new List<bool>();
    private List<GameObject> playerSlots = new List<GameObject>();

    private void Start()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.ClearSelections();
        }

        connectedPlayers = Mathf.Clamp(Gamepad.all.Count, 1, 4);
        Debug.Log("Connected controllers: " + connectedPlayers);

        for (int i = 0; i < connectedPlayers; i++)
        {
            readyStates.Add(false);

            GameObject slot = Instantiate(playerSlotPrefab, playerSlotParent);
            playerSlots.Add(slot);

            int playerIndex = i;

            Button button = slot.GetComponent<Button>();
            TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (button == null)
            {
                Debug.LogError("Player slot prefab is missing a Button component.");
                return;
            }

            if (text == null)
            {
                Debug.LogError("Player slot prefab is missing a TextMeshProUGUI component.");
                return;
            }

            UpdateSlotText(playerIndex, text);
            button.onClick.AddListener(() => ToggleReady(playerIndex));
        }
    }

    private void Update()
    {
        if (Gamepad.all.Count == 0) return;

        foreach (Gamepad gamepad in Gamepad.all)
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
        SceneManager.LoadScene(0);
    }

    public void TurnSettingsMenuOn()
    {
        isActive = true;

        if (settingsMenu != null)
            settingsMenu.SetActive(isActive);
    }

    public void TurnSettingsMenuOff()
    {
        isActive = false;

        if (settingsMenu != null)
            settingsMenu.SetActive(isActive);
    }

    private void ToggleReady(int playerIndex)
    {
        readyStates[playerIndex] = !readyStates[playerIndex];

        TextMeshProUGUI text = playerSlots[playerIndex].GetComponentInChildren<TextMeshProUGUI>();
        UpdateSlotText(playerIndex, text);

        if (readyStates[playerIndex])
        {
            SavePlayerSelection(playerIndex);
        }

        CheckIfAllReady();
    }

    private void UpdateSlotText(int playerIndex, TextMeshProUGUI text)
    {
        string state = readyStates[playerIndex] ? "READY" : "NOT READY";
        text.text = $"Player {playerIndex + 1}\n{state}";
    }

    private void SavePlayerSelection(int playerIndex)
    {
        PlayerSelectionData selection = new PlayerSelectionData
        {
            playerIndex = playerIndex,
            characterIndex = 0,
            ratColorIndex = Random.Range(0, 4),
            hatIndex = Random.Range(0, 5)
        };

        if (GameSession.Instance != null)
        {
            GameSession.Instance.SavePlayerSelection(selection);
        }
    }

    private void CheckIfAllReady()
    {
        int readyPlayers = 0;

        for (int i = 0; i < connectedPlayers; i++)
        {
            if (readyStates[i])
                readyPlayers++;
        }

        Debug.Log("Players Ready: " + readyPlayers + "/" + connectedPlayers);

        if (readyPlayers >= connectedPlayers)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}