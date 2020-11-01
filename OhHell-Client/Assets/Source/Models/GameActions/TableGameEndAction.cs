using System;
using UnityEngine;

[Serializable]
public class TableGameEndAction : IGameAction
{
    public string ActionType
    {
        get
        {
            return "TableGameEndAction";
        }
    }

    public bool IsEndOfGame;

    public void ExecuteAction(Action onDone)
    {

    }

    public void PopulateFromJson(string json)
    {
        TableGameEndAction parsedAction = JsonUtility.FromJson<TableGameEndAction>(json);
        IsEndOfGame = parsedAction.IsEndOfGame;
    }
}
