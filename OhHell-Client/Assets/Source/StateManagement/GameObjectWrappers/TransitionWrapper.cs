//TransitionWrapper.cs
//Wrapper class for the transition screen.

using System;
using Controllers;
using Controllers.Interfaces;
using UnityEngine;

namespace GameObjectWrappers
{
	public class TransitionWrapper : ITransitionWrapper
	{
		private const string TRANSITION_OVER_EVENT = "transitionOver";
		private const string TRANSITION_IN_ID = "fadeIn";
		private const string TRANSITION_OUT_ID = "fadeOut";

		private GameObject m_wrappedObject;
		private Action m_onAnimationOver;
		private Animator m_animator;
		private float currentTime;

		private Transform vrCam;

		public TransitionWrapper (GameObject wrappedObject)
		{
			m_wrappedObject = wrappedObject;
			m_animator = m_wrappedObject.GetComponent<Animator>();
			vrCam = GameObject.Find ("CenterEyeAnchor").transform;
			ReParentTransitionWrapper (vrCam);
			Service.UpdateManager.AddObserver (Update);
		}

		public void PlayTransitionIn(float duration, Action onAnimationOver)
		{
			m_onAnimationOver = onAnimationOver;
			m_animator.SetTrigger(TRANSITION_IN_ID);
			currentTime = duration;

		}

		public void PlayTransitionOut(float duration, Action onAnimationOver)
		{
			m_onAnimationOver = onAnimationOver;
			m_animator.SetTrigger(TRANSITION_OUT_ID);
			currentTime = duration;
		}

		public void SetActive(bool active)
		{
			m_wrappedObject.SetActive(active);
		}

		private void Update(float dt)
		{
			if (currentTime > 0f)
			{
				currentTime -= dt;

				if (currentTime <= 0f && m_onAnimationOver != null)
				{
					m_onAnimationOver();
					m_onAnimationOver = null;
				}
			}
		}

		public void OnAnimationEvent(string eventType)
		{
			if(eventType == TRANSITION_OVER_EVENT && m_onAnimationOver != null)
			{
				m_onAnimationOver();
				m_onAnimationOver = null;
			}
		}

		private void ReParentTransitionWrapper(Transform newParent)
		{
			m_wrappedObject.transform.SetParent (newParent);
			m_wrappedObject.transform.localPosition = Vector3.zero;
		}
	}
}

