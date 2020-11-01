using System;
using UnityEngine;

[Serializable]
public class TableRoundEndAction : IGameAction
{
    public string ActionType
    {
        get
        {
            return "TableRoundEndAction";
        }
    }

    public bool IsRoundEnded;

    public void ExecuteAction(Action onDone)
    {

    }

    public void PopulateFromJson(string json)
    {
        TableRoundEndAction parsedAction = JsonUtility.FromJson<TableRoundEndAction>(json);
        IsRoundEnded = parsedAction.IsRoundEnded;
    }
}
