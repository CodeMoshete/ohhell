using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Transform GameListPanel;
    public InputField NameField;
    public InputField NewGameNameField;
    public Button NewGameButton;
    public Button RefreshButton;

    private LobbyData currentLobbyData;
    private Action<GameData, string> onJoinGame;
    private List<GameListItem> activeGamesList;

    public void Initialize(
        LobbyData lobbyData, Action<GameData, string> onJoinGamePressed)
    {
        onJoinGame = onJoinGamePressed;
        activeGamesList = new List<GameListItem>();

        RefreshLobbyContent(lobbyData);

        NewGameButton.onClick.AddListener(OnNewGamePressed);
        RefreshButton.onClick.AddListener(RefreshLobbyPressed);

        //GameData gameData = new GameData();
        //gameData.GameName = NewGameNameField.text;
        //gameData.Players = new List<PlayerData>();
        //gameData.CurrentTrumpCard = new Card(CardSuit.Clubs, 7);

        //PlayerData localPlayer = new PlayerData();
        //localPlayer.IsHost = true;
        //localPlayer.PlayerName = NameField.text;
        //localPlayer.CurrentRoundCard = new Card(CardSuit.Clubs, 10);
        //gameData.Players.Add(localPmmmmmmmmlayer);

        //PlayerData dummyPlayer = new PlayerData();
        //dummyPlayer.PlayerName = "dummy";
        //dummyPlayer.CurrentHand = new List<Card>();
        //dummyPlayer.Bids = new List<int>();
        //dummyPlayer.Tricks = new List<int>();
        //dummyPlayer.CurrentRoundCard = new Card(CardSuit.Hearts, 0);
        //gameData.Players.Add(dummyPlayer);

        //Debug.Log("Winner: " + gameData.TurnLeader.CurrentRoundCard.ToString());
    }

    private void OnNewGamePressed()
    {
        if (NewGameNameField.text != string.Empty && NameField.text != string.Empty)
        {
            GameData gameData = new GameData();
            gameData.GameName = NewGameNameField.text;
            gameData.Players = new List<PlayerData>();

            PlayerData localPlayer = new PlayerData();
            localPlayer.IsHost = true;
            localPlayer.PlayerName = NameField.text;
            gameData.Players.Add(localPlayer);

            onJoinGame(gameData, localPlayer.PlayerName);
        }
    }

    private void RefreshLobbyPressed()
    {
        Service.EventManager.SendEvent(EventId.RefreshLobby, null);
    }

    private void OnJoinGame(GameData gameData)
    {
        string playerName = NameField.text;
        if (playerName != string.Empty)
        {
            if (!gameData.GetHasPlayer(NameField.text))
            {
                PlayerData localPlayer = new PlayerData();
                localPlayer.IsHost = false;
                localPlayer.PlayerName = NameField.text;
                gameData.Players.Add(localPlayer);
            }
            onJoinGame(gameData, playerName);
        }
    }

    public void RefreshLobbyContent(LobbyData lobbyData)
    {
        for (int i = 0, numGames = activeGamesList.Count; i < numGames; ++i)
        {
            GameObject.Destroy(activeGamesList[i]);
        }
        activeGamesList.Clear();

        currentLobbyData = lobbyData;
        for (int i = 0, numGames = currentLobbyData.ActiveGames.Count; i < numGames; ++i)
        {
            GameData gameData = currentLobbyData.ActiveGames[i];
            if (!gameData.IsLaunched || gameData.IsLaunched && gameData.GetHasPlayer(NameField.text))
            {
                GameObject gameListItemObj = GameObject.Instantiate(Resources.Load<GameObject>("GameListItem"), GameListPanel);
                GameListItem listItem = gameListItemObj.GetComponent<GameListItem>();
                listItem.Initialize(gameData, OnJoinGame);
                activeGamesList.Add(listItem);
            }
        }
    }
}
