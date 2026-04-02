using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private GameObject playerSlotPrefab;
    [SerializeField] private Transform playerSlotParent;
    [SerializeField] private TextMeshProUGUI noControllerText;

    private readonly List<Gamepad> connectedGamepads = new List<Gamepad>();
    private readonly List<bool> readyStates = new List<bool>();
    private readonly List<GameObject> playerSlots = new List<GameObject>();

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        InitializePlayerSlots();
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Update()
    {
        for (int i = 0; i < connectedGamepads.Count; i++)
        {
            Gamepad gamepad = connectedGamepads[i];

            if (gamepad == null) continue;

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                ToggleReady(i);
            }
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Gamepad.all.Count = " + Gamepad.all.Count);
        }

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonSouth.wasPressedThisFrame)
            {
                Debug.Log("A pressed on gamepad index: " + i);
            }
        }

    }

    private void InitializePlayerSlots()
    {
        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (playerSlots[i] != null)
                Destroy(playerSlots[i]);
        }

        playerSlots.Clear();
        readyStates.Clear();
        connectedGamepads.Clear();

        for (int i = 0; i < Gamepad.all.Count && connectedGamepads.Count < 4; i++)
        {
            connectedGamepads.Add(Gamepad.all[i]);
        }

        Debug.Log("Detected gamepads in selection scene: " + connectedGamepads.Count);

        if (noControllerText != null)
            noControllerText.gameObject.SetActive(connectedGamepads.Count == 0);

        if (connectedGamepads.Count == 0)
            return;

        for (int i = 0; i < connectedGamepads.Count; i++)
        {
            readyStates.Add(false);

            GameObject slot = Instantiate(playerSlotPrefab, playerSlotParent);
            playerSlots.Add(slot);

            Button button = slot.GetComponent<Button>();
            TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (button == null)
            {
                Debug.LogError("PlayerSlotButton prefab is missing a Button on the root object.");
                continue;
            }

            if (text == null)
            {
                Debug.LogError("PlayerSlotButton prefab is missing a TextMeshProUGUI child.");
                continue;
            }

            int playerIndex = i;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ToggleReady(playerIndex));

            UpdateSlotText(playerIndex);
        }
    }

    private void ToggleReady(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= readyStates.Count)
            return;

        readyStates[playerIndex] = !readyStates[playerIndex];
        UpdateSlotText(playerIndex);

        Debug.Log($"Player {playerIndex + 1} ready = {readyStates[playerIndex]}");
    }

    private void UpdateSlotText(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerSlots.Count)
            return;

        TextMeshProUGUI text = playerSlots[playerIndex].GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) return;

        string state = readyStates[playerIndex] ? "READY" : "NOT READY";
        text.text = $"Player {playerIndex + 1}\n{state}";
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            InitializePlayerSlots();
        }
    }
}