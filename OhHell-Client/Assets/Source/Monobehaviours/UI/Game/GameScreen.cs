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

    private void Start()
    {
        playerHand = new List<CardView>();
        playerList = new List<PlayerNameItem>();
    }
}
