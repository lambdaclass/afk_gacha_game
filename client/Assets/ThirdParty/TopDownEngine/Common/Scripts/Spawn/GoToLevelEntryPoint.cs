using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used to go from one level to the next while specifying an entry point in the target level. 
	/// Entry points are defined in each level's LevelManager component. They're simply Transforms in a list. 
	/// The index in the list is the identifier for the entry point. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Spawn/GoToLevelEntryPoint")]
	public class GoToLevelEntryPoint : FinishLevel 
	{
		[Space(10)]
		[Header("Points of Entry")]

		/// Whether or not to use entry points. If you don't, you'll simply move on to the next level
		[Tooltip("Whether or not to use entry points. If you don't, you'll simply move on to the next level")]
		public bool UseEntryPoints = false;
		/// The index of the point of entry to move to in the next level
		[Tooltip("The index of the point of entry to move to in the next level")]
		public int PointOfEntryIndex;
		/// The direction to face when moving to the next level
		[Tooltip("The direction to face when moving to the next level")]
		public Character.FacingDirections FacingDirection;

		/// <summary>
		/// Loads the next level and stores the target entry point index in the game manager
		/// </summary>
		public override void GoToNextLevel()
		{
			if (UseEntryPoints)
			{
				GameManager.Instance.StorePointsOfEntry(LevelName, PointOfEntryIndex, FacingDirection);
			}
			
			base.GoToNextLevel ();
		}
	}
}