// StateFactory.cs
// Controller class for Loading and Unloading game scenes

using Game.Controllers.Interfaces;
using System;
using UnityEngine;

namespace Game.Factories
{
    public class StateFactory
	{
		private IStateController newScene;
		private IStateController currentScene;

		private Action onNewSceneLoaded;

		private bool isTransitionDone;
		private bool isSceneLoaded;

		private TransitionScreen m_transitionScreen;

		public StateFactory()
		{
            //Don't show transition screen if it's our first load.
            isTransitionDone = true;
			isSceneLoaded = false;
			m_transitionScreen = GameObject.Find("TransitionScreen").GetComponent<TransitionScreen>();
        }

		public void LoadScene<T>(Action callback, object passedParams) where T : IStateController, new()
		{
			onNewSceneLoaded = callback;

			//Don't show transition screen if it's our first load.
			if(!isTransitionDone)
			{
				m_transitionScreen.StartFade(FadeType.FadeOut, onTransitionShown);
			}

			newScene = new T();
			newScene.Load(onSceneLoaded, passedParams);
		}

		private void onTransitionShown()
		{
			isTransitionDone = true;
			if(isSceneLoaded)
			{
				onSceneReady();
			}
		}

		private void onSceneLoaded()
		{
			isSceneLoaded = true;
			if(isTransitionDone)
			{
				onSceneReady();
			}
		}

		public void onSceneReady()
		{
			isTransitionDone = isSceneLoaded = false;

			if(currentScene != null)
			{
				currentScene.Unload();
			}

			currentScene = newScene;
			currentScene.Start();
			m_transitionScreen.StartFade(FadeType.FadeIn, null);

			if(onNewSceneLoaded != null)
			{
				onNewSceneLoaded();
			}
		}

		public void StartScene<T>() where T : IStateController
		{
			if(currentScene is T)
			{
				currentScene.Start();
			}
		}
	}
}
