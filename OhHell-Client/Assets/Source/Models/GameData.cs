using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    // Room meta properties.
    public string GameName;
    public List<PlayerData> Players;
    public bool IsLaunched;
    public bool IsFinished;
    
    // Game logic.
    public string CurrentPlayerTurn;
    public string CurrentDealer;
    public int NumDealtCards;
    public int CurrentRoundNumber;
    public Card CurrentTrumpCard;
    public CardDeck CurrentDeck;

    public bool GetHasPlayer(string playerName)
    {
        for (int i = 0, count = Players.Count; i < count; ++i)
        {
            if (Players[i].PlayerName == playerName)
            {
                return true;
            }
        }
        return false;
    }

    public PlayerData GetPlayerByName(string name)
    {
        for (int i = 0, count = Players.Count; i < count; ++i)
        {
            PlayerData thisData = Players[i];
            if (thisData.PlayerName == name)
            {
                return thisData;
            }
        }
        return null;
    }
}
