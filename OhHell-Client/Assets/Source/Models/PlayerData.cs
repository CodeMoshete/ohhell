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
    public bool Advanced;
    public int TotalScore
    {
        get
        {
            int score = 0;
            for (int i = 0, count = Bids.Count; i < count; ++i)
            {
                score = Bids[i] == Tricks[i] ? score + 10 : score;
                score += Tricks[i];
            }
            return score;
        }
    }

    public PlayerData()
    {
        CurrentHand = new List<Card>();
        Bids = new List<int>();
        Tricks = new List<int>();
        Advanced = false;
    }

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
