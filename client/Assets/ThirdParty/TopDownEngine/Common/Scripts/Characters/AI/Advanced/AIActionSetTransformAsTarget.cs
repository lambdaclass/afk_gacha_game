using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An AIACtion used to set a specified Transform as the target
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionSetTransformAsTarget")]
	public class AIActionSetTransformAsTarget : AIAction
	{
		public Transform TargetTransform;
		public bool OnlyRunOnce = true;
    
		protected bool _alreadyRan = false;
    
		/// <summary>
		/// On init we initialize our action
		/// </summary>
		public override void Initialization()
		{
			base.Initialization();
			_alreadyRan = false;
		}

		/// <summary>
		/// Sets a new target
		/// </summary>
		public override void PerformAction()
		{
			if (OnlyRunOnce && _alreadyRan)
			{
				return;
			}
			_brain.Target = TargetTransform;
		}

		/// <summary>
		/// On enter state we reset our flag
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			_alreadyRan = false;
		}
	}
}