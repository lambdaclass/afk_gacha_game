using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This component allows the definition of a level that can then be accessed and loaded. Used mostly in the level map scene.
	/// </summary>
	[AddComponentMenu("TopDown Engine/GUI/LevelSelector")]
	public class LevelSelector : TopDownMonoBehaviour
	{
		/// the exact name of the target level
		[Tooltip("the exact name of the target level")]
		public string LevelName;

		/// if this is true, GoToLevel will ignore the LevelManager and do a direct call
		[Tooltip("if this is true, GoToLevel will ignore the LevelManager and do a direct call")]
		public bool DoNotUseLevelManager = false;

		/// if this is true, any persistent character will be destroyed when loading the new level
		[Tooltip("if this is true, any persistent character will be destroyed when loading the new level")]
		public bool DestroyPersistentCharacter = false;

		/// <summary>
		/// Loads the level specified in the inspector
		/// </summary>
		public virtual void GoToLevel()
		{
			LoadScene(LevelName);
		}

		/// <summary>
		/// Loads a new scene, either via the LevelManager or not
		/// </summary>
		/// <param name="newSceneName"></param>
		protected virtual void LoadScene(string newSceneName)
		{
			if (DestroyPersistentCharacter)
			{
				GameManager.Instance.DestroyPersistentCharacter();
			}
			
			if (GameManager.Instance.Paused)
			{
				TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);
			}
				
			if (DoNotUseLevelManager)
			{
				MMAdditiveSceneLoadingManager.LoadScene(newSceneName);    
			}
			else
			{
				LevelManager.Instance.GotoLevel(newSceneName);   
			}
		}

		/// <summary>
		/// Restarts the current level, without reloading the whole scene
		/// </summary>
		public virtual void RestartLevel()
		{
			if (GameManager.Instance.Paused)
			{
				TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);
			}            
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RespawnStarted, null);
		}

		/// <summary>
		/// Reloads the current level
		/// </summary>
		public virtual void ReloadLevel()
		{
			// we trigger an unPause event for the GameManager (and potentially other classes)
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);
			LoadScene(SceneManager.GetActiveScene().name);
		}
		
	}
}