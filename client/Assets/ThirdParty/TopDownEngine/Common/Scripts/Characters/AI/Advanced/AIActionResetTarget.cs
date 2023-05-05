using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An Action that will set the target to null, resetting it
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionResetTarget")]
	public class AIActionResetTarget : AIAction
	{
		/// <summary>
		/// we reset our target
		/// </summary>
		public override void PerformAction()
		{
			_brain.Target = null;
		}
	}
}