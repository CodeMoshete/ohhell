using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
public class GameData
{
    public string GameName;
    public List<PlayerData> Players;
    public bool IsLaunched;

    public bool GetHasPlayer(string playerName)
    {
        for (int i = 0, count = Players.Count; i < count; ++i)
        {
            if (Players[i].PlayerName == playerName)
            {
                return true;
            }
        }
        return false;
    }
}
