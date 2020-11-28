using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameItem : MonoBehaviour
{
    public Text NameField;
    public Text TrickField;
    public Button JoinButton;

    private Action<string> onJoinAsPlayer;

    public void SetName(string name, Action<string> onJoinAsPlayer)
    {
        SetName(name);
        this.onJoinAsPlayer = onJoinAsPlayer;
        JoinButton.onClick.AddListener(OnJoinAsPlayer);
        JoinButton.gameObject.SetActive(true);
    }

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

    private void OnJoinAsPlayer()
    {
        if (onJoinAsPlayer != null)
        {
            onJoinAsPlayer(NameField.text);
        }
    }
}
