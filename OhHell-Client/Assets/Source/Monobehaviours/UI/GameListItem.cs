using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameListItem : MonoBehaviour
{
    public Text GameNameField;
    public Text NumPlayersField;
    public Button JoinGameButton;

    private GameData gameData;
    private Action<GameData> onJoinGamePressed;

    public void Initialize(GameData targetGameData, Action<GameData> onJoinGame)
    {
        gameData = targetGameData;
        GameNameField.text = gameData.GameName;
        NumPlayersField.text = gameData.Players.Count.ToString();
        onJoinGamePressed = onJoinGame;
        JoinGameButton.onClick.AddListener(JoinGame);
    }

    private void JoinGame()
    {
        onJoinGamePressed(gameData);
    }
}
