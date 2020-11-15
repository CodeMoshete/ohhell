using UnityEngine;
using UnityEngine.UI;

public class PlayerNameItem : MonoBehaviour
{
    public Text NameField;
    public Text TrickField;

    public void SetName(string name)
    {
        NameField.text = name;
    }

    public void SetTurnHighlight(bool isTurn)
    {
        NameField.color = isTurn ? Color.red : Color.black;
    }

    public void SetNumTricks(PlayerData playerData)
    {
        if (playerData.CurrentBid >= 0)
        {
            TrickField.text = "Tricks: " + playerData.CurrentTricks + "/" + playerData.CurrentBid;
        }
        else
        {
            TrickField.text = string.Empty;
        }
    }
}
