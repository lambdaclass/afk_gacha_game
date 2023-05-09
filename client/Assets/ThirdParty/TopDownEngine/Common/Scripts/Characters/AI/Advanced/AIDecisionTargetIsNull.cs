using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true if the Brain's current target is null, false otherwise
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionTargetIsNull")]
	public class AIDecisionTargetIsNull : AIDecision
	{        
		/// <summary>
		/// On Decide we check whether the Target is null
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return CheckIfTargetIsNull();
		}

		/// <summary>
		/// Returns true if the Brain's Target is null
		/// </summary>
		/// <returns></returns>
		protected virtual bool CheckIfTargetIsNull()
		{
			if (_brain.Target == null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}