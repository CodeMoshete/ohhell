using Game.Controllers.Interfaces;
using System;
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
    private const float HOST_GAME_STATE_SYNC_TIME = 30f;
    private Action onLoaded;
    private GameObject gameUi;
    private GameScreen gameScreen;
    private GameData gameData;
    private PlayerData localPlayer;

    private int seenActionIndex;
    private Queue<IGameAction> currentPendingActions;

    private Card currentSelectedCard;
    private Card autoPlayCard;

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
        gameUi = GameObject.Instantiate(
            Resources.Load<GameObject>("GameScreen"), 
            gameUiLayer);
        gameUi.SetActive(false);
        gameScreen = gameUi.GetComponent<GameScreen>();

        if (gameData.IsLaunched)
        {
            // Joined a game in progress.
            SyncGameState(() =>
            {
                seenActionIndex = gameData.CurrentActionIndex;
                onLoaded();
            });
        }
        else
        {
            // Joined a newly launched game.
            if (localPlayer.IsHost)
            {
                gameData.CurrentDealerIndex = gameData.Players.IndexOf(localPlayer);
                gameData.IsLaunched = true;
                DealCards();
                Service.WebRequests.SetGameState(gameData, (response) =>
                {
                    TableRoundBeginAction beginRoundAction = new TableRoundBeginAction();
                    beginRoundAction.IsRoundBegun = true;
                    Service.WebRequests.SendGameAction(gameData, beginRoundAction,
                        (beginResponse) =>
                        {
                            onLoaded();
                        });
                });
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
        if (localPlayer.IsHost)
        {
            Service.TimerManager.CreateTimer(HOST_GAME_STATE_SYNC_TIME, UpdateServerGameData, null);
        }

        bool isFirstCard = gameData.CurrentLeaderIndex == gameData.CurrentPlayerTurnIndex;
        gameScreen.SyncGameState(gameData, localPlayer, isFirstCard);
        Service.EventManager.AddListener(EventId.CardSelected, OnCardSelected);
        Service.EventManager.AddListener(EventId.AutoPlayCardSelected, OnAutoPlayCardSelected);
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
        gameData.CurrentPlayerTurnIndex = 
            (dealerIndex == playerCount - 1) ? 0 : dealerIndex + 1;
        gameData.CurrentLeaderIndex = gameData.CurrentPlayerTurnIndex;
        dealerIndex = (dealerIndex == playerCount - 1) ? 0 : dealerIndex + 1;
        gameData.CurrentDealerIndex = dealerIndex;
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
            bool isFirstCard = gameData.CurrentLeaderIndex == gameData.CurrentPlayerTurnIndex;
            gameScreen.SyncGameState(gameData, localPlayer, isFirstCard);

            if (onSynced != null)
            {
                onSynced();
            }
        });
    }

    private void GetGameUpdates(object cookie)
    {
        Service.WebRequests.GetGameActions(gameData, 
            seenActionIndex, ApplyNewGameActions);
    }

    private void ApplyNewGameActions(string actionsResponse)
    {
        GetActionsResponse actions = 
            JsonUtility.FromJson<GetActionsResponse>(actionsResponse);
        List<IGameAction> newActions = actions.GetGameActionsFromRecord();
        int numNewActions = newActions.Count;

        bool keepListening = true;
        if (numNewActions > 0)
        {
            seenActionIndex = actions.ActionIndex;
            bool areActionsRunning = currentPendingActions.Count > 0;
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
            gameData.CurrentActionIndex = seenActionIndex - currentPendingActions.Count;
        }
    }

    private bool OnRoundBegun(object cookie)
    {
        Service.WebRequests.GetGameState(gameData, (response) =>
        {
            gameData = JsonUtility.FromJson<GameData>(response);
            localPlayer = gameData.GetPlayerByName(localPlayer.PlayerName);
            gameScreen.SyncGameState(gameData, localPlayer, true);
            gameScreen.BeginBidding(gameData, localPlayer);
            Action OnDone = (Action)cookie;
            OnDone();
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
        if (player.Bids.Count < gameData.CurrentRoundNumber)
        {
            player.CurrentBid = bidAction.PlayerBid;
            player.Bids.Add(bidAction.PlayerBid);
            player.Tricks.Add(0);
            Debug.Log("Player " + player.PlayerName + " placed a bid of " + player.CurrentBid + ". " + gameData.AllBidsPlaced);
        }
        else
        {
            Debug.LogWarning("Player " + player.PlayerName + " tried to bid again: " + bidAction.PlayerBid);
        }

        if (gameData.AllBidsPlaced)
        {
            Debug.Log("All bids placed!");
            gameScreen.EndBidding();
            gameScreen.SyncGameState(gameData, localPlayer, true);
        }
        return true;
    }

    private bool OnCardSelected(object cookie)
    {
        CardView selectedCard = (CardView)cookie;
        currentSelectedCard = selectedCard.CardData;
        return false;
    }

    private bool OnAutoPlayCardSelected(object cookie)
    {
        CardView autoPlayCard = (CardView)cookie;
        this.autoPlayCard = autoPlayCard.CardData;
        return false;
    }

    private bool OnLocalCardPlayed(object cookie)
    {
        if (currentSelectedCard != null && gameData.IsCardValid(currentSelectedCard, localPlayer))
        {
            PlayerTurnAction turnAction = new PlayerTurnAction();
            turnAction.CardPlayed = currentSelectedCard;
            turnAction.PlayerIndex = gameData.Players.IndexOf(localPlayer);
            gameScreen.CardPlayed();
            Service.WebRequests.SendGameAction(
                gameData, turnAction, (response) => {}, seenActionIndex);
            currentSelectedCard = null;
            autoPlayCard = null;
            gameScreen.DisableHand();
        }
        else if (currentSelectedCard == null)
        {
            string msg = "Select a card to play before clicking the submit button.";
            Service.EventManager.SendEvent(EventId.ShowCardNotification, msg);
        }
        return false;
    }

    private bool OnRemoteCardPlayed(object cookie)
    {
        PlayerTurnAction turn = (PlayerTurnAction)cookie;
        PlayerData turnPlayer = gameData.Players[turn.PlayerIndex];
        PlayerData lastTurnLeader = gameData.TurnLeader;
        turnPlayer.PlayCardFromHand(turn.CardPlayed);

        if (autoPlayCard != null && lastTurnLeader != gameData.TurnLeader)
        {
            autoPlayCard = null;
        }

        gameData.IncrementTurnCounter();
        Debug.Log("Processed player turn: " + turnPlayer.PlayerName + ", new index: " + gameData.CurrentPlayerTurnIndex);
        PlayerData nextPlayer = gameData.Players[gameData.CurrentPlayerTurnIndex];

        if (nextPlayer.PlayerName == 
            gameData.Players[gameData.CurrentLeaderIndex].PlayerName)
        {
            gameScreen.SetHighCard(gameData);
            // Award trick.
            if (localPlayer.IsHost && currentPendingActions.Count == 0)
            {
                Debug.Log("End of turns!");
                // Start next table turn.
                TableTurnEndAction turnEndAction = new TableTurnEndAction();
                turnEndAction.IsEndOfTurn = true;
                Service.WebRequests.SendGameAction(gameData, turnEndAction, 
                    (response) => { });
            }
            return false;
        }

        gameScreen.SyncGameState(gameData, localPlayer, false, autoPlayCard);

        if (nextPlayer == localPlayer && autoPlayCard != null)
        {
            currentSelectedCard = autoPlayCard;
            Service.EventManager.SendEvent(EventId.PlayCardPressed, null);
        }
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
            if (gameData.RoundOver)
            {
                if (localPlayer.IsHost)
                {
                    TableRoundEndAction roundEndAction = new TableRoundEndAction();
                    roundEndAction.IsRoundEnded = true;
                    Service.WebRequests.SendGameAction(gameData, roundEndAction, 
                        (response) => { });
                }
            }
            else
            {
                gameScreen.HideHandresult();
            }
            gameScreen.SyncGameState(gameData, localPlayer, true);
        }, null);

        return false;
    }

    private bool OnRemoteRoundEnded(object cookie)
    {
        for (int i = 0, count = gameData.Players.Count; i < count; ++i)
        {
            PlayerData player = gameData.Players[i];
            player.Tricks[player.Tricks.Count - 1] = player.CurrentTricks;
            player.CurrentTricks = 0;
        }

        bool isGameOver = gameData.CurrentRoundNumber >= 12;
        if (!isGameOver)
        {
            gameData.CurrentRoundNumber++;
        }

        gameScreen.HideHandresult();
        gameScreen.ShowRoundResult(gameData, isGameOver);
        if (localPlayer.IsHost && currentPendingActions.Count == 0)
        {
            if (!isGameOver)
            {
                DealCards();
                Service.WebRequests.SetGameState(gameData, (response) =>
                {
                    TableRoundBeginAction roundBeginAction = new TableRoundBeginAction();
                    roundBeginAction.IsRoundBegun = true;
                    Service.WebRequests.SendGameAction(gameData, 
                        roundBeginAction, (res) => {});
                });
            }
            else
            {
                // Game over!
                gameData.IsFinished = true;
                Service.WebRequests.SetGameState(gameData, (response) =>
                {
                    Debug.Log("Game Over!");
                });
            }
        }
        return false;
    }

    private bool OnGameEnded(object cookie)
    {
        return false;
    }

    private void UpdateServerGameData(object cookie)
    {
        if (!gameData.IsFinished)
        {
            Debug.Log("Host - Syncing current game state to server.");
            Service.WebRequests.SetGameState(gameData, (response) => {});
            Service.TimerManager.CreateTimer(HOST_GAME_STATE_SYNC_TIME, UpdateServerGameData, null);
        }
    }

    public void Unload()
    {
        GameObject.Destroy(gameUi);
    }
}
