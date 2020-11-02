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
    public int CurrentPlayerTurnIndex;
    public int CurrentDealerIndex;
    public int CurrentLeaderIndex;
    public int NumDealtCards;
    public int CurrentRoundNumber;
    public Card CurrentTrumpCard;
    public Card CurrentLedCard;

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

    public int NumCardsToDeal
    {
        get
        {
            return Math.Abs(CurrentRoundNumber - 7) + 1;
        }
    }

    public void IncrementTurnCounter()
    {
        CurrentPlayerTurnIndex = (CurrentPlayerTurnIndex == Players.Count - 1) ? 0 : CurrentPlayerTurnIndex + 1;
    }

    public void IncrementDealerIndex()
    {
        CurrentDealerIndex = (CurrentDealerIndex == Players.Count - 1) ? 0 : CurrentDealerIndex + 1;
    }

    public PlayerData TurnLeader
    {
        get
        {
            if (CurrentLedCard != null && !CurrentLedCard.InitializedCard)
            {
                CurrentLedCard = null;
            }

            PlayerData currentPlayerLeader = null;
            Card highCard = CurrentLedCard;
            for (int i = 0, count = Players.Count; i < count; ++i)
            {
                PlayerData player = Players[i];
                if (highCard == null)
                {
                    // First card compared.
                    if (player.CurrentRoundCard != null)
                    {
                        highCard = player.CurrentRoundCard;
                        currentPlayerLeader = player;
                    }
                }
                else
                {
                    if (player.CurrentRoundCard != null)
                    {
                        Card currentPlayerCard = player.CurrentRoundCard;
                        // Ace of trump.
                        if (currentPlayerCard.Suit == CurrentTrumpCard.Suit && currentPlayerCard.IntValue == 0)
                        {
                            highCard = currentPlayerCard;
                            currentPlayerLeader = player;
                            break;
                        }

                        // Non-ace trump card played.
                        if (currentPlayerCard.Suit == CurrentTrumpCard.Suit && 
                            highCard.Suit != CurrentTrumpCard.Suit)
                        {
                            highCard = currentPlayerCard;
                            currentPlayerLeader = player;
                            continue;
                        }

                        if (currentPlayerCard.Suit == highCard.Suit)
                        {
                            // Non-trump ace.
                            if (currentPlayerCard.IntValue == 0 && highCard.IntValue != 0)
                            {
                                highCard = currentPlayerCard;
                                currentPlayerLeader = player;
                                continue;
                            }
                            
                            // Non-trump non-ace.
                            if (currentPlayerCard.IntValue > highCard.IntValue)
                            {
                                highCard = currentPlayerCard;
                                currentPlayerLeader = player;
                            }
                        }
                    }
                }
            }
            return currentPlayerLeader;
        }
    }
}
