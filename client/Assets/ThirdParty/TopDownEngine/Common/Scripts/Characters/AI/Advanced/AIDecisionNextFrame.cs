using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true when entering the state this Decision is on.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionNextFrame")]
	public class AIDecisionNextFrame : AIDecision
	{
		/// <summary>
		/// We return true on Decide
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return true;
		}
	}
}