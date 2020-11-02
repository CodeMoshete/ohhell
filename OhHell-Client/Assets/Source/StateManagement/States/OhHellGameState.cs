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
    private Action onLoaded;
    private GameObject gameUi;
    private GameScreen gameScreen;
    private GameData gameData;
    private PlayerData localPlayer;

    private int actionIndex;
    private Queue<IGameAction> currentPendingActions;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        onLoaded = onLoadedCallback;
        OhHellGameLoadParams loadParams = (OhHellGameLoadParams)passedParams;
        gameData = loadParams.GameData;
        localPlayer = gameData.GetPlayerByName(loadParams.LocalPlayerName);

        Service.EventManager.AddListener(EventId.RoundBegun, OnRoundBegun);
        Service.EventManager.AddListener(EventId.CardPlayed, OnCardPlayed);
        Service.EventManager.AddListener(EventId.TurnEnded, OnTurnEnded);
        Service.EventManager.AddListener(EventId.RoundEnded, OnRoundEnded);
        Service.EventManager.AddListener(EventId.GameEnded, OnGameEnded);

        Transform gameUiLayer = GameObject.Find("GameUILayer").transform;
        gameUi = GameObject.Instantiate(Resources.Load<GameObject>("GameScreen"), gameUiLayer);
        gameUi.SetActive(false);
        gameScreen = gameUi.GetComponent<GameScreen>();

        if (gameData.IsLaunched)
        {
            SyncGameState(() =>
            {
                onLoaded();
            });
        }
        else
        {
            if (localPlayer.IsHost)
            {
                gameData.CurrentDealerIndex = gameData.Players.IndexOf(localPlayer);
                gameData.IsLaunched = true;
                DealCards();
            }
            else
            {
                Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameUpdates, null);
                SyncGameState(() =>
                {
                    onLoaded();
                });
            }
        }
    }

    public void Start()
    {
        currentPendingActions = new Queue<IGameAction>();
        gameScreen.gameObject.SetActive(true);
        Debug.Log("Game started!");
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
            Service.WebRequests.SendGameAction(gameData, beginRoundAction, (beginResponse) => 
            {
                Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameUpdates, null);
                onLoaded();
            });
        });
    }

    private void SyncGameState(Action onSynced)
    {
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData = JsonUtility.FromJson<GameData>(response);
            localPlayer = gameData.GetPlayerByName(localPlayer.PlayerName);
            gameScreen.SyncGameState(gameData, localPlayer);
            Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameUpdates, null);

            if (onSynced != null)
            {
                onSynced();
            }
        });
    }

    private void GetGameUpdates(object cookie)
    {
        Service.WebRequests.GetGameActions(gameData, actionIndex, ApplyNewGameActions);
    }

    private void ApplyNewGameActions(string actionsResponse)
    {
        GetActionsResponse actions = JsonUtility.FromJson<GetActionsResponse>(actionsResponse);
        List<IGameAction> newActions = actions.GetGameActionsFromRecord();
        actionIndex = actions.ActionIndex;
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

    private bool OnRoundBegun(object cookie)
    {
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData = JsonUtility.FromJson<GameData>(response);
            localPlayer = gameData.GetPlayerByName(localPlayer.PlayerName);
            gameScreen.SyncGameState(gameData, localPlayer);
        });
        return false;
    }

    private bool OnCardPlayed(object cookie)
    {
        return false;
    }

    private bool OnTurnEnded(object cookie)
    {
        return false;
    }

    private bool OnRoundEnded(object cookie)
    {
        return false;
    }

    private bool OnGameEnded(object cookie)
    {
        return false;
    }

    public void Unload()
    {
        GameObject.Destroy(gameUi);
    }
}
