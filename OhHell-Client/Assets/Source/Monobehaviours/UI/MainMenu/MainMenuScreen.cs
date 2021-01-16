using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Transform GameListPanel;
    public Button NewGameButton;
    public Toggle AdvancedModeToggle;

    public JoinGamePopup JoinGamePopup;
    public JoinGameInProgressPopup JoinGameInProgressPopup;
    public CreateGamePopup CreateGamePopup;

    private LobbyData currentLobbyData;
    private Action<string, string> onJoinGame;
    private Action<GameData, string> onJoinGameInProgress;
    private Action<string, string> onCreateGame;
    private List<GameListItem> activeGamesList;

    public void Initialize(
        LobbyData lobbyData, 
        Action<string, string> onJoinGamePressed,
        Action<GameData, string> onJoinGameInProgressPressed,
        Action<string, string> onCreateGamePressed)
    {
        onJoinGame = onJoinGamePressed;
        onJoinGameInProgress = onJoinGameInProgressPressed;
        onCreateGame = onCreateGamePressed;
        activeGamesList = new List<GameListItem>();

        CreateGamePopup.Initialize(OnNewGameCreated);
        JoinGamePopup.Initialize(OnGameJoined);

        RefreshLobbyContent(lobbyData);
        NewGameButton.onClick.AddListener(OnNewGamePressed);
        AdvancedModeToggle.onValueChanged.AddListener(OnAdvancedChecked);
        AdvancedModeToggle.isOn = false;

        //GameData gameData = new GameData();
        //gameData.GameName = NewGameNameField.text;
        //gameData.Players = new List<PlayerData>();
        //gameData.CurrentTrumpCard = new Card(CardSuit.Diamonds, 0);

        //PlayerData localPlayer = new PlayerData();
        //localPlayer.IsHost = true;
        //localPlayer.PlayerName = NameField.text;
        //localPlayer.CurrentRoundCard = new Card(CardSuit.Spades, 0);
        //gameData.Players.Add(localPlayer);

        //PlayerData dummyPlayer = new PlayerData();
        //dummyPlayer.PlayerName = "dummy";
        //dummyPlayer.CurrentHand = new List<Card>();
        //dummyPlayer.Bids = new List<int>();
        //dummyPlayer.Tricks = new List<int>();
        //dummyPlayer.CurrentRoundCard = new Card(CardSuit.Spades, 4);
        //gameData.Players.Add(dummyPlayer);

        //Debug.Log("Winner: " + gameData.TurnLeader.CurrentRoundCard.ToString());
    }

    private void OnNewGamePressed()
    {
        CreateGamePopup.ShowPopup();
    }

    private void OnNewGameCreated(string gameName, string playerName)
    {
        onCreateGame(gameName, playerName);
    }

    private void OnJoinGame(GameDataSimple gameData)
    {
        if (gameData.isLaunched)
        {
            Service.WebRequests.GetGameState(gameData.gameName, (response) =>
            {
                GameData fullGameData = JsonUtility.FromJson<GameData>(response);
                JoinGameInProgressPopup.ShowPopup(fullGameData, OnInProgressGameJoined);
            });
        }
        else
        { 
            JoinGamePopup.ShowPopup(gameData.gameName);
        }
    }

    private void OnInProgressGameJoined(GameData data, string playerName)
    {
        onJoinGameInProgress(data, playerName);
    }

    private void OnGameJoined(string gameName, string playerName)
    {
        onJoinGame(gameName, playerName);
    }

    private void OnAdvancedChecked(bool toggle)
    {
        //AdvancedModeToggle.isOn = toggle;
    }

    public void RefreshLobbyContent(LobbyData lobbyData)
    {
        for (int i = 0, numGames = activeGamesList.Count; i < numGames; ++i)
        {
            GameObject.Destroy(activeGamesList[i].gameObject);
        }
        activeGamesList.Clear();

        currentLobbyData = lobbyData;
        for (int i = 0, numGames = currentLobbyData.ActiveGames.Count; i < numGames; ++i)
        {
            GameDataSimple gameData = currentLobbyData.ActiveGames[i];
            if (!gameData.isFinished)
            {
                GameObject gameListItemObj = GameObject.Instantiate(
                    Resources.Load<GameObject>("GameListItem"), GameListPanel);
                GameListItem listItem = gameListItemObj.GetComponent<GameListItem>();
                listItem.Initialize(gameData, OnJoinGame);
                activeGamesList.Add(listItem);
            }
        }
    }
}
