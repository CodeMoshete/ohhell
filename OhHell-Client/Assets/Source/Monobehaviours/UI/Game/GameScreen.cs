﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public Text RoundText;
    public Transform PlayerListContainer;
    public Transform LastPlayedCardContainer;
    public Text LedSuitField;
    public Text HighCardPlayerName;
    public Transform HighCardContainer;
    public Transform TrumpCardContainer;
    public Text YourBid;
    public Text YourTricks;
    public Transform YourHandContainer;
    public Button ScoreSheetButton;
    public Button PlayCardButton;
    public Button OptionsButton;
    public GameObject YourTurnNotif;
    public GameObject TurnProcessingNotif;
    public GameObject CardNotificationContainer;
    public Text CardNotificationText;
    public AudioSource TurnAudioObject;

    public BidPlacerPopup BidPopup;
    public HandResultPopup HandResultPopup;
    public RoundResultPopup RoundResultPopup;
    public ScorePopup ScorePopup;
    public OptionsPopup OptionsPopup;

    private List<CardView> playerHand;
    private List<PlayerNameItem> playerList;
    private CardView highCard;
    private CardView lastCard;
    private CardView trumpCard;

    public GameScreen()
    {
        playerHand = new List<CardView>();
        playerList = new List<PlayerNameItem>();
    }

    private void Start()
    {
        OptionsButton.onClick.AddListener(ShowOptionsPopup);
        PlayCardButton.onClick.AddListener(PlayCardPressed);
        ScoreSheetButton.onClick.AddListener(() =>
        {
            Service.EventManager.SendEvent(EventId.OnShowScoresClicked, null);
        });

        Service.EventManager.AddListener(EventId.ShowCardNotification, OnCardNotification);
    }

    private bool OnCardNotification(object cookie)
    {
        CardNotificationContainer.SetActive(true);
        CardNotificationText.text = (string)cookie;
        Service.TimerManager.CreateTimer(5f, (c) =>
        {
            CardNotificationContainer.SetActive(false);
        }, null);
        return true;
    }

    private void PlayCardPressed()
    {
        Service.EventManager.SendEvent(EventId.PlayCardPressed, null);
    }

    private void ShowOptionsPopup()
    {
        OptionsPopup.ShowPopup();
    }

    public void DisableHand()
    {
        for (int i = 0, count = playerHand.Count; i < count; ++i)
        {
            playerHand[i].SetEnabled(false);
        }
    }

    public void CardPlayed()
    {
        TurnProcessingNotif.SetActive(true);
        PlayCardButton.gameObject.SetActive(false);
    }

    public void SyncGameState(GameData gameState, PlayerData localPlayer, bool firstTurn = false, Card autoPlayCard = null)
    {
        RoundText.text = string.Format("Round {0}/13", gameState.CurrentRoundNumber + 1);
        RefreshPlayerList(gameState);
        SetHighCard(gameState);
        SetTrumpCard(gameState);
        bool localPlayersTurn = gameState.Players[gameState.CurrentPlayerTurnIndex].PlayerName == localPlayer.PlayerName;
        bool allowAutoPlay = !(localPlayersTurn || firstTurn) && gameState.IsPlayersTurnStillComing(localPlayer);
        SetPlayerHand(localPlayer.CurrentHand, allowAutoPlay, autoPlayCard, gameState, localPlayer);
        YourBid.text = localPlayer.CurrentBid.ToString();
        YourTricks.text = localPlayer.CurrentTricks.ToString();
        Card ledCard = gameState.Players[gameState.CurrentLeaderIndex].CurrentRoundCard;

        LedSuitField.text = 
            ledCard.Suit != CardSuit.None ? 
            string.Format("Led Suit: {0}", ledCard.Suit.ToString()) : 
            string.Empty;

        if (!Service.LocalPreferences.DisableTurnNotification)
        {
            TurnAudioObject.volume = 0.025f;
            TurnAudioObject.gameObject.SetActive(localPlayersTurn && gameState.IsLaunched);
        }

        YourTurnNotif.gameObject.SetActive(localPlayersTurn && gameState.IsLaunched);
        PlayCardButton.gameObject.SetActive(localPlayersTurn && gameState.IsLaunched);

        TurnProcessingNotif.SetActive(false);
    }

    public void SetPlayerHand(List<Card> hand, bool allowAutoPlay, Card autoPlayCard, GameData gameState, PlayerData localPlayer)
    {
        for (int i = 0, count = playerHand.Count; i < count; ++i)
        {
            GameObject.Destroy(playerHand[i].gameObject);
        }
        playerHand.Clear();

        for (int i = 0, count = hand.Count; i < count; ++i)
        {
            Card card = hand[i];
            bool allowAutoPlayCard = allowAutoPlay && gameState.IsCardValid(card, localPlayer, true);
            playerHand.Add(CardView.CreateFromModel(card, YourHandContainer, true, allowAutoPlayCard, autoPlayCard));
        }
    }

    public void SetHighCard(GameData gameState)
    {
        if (highCard != null)
        {
            GameObject.Destroy(highCard.gameObject);
            highCard = null;
            HighCardPlayerName.text = string.Empty;
        }

        PlayerData turnLeader = gameState.TurnLeader;
        if (turnLeader != null)
        { 
            highCard = CardView.CreateFromModel(turnLeader.CurrentRoundCard, HighCardContainer);
            HighCardPlayerName.text = turnLeader.PlayerName;
        }

        if (lastCard != null)
        {
            GameObject.Destroy(lastCard.gameObject);
            lastCard = null;
        }

        PlayerData lastPlayer = gameState.LastPlayer;
        if (lastPlayer != null)
        {
            lastCard = CardView.CreateFromModel(lastPlayer.CurrentRoundCard, LastPlayedCardContainer);
        }
    }

    private void SetTrumpCard(GameData gameState)
    {
        if (trumpCard != null)
        {
            GameObject.Destroy(trumpCard.gameObject);
            trumpCard = null;
        }

        Debug.Log("Trump card set to " + gameState.CurrentTrumpCard.ToString());
        trumpCard = CardView.CreateFromModel(gameState.CurrentTrumpCard, TrumpCardContainer);
    }

    private void RefreshPlayerList(GameData gameState)
    {
        for (int i = 0, count = playerList.Count; i < count; ++i)
        {
            GameObject.Destroy(playerList[i].gameObject);
        }
        playerList.Clear();

        for (int i = 0, count = gameState.Players.Count; i < count; ++i)
        {
            PlayerData player = gameState.Players[i];
            GameObject playerEntry = GameObject.Instantiate(
                Resources.Load<GameObject>("LobbyNameEntry"),
                PlayerListContainer);
            PlayerNameItem nameItem = playerEntry.GetComponent<PlayerNameItem>();
            nameItem.SetName(player.PlayerName);
            nameItem.SetNumTricks(player);
            nameItem.SetTurnHighlight(gameState.CurrentPlayerTurnIndex == gameState.Players.IndexOf(player));
            nameItem.SetLeaderHighlight(gameState.CurrentLeaderIndex == gameState.Players.IndexOf(player));
            playerList.Add(nameItem);
        }
    }

    public void ShowScoreSheet(GameData gameData)
    {
        ScorePopup.ShowScores(gameData);
    }

    public void BeginBidding(GameData gameData, PlayerData localPlayer)
    {
        if (RoundResultPopup.gameObject.activeSelf)
        {
            AllowRoundResultContinue(() =>
            {
                BidPopup.ShowBidPopup(gameData, localPlayer);
            });
            return;
        }
        BidPopup.ShowBidPopup(gameData, localPlayer);
    }

    public void EndBidding()
    {
        BidPopup.HideBidPlacerPopup();
    }

    public void ShowHandResult(GameData gameData)
    {
        HandResultPopup.ShowHandResult(gameData);
    }

    public void HideHandresult()
    {
        HandResultPopup.HideHandResultPopup();
    }

    public void ShowRoundResult(GameData gameData, bool isGameOver)
    {
        RoundResultPopup.ShowRoundResult(gameData, isGameOver);
    }

    public void AllowRoundResultContinue(Action onClosed = null)
    {
        RoundResultPopup.ShowContinueButton(onClosed);
    }
}
