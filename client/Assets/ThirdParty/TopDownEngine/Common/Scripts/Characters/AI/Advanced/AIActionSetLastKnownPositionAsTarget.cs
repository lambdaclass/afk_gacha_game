using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An Action that will set the last known position of the target as the new Target
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionSetLastKnownPositionAsTarget")]
	public class AIActionSetLastKnownPositionAsTarget : AIAction
	{
		protected Transform _targetTransform;

		/// <summary>
		/// On init we prepare a new game object to act as the new target
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			GameObject newGo = new GameObject();
			newGo.name = "AIActionSetLastKnownPositionAsTarget_target";
			newGo.transform.SetParent(null);
			_targetTransform = newGo.transform;
		}

		/// <summary>
		/// we move our target to the last known position and assign it to the brain
		/// </summary>
		public override void PerformAction()
		{
			_targetTransform.position = _brain._lastKnownTargetPosition;
			_brain.Target = _targetTransform;
		}
	}
}