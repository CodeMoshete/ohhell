using Game.Controllers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OhHellGameLoadParams
{
    public GameData GameData;
    public Action OnGameExit;

    public OhHellGameLoadParams(GameData gameData, Action onGameExit)
    {
        GameData = gameData;
        OnGameExit = onGameExit;
    }
}

public class OhHellGameState : IStateController
{
    private const float GAME_UPDATE_TIME = 5f;
    private GameObject gameUi;
    private GameData gameData;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        OhHellGameLoadParams loadParams = (OhHellGameLoadParams)passedParams;
        gameData = loadParams.GameData;

        //Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        //gameUi = GameObject.Instantiate(Resources.Load<GameObject>("GameLobbyScreen"), gameUiLayer);
        //lobbyScreen = gameUi.GetComponent<GameLobbyScreen>();
        //lobbyScreen.Initialize(gameData);

        onLoadedCallback();
    }

    public void Start()
    {
        Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameStatus, null);
        Debug.Log("Game started!");
    }

    private void GetGameStatus(object cookie)
    {
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData = JsonUtility.FromJson<GameData>(response);
            //lobbyScreen.RefreshPlayerList(gameData);
        });
    }

    public void Unload()
    {
        GameObject.Destroy(gameUi);
    }
}
