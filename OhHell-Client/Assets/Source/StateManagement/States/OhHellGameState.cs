using DG.Tweening;
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

    private Card CurrentSelectedCard;

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
        gameUi.SetActive(true);
        Service.TimerManager.CreateTimer(GAME_UPDATE_TIME, GetGameUpdates, null);
        gameScreen.SyncGameState(gameData, localPlayer);
        Service.EventManager.AddListener(EventId.CardSelected, OnCardSelected);
        Service.EventManager.AddListener(EventId.PlayCardPressed, OnPlayCardPressed);
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
        gameData.CurrentLeaderIndex = dealerIndex;
        gameData.CurrentPlayerTurnIndex = (dealerIndex == playerCount - 1) ? 0 : dealerIndex + 1;
        Service.WebRequests.SetGameState(gameData, (response) =>
        {
            TableRoundBeginAction beginRoundAction = new TableRoundBeginAction();
            beginRoundAction.IsRoundBegun = true;
            Service.WebRequests.SendGameAction(gameData, beginRoundAction, (beginResponse) => 
            {
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
        int numNewActions = newActions.Count;

        actionIndex = actions.ActionIndex;
        bool areActionsRunning = currentPendingActions.Count > 0;
        bool keepListening = true;
        for (int i = 0; i < numNewActions; ++i)
        {
            Debug.Log("ENQUEUE ACTION: " + newActions[i].ActionType);
            currentPendingActions.Enqueue(newActions[i]);
            keepListening = newActions[i].ActionType != "TableGameEndAction";
        }

        if (!areActionsRunning && numNewActions > 0)
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
            Debug.Log("EXECUTE ACTION: " + nextAction.ActionType);
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

    private bool OnCardSelected(object cookie)
    {
        CardView selectedCard = (CardView)cookie;
        CurrentSelectedCard = selectedCard.CardData;
        return false;
    }

    private bool OnPlayCardPressed(object cookie)
    {
        if (CurrentSelectedCard != null)
        {
            PlayerTurnAction turnAction = new PlayerTurnAction();
            turnAction.CardPlayed = CurrentSelectedCard;
            turnAction.PlayerIndex = gameData.Players.IndexOf(localPlayer);
            turnAction.IsRoundEnded = gameData.CurrentPlayerTurnIndex == gameData.CurrentLeaderIndex;
            Service.WebRequests.SendGameAction(gameData, turnAction, (response) => {});
        }
        return false;
    }

    private bool OnCardPlayed(object cookie)
    {
        PlayerTurnAction turn = (PlayerTurnAction)cookie;
        PlayerData turnPlayer = gameData.Players[turn.PlayerIndex];
        turnPlayer.PlayCardFromHand(turn.CardPlayed);
        gameScreen.SetHighCard(gameData);

        if (turnPlayer.PlayerName == gameData.Players[gameData.CurrentLeaderIndex].PlayerName)
        {
            // Award trick.
            if (localPlayer.IsHost)
            {
                // Start next table turn.
            }
            return false;
        }

        gameData.IncrementTurnCounter();
        gameScreen.SyncGameState(gameData, localPlayer);
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
