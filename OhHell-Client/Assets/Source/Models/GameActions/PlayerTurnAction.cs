﻿using System;
using UnityEngine;

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

    public void ExecuteAction(Action onDone)
    {
        Service.EventManager.SendEvent(EventId.RemoteCardPlayed, this);
        onDone();
    }

    public void PopulateFromJson(string json)
    {
        PlayerTurnAction parsedAction = JsonUtility.FromJson<PlayerTurnAction>(json);
        PlayerIndex = parsedAction.PlayerIndex;
        CardPlayed = parsedAction.CardPlayed;
    }
}
