using Game.Controllers.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct MainMenuLoadParams
{
    public Action<GameData, string> OnJoinGame;

    public MainMenuLoadParams(Action<GameData, string> onJoinGame)
    {
        OnJoinGame = onJoinGame;
    }
}

public class MainMenuState : IStateController
{
    private GameObject mainMenuUi;
    private MainMenuScreen mainMenuScreen;
    private Action<GameData, string> onJoinGame;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        MainMenuLoadParams loadParams = (MainMenuLoadParams)passedParams;
        onJoinGame = loadParams.OnJoinGame;

        Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        mainMenuUi = GameObject.Instantiate(Resources.Load<GameObject>("MainMenu"), gameUiLayer);
        mainMenuScreen = mainMenuUi.GetComponent<MainMenuScreen>();

        Service.WebRequests.GetGamesList((response) =>
        {
            Service.EventManager.AddListener(EventId.RefreshLobby, RefreshLobby);
            LobbyData lobbyData = JsonUtility.FromJson<LobbyData>(response);
            mainMenuScreen.Initialize(lobbyData, JoinGame, CreateGame);
            onLoadedCallback();
        });
    }

    public void Start()
    {
        Debug.Log("Main menu loaded!");
    }

    public void CreateGame(string gameName, string playerName)
    {
        GameData gameData = new GameData();
        gameData.GameName = gameName;
        gameData.Players = new List<PlayerData>();

        PlayerData localPlayer = new PlayerData();
        localPlayer.IsHost = true;
        localPlayer.PlayerName = playerName;
        gameData.Players.Add(localPlayer);

        Service.WebRequests.CreateGame(gameData, (response) =>
        {
            if (response != "false")
            {
                GameData game = JsonUtility.FromJson<GameData>(response);
                onJoinGame(game, playerName);
            }
        });
    }

    public void JoinGame(GameData game, string localPlayerName)
    {
        Service.WebRequests.JoinGame(game, localPlayerName, (response) =>
        {
            if (response != "false")
            {
                game = JsonUtility.FromJson<GameData>(response);
            }

            if (!game.GetHasPlayer(localPlayerName))
            {
                PlayerData localPlayer = new PlayerData();
                localPlayer.IsHost = false;
                localPlayer.PlayerName = localPlayerName;
                game.Players.Add(localPlayer);
            }

            Service.WebRequests.SetGameState(game, (res) =>
            {
                onJoinGame(game, localPlayerName);
            });
        });
    }

    private bool RefreshLobby(object cookie)
    {
        Service.WebRequests.GetGamesList((response) =>
        {
            LobbyData lobbyData = JsonUtility.FromJson<LobbyData>(response);
            mainMenuScreen.RefreshLobbyContent(lobbyData);
        });
        return true;
    }

    public void Unload()
    {
        GameObject.Destroy(mainMenuUi);
    }
}
