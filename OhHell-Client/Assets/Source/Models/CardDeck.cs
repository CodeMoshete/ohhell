using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
    private const int NUM_SUITS = 4;
    private const int NUM_CARDS = 13;
    public List<Card> AvailableCards { get; private set; }
    public List<Card> DealtCards { get; private set; }


    public CardDeck(uint numDecks = 1)
    {
        DealtCards = new List<Card>();
        AvailableCards = new List<Card>();
        for (uint k = 0; k < numDecks; ++k)
        {
            for (uint i = 1; i < NUM_SUITS + 1; ++i)
            {
                for (uint j = 0; j < NUM_CARDS; ++j)
                {
                    Card newCard = new Card(i, j);
                    AvailableCards.Add(newCard);
                }
            }
        }
    }

    public void Shuffle(bool returnDealtCards = false)
    {
        if (returnDealtCards)
        {
            AvailableCards.AddRange(DealtCards);
        }

        int numCardsLeft = AvailableCards.Count;
        int maxCardIndex = numCardsLeft - 1;
        for (int i = 0; i < numCardsLeft; ++i)
        {
            Card thisCard = AvailableCards[i];
            int randomCardIndex = Random.Range(0, maxCardIndex);
            AvailableCards[i] = AvailableCards[randomCardIndex];
            AvailableCards[randomCardIndex] = thisCard;
        }
    }

    public Card DealCard()
    {
        Card dealtCard = null;
        if (AvailableCards.Count > 0)
        {
            int topIndex = AvailableCards.Count - 1;
            dealtCard = AvailableCards[topIndex];
            AvailableCards.RemoveAt(topIndex);
        }
        return dealtCard;
    }
}
