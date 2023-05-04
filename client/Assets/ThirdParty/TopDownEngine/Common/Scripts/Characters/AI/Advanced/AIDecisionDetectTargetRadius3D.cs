using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectTargetRadius3D")]
	//[RequireComponent(typeof(Character))]
	public class AIDecisionDetectTargetRadius3D : AIDecision
	{
		/// the radius to search our target in
		[Tooltip("the radius to search our target in")]
		public float Radius = 3f;
		/// the offset to apply (from the collider's center)
		[Tooltip("the offset to apply (from the collider's center)")]
		public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);
		/// the layer(s) to search our target on
		[Tooltip("the layer(s) to search our target on")]
		public LayerMask TargetLayerMask;
		/// the layer(s) to block the sight
		[Tooltip("the layer(s) to block the sight")]
		public LayerMask ObstacleMask = LayerManager.ObstaclesLayerMask;
		/// the frequency (in seconds) at which to check for obstacles
		[Tooltip("the frequency (in seconds) at which to check for obstacles")]
		public float TargetCheckFrequency = 1f;
		/// if this is true, this AI will be able to consider itself (or its children) a target
		[Tooltip("if this is true, this AI will be able to consider itself (or its children) a target")] 
		public bool CanTargetSelf = false;
		/// the maximum amount of targets the overlap detection can acquire
		[Tooltip("the maximum amount of targets the overlap detection can acquire")]
		public int OverlapMaximum = 10;

		protected Collider _collider;
		protected Vector3 _raycastOrigin;
		protected Character _character;
		protected Color _gizmoColor = Color.yellow;
		protected bool _init = false;
		protected Vector3 _raycastDirection;
		protected Collider[] _hits;
		protected float _lastTargetCheckTimestamp = 0f;
		protected bool _lastReturnValue = false;
		protected List<Transform> _potentialTargets;

		/// <summary>
		/// On init we grab our Character component
		/// </summary>
		public override void Initialization()
		{
			_lastTargetCheckTimestamp = 0f;
			_potentialTargets = new List<Transform>();
			_character = this.gameObject.GetComponentInParent<Character>();
			_collider = this.gameObject.GetComponentInParent<Collider>();
			_gizmoColor.a = 0.25f;
			_init = true;
			_lastReturnValue = false;
			_hits = new Collider[OverlapMaximum];
		}

		/// <summary>
		/// On Decide we check for our target
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return DetectTarget();
		}

		/// <summary>
		/// Returns true if a target is found within the circle
		/// </summary>
		/// <returns></returns>
		protected virtual bool DetectTarget()
		{
			// we check if there's a need to detect a new target
			if (Time.time - _lastTargetCheckTimestamp < TargetCheckFrequency)
			{
				return _lastReturnValue;
			}
			_potentialTargets.Clear();

			_lastTargetCheckTimestamp = Time.time;
			_raycastOrigin = _collider.bounds.center + DetectionOriginOffset / 2;
			int numberOfCollidersFound = Physics.OverlapSphereNonAlloc(_raycastOrigin, Radius, _hits, TargetLayerMask);

			// if there are no targets around, we exit
			if (numberOfCollidersFound == 0)
			{
				_lastReturnValue = false;
				return false;
			}
            
			// we go through each collider found
			int min = Mathf.Min(OverlapMaximum, numberOfCollidersFound);
			for (int i = 0; i < min; i++)
			{
				if (_hits[i] == null)
				{
					continue;
				}
                
				if (!CanTargetSelf)
				{
					if ((_hits[i].gameObject == _brain.Owner) || (_hits[i].transform.IsChildOf(this.transform)))
					{
						continue;
					}    
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
				if (hit.collider == null)
				{
					_brain.Target = t;
					_lastReturnValue = true;
					return true;
				}
			}

			_lastReturnValue = false;
			return false;
		}

		/// <summary>
		/// Draws gizmos for the detection circle
		/// </summary>
		protected virtual void OnDrawGizmosSelected()
		{
			_raycastOrigin = transform.position + DetectionOriginOffset / 2;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(_raycastOrigin, Radius);
			if (_init)
			{
				Gizmos.color = _gizmoColor;
				Gizmos.DrawSphere(_raycastOrigin, Radius);
			}            
		}
	}
}