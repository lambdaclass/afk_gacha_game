using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A simple component meant to be added to the pause button
	/// </summary>
	[AddComponentMenu("TopDown Engine/GUI/PauseButton")]
	public class PauseButton : TopDownMonoBehaviour
	{
		/// <summary>
		/// Triggers a pause event
		/// </summary>
		public virtual void PauseButtonAction()
		{
			// we trigger a Pause event for the GameManager and other classes that could be listening to it too
			StartCoroutine(PauseButtonCo());

		}	

		/// <summary>
		/// Unpauses the game via an UnPause event
		/// </summary>
		public virtual void UnPause()
		{
			StartCoroutine(PauseButtonCo());
		}

		/// <summary>
		/// A coroutine used to trigger the pause event
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator PauseButtonCo()
		{
			yield return null;
			// we trigger a Pause event for the GameManager and other classes that could be listening to it too
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.TogglePause, null);
		}

	}
}