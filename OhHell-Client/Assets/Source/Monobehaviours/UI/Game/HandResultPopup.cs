using UnityEngine;
using UnityEngine.UI;

public class HandResultPopup : MonoBehaviour
{
    public Text ResultText;

    public void ShowHandResult(GameData gameData, float displayTime = 0f)
    {
        Debug.Log("[POPUP] Show hand result");
        gameObject.SetActive(true);
        PlayerData winningPlayer = gameData.TurnLeader;
        string resultMessage = string.Format(
            "{0} won the trick with {1}!", 
            winningPlayer.PlayerName, 
            winningPlayer.CurrentRoundCard.ToString());

        if (displayTime > 0f)
        {
            Service.TimerManager.CreateTimer(displayTime, (cookie) => { HideHandResultPopup(); }, null);
        }
    }

    public void HideHandResultPopup()
    {
        gameObject.SetActive(false);
    }
}
