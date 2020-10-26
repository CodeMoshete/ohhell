using Game.Controllers.Interfaces;
using System;
using UnityEngine;

public struct MainMenuLoadParams
{
    public Action<GameData> OnJoinGame;

    public MainMenuLoadParams(Action<GameData> onJoinGame)
    {
        OnJoinGame = onJoinGame;
    }
}

public class MainMenuState : IStateController
{
    private GameObject mainMenuUi;
    private MainMenuScreen mainMenuScreen;
    private Action<GameData> onJoinGame;

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
            mainMenuScreen.Initialize(lobbyData, JoinGame);
            onLoadedCallback();
        });
    }

    public void Start()
    {
        Debug.Log("Main menu loaded!");
    }

    public void JoinGame(GameData game)
    {
        Service.WebRequests.SetGameState(game, (response) =>
        {
            onJoinGame(game);
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
