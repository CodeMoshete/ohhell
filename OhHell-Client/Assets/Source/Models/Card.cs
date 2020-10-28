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

    public uint IntValue;
    public string FaceValue
    {
        get
        {
            switch (IntValue)
            {
                case 0:
                    return "A";
                case 11:
                    return "J";
                case 12:
                    return "Q";
                case 13:
                    return "K";
                default:
                    return (IntValue + 1).ToString();
            }
        }
    }

    public Card(CardSuit suit, uint faceValue)
    {
        IntSuit = (uint)suit;
        IntValue = faceValue;
    }

    public Card(uint suit, uint faceValue)
    {
        IntSuit = (uint)suit;
        IntValue = faceValue;
    }
}
