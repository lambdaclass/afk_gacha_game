using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class to handle a moving platform in 3D space, moving along a set of nodes
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Moving Platform 3D")]
	public class MovingPlatform3D : MMPathMovement
	{
		/// The force to apply when pushing a character that'd be in the way of the moving platform
		[Tooltip("The force to apply when pushing a character that'd be in the way of the moving platform")]
		public float PushForce = 5f;       
	}
}