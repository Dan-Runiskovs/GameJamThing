using System.Collections.Generic;
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

    //Countdown
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int startCountdownDuration = 3;

    private bool countdownActive = false;
    private float countdownTimer = 0f;

    private OptionsManager _optionsManager;
    private void Start()
    {
        _optionsManager = FindFirstObjectByType<OptionsManager>();
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

            if (pad.buttonSouth.wasPressedThisFrame)
            {
                int existingSlot = GetSlotIndexForPad(pad);

                if (existingSlot == -1)
                {
                    AssignPadToNextFreeSlot(pad);
                }
                else
                {
                    ToggleReady(existingSlot);
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

    private void AssignPadToNextFreeSlot(Gamepad pad)
    {
        for (int i = 0; i < assignedPads.Length; i++)
        {
            if (assignedPads[i] == null)
            {
                assignedPads[i] = pad;
                readyStates[i] = false;
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
                UpdateSlotVisual(i);

                CancelCountdown();
            }
        }        
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < playerSlots.Length; i++)
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
        }
        else
        {
            string readyText = readyStates[slotIndex] ? "READY" : "NOT READY";
            slotTexts[slotIndex].text = $"Player {slotIndex + 1}\n{readyText}";
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

    //Countdown
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
               countdownText.text = "Starting in " + countdownTimer.ToString("0.0");
            }

            if (countdownTimer <= 0f)
            {
                Debug.Log("should load new scene.");
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

        Debug.Log("Countdown active: " + countdownActive + " | Timer: " + countdownTimer);
    }

    private void CancelCountdown()
    {
        countdownActive = false;
        countdownTimer = startCountdownDuration;

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }
}