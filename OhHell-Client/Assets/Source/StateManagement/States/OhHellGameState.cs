using Game.Controllers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OhHellLoadParams
{
    public string GameName;

    public OhHellLoadParams(string gameName)
    {
        GameName = gameName;
    }
}

public class OhHellGameState : IStateController
{
    public void Load(Action onLoadedCallback, object passedParams)
    {
        onLoadedCallback();
    }

    public void Start()
    {
        Debug.Log("Game started!");
    }

    public void Unload()
    {

    }
}
