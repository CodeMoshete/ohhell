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

    }

    public void PopulateFromJson(string json)
    {
        TableTurnEndAction parsedAction = JsonUtility.FromJson<TableTurnEndAction>(json);
        IsEndOfTurn = parsedAction.IsEndOfTurn;
    }
}
