using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string PlayerName;
    public bool IsHost;
    public Card CurrentRoundCard;
    public List<Card> CurrentHand;
    public int CurrentBid;
    public int CurrentTricks;
    public List<int> Bids;
    public List<int> Tricks;
    public int TotalScore;

    public void PlayCardFromHand(Card card)
    {
        int cardIndex = -1;
        for (int i = 0, count = CurrentHand.Count; i < count; ++i)
        {
            Card handCard = CurrentHand[i];
            if (handCard.Suit == card.Suit && handCard.IntValue == card.IntValue)
            {
                cardIndex = i;
            }
        }

        if (cardIndex >= 0)
        {
            CurrentHand.RemoveAt(cardIndex);
        }
        CurrentRoundCard = card;
    }
}
