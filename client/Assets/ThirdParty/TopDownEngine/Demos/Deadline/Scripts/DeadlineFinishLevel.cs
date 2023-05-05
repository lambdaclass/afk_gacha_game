using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// A Deadline demo dedicated class that will load the next level
	/// </summary>
	public class DeadlineFinishLevel : FinishLevel 
	{
		/// <summary>
		/// Loads the next level
		/// </summary>
		public override void GoToNextLevel()
		{
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelComplete, null);
			MMGameEvent.Trigger("Save");
			LevelManager.Instance.GotoLevel (LevelName);
		}	
	}
}