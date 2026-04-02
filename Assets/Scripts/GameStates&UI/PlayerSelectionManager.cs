using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    [Header("Fixed player slots in scene, left to right")]
    [SerializeField] private GameObject[] playerSlots = new GameObject[4];
    [SerializeField] private TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[4];

    private Gamepad[] assignedPads = new Gamepad[4];
    private bool[] readyStates = new bool[4];

    [Header("Game start countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int startCountdownDuration = 3;

    private bool countdownActive = false;
    private float countdownTimer = 0f;

    private OptionsManager _optionsManager;

    // Per-player selection state
    // These arrays are per SLOT, so size 4
    private int[] selectedHatIndices = new int[4];
    private int[] selectedColorIndices = new int[4];
    private int[] currentRows = new int[4]; // 0 = Hat, 1 = Color, 2 = Ready

    // Display names for debug/UI text
    private readonly string[] hatNames = { "No Hat", "Hat 1", "Hat 2", "Hat 3", "Hat 4" };
    private readonly string[] colorNames = { "Color 1", "Color 2", "Color 3", "Color 4" };

    //Avatar visuals
    [SerializeField] private PlayerAvatarVisuals[] slotAvatarVisuals = new PlayerAvatarVisuals[4];

    private void Start()
    {
        _optionsManager = FindFirstObjectByType<OptionsManager>();


        if (GameSession.Instance != null)
        {
            GameSession.Instance.ClearSelections();
        }

        RefreshAllSlots();

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        RemoveDisconnectedPads();

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Gamepad pad = Gamepad.all[i];
            if (pad == null) continue;

            int existingSlot = GetSlotIndexForPad(pad);

            // A / Cross = join if not assigned, otherwise toggle ready
            if (pad.buttonSouth.wasPressedThisFrame)
            {
                if (existingSlot == -1)
                {
                    AssignGPadToFreeSlot(pad);
                }
                else
                {
                    ToggleReady(existingSlot);
                }
            }

            // D-pad editing only for already joined players who are not ready
            if (existingSlot != -1 && !readyStates[existingSlot])
            {
                if (pad.dpad.up.wasPressedThisFrame)
                {
                    currentRows[existingSlot]--;
                    if (currentRows[existingSlot] < 0) currentRows[existingSlot] = 2;
                    UpdateSlotVisual(existingSlot);
                }
                else if (pad.dpad.down.wasPressedThisFrame)
                {
                    currentRows[existingSlot]++;
                    if (currentRows[existingSlot] > 2) currentRows[existingSlot] = 0;
                    UpdateSlotVisual(existingSlot);
                }
                else if (pad.dpad.left.wasPressedThisFrame)
                {
                    ChangeCurrentOption(existingSlot, -1);
                }
                else if (pad.dpad.right.wasPressedThisFrame)
                {
                    ChangeCurrentOption(existingSlot, 1);
                }
            }
        }

        HandleCountdown();
    }

    private int GetSlotIndexForPad(Gamepad pad)
    {
        for (int i = 0; i < assignedPads.Length; i++)
        {
            if (assignedPads[i] == pad)
                return i;
        }

        return -1;
    }

    private void AssignGPadToFreeSlot(Gamepad pad)
    {
        for (int i = 0; i < assignedPads.Length; i++)
        {
            if (assignedPads[i] == null)
            {
                assignedPads[i] = pad;
                readyStates[i] = false;

                selectedHatIndices[i] = 0;
                selectedColorIndices[i] = 0;
                currentRows[i] = 0;

                SavePlayerSelection(i);
                UpdateSlotVisual(i);

                Debug.Log($"Assigned {pad.displayName} to slot {i + 1}");
                return;
            }
        }

        Debug.Log("No free player slots left.");
    }

    private void ToggleReady(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= readyStates.Length) return;
        if (assignedPads[slotIndex] == null) return;

        readyStates[slotIndex] = !readyStates[slotIndex];
        UpdateSlotVisual(slotIndex);

        CancelCountdown();

        Debug.Log($"Player {slotIndex + 1} ready = {readyStates[slotIndex]}");
    }

    private void RemoveDisconnectedPads()
    {
        for (int i = 0; i < assignedPads.Length; i++)
        {
            if (assignedPads[i] == null) continue;

            bool stillConnected = false;

            for (int j = 0; j < Gamepad.all.Count; j++)
            {
                if (Gamepad.all[j] == assignedPads[i])
                {
                    stillConnected = true;
                    break;
                }
            }

            if (!stillConnected)
            {
                Debug.Log($"Player {i + 1} controller disconnected. Clearing slot.");

                assignedPads[i] = null;
                readyStates[i] = false;
                selectedHatIndices[i] = 0;
                selectedColorIndices[i] = 0;
                currentRows[i] = 0;

                UpdateSlotVisual(i);
                CancelCountdown();
            }
        }
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            UpdateSlotVisual(i);
        }
    }

    private void UpdateSlotVisual(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotTexts.Length) return;
        if (slotTexts[slotIndex] == null) return;

        if (assignedPads[slotIndex] == null)
        {
            slotTexts[slotIndex].text = "Press A to Join";

            if (slotAvatarVisuals != null && slotIndex < slotAvatarVisuals.Length)
            {
                if (slotAvatarVisuals[slotIndex] != null)
                {
                    slotAvatarVisuals[slotIndex].ApplySelection(0, 0);
                }
            }

            return;
        }

        string hatPrefix = currentRows[slotIndex] == 0 ? "> " : "  ";
        string colorPrefix = currentRows[slotIndex] == 1 ? "> " : "  ";
        string readyPrefix = currentRows[slotIndex] == 2 ? "> " : "  ";

        string readyText = readyStates[slotIndex] ? "READY" : "NOT READY";

        slotTexts[slotIndex].text =
            $"Player {slotIndex + 1}\n" +
            $"{hatPrefix}Hat: {hatNames[selectedHatIndices[slotIndex]]}\n" +
            $"{colorPrefix}Color: {colorNames[selectedColorIndices[slotIndex]]}\n" +
            $"{readyPrefix}Status: {readyText}";

        if (slotAvatarVisuals != null && slotIndex < slotAvatarVisuals.Length)
        {
            if (slotAvatarVisuals[slotIndex] != null)
            {
                slotAvatarVisuals[slotIndex].ApplySelection(
                    selectedColorIndices[slotIndex],selectedHatIndices[slotIndex]);
            }
        }

    }

    public int GetJoinedPlayerCount()
    {
        int count = 0;

        for (int i = 0; i < assignedPads.Length; i++)
        {
            if (assignedPads[i] != null)
                count++;
        }

        return count;
    }

    public int GetReadyPlayerCount()
    {
        int count = 0;

        for (int i = 0; i < readyStates.Length; i++)
        {
            if (assignedPads[i] != null && readyStates[i])
                count++;
        }

        return count;
    }

    public bool AreAllJoinedPlayersReady()
    {
        int joined = 0;
        int ready = 0;

        for (int i = 0; i < assignedPads.Length; i++)
        {
            if (assignedPads[i] != null)
            {
                joined++;

                if (readyStates[i])
                    ready++;
            }
        }

        return joined > 0 && joined == ready;
    }

    private void HandleCountdown()
    {
        bool shouldCountDown = AreAllJoinedPlayersReady();

        if (shouldCountDown)
        {
            if (!countdownActive)
            {
                countdownActive = true;
                countdownTimer = startCountdownDuration;

                if (countdownText != null)
                    countdownText.gameObject.SetActive(true);
            }

            countdownTimer -= Time.deltaTime;

            if (countdownText != null)
            {
                float displayTime = Mathf.Max(0f, countdownTimer);
                countdownText.text = "Starting in " + displayTime.ToString("0.0");
            }

            if (countdownTimer <= 0f)
            {
                countdownActive = false;

                if (countdownText != null)
                    countdownText.gameObject.SetActive(false);

                if (_optionsManager != null)
                {
                    _optionsManager.LoadGame();
                }
                else
                {
                    Debug.LogError("OptionsManager not found in scene.");
                }
            }
        }
        else
        {
            CancelCountdown();
        }
    }

    private void CancelCountdown()
    {
        countdownActive = false;
        countdownTimer = startCountdownDuration;

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private void SavePlayerSelection(int slotIndex)
    {
        if (GameSession.Instance == null) return;
        if (assignedPads[slotIndex] == null) return;

        PlayerSelectionData selection = new PlayerSelectionData
        {
            playerIndex = slotIndex,
            ratColorIndex = selectedColorIndices[slotIndex],
            hatIndex = selectedHatIndices[slotIndex],
            gamepadDeviceId = assignedPads[slotIndex].deviceId
        };

        GameSession.Instance.SavePlayerSelection(selection);
        Debug.Log($"Saved Player {slotIndex + 1} | Color={selectedColorIndices[slotIndex]}" +
            $" | Hat={selectedHatIndices[slotIndex]} | DeviceId={assignedPads[slotIndex].deviceId}");
    }

    private void ChangeCurrentOption(int slotIndex, int direction)
    {
        switch (currentRows[slotIndex])
        {
            case 0: // Hat
                selectedHatIndices[slotIndex] += direction;
                if (selectedHatIndices[slotIndex] < 0) selectedHatIndices[slotIndex] = 4;
                if (selectedHatIndices[slotIndex] > 4) selectedHatIndices[slotIndex] = 0;
                break;

            case 1: // Color
                selectedColorIndices[slotIndex] += direction;
                if (selectedColorIndices[slotIndex] < 0) selectedColorIndices[slotIndex] = 3;
                if (selectedColorIndices[slotIndex] > 3) selectedColorIndices[slotIndex] = 0;
                break;

            case 2: // Ready row
                // no left/right behavior here for now
                break;
        }

        SavePlayerSelection(slotIndex);
        UpdateSlotVisual(slotIndex);
    }
}