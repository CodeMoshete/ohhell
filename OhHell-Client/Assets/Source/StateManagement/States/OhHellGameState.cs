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

    private int seenActionIndex;
    private Queue<IGameAction> currentPendingActions;

    private Card CurrentSelectedCard;

    public void Load(Action onLoadedCallback, object passedParams)
    {
        onLoaded = onLoadedCallback;
        OhHellGameLoadParams loadParams = (OhHellGameLoadParams)passedParams;
        gameData = loadParams.GameData;
        localPlayer = gameData.GetPlayerByName(loadParams.LocalPlayerName);

        Service.EventManager.AddListener(EventId.RoundBegun, OnRoundBegun);
        Service.EventManager.AddListener(EventId.RemoteCardPlayed, OnRemoteCardPlayed);
        Service.EventManager.AddListener(EventId.RemoteTurnEnded, OnRemoteTurnEnded);
        Service.EventManager.AddListener(EventId.RemoteRoundEnded, OnRemoteRoundEnded);
        Service.EventManager.AddListener(EventId.GameEnded, OnGameEnded);
        Service.EventManager.AddListener(EventId.LocalBidPlaced, OnLocalBidPlaced);
        Service.EventManager.AddListener(EventId.RemoteBidPlaced, OnRemoteBidPlaced);
        Service.EventManager.AddListener(EventId.OnShowScoresClicked, ShowScores);

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
        Service.EventManager.AddListener(EventId.PlayCardPressed, OnLocalCardPlayed);
        Debug.Log("Game started!");
    }

    private void DealCards()
    {
        Debug.Log("Dealing cards...");
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
                int playerIndex = (dealerIndex + j) % (playerCount);
                gameData.Players[playerIndex].CurrentHand.Add(newDeck.DealCard());
            }
        }

        for (int j = 0; j < playerCount; ++j)
        {
            gameData.Players[j].CurrentBid = -1;
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

    private bool ShowScores(object cookie)
    {
        gameScreen.ShowScoreSheet(gameData);
        return true;
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
        Service.WebRequests.GetGameActions(gameData, seenActionIndex, ApplyNewGameActions);
    }

    private void ApplyNewGameActions(string actionsResponse)
    {
        GetActionsResponse actions = JsonUtility.FromJson<GetActionsResponse>(actionsResponse);
        List<IGameAction> newActions = actions.GetGameActionsFromRecord();
        int numNewActions = newActions.Count;

        seenActionIndex = actions.ActionIndex;
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
        }
    }

    private bool OnRoundBegun(object cookie)
    {
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData = JsonUtility.FromJson<GameData>(response);
            localPlayer = gameData.GetPlayerByName(localPlayer.PlayerName);
            gameScreen.SyncGameState(gameData, localPlayer);
            gameScreen.BeginBidding(gameData);
        });
        return false;
    }

    private bool OnLocalBidPlaced(object cookie)
    {
        PlayerBidAction bidAction = new PlayerBidAction();
        bidAction.PlayerBid = (int)cookie;
        bidAction.PlayerIndex = gameData.Players.IndexOf(localPlayer);
        Service.WebRequests.SendGameAction(gameData, bidAction, (response) => { });
        return true;
    }

    private bool OnRemoteBidPlaced(object cookie)
    {
        PlayerBidAction bidAction = (PlayerBidAction)cookie;
        PlayerData player = gameData.Players[bidAction.PlayerIndex];
        player.CurrentBid = bidAction.PlayerBid;
        player.Bids.Add(bidAction.PlayerBid);
        if (gameData.AllBidsPlaced)
        {
            Debug.Log("All bids placed!");
            gameScreen.EndBidding();
            gameScreen.SyncGameState(gameData, localPlayer);
        }
        return true;
    }

    private bool OnCardSelected(object cookie)
    {
        CardView selectedCard = (CardView)cookie;
        CurrentSelectedCard = selectedCard.CardData;
        return false;
    }

    private bool OnLocalCardPlayed(object cookie)
    {
        if (CurrentSelectedCard != null)
        {
            PlayerTurnAction turnAction = new PlayerTurnAction();
            turnAction.CardPlayed = CurrentSelectedCard;
            turnAction.PlayerIndex = gameData.Players.IndexOf(localPlayer);
            Service.WebRequests.SendGameAction(gameData, turnAction, (response) => {});
        }
        return false;
    }

    private bool OnRemoteCardPlayed(object cookie)
    {
        PlayerTurnAction turn = (PlayerTurnAction)cookie;
        PlayerData turnPlayer = gameData.Players[turn.PlayerIndex];
        turnPlayer.PlayCardFromHand(turn.CardPlayed);
        gameScreen.SetHighCard(gameData);

        gameData.IncrementTurnCounter();
        PlayerData nextPlayer = gameData.Players[gameData.CurrentPlayerTurnIndex];

        if (nextPlayer.PlayerName == gameData.Players[gameData.CurrentLeaderIndex].PlayerName)
        {
            Debug.Log("End of turns!");
            // Award trick.
            if (localPlayer.IsHost)
            {
                // Start next table turn.
                if (gameData.RoundOver)
                {
                    TableRoundEndAction roundEndAction = new TableRoundEndAction();
                    roundEndAction.IsRoundEnded = true;
                    Service.WebRequests.SendGameAction(gameData, roundEndAction, (response) => { });
                }
                else
                {
                    TableTurnEndAction turnEndAction = new TableTurnEndAction();
                    turnEndAction.IsEndOfTurn = true;
                    Service.WebRequests.SendGameAction(gameData, turnEndAction, (response) => { });
                }
            }
            return false;
        }

        gameScreen.SyncGameState(gameData, localPlayer);
        return false;
    }

    private bool OnRemoteTurnEnded(object cookie)
    {
        gameData.TurnLeader.CurrentTricks++;
        gameScreen.ShowHandResult(gameData);

        gameData.CurrentLeaderIndex = gameData.Players.IndexOf(gameData.TurnLeader);
        gameData.CurrentPlayerTurnIndex = gameData.CurrentLeaderIndex;
        gameData.ClearTable();

        Service.TimerManager.CreateTimer(5f, (timerCookie) =>
        {
            gameScreen.HideHandresult();
            gameScreen.SyncGameState(gameData, localPlayer);
        }, null);

        return false;
    }

    private bool OnRemoteRoundEnded(object cookie)
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
