using Game.Controllers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OhHellGameLoadParams
{
    public GameData GameData;
    public Action OnGameExit;
    public string LocalPlayerName;

    public OhHellGameLoadParams(
        GameData gameData, Action onGameExit, string localPlayerName)
    {
        GameData = gameData;
        OnGameExit = onGameExit;
        LocalPlayerName = localPlayerName;
    }
}

public class OhHellGameState : IStateController
{
    private const float GAME_UPDATE_TIME = 5f;
    private GameObject gameUi;
    private GameData gameData;
    private PlayerData localPlayer;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        OhHellGameLoadParams loadParams = (OhHellGameLoadParams)passedParams;
        gameData = loadParams.GameData;
        localPlayer = gameData.GetPlayerByName(loadParams.LocalPlayerName);

        if (localPlayer.IsHost)
        {
            gameData.CurrentDealerIndex = gameData.Players.IndexOf(localPlayer);
            DealCards();
            gameData.IsLaunched = true;
        }

        //Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        //gameUi = GameObject.Instantiate(Resources.Load<GameObject>("GameLobbyScreen"), gameUiLayer);
        //lobbyScreen = gameUi.GetComponent<GameLobbyScreen>();
        //lobbyScreen.Initialize(gameData);

        onLoadedCallback();
    }

    private void DealCards()
    {
        int playerCount = gameData.Players.Count;
        int totalCardsNeeded = playerCount * 7;
        uint numDecks = (uint)Mathf.CeilToInt((float)totalCardsNeeded / 52f);
        int dealerIndex = gameData.CurrentDealerIndex;
        CardDeck newDeck = new CardDeck(numDecks);
        newDeck.Shuffle();
        gameData.CurrentRoundNumber = 1;

        for (int i = 0, count = gameData.NumCardsToDeal; i < count; ++i)
        {
            for (int j = 0; j < playerCount; ++j)
            {
                int playerIndex = (dealerIndex + j) % (playerCount - 1);
                gameData.Players[j].CurrentHand.Add(newDeck.DealCard());
            }
        }

        gameData.CurrentTrumpCard = newDeck.DealCard();
        dealerIndex = (dealerIndex == playerCount - 1) ? 0 : dealerIndex + 1;
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
