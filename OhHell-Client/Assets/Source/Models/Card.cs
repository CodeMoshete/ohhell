using System;

public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

[Serializable]
public class Card
{
    // 0 = Hearts
    // 1 = Diamonds
    // 2 = Clubs
    // 3 = Spades
    public uint IntSuit;
    public CardSuit Suit
    {
        get
        {
            return (CardSuit)IntSuit;
        }
    }
    public uint FaceValue;

    public Card(CardSuit suit, uint faceValue)
    {
        IntSuit = (uint)suit;
        FaceValue = faceValue;
    }

    public Card(uint suit, uint faceValue)
    {
        IntSuit = (uint)suit;
        FaceValue = faceValue;
    }
}
