using System;
using UnityEngine;

[Serializable]
public class TableTurnEndAction : IGameAction
{
    public string ActionType
    {
        get
        {
            return "TableTurnEndAction";
        }
    }

    public bool IsEndOfTurn;

    public void ExecuteAction(Action onDone)
    {
        Service.EventManager.SendEvent(EventId.TurnEnded, null);
    }

    public void PopulateFromJson(string json)
    {
        TableTurnEndAction parsedAction = JsonUtility.FromJson<TableTurnEndAction>(json);
        IsEndOfTurn = parsedAction.IsEndOfTurn;
    }
}
