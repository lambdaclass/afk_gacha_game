using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// The 2D version of the WeaponAutoAim, meant to be used on objects equipped with a WeaponAim2D.
	/// It'll detect targets within the defined radius, pick the closest, and force the WeaponAim component to aim at them if a target is found
	/// </summary>
	[RequireComponent(typeof(WeaponAim2D))]
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Auto Aim 2D")]
	public class WeaponAutoAim2D : WeaponAutoAim
	{
		/// the maximum amount of targets the overlap detection can acquire
		[Tooltip("the maximum amount of targets the overlap detection can acquire")]
		public int OverlapMaximum = 10;
        
		protected CharacterOrientation2D _orientation2D;
		protected Vector2 _facingDirection;
		protected Vector3 _boxcastDirection;
		protected Vector3 _aimDirection;
		protected bool _initialized = false;
		protected List<Transform> _potentialTargets;
		protected Collider2D[] _results;
		protected RaycastHit2D _hit;

		/// <summary>
		/// On init we grab our orientation to be able to detect facing direction
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_orientation2D = _weapon.Owner.GetComponent<Character>()?.FindAbility<CharacterOrientation2D>();
			_initialized = true;
			_results = new Collider2D[OverlapMaximum];
			_potentialTargets = new List<Transform>();
		}

		/// <summary>
		/// Scans for targets by performing an overlap detection, then verifying line of fire with a boxcast
		/// </summary>
		/// <returns></returns>
		protected override bool ScanForTargets()
		{
			if (!_initialized)
			{
				Initialization();
			}

			Target = null;

			int numberOfResults = Physics2D.OverlapCircleNonAlloc(_raycastOrigin, ScanRadius, _results, TargetsMask);     
			// if there are no targets around, we exit
			if (numberOfResults == 0)
			{
				return false;
			}
			_potentialTargets.Clear();
            
			// we go through each collider found
            
			int min = Mathf.Min(OverlapMaximum, numberOfResults);
			for (int i = 0; i < min; i++)
			{
				if (_results[i] == null)
				{
					continue;
				}
                
				_potentialTargets.Add(_results[i].gameObject.transform);
			}
            
            
			// we sort our targets by distance
			_potentialTargets.Sort(delegate(Transform a, Transform b)
			{return Vector2.Distance(this.transform.position,a.transform.position)
				.CompareTo(
					Vector2.Distance(this.transform.position,b.transform.position) );
			});
            
			// we return the first unobscured target
			foreach (Transform t in _potentialTargets)
			{
				_boxcastDirection = (Vector2)(t.gameObject.MMGetComponentNoAlloc<Collider2D>().bounds.center - _raycastOrigin);
                
				_hit = Physics2D.BoxCast(_raycastOrigin, LineOfFireBoxcastSize, 0f, _boxcastDirection.normalized, _boxcastDirection.magnitude, ObstacleMask); 
                
				if (!_hit && CanAcquireNewTargets())
				{
					Target = t;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Sets the aim to the relative direction of the target
		/// </summary>
		protected override void SetAim()
		{
			_aimDirection = (Target.transform.position - _raycastOrigin).normalized;
			_weaponAim.SetCurrentAim(_aimDirection, ApplyAutoAimAsLastDirection);
		}

		/// <summary>
		/// To determine our raycast origin we apply an offset
		/// </summary>
		protected override void DetermineRaycastOrigin()
		{
			if (_orientation2D != null)
			{
				_facingDirection = _orientation2D.IsFacingRight ? Vector2.right : Vector2.left;
				_raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
				_raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;
			}
			else
			{
				_raycastOrigin = transform.position + DetectionOriginOffset;
			}
		}
	}
}