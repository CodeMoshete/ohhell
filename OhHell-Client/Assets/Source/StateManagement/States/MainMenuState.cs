using Game.Controllers.Interfaces;
using System;
using UnityEngine;

public struct MainMenuLoadParams
{
    public Action<string, string> OnJoinGame;

    public MainMenuLoadParams(Action<string, string> onJoinGame)
    {
        OnJoinGame = onJoinGame;
    }
}

public class MainMenuState : IStateController
{
    private GameObject mainMenuUi;
    private MainMenuScreen mainMenuScreen;
    private Action<string, string> onJoinGame;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        MainMenuLoadParams loadParams = (MainMenuLoadParams)passedParams;
        onJoinGame = loadParams.OnJoinGame;

        // TODO: Get LobbyData from backend.
        LobbyData lobbyData = new LobbyData();
        lobbyData.ActiveGames = new System.Collections.Generic.List<GameData>();

        Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        mainMenuUi = GameObject.Instantiate(Resources.Load<GameObject>("MainMenu"), gameUiLayer);
        mainMenuScreen = mainMenuUi.GetComponent<MainMenuScreen>();
        mainMenuScreen.Initialize(lobbyData, JoinGame);
        onLoadedCallback();
    }

    public void Start()
    {
        Debug.Log("Game started!");
    }

    public void JoinGame(string gameName, string playerName)
    {
        onJoinGame(gameName, playerName);
    }

    public void Unload()
    {
        GameObject.Destroy(mainMenuUi);
    }
}
