using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This action is used to trigger a UnityEvent
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionUnityEvents")]
	public class AIActionUnityEvents : AIAction
	{
		/// The UnityEvent to trigger when this action gets performed by the AIBrain
		[Tooltip("The UnityEvent to trigger when this action gets performed by the AIBrain")]
		public UnityEvent TargetEvent;
		/// If this is false, the Unity Event will be triggered every PerformAction (by default every frame while in this state), otherwise it'll only play once, when entering the state
		[Tooltip("If this is false, the Unity Event will be triggered every PerformAction (by default every frame while in this state), otherwise it'll only play once, when entering the state")]
		public bool OnlyPlayWhenEnteringState = true;

		protected bool _played = false;

		/// <summary>
		/// On PerformAction we trigger our event
		/// </summary>
		public override void PerformAction()
		{
			TriggerEvent();
		}

		/// <summary>
		/// Triggers the target event
		/// </summary>
		protected virtual void TriggerEvent()
		{
			if (OnlyPlayWhenEnteringState && _played)
			{
				return;
			}

			if (TargetEvent != null)
			{
				TargetEvent.Invoke();
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