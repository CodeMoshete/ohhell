using System;

public enum CardSuit
{
    None,
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

[Serializable]
public class Card
{
    public static Card DefaultCard = new Card(CardSuit.None, 0);

    // 0 = None
    // 1 = Hearts
    // 2 = Diamonds
    // 3 = Clubs
    // 4 = Spades
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
                case 10:
                    return "J";
                case 11:
                    return "Q";
                case 12:
                    return "K";
                default:
                    return (IntValue + 1).ToString();
            }
        }
    }

    public bool InitializedCard;

    public Card(CardSuit suit, uint faceValue)
    {
        IntSuit = (uint)suit;
        IntValue = faceValue;
        InitializedCard = true;
    }

    public Card(uint suit, uint faceValue)
    {
        IntSuit = (uint)suit;
        IntValue = faceValue;
        InitializedCard = true;
    }

    public override string ToString()
    {
        return string.Format("{0} of {1}", FaceValue, Suit.ToString());
    }
}
