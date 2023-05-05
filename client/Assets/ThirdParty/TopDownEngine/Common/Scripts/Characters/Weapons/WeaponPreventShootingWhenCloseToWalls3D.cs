using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a weapon and it'll prevent shooting when close to an obstacle (as defined by the ObstacleLayerMask)
	/// </summary>
	[RequireComponent(typeof(Weapon))]
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Prevent Shooting when Close to Walls 3D")]
	public class WeaponPreventShootingWhenCloseToWalls3D : WeaponPreventShooting
	{
		[Header("Raycast Settings")]
		/// the angle to consider when deciding whether or not there's a wall in front of the weapon (usually 5 degrees is fine)
		[Tooltip("the angle to consider when deciding whether or not there's a wall in front of the weapon (usually 5 degrees is fine)")]
		public float Angle = 5f;
		/// the max distance to the wall we want to prevent shooting from
		[Tooltip("the max distance to the wall we want to prevent shooting from")]
		public float Distance = 2f;
		/// the offset to apply to the detection (in addition and relative to the weapon's position)
		[Tooltip("the offset to apply to the detection (in addition and relative to the weapon's position)")]
		public Vector3 RaycastOriginOffset = Vector3.zero;
		/// the layers to consider as obstacles
		[Tooltip("the layers to consider as obstacles")]
		public LayerMask ObstacleLayerMask = LayerManager.ObstaclesLayerMask;

		protected RaycastHit _hitLeft;
		protected RaycastHit _hitMiddle;
		protected RaycastHit _hitRight;
   

		/// <summary>
		/// Casts rays in front of the weapon to check for obstacles
		/// Returns true if an obstacle was found
		/// </summary>
		/// <returns></returns>
		protected virtual bool CheckForObstacles()
		{
			_hitLeft = MMDebug.Raycast3D(this.transform.position + this.transform.rotation * RaycastOriginOffset, Quaternion.Euler(0f, -Angle/2f, 0f) * this.transform.forward, Distance, ObstacleLayerMask, Color.yellow, true);
			_hitMiddle = MMDebug.Raycast3D(this.transform.position + this.transform.rotation * RaycastOriginOffset, this.transform.forward, Distance, ObstacleLayerMask, Color.yellow, true);
			_hitRight = MMDebug.Raycast3D(this.transform.position + this.transform.rotation * RaycastOriginOffset, Quaternion.Euler(0f, Angle / 2f, 0f) * this.transform.forward, Distance, ObstacleLayerMask, Color.yellow, true);

			if ((_hitLeft.collider == null) && (_hitMiddle.collider == null) && (_hitRight.collider == null))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Shooting is allowed if no obstacle is in front of the weapon
		/// </summary>
		/// <returns></returns>
		public override bool ShootingAllowed()
		{
			return !CheckForObstacles();
		}
	}
}