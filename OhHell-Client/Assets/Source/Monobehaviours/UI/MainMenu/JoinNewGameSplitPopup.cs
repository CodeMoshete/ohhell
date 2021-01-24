using System;
using UnityEngine;
using UnityEngine.UI;

public class JoinNewGameSplitPopup : MonoBehaviour
{
    public Button JoinAsNewButton;
    public Button JoinTeamButton;
    public Button CancelButton;

    private Action<string> onShowJoinAsNew;
    private Action<GameDataSimple, bool> onShowJoinTeam;
    private GameDataSimple data;

    private void Start()
    {
        JoinAsNewButton.onClick.AddListener(OnJoinAsNewButtonClicked);
        JoinTeamButton.onClick.AddListener(OnJoinTeamButtonClicked);
        CancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    public void ShowPopup(
        GameDataSimple gameData, 
        Action<string> onJoinAsNew,
        Action<GameDataSimple, bool> onJoinExisting)
    {
        data = gameData;
        onShowJoinTeam = onJoinExisting;
        onShowJoinAsNew = onJoinAsNew;
        gameObject.SetActive(true);
    }

    private void OnJoinAsNewButtonClicked()
    {
        gameObject.SetActive(false);
        onShowJoinAsNew(data.gameName);
    }

    private void OnJoinTeamButtonClicked()
    {
        gameObject.SetActive(false);
        onShowJoinTeam(data, true);
    }

    private void OnCancelButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
