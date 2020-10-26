using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string PlayerName;
    public bool IsHost;
    public Card CurrentRoundCard;
    public List<Card> CurrentHand;
    public int CurrentBid;
    public int CurrentTricks;
    public int TotalScore;
}
