using System;
using UnityEngine;

[Serializable]
public class AdjustBidAction : IGameAction
{
    public string ActionType
    {
        get
        {
            return "AdjustBidAction";
        }
    }

    public int PlayerId;
    public int HandNum;
    public int BidNum;

    public void ExecuteAction(Action onDone)
    {
        Service.EventManager.SendEvent(EventId.BidAdjusted, this);
        onDone();
    }

    public void PopulateFromJson(string json)
    {
        AdjustBidAction parsedAction = JsonUtility.FromJson<AdjustBidAction>(json);
        PlayerId = parsedAction.PlayerId;
        HandNum = parsedAction.HandNum;
        BidNum = parsedAction.BidNum;
    }
}
