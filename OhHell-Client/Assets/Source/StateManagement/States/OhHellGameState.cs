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
    private const float GAME_UPDATE_TIME = 3f;
    private GameObject gameUi;
    private GameScreen gameScreen;
    private GameData gameData;
    private PlayerData localPlayer;

    private int actionIndex;
    private Queue<IGameAction> currentPendingActions;

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

        Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        gameUi = GameObject.Instantiate(Resources.Load<GameObject>("GameScreen"), gameUiLayer);
        gameScreen = gameUi.GetComponent<GameScreen>();

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
        gameData.CurrentDealerIndex = dealerIndex;
        gameData.CurrentPlayerTurnIndex = (dealerIndex == playerCount - 1) ? 0 : dealerIndex + 1;
        Service.WebRequests.SetGameState(gameData, (response) =>
        {
            TableRoundBeginAction beginRoundAction = new TableRoundBeginAction();
            beginRoundAction.IsRoundBegun = true;
            Service.WebRequests.SendGameAction(beginRoundAction, (beginResponse) => 
            {
                Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameUpdates, null);
            });
        });
    }

    public void Start()
    {
        currentPendingActions = new Queue<IGameAction>();
        Debug.Log("Game started!");
    }

    private void GetGameUpdates(object cookie)
    {
        Service.WebRequests.GetGameActions(gameData, actionIndex, ApplyNewGameActions);
    }

    private void ApplyNewGameActions(string actionsResponse)
    {
        GameActionRecord actions = JsonUtility.FromJson<GameActionRecord>(actionsResponse);
        List<IGameAction> newActions = actions.GetGameActionsFromRecord();
        bool areActionsRunning = currentPendingActions.Count > 0;
        bool keepListening = true;
        for (int i = 0, count = newActions.Count; i < count; ++i)
        {
            currentPendingActions.Enqueue(newActions[i]);
            keepListening = newActions[i].ActionType != "TableGameEndAction";
        }

        if (!areActionsRunning)
        {
            InvokeNextAction();
        }

        if (keepListening)
        {
            Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameUpdates, null);
        }
    }

    private void InvokeNextAction()
    {
        if (currentPendingActions.Count > 0)
        {
            IGameAction nextAction = currentPendingActions.Dequeue();
            nextAction.ExecuteAction(InvokeNextAction);
            actionIndex++;
        }
    }

    public void Unload()
    {
        GameObject.Destroy(gameUi);
    }
}
