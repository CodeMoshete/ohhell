using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultPopup : MonoBehaviour
{
    public Text RoundNameText;
    public List<Text> LeaderNames;
    public List<Text> LeaderScores;
    public Button ContinueButton;
    public Button ScoreSheetButton;

    private Action onScreenClosed;

    private void Start()
    {
        ContinueButton.onClick.AddListener(HideRoundResult);
        ScoreSheetButton.onClick.AddListener(() => 
        {
            Service.EventManager.SendEvent(EventId.OnShowScoresClicked, null);
        });
    }

    public void ShowRoundResult(GameData gameData)
    {
        RoundNameText.text = string.Format("Round {0} Over!", gameData.CurrentRoundNumber);
        List<PlayerData> leaderboard = gameData.Leaderboard;
        int leaderCount = Mathf.Min(3, leaderboard.Count);
        for (int i = 0; i < 3; ++i)
        {
            if (i < leaderCount)
            {
                LeaderNames[i].text = leaderboard[i].PlayerName;
                LeaderScores[i].text = leaderboard[i].TotalScore.ToString();
            }
            else
            {
                LeaderNames[i].text = string.Empty;
                LeaderScores[i].text = string.Empty;
            }
        }
        ContinueButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void ShowContinueButton(Action onClosed = null)
    {
        onScreenClosed = onClosed;
        ContinueButton.gameObject.SetActive(true);
    }

    public void HideRoundResult()
    {
        gameObject.SetActive(false);
        if (onScreenClosed != null)
        {
            onScreenClosed();
        }
    }
}
