﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestService
{
    private Dictionary<UnityWebRequest, Action<string>> activeRequests;
    private Dictionary<UnityWebRequest, Action<string>> newRequests;
    private List<UnityWebRequest> cleanupRequests;

    public WebRequestService()
    {
        cleanupRequests = new List<UnityWebRequest>();
        activeRequests = new Dictionary<UnityWebRequest, Action<string>>();
        newRequests = new Dictionary<UnityWebRequest, Action<string>>();
        Service.UpdateManager.AddObserver(OnUpdate, true);
    }

    private void OnUpdate(float dt)
    {
        foreach (KeyValuePair<UnityWebRequest, Action<string>> pair in newRequests)
        {
            activeRequests.Add(pair.Key, pair.Value);
        }
        newRequests.Clear();

        foreach (KeyValuePair<UnityWebRequest, Action<string>> pair in activeRequests)
        {
            if (pair.Key.isDone)
            {
                Debug.Log("Request done: " + pair.Key.url);
                pair.Value(pair.Key.downloadHandler.text);
                cleanupRequests.Add(pair.Key);
            }
            else if (pair.Key.isNetworkError)
            {
                Debug.LogError(pair.Key.error);
                cleanupRequests.Add(pair.Key);
            }
        }

        int numCleanupRequests = cleanupRequests.Count;
        for (int i = 0; i < numCleanupRequests; ++i)
        {
            activeRequests.Remove(cleanupRequests[i]);
        }

        if (numCleanupRequests > 0)
        {
            cleanupRequests.Clear();
        }
    }

    public void GetGamesList(Action<string> onFinished)
    {
        UnityWebRequest gamesListRequest = UnityWebRequest.Get("localhost:8082/game/getGamesList");
        gamesListRequest.SendWebRequest();
        newRequests.Add(gamesListRequest, onFinished);
    }

    public void GetGameState(GameData game, Action<string> onFinished)
    {
        string url = string.Format("localhost:8082/game/getGameState?gameName={0}", game.GameName);
        UnityWebRequest gamesListRequest = UnityWebRequest.Get(url);
        gamesListRequest.SendWebRequest();
        newRequests.Add(gamesListRequest, onFinished);
    }

    public void SetGameState(GameData game, Action<string> onFinished)
    {
        UnityWebRequest setStateRequest = new UnityWebRequest("localhost:8082/game/setGameState");
        string messageBody = JsonUtility.ToJson(game);
        setStateRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(messageBody));
        setStateRequest.downloadHandler = new DownloadHandlerBuffer();
        setStateRequest.method = UnityWebRequest.kHttpVerbPOST;
        setStateRequest.SetRequestHeader("Content-Type", "application/json");
#if UNITY_EDITOR
        setStateRequest.SetRequestHeader("Accept", "*/*");
        setStateRequest.SetRequestHeader("Accept-Encoding", "gzip, deflate");
        setStateRequest.SetRequestHeader("User-Agent", "runscope/0.1");
#endif
        setStateRequest.SendWebRequest();
        newRequests.Add(setStateRequest, onFinished);
    }

    public void SendGameAction(IGameAction gameAction, Action<string> onFinished)
    {
        UnityWebRequest setStateRequest = new UnityWebRequest("localhost:8082/game/addGameAction");
        string messageBody = JsonUtility.ToJson(gameAction);
        Debug.Log("GAME ACTION BODY: " + messageBody);
        setStateRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(messageBody));
        setStateRequest.downloadHandler = new DownloadHandlerBuffer();
        setStateRequest.method = UnityWebRequest.kHttpVerbPOST;
        setStateRequest.SetRequestHeader("Content-Type", "application/json");
#if UNITY_EDITOR
        setStateRequest.SetRequestHeader("Accept", "*/*");
        setStateRequest.SetRequestHeader("Accept-Encoding", "gzip, deflate");
        setStateRequest.SetRequestHeader("User-Agent", "runscope/0.1");
#endif
        setStateRequest.SendWebRequest();
        newRequests.Add(setStateRequest, onFinished);
    }

    public void GetGameActions(GameData game, int currentActionIndex, Action<string> onFinished)
    {
        string url = string.Format("localhost:8082/game/getGameActions?gameName={0}&actionIndex={1}", game.GameName, currentActionIndex);
        UnityWebRequest gamesListRequest = UnityWebRequest.Get(url);
        gamesListRequest.SendWebRequest();
        newRequests.Add(gamesListRequest, onFinished);
    }
}