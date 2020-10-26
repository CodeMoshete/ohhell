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
            MainMenuLoadParams loadParams = new MainMenuLoadParams(JoinGame);
			sceneFactory.LoadScene<MainMenuState>(OnSceneLoaded, loadParams);
		}

        public void JoinGame(GameData gameData)
        {
			OhHellLoadParams loadParams = new OhHellLoadParams(gameData);
			sceneFactory.LoadScene<OhHellGameState>(OnSceneLoaded, loadParams);
        }

		public void OnSceneLoaded()
		{
			// Intentionally empty for now...
		}
	}
}

