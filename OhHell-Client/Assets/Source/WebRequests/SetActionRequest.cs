using System;
using UnityEngine;

[Serializable]
public class SetActionRequest
{
    public string GameName;
    public string ActionType;
    public string ActionData;

    public SetActionRequest(IGameAction action, string gameName)
    {
        ActionType = action.ActionType;
        ActionData = JsonUtility.ToJson(action);
        GameName = gameName;
    }
}
