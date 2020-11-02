using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public Text RoundText;
    public Transform PlayerListContainer;
    public Text CurrentPlayerName;
    public Transform LastPlayedCardContainer;
    public Text HighCardPlayerName;
    public Transform HighCardContainer;
    public Transform TrumpCardContainer;
    public Text YourBid;
    public Text YourTricks;
    public Transform YourHandContainer;
    public Button ScoreSheetButton;
    public Button PlayCardButton;

    public BidPlacerPopup BidPopup;
    public HandResultPopup HandResultPopup;
    public RoundResultPopup RoundResultPopup;
    public ScorePopup ScorePopup;

    private List<CardView> playerHand;
    private List<PlayerNameItem> playerList;
    private CardView HighCard;
    private CardView TrumpCard;

    public GameScreen()
    {
        playerHand = new List<CardView>();
        playerList = new List<PlayerNameItem>();
    }

    public void SyncGameState(GameData gameState, PlayerData localPlayer)
    {
        RoundText.text = string.Format("Round {0}/13", gameState.CurrentRoundNumber);
        RefreshPlayerList(gameState);
        CurrentPlayerName.text = gameState.Players[gameState.CurrentPlayerTurnIndex].PlayerName;
        SetHighCard(gameState);
        SetTrumpCard(gameState);
        SetPlayerHand(localPlayer.CurrentHand);
        bool localPlayersTurn = gameState.Players[gameState.CurrentPlayerTurnIndex].PlayerName == localPlayer.PlayerName;
        PlayCardButton.gameObject.SetActive(localPlayersTurn);
    }

    public void SetPlayerHand(List<Card> hand)
    {
        for (int i = 0, count = playerHand.Count; i < count; ++i)
        {
            GameObject.Destroy(playerHand[i].gameObject);
        }
        playerHand.Clear();

        for (int i = 0, count = hand.Count; i < count; ++i)
        {
            Card card = hand[i];
            playerHand.Add(CardView.CreateFromModel(card, YourHandContainer));
        }
    }

    private void SetHighCard(GameData gameState)
    {
        if (HighCard != null)
        {
            GameObject.Destroy(HighCard);
            HighCard = null;
        }

        PlayerData turnLeader = gameState.TurnLeader;
        if (turnLeader != null)
        { 
            HighCard = CardView.CreateFromModel(turnLeader.CurrentRoundCard, HighCardContainer);
            HighCardPlayerName.text = turnLeader.PlayerName;
        }
    }

    private void SetTrumpCard(GameData gameState)
    {
        if (TrumpCard != null)
        {
            GameObject.Destroy(TrumpCard);
            TrumpCard = null;
        }
        TrumpCard = CardView.CreateFromModel(gameState.CurrentTrumpCard, HighCardContainer);
    }

    private void RefreshPlayerList(GameData gameState)
    {
        for (int i = 0, count = playerList.Count; i < count; ++i)
        {
            GameObject.Destroy(playerList[i]);
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
            nameItem.SetNumTricks(player.CurrentTricks);
        }
    }
}
