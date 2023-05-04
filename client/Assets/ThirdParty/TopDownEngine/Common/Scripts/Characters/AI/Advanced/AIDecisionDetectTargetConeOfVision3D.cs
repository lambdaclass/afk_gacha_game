using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This Decision will return true if its MMConeOfVision has detected at least one target, and will set it as the Brain's target
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectTargetConeOfVision3D")]
	public class AIDecisionDetectTargetConeOfVision3D : AIDecision
	{
		/// if this is true, this decision will set the AI Brain's Target to null if no target is found
		[Tooltip("if this is true, this decision will set the AI Brain's Target to null if no target is found")]
		public bool SetTargetToNullIfNoneIsFound = true;

		public MMConeOfVision TargetConeOfVision;

		/// <summary>
		/// On Init we grab our MMConeOfVision
		/// </summary>
		public override void Initialization()
		{
			base.Initialization();
			if (TargetConeOfVision == null)
			{
				TargetConeOfVision = this.gameObject.GetComponent<MMConeOfVision>();    
			}
		}

		/// <summary>
		/// On Decide we look for a target
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return DetectTarget();
		}

		/// <summary>
		/// If the MMConeOfVision has at least one target, it becomes our new brain target and this decision is true, otherwise it's false.
		/// </summary>
		/// <returns></returns>
		protected virtual bool DetectTarget()
		{
			if (TargetConeOfVision.VisibleTargets.Count == 0)
			{
				if (SetTargetToNullIfNoneIsFound) { _brain.Target = null; }               
				return false;
			}
			else
			{
				_brain.Target = TargetConeOfVision.VisibleTargets[0];
				return true;
			}
		}
	}
}