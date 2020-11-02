using System;
using System.Collections.Generic;

[Serializable]
public class GetActionsResponse
{
    public int ActionIndex;
    public List<string> ActionTypes;
    public List<string> ActionDatas;

    public List<IGameAction> GetGameActionsFromRecord()
    {
        List<IGameAction> gameActions = new List<IGameAction>();

        for (int i = 0, count = ActionTypes.Count; i < count; ++i)
        {
            Type actionType = Type.GetType(ActionTypes[i]);
            IGameAction gameAction = (IGameAction)Activator.CreateInstance(actionType);
            gameAction.PopulateFromJson(ActionDatas[i]);
            gameActions.Add(gameAction);
        }

        return gameActions;
    }
}
