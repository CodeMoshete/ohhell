﻿using System;

[Serializable]
public class PlayerTurnAction : IGameAction
{
    public int PlayerIndex;
    public Card CardPlayed;

    public string ActionType
    {
        get
        {
            return "PlayerTurnAction";
        }
    }

    public bool IsRoundEnded;

    public void ExecuteAction()
    {

    }
}
