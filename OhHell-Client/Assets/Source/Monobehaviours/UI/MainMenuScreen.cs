using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Transform GameListPanel;
    public InputField NameField;
    public InputField NewGameNameField;
    public Button NewGameButton;
    public Button RefreshButton;

    private LobbyData currentLobbyData;
    private Action<string, string> onJoinGame;

    public void Initialize(LobbyData lobbyData, Action<string, string> onJoinGamePressed)
    {
        currentLobbyData = lobbyData;
        onJoinGame = onJoinGamePressed;

        for (int i = 0, numGames = currentLobbyData.ActiveGames.Count; i < numGames; ++i)
        {
            GameData gameData = currentLobbyData.ActiveGames[i];
            if (!gameData.IsLaunched || gameData.IsLaunched && gameData.GetHasPlayer(NameField.text))
            {
                GameObject gameListItemObj = GameObject.Instantiate(Resources.Load<GameObject>("GameListItem"));
                GameListItem listItem = gameListItemObj.GetComponent<GameListItem>();
                listItem.Initialize(gameData, OnJoinGamePressed);
            }
        }
    }

    private void OnJoinGamePressed(GameData data)
    {
        onJoinGame(data.GameName, NameField.text);
    }
}
