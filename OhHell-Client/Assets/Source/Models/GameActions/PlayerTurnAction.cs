using System;
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

    }

    public void PopulateFromJson(string json)
    {
        PlayerTurnAction parsedAction = JsonUtility.FromJson<PlayerTurnAction>(json);
        PlayerIndex = parsedAction.PlayerIndex;
        CardPlayed = parsedAction.CardPlayed;
    }
}
