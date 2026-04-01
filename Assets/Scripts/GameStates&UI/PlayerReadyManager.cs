using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReadyManager : MonoBehaviour
{
    [SerializeField] private int totalPlayers = 4;
    [SerializeField] private string nextScene = "GameScene";

    private int readyPlayers = 0;

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