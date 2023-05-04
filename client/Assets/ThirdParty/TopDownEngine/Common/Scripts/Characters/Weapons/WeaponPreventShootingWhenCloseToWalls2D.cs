using System;
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
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Prevent Shooting when Close to Walls 2D")]
	public class WeaponPreventShootingWhenCloseToWalls2D : WeaponPreventShooting
	{
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
        
		protected RaycastHit2D _hitLeft;
		protected RaycastHit2D _hitMiddle;
		protected RaycastHit2D _hitRight;
		protected WeaponAim _weaponAim;

		/// <summary>
		/// On Awake we grab our weapon
		/// </summary>
		protected virtual void Awake()
		{
			_weaponAim = this.GetComponent<WeaponAim>();
		}

		/// <summary>
		/// Casts rays in front of the weapon to check for obstacles
		/// Returns true if an obstacle was found
		/// </summary>
		/// <returns></returns>
		protected virtual bool CheckForObstacles()
		{
			_hitLeft = MMDebug.RayCast(this.transform.position + _weaponAim.CurrentRotation * RaycastOriginOffset, (Quaternion.Euler(0f, 0f, -Angle / 2f) * _weaponAim.CurrentAimAbsolute).normalized, Distance, ObstacleLayerMask, Color.yellow, true);
			_hitMiddle = MMDebug.RayCast(this.transform.position + _weaponAim.CurrentRotation * RaycastOriginOffset, _weaponAim.CurrentAimAbsolute.normalized, Distance, ObstacleLayerMask, Color.yellow, true);
			_hitRight = MMDebug.RayCast(this.transform.position + _weaponAim.CurrentRotation * RaycastOriginOffset, (Quaternion.Euler(0f, 0f, Angle / 2f) * _weaponAim.CurrentAimAbsolute).normalized, Distance, ObstacleLayerMask, Color.yellow, true);

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