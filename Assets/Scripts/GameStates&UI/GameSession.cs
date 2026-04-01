using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public List<PlayerSelectionData> playerSelections = new List<PlayerSelectionData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClearSelections()
    {
        playerSelections.Clear();
    }

    public void SavePlayerSelection(PlayerSelectionData selection)
    {
        int existingIndex = playerSelections.FindIndex(p => p.playerIndex == selection.playerIndex);

        if (existingIndex >= 0)
        {
            playerSelections[existingIndex] = selection;
        }
        else
        {
            playerSelections.Add(selection);
        }           
    }

    public PlayerSelectionData GetPlayerSelection(int playerIndex)
    {
        return playerSelections.Find(p => p.playerIndex == playerIndex);
    }
}