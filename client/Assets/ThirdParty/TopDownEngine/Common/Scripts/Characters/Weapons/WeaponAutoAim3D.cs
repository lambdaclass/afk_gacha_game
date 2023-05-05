using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// The 3D version of the WeaponAutoAim, meant to be used on objects equipped with a WeaponAim3D.
	/// It'll detect targets within the defined radius, pick the closest, and force the WeaponAim component to aim at them if a target is found
	/// </summary>
	[RequireComponent(typeof(WeaponAim3D))]
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Auto Aim 3D")]
	public class WeaponAutoAim3D : WeaponAutoAim
	{
		[Header("Overlap Detection")]
		/// the maximum amount of targets the overlap detection can acquire
		[Tooltip("the maximum amount of targets the overlap detection can acquire")]
		public int OverlapMaximum = 10;
        
		protected Vector3 _aimDirection;
		protected Collider[] _hits;
		protected Vector3 _raycastDirection;
		protected Collider _potentialHit;
		protected TopDownController3D _topDownController3D;
		protected Vector3 _origin;
		protected List<Transform> _potentialTargets;
        
		public Vector3 Origin
		{
			get
			{
				_origin = this.transform.position;
				if (_topDownController3D != null)
				{
					_origin += Quaternion.FromToRotation(Vector3.forward, _topDownController3D.CurrentDirection.normalized) * DetectionOriginOffset;
				}
				return _origin;
			}
		}

		/// <summary>
		/// On init we grab our orientation to be able to detect facing direction
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_potentialTargets = new List<Transform>();
			_hits = new Collider[10];
			if (_weapon.Owner != null)
			{
				_topDownController3D = _weapon.Owner.GetComponent<TopDownController3D>();
			}
		}

		/// <summary>
		/// Scans for targets by performing an overlap detection, then verifying line of fire with a boxcast
		/// </summary>
		/// <returns></returns>
		protected override bool ScanForTargets()
		{
			Target = null;
            
			int numberOfHits = Physics.OverlapSphereNonAlloc(Origin, ScanRadius, _hits, TargetsMask);
            
			if (numberOfHits == 0)
			{
				return false;
			}
            
			_potentialTargets.Clear();
            
			// we go through each collider found
			int min = Mathf.Min(OverlapMaximum, numberOfHits);
			for (int i = 0; i < min; i++)
			{
				if (_hits[i] == null)
				{
					continue;
				}
				if ((_hits[i].gameObject == this.gameObject) || (_hits[i].transform.IsChildOf(this.transform)))
				{
					continue;
				}  
                
				_potentialTargets.Add(_hits[i].gameObject.transform);
			}
            
			// we sort our targets by distance
			_potentialTargets.Sort(delegate(Transform a, Transform b)
			{return Vector3.Distance(this.transform.position,a.transform.position)
				.CompareTo(
					Vector3.Distance(this.transform.position,b.transform.position) );
			});
            
			// we return the first unobscured target
			foreach (Transform t in _potentialTargets)
			{
				_raycastDirection = t.position - _raycastOrigin;
				RaycastHit hit = MMDebug.Raycast3D(_raycastOrigin, _raycastDirection, _raycastDirection.magnitude, ObstacleMask.value, Color.yellow, true);
				if ((hit.collider == null) && CanAcquireNewTargets())
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
		/// Determines the raycast origin
		/// </summary>
		protected override void DetermineRaycastOrigin()
		{
			_raycastOrigin = Origin;
		}
        
		protected override void OnDrawGizmos()
		{
			if (DrawDebugRadius)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(Origin, ScanRadius);
			}
		}
	}
}