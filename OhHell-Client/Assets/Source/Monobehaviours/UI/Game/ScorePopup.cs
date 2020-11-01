﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour
{
    public Transform ScoreContainer;
    public Button CloseButton;
    private List<ScoreColumn> Columns;

    private void Start()
    {
        Columns = new List<ScoreColumn>();
        CloseButton.onClick.AddListener(OnClose);
    }

    public void ShowScores(GameData gameData)
    {
        for (int i = 0, count = gameData.Players.Count; i < count; ++i)
        {
            PlayerData currentPlayer = gameData.Players[i];
            GameObject newColumnObj = 
                GameObject.Instantiate(Resources.Load<GameObject>("Scoring/ScoreContainer"), ScoreContainer);
            ScoreColumn newColumn = newColumnObj.GetComponent<ScoreColumn>();
            newColumn.SetPlayerScore(currentPlayer);
            Columns.Add(newColumn);
        }
        gameObject.SetActive(true);
    }

    public void ClearScores()
    {
        for (int i = 0, count = Columns.Count; i < count; ++i)
        {
            GameObject.Destroy(Columns[i].gameObject);
        }
        Columns.Clear();
    }

    private void OnClose()
    {
        ClearScores();
        gameObject.SetActive(false);
    }
}