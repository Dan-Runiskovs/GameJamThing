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
        var gamepads = Gamepad.all;

        for (int i = 0; i < Mathf.Min(4, gamepads.Count); i++)
        {
            PlayerInput player = PlayerInput.Instantiate(
                playerPrefab.gameObject,
                playerIndex: i,
                pairWithDevice: gamepads[i]
            );
            //player.currentActionMap
            // --- Move player to spawn point ---
            player.GetComponent<Rigidbody>().position = playerSpawnPoints[i].position;
            player.GetComponent<Rigidbody>().rotation = playerSpawnPoints[i].rotation;
        }
        PlayersSpawned?.Invoke();
    }
}
