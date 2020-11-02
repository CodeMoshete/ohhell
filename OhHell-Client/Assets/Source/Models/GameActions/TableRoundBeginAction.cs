using System;
using UnityEngine;

public class TableRoundBeginAction : IGameAction
{
    public string ActionType
    {
        get
        {
            return "TableRoundBeginAction";
        }
    }

    public bool IsRoundBegun;

    public void ExecuteAction(Action onDone)
    {
        Service.EventManager.SendEvent(EventId.RoundBegun, null);
    }

    public void PopulateFromJson(string json)
    {
        TableRoundBeginAction parsedAction = JsonUtility.FromJson<TableRoundBeginAction>(json);
        IsRoundBegun = parsedAction.IsRoundBegun;
    }
}
