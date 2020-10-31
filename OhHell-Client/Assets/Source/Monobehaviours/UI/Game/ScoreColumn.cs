using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreColumn : MonoBehaviour
{
    public Text PlayerName;
    private List<ScoreCell> Rounds;

    private void Start()
    {
        Rounds = new List<ScoreCell>();
    }

    public void SetPlayerScore(PlayerData player)
    {
        PlayerName.text = player.PlayerName;
        int cumulativeScore = 0;
        for (int i = 0, count = player.Bids.Count; i < count; ++i)
        {
            GameObject newCellObj =
                GameObject.Instantiate(Resources.Load<GameObject>("Scoring/ScoreField"), transform);
            ScoreCell newCell = newCellObj.GetComponent<ScoreCell>();
            int roundNum = i + 1;
            int roundBid = player.Bids[i];
            int roundTricks = player.Tricks[i];
            int points = roundBid == roundTricks ? 10 + roundTricks : roundTricks;
            cumulativeScore += points;
            newCell.SetCellContents(roundNum, roundBid, roundTricks, points, cumulativeScore);
        }
    }
}
