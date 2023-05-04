using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class can be used to trigger feedbacks from an animator, typically used to trigger footstep particles and/or sounds
	/// </summary>
	public class CharacterAnimationFeedbacks : TopDownMonoBehaviour
	{
		/// a feedback that will play every time a foot touches the ground while walking
		[Tooltip("a feedback that will play every time a foot touches the ground while walking")]
		public MMFeedbacks WalkFeedbacks;

		/// a feedback that will play every time a foot touches the ground while running
		[Tooltip("a feedback that will play every time a foot touches the ground while running")]
		public MMFeedbacks RunFeedbacks;

		/// <summary>
		/// Plays the walk feedback if there's one, when a foot touches the ground (triggered via animation events)
		/// </summary>
		public virtual void WalkStep()
		{
			WalkFeedbacks?.PlayFeedbacks();
		}

		/// <summary>
		/// Plays the run feedback if there's one, when a foot touches the ground (triggered via animation events)
		/// </summary>
		public virtual void RunStep()
		{
			RunFeedbacks?.PlayFeedbacks();
		}
	}
}