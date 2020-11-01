using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameActionRecord
{
    public int ActionIndex;
    public List<string> ActionTypes;
    public List<string> ActionDatas;

    public GameActionRecord()
    {
        ActionTypes = new List<string>();
        ActionDatas = new List<string>();
    }

    public void AddAction(IGameAction action)
    {
        ActionTypes.Add(action.ActionType);
        ActionDatas.Add(JsonUtility.ToJson(action));
    }

    public List<IGameAction> GetGameActionsFromRecord()
    {
        List<IGameAction> gameActions = new List<IGameAction>();

        for (int i = 0, count = ActionTypes.Count; i < count; ++i)
        {
            Type actionType = Type.GetType(ActionTypes[i]);
            IGameAction gameAction = (IGameAction)Activator.CreateInstance(actionType);
            gameAction.PopulateFromJson(ActionDatas[i]);
        }

        return gameActions;
    }
}
