using System;
using UnityEngine;

public class PlayerBidAction : IGameAction
{
    public int PlayerIndex;
    public int PlayerBid;

    public string ActionType
    {
        get
        {
            return "PlayerBidAction";
        }
    }

    public bool IsRoundEnded;

    public void ExecuteAction(Action onDone)
    {
        Service.EventManager.SendEvent(EventId.RemoteBidPlaced, this);
        onDone();
    }

    public void PopulateFromJson(string json)
    {
        PlayerBidAction parsedAction = JsonUtility.FromJson<PlayerBidAction>(json);
        PlayerIndex = parsedAction.PlayerIndex;
        PlayerBid = parsedAction.PlayerBid;
    }
}
