using System;
using UnityEngine;
using UnityEngine.UI;

public class GameListItem : MonoBehaviour
{
    public Text GameNameField;
    public Text NumPlayersField;
    public Button JoinGameButton;
    public GameObject WaitingForPlayersLabel;
    public GameObject InProgressLabel;

    private GameDataSimple gameData;
    private Action<GameDataSimple> onJoinGamePressed;

    public void Initialize(GameDataSimple targetGameData, Action<GameDataSimple> onJoinGame)
    {
        gameData = targetGameData;
        GameNameField.text = gameData.gameName;
        NumPlayersField.text = gameData.playerCount.ToString();
        onJoinGamePressed = onJoinGame;
        JoinGameButton.onClick.AddListener(JoinGame);
        WaitingForPlayersLabel.SetActive(!targetGameData.isLaunched);
        InProgressLabel.SetActive(targetGameData.isLaunched);
    }

    private void JoinGame()
    {
        onJoinGamePressed(gameData);
    }
}
