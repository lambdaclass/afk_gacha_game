using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This action is used to play a MMFeedbacks
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMMFeedbacks")]
	public class AIActionMMFeedbacks : AIAction
	{
		/// The MMFeedbacks to play when this action gets performed by the AIBrain
		[Tooltip("The MMFeedbacks to play when this action gets performed by the AIBrain")]
		public MMFeedbacks TargetFeedbacks;
		/// If this is false, the feedback will be played every PerformAction (by default every frame while in this state), otherwise it'll only play once, when entering the state
		[Tooltip("If this is false, the feedback will be played every PerformAction (by default every frame while in this state), otherwise it'll only play once, when entering the state")]
		public bool OnlyPlayWhenEnteringState = true;
		/// If this is true, the target game object the TargetFeedbacks is on will be set active when performing this action
		[Tooltip("If this is true, the target game object the TargetFeedbacks is on will be set active when performing this action")]
		public bool SetTargetGameObjectActive = false;

		protected bool _played = false;

		/// <summary>
		/// On PerformAction we play our MMFeedbacks
		/// </summary>
		public override void PerformAction()
		{
			PlayFeedbacks();
		}

		/// <summary>
		/// Plays the target MMFeedbacks
		/// </summary>
		protected virtual void PlayFeedbacks()
		{
			if (OnlyPlayWhenEnteringState && _played)
			{
				return;
			}

			if (TargetFeedbacks != null)
			{
				if (SetTargetGameObjectActive)
				{
					TargetFeedbacks.gameObject.SetActive(true);
				}
				TargetFeedbacks.PlayFeedbacks();
				_played = true;
			}
		}

		/// <summary>
		/// On enter state we initialize our _played bool
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			_played = false;
		}
	}
}