using System;
using UnityEngine;
using UnityEngine.UI;

public class GameListItem : MonoBehaviour
{
    public Text GameNameField;
    public Text NumPlayersField;
    public Button JoinGameButton;

    private GameDataSimple gameData;
    private Action<GameDataSimple> onJoinGamePressed;

    public void Initialize(GameDataSimple targetGameData, Action<GameDataSimple> onJoinGame)
    {
        gameData = targetGameData;
        GameNameField.text = gameData.gameName;
        NumPlayersField.text = gameData.playerCount.ToString();
        onJoinGamePressed = onJoinGame;
        JoinGameButton.onClick.AddListener(JoinGame);
    }

    private void JoinGame()
    {
        onJoinGamePressed(gameData);
    }
}
