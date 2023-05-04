using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// As the name implies, an action that does nothing. Just waits there.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionDoNothing")]
	public class AIActionDoNothing : AIAction
	{
		/// <summary>
		/// On PerformAction we do nothing
		/// </summary>
		public override void PerformAction()
		{

		}
	}
}