using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A pickable star, that triggers a TopDownEngineStarEvent if picked
	/// It's up to you to implement something that will handle that event.
	/// You can look at the DeadlineStar and DeadlineProgressManager for examples of that.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/Star")]
	public class Star : PickableItem
	{
		/// the ID of this star, used by the progress manager to know which one got unlocked
		[Tooltip("the ID of this star, used by the progress manager to know which one got unlocked")]
		public int StarID;

		/// <summary>
		/// Triggered when something collides with the star
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker) 
		{
			// we send a new star event for anyone to catch 
			TopDownEngineStarEvent.Trigger(SceneManager.GetActiveScene().name, StarID);
		}
	}

	public struct TopDownEngineStarEvent
	{
		public string SceneName;
		public int StarID;

		public TopDownEngineStarEvent(string sceneName, int starID)
		{
			SceneName = sceneName;
			StarID = starID;
		}

		static TopDownEngineStarEvent e;
		public static void Trigger(string sceneName, int starID)
		{
			e.SceneName = sceneName;
			e.StarID = starID;
			MMEventManager.TriggerEvent(e);
		}
	}
}