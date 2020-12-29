using System;
using UnityEngine;

[Serializable]
public class SetActionRequest
{
    public string GameName;
    public string ActionType;
    public string ActionData;
    public int EnforceIndex;

    public SetActionRequest(IGameAction action, string gameName, int enforceIndex)
    {
        ActionType = action.ActionType;
        ActionData = JsonUtility.ToJson(action);
        GameName = gameName;
        EnforceIndex = enforceIndex;
    }
}
