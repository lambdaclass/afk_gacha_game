using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this component to a button (for example) to be able to store a selected character, and optionally to go to another scene
	/// You can see an example of its use in the DeadlineCharacterSelection demo scene
	/// </summary>
	public class CharacterSelector : TopDownMonoBehaviour 
	{
		/// The name of the scene to go to when calling LoadNextScene()
		[Tooltip("The name of the scene to go to when calling LoadNextScene()")]
		public string DestinationSceneName;
		/// The character prefab to store in the GameManager
		[Tooltip("The character prefab to store in the GameManager")]
		public Character CharacterPrefab;

		/// <summary>
		/// Stores the selected character prefab in the Game Manager
		/// </summary>
		public virtual void StoreCharacterSelection()
		{
			GameManager.Instance.StoreSelectedCharacter (CharacterPrefab);
		}

		/// <summary>
		/// Loads the next scene after having stored the selected character in the Game Manager.
		/// </summary>
		public virtual void LoadNextScene()
		{
			StoreCharacterSelection ();
			MMSceneLoadingManager.LoadScene(DestinationSceneName);
		}
	}
}