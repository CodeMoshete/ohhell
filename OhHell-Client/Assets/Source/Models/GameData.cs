using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    // Room meta properties.
    public string GameName;
    public List<PlayerData> Players;
    public int CurrentActionIndex;
    public bool IsLaunched;
    public bool IsFinished;
    
    // Game logic.
    public int CurrentPlayerTurnIndex;
    public int CurrentDealerIndex;
    public int CurrentLeaderIndex;
    public int NumDealtCards;
    public int CurrentRoundNumber;
    public Card CurrentTrumpCard;

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
            return Math.Abs(CurrentRoundNumber - 6) + 1;
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

    public void ClearTable()
    {
        for (int i = 0, count = Players.Count; i < count; ++i)
        {
            PlayerData currentPlayer = Players[i];
            currentPlayer.CurrentRoundCard = null;
        }
    }

    public bool AllBidsPlaced
    {
        get
        {
            for (int i = 0, count = Players.Count; i < count; ++i)
            {
                if (Players[i].CurrentBid < 0)
                {
                    Debug.Log("Player " + Players[i].PlayerName + " has not placed a bid yet!");
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
                    return -1;
                }
                else if (p1.TotalScore < p2.TotalScore)
                {
                    return 1;
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
            PlayerData lastPlayer = null;
            PlayerData turnLeader = TurnLeader;
            int playerIndex = CurrentLeaderIndex;
            if (CurrentLeaderIndex == CurrentPlayerTurnIndex)
            {
                playerIndex = playerIndex == Players.Count - 1 ? 0 : playerIndex + 1;
            }

            while (playerIndex != -1 && playerIndex != CurrentPlayerTurnIndex)
            {
                PlayerData testPlayer = Players[playerIndex];
                if (testPlayer != turnLeader && testPlayer.CurrentRoundCard != null)
                {
                    lastPlayer = testPlayer;
                }
                playerIndex = playerIndex < Players.Count - 1 ? playerIndex + 1 : 0;
            }
            return lastPlayer;
        }
    }

    public bool IsCardValid(Card card, PlayerData localPlayer)
    {
        bool isValid = true;
        PlayerData leadPlayer = Players[CurrentLeaderIndex];
        Card ledCard = Players[CurrentLeaderIndex].CurrentRoundCard;
        bool isFirstTurn = true;
        for (int i = 0, count = Players.Count; i < count; ++i)
        {
            if (Players[i].CurrentTricks > 0)
            {
                isFirstTurn = false;
                break;
            }
        }

        if (ledCard != null)
        {
            // This is not the leading card.
            bool isLedSuit = card.Suit == ledCard.Suit;
            bool playerHasLedSuit = false;
            for (int i = 0, count = localPlayer.CurrentHand.Count; i < count; ++i)
            {
                if (localPlayer.CurrentHand[i].Suit == ledCard.Suit)
                {
                    playerHasLedSuit = true;
                    break;
                }
            }

            if (!isLedSuit && playerHasLedSuit)
            {
                isValid = false;
                string msg = "You must follow the suit that was led.";
                Service.EventManager.SendEvent(EventId.ShowCardNotification, msg);
            }
        }
        else
        {
            // This is the leading card.
            bool isTrumpSuit = card.Suit == CurrentTrumpCard.Suit;
            bool playerHasNonTrump = false;
            for (int i = 0, count = leadPlayer.CurrentHand.Count; i < count; ++i)
            {
                if (leadPlayer.CurrentHand[i].Suit != CurrentTrumpCard.Suit)
                {
                    playerHasNonTrump = true;
                    break;
                }
            }

            if (isTrumpSuit && playerHasNonTrump && isFirstTurn)
            {
                isValid = false;
                string msg = "You cannot lead trump on the first turn of a round.";
                Service.EventManager.SendEvent(EventId.ShowCardNotification, msg);
            }
        }

        return isValid;
    }

    public PlayerData TurnLeader
    {
        get
        {
            PlayerData currentPlayerLeader = null;
            Card highCard = null;
            int playerIndex = CurrentLeaderIndex;
            for (int i = 0, count = Players.Count; i < count; ++i)
            {
                PlayerData player = Players[playerIndex];
                playerIndex = playerIndex >= Players.Count - 1 ? 0 : playerIndex + 1;

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
                            if (highCard.IntValue != 0 && currentPlayerCard.IntValue > highCard.IntValue)
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
