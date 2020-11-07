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

    public bool AllBidsPlaced
    {
        get
        {
            for (int i = 0, count = Players.Count; i < count; ++i)
            {
                if (Players[i].CurrentBid < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool RoundOver
    {
        get
        {
            return Players[0].CurrentHand.Count == 0;
        }
    }

    public List<PlayerData> Leaderboard
    {
        get
        {
            List<PlayerData> orderedPlayers = new List<PlayerData>();
            orderedPlayers.AddRange(Players);
            orderedPlayers.Sort((p1, p2) => 
            { 
                if (p1.TotalScore > p2.TotalScore)
                {
                    return 1;
                }
                else if (p1.TotalScore < p2.TotalScore)
                {
                    return -1;
                }
                return 0;
            });
            return orderedPlayers;
        }
    }

    public PlayerData LastPlayer
    {
        get
        {
            PlayerData turnLeader = TurnLeader;
            if (CurrentLedCard != null)
            {
                int playerIndex = CurrentPlayerTurnIndex > 0 ? CurrentPlayerTurnIndex - 1 : Players.Count - 1;
                while (playerIndex != CurrentPlayerTurnIndex)
                {
                    PlayerData testPlayer = Players[playerIndex];
                    if (testPlayer != turnLeader && testPlayer.CurrentRoundCard != null)
                    {
                        return testPlayer;
                    }
                    playerIndex = playerIndex > 0 ? playerIndex - 1 : Players.Count - 1;
                }
            }
            return null;
        }
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
