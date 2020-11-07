using UnityEngine;
using UnityEngine.UI;

public class HandResultPopup : MonoBehaviour
{
    public Text ResultText;

    public void ShowHandResult(GameData gameData)
    {
        gameObject.SetActive(true);
        PlayerData winningPlayer = gameData.TurnLeader;
        string resultMessage = string.Format(
            "{0} won the trick with {1}!", 
            winningPlayer.PlayerName, 
            winningPlayer.CurrentRoundCard.ToString());
    }

    public void HideHandResultPopup()
    {
        gameObject.SetActive(false);
    }
}
