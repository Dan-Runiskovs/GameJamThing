using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerInput playerPrefab;

    [Header("Player Spawn positions (in order of spawning)")]
    [SerializeField] private List<Transform> playerSpawnPoints = new List<Transform>();

    public static event System.Action PlayersSpawned;

    private void Start()
    {
        Debug.Log("GameSession exists? " + (GameSession.Instance != null));

        if (GameSession.Instance != null)
        {
            Debug.Log("Saved selections count: " + GameSession.Instance.playerSelections.Count);
        }

        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession not found.");
            return;
        }

        List<PlayerSelectionData> selections = GameSession.Instance.playerSelections;

        for (int i = 0; i < selections.Count; i++)
        {
            PlayerSelectionData selection = selections[i];

            Gamepad matchingPad = FindGamepadByDeviceId(selection.gamepadDeviceId);

            if (matchingPad == null)
            {
                Debug.LogWarning($"No matching gamepad found for player {selection.playerIndex + 1}");
                continue;
            }

            PlayerInput player = PlayerInput.Instantiate(
                playerPrefab.gameObject,
                playerIndex: selection.playerIndex,
                pairWithDevice: matchingPad
            );

            if (selection.playerIndex < playerSpawnPoints.Count)
            {
                player.transform.position = playerSpawnPoints[selection.playerIndex].position;
                player.transform.rotation = playerSpawnPoints[selection.playerIndex].rotation;
            }

            PlayerAvatarVisuals avatarVisual = player.GetComponent<PlayerAvatarVisuals>();
            if (avatarVisual != null)
            {
                avatarVisual.ApplySelection(selection.ratColorIndex, selection.hatIndex);
            }
        }

        PlayersSpawned?.Invoke();
    }

    private Gamepad FindGamepadByDeviceId(int deviceId)
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].deviceId == deviceId)
                return Gamepad.all[i];
        }

        return null;
    }
}