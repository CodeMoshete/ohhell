using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyScreen : MonoBehaviour
{
    public Transform PlayerListContainer;
    public GameObject HostWelcomeText;
    public GameObject PlayerWelcomeText;
    public Button LaunchGameButton;
    public Text GameNameText;
    public Text PlayerCountText;

    public GameObject PlayerNameEntryPrefab;
    private List<GameObject> nameEntries;
    private PlayerData localPlayer;

    private Action onLaunchGame;

    public void Initialize(
        GameData gameData, PlayerData localPlayer, Action onLaunchGame)
    {
        this.onLaunchGame = onLaunchGame;

        this.localPlayer = localPlayer;
        GameNameText.text = string.Format("Game Name: {0}", gameData.GameName);
        PlayerCountText.text = string.Format("Players: {0}", gameData.Players.Count);

        LaunchGameButton.onClick.AddListener(LaunchGame);
        LaunchGameButton.gameObject.SetActive(localPlayer.IsHost);

        PlayerWelcomeText.SetActive(!localPlayer.IsHost);
        HostWelcomeText.SetActive(localPlayer.IsHost);

        nameEntries = new List<GameObject>();
        RefreshPlayerList(gameData);
    }

    private void LaunchGame()
    {
        onLaunchGame();
    }

    public void RefreshPlayerList(GameData gameData)
    {
        for (int i = 0, count = nameEntries.Count; i < count; ++i)
        {
            GameObject.Destroy(nameEntries[i]);
        }
        nameEntries.Clear();

        int numPlayers = gameData.Players.Count;
        for (int i = 0; i < numPlayers; ++i)
        {
            GameObject playerEntry =
                GameObject.Instantiate(PlayerNameEntryPrefab, PlayerListContainer);
            PlayerNameItem namePanel = playerEntry.GetComponent<PlayerNameItem>();
            namePanel.SetName(gameData.Players[i].PlayerName);
            nameEntries.Add(playerEntry);
        }

        PlayerCountText.text = string.Format("Players: {0}", numPlayers);
    }
}
