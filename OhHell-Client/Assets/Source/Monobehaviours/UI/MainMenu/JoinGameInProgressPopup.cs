using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameInProgressPopup : MonoBehaviour
{
    public Transform PlayerList;
    public Button CancelButton;
    public GameObject PlayerNameEntryPrefab;

    private GameData game;
    private Action<GameData, string> onJoinGame;
    private List<GameObject> playerNameItems;

    public JoinGameInProgressPopup()
    {
        playerNameItems = new List<GameObject>();
    }

    private void Start()
    {
        CancelButton.onClick.AddListener(OnCancel);
    }

    public void ShowPopup(GameData gameData, Action<GameData, string> onJoinGame)
    {
        for (int i = 0, count = playerNameItems.Count; i < count; ++i)
        {
            GameObject.Destroy(playerNameItems[i]);
        }
        playerNameItems.Clear();

        game = gameData;
        List<PlayerData> playerList = gameData.Players;
        for (int i = 0, count = playerList.Count; i < count; ++i)
        {
            PlayerData player = playerList[i];
            GameObject playerEntry =
                GameObject.Instantiate(PlayerNameEntryPrefab, PlayerList);
            PlayerNameItem namePanel = playerEntry.GetComponent<PlayerNameItem>();
            namePanel.SetName(gameData.Players[i].PlayerName, OnJoinAsPlayer);
            playerNameItems.Add(playerEntry);
        }
        this.onJoinGame = onJoinGame;
        gameObject.SetActive(true);
    }
    
    private void OnJoinAsPlayer(string playerName)
    {
        onJoinGame(game, playerName);
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
