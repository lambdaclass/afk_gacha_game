using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An abstract class used to define additional conditions on a weapon to prevent it from firing
	/// </summary>
	public abstract class WeaponPreventShooting : TopDownMonoBehaviour
	{
		/// <summary>
		/// Override this method to define shooting conditions
		/// </summary>
		/// <returns></returns>
		public abstract bool ShootingAllowed();
	}
}