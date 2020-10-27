using Game.Controllers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OhHellLobbyLoadParams
{
    public GameData GameData;
    public Action<GameData,  string> OnGameLaunched;
    public string LocalPlayerName;

    public OhHellLobbyLoadParams(
        GameData gameData, 
        Action<GameData, string> onGameLaunched, 
        string localPlayerName)
    {
        GameData = gameData;
        OnGameLaunched = onGameLaunched;
        LocalPlayerName = localPlayerName;
    }
}

public class OhHellLobbyState : IStateController
{
    private const float LOBBY_UPDATE_TIME = 5f;
    private GameObject lobbyUi;
    private GameLobbyScreen lobbyScreen;
    private GameData gameData;
    private Action<GameData, string> onLaunchGame;
    private string localPlayerName;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        OhHellLobbyLoadParams loadParams = (OhHellLobbyLoadParams)passedParams;
        gameData = loadParams.GameData;
        onLaunchGame = loadParams.OnGameLaunched;
        localPlayerName = loadParams.LocalPlayerName;

        Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        lobbyUi = GameObject.Instantiate(Resources.Load<GameObject>("GameLobbyScreen"), gameUiLayer);
        lobbyScreen = lobbyUi.GetComponent<GameLobbyScreen>();
        lobbyScreen.Initialize(gameData, gameData.GetPlayerByName(localPlayerName), LaunchGame);
        lobbyUi.SetActive(false);

        onLoadedCallback();
    }

    public void Start()
    {
        lobbyUi.SetActive(true);
        Service.TimerManager.CreateTimer(LOBBY_UPDATE_TIME, GetLobbyStatus, null);
        Debug.Log("Lobby started!");
    }

    private void GetLobbyStatus(object cookie)
    {
        if (gameData.IsLaunched)
        {
            return;
        }

        Debug.Log("Checking lobby status");
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData = JsonUtility.FromJson<GameData>(response);
            lobbyScreen.RefreshPlayerList(gameData);
            if (gameData.IsLaunched)
            {
                Debug.Log("Game started!");
                onLaunchGame(gameData, localPlayerName);
            }
            else
            {
                Service.TimerManager.CreateTimer(LOBBY_UPDATE_TIME, GetLobbyStatus, null);
            }
        });
    }

    private void LaunchGame()
    {
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData.IsLaunched = true;
            Service.WebRequests.SetGameState(gameData, (setResponse) =>
            {
                onLaunchGame(gameData, localPlayerName);
            });
        });
    }

    public void Unload()
    {
        GameObject.Destroy(lobbyUi);
    }
}
