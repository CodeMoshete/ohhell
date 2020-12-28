using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameItem : MonoBehaviour
{
    private const float TRICKS_TEXT_Y = -24.4f;
    private const float TRICKS_TEXT_Y_SHIFT = -17.65f;

    public Text NameField;
    public Text TrickField;
    public Button JoinButton;
    public GameObject LeaderNotification;

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

    public void SetLeaderHighlight(bool isLeader)
    {
        LeaderNotification.SetActive(isLeader);
        Vector3 trickFieldPos = TrickField.rectTransform.anchoredPosition;
        trickFieldPos.y = isLeader ? TRICKS_TEXT_Y_SHIFT : TRICKS_TEXT_Y;
        TrickField.rectTransform.anchoredPosition = trickFieldPos;
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
