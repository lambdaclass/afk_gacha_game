using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this class to a trigger and it will send your player to the next level
	/// </summary>
	[AddComponentMenu("TopDown Engine/Spawn/Finish Level")]
	public class FinishLevel : ButtonActivated
	{
		[Header("Finish Level")]
		/// the exact name of the level to transition to 
		[Tooltip("the exact name of the level to transition to ")]
		public string LevelName;

		/// <summary>
		/// When the button is pressed we start the dialogue
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			base.TriggerButtonAction ();
			GoToNextLevel();
		}			

		/// <summary>
		/// Loads the next level
		/// </summary>
		public virtual void GoToNextLevel()
		{
			if (LevelManager.HasInstance)
			{
				LevelManager.Instance.GotoLevel(LevelName);
			}
			else
			{
				MMSceneLoadingManager.LoadScene(LevelName);
			}
		}
	}
}