//FlowController
//Controller class that strings any and all game states together.

using Controllers;
using Game.Factories;

namespace Game.Controllers
{
	public class FlowController
	{
		private StateFactory sceneFactory;

		public FlowController()
		{
			sceneFactory = new StateFactory();
		}

		public void StartGame()
		{
			LoadMainMenu();
		}

		public void LoadMainMenu()
		{
            MainMenuLoadParams loadParams = new MainMenuLoadParams(JoinGame, LaunchGame);
			sceneFactory.LoadScene<MainMenuState>(OnSceneLoaded, loadParams);
		}

        public void JoinGame(GameData gameData, string localPlayerName)
        {
			OhHellLobbyLoadParams loadParams = 
                new OhHellLobbyLoadParams(gameData, LaunchGame, localPlayerName);
			sceneFactory.LoadScene<OhHellLobbyState>(OnSceneLoaded, loadParams);
        }

        public void LaunchGame(GameData gameData, string localPlayerName)
        {
            OhHellGameLoadParams loadParams = 
                new OhHellGameLoadParams(gameData, LoadMainMenu, localPlayerName);
            sceneFactory.LoadScene<OhHellGameState>(OnSceneLoaded, loadParams);
        }

		public void OnSceneLoaded()
		{
			// Intentionally empty for now...
		}
	}
}

