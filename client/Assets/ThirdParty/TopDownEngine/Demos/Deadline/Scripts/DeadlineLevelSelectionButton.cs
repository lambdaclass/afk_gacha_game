using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class to handle the display of levels in the Deadline demo level selector
	/// </summary>
	public class DeadlineLevelSelectionButton : TopDownMonoBehaviour 
	{
		/// the name of the scene to bind to this element
		[Tooltip("the name of the scene to bind to this element")]
		public string SceneName;
		/// the icon showing whether or not the level is locked
		[Tooltip("the icon showing whether or not the level is locked")]
		public Image LockedIcon;
		/// the icon showing whether or not the level has been completed
		[Tooltip("the icon showing whether or not the level has been completed")]
		public Image CompletedIcon;

		[Header("Stars")]
		/// the stars to display in the level element
		[Tooltip("the stars to display in the level element")]
		public Image[] Stars;
		/// the color to apply to stars when they're locked
		[Tooltip("the color to apply to stars when they're locked")]
		public Color StarOffColor;
		/// the color to apply to stars once they've been unlocked
		[Tooltip("the color to apply to stars once they've been unlocked")]
		public Color StarOnColor;

		protected Button _button;

		/// <summary>
		/// The method to call to go the level specified in the inspector
		/// </summary>
		public virtual void GoToLevel()
		{
			MMSceneLoadingManager.LoadScene(SceneName);
		}

		/// <summary>
		/// On start we initialize our setup
		/// </summary>
		protected virtual void Start()
		{
			InitialSetup ();
		}

		/// <summary>
		/// Sets various elements (stars, locked icon) based on current saved data
		/// </summary>
		protected virtual void InitialSetup()
		{
			_button = this.gameObject.GetComponent<Button>();
			
			foreach (DeadlineScene scene in DeadlineProgressManager.Instance.Scenes)
			{
				if (scene.SceneName == SceneName)
				{
					CompletedIcon.gameObject.SetActive(scene.LevelComplete);
					LockedIcon.gameObject.SetActive(!scene.LevelUnlocked);
					
					if (!scene.LevelUnlocked)
					{
						_button.interactable = false;
					}
					
					for (int i=0; i<Stars.Length; i++)
					{
						Stars [i].color = (scene.CollectedStars [i]) ? StarOnColor : StarOffColor;							
					}
				}
			}
		}
	}
}