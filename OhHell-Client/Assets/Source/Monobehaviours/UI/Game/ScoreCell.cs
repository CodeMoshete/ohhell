using UnityEngine;
using UnityEngine.UI;

public class ScoreCell : MonoBehaviour
{
    public Text RoundLabel;
    public Text BidLabel;
    public Text TricksLabel;
    public Text PointsLabel;
    public Text ScoreLabel;

    public void SetCellContents(int round, int bid, int tricks, int points, int score)
    {
        RoundLabel.text = string.Format("Round {0}", round);
        BidLabel.text = string.Format("Bid\n{0}", bid);
        TricksLabel.text = string.Format("Tricks\n{0}", tricks);
        PointsLabel.text = string.Format("Points\n{0}", points);
        ScoreLabel.text = string.Format("Score\n{0}", score);
    }
}
