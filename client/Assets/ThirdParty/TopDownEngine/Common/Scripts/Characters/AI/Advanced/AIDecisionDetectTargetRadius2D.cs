using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectTargetRadius2D")]
	//[RequireComponent(typeof(CharacterOrientation2D))]
	public class AIDecisionDetectTargetRadius2D : AIDecision
	{
		public enum ObstaclesDetectionModes { Boxcast, Raycast }
        
		/// the radius to search our target in
		[Tooltip("the radius to search our target in")]
		public float Radius = 3f;
		/// the center of the search circle
		[Tooltip("the center of the search circle")]
		public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);
		/// the layer(s) to search our target on
		[Tooltip("the layer(s) to search our target on")]
		public LayerMask TargetLayer;
		/// whether or not to look for obstacles
		[Tooltip("whether or not to look for obstacles")]
		public bool ObstacleDetection = true;
		/// the layer(s) to look for obstacles on
		[Tooltip("the layer(s) to look for obstacles on")]
		public LayerMask ObstacleMask = LayerManager.ObstaclesLayerMask;
		/// the method to use to detect obstacles
		[Tooltip("the method to use to detect obstacles")]
		public ObstaclesDetectionModes ObstaclesDetectionMode = ObstaclesDetectionModes.Raycast;
		/// if this is true, this AI will be able to consider itself (or its children) a target
		[Tooltip("if this is true, this AI will be able to consider itself (or its children) a target")] 
		public bool CanTargetSelf = false;
		/// the frequency (in seconds) at which to check for obstacles
		[Tooltip("the frequency (in seconds) at which to check for obstacles")]
		public float TargetCheckFrequency = 1f;
		/// the maximum amount of targets the overlap detection can acquire
		[Tooltip("the maximum amount of targets the overlap detection can acquire")]
		public int OverlapMaximum = 10;

		protected Collider2D _collider;
		protected Vector2 _facingDirection;
		protected Vector2 _raycastOrigin;
		protected Character _character;
		protected CharacterOrientation2D _orientation2D;
		protected Color _gizmoColor = Color.yellow;
		protected bool _init = false;
		protected Vector2 _boxcastDirection;
		protected Collider2D[] _results;
		protected List<Transform> _potentialTargets;
		protected float _lastTargetCheckTimestamp = 0f;
		protected bool _lastReturnValue = false;
		protected RaycastHit2D _hit;

		/// <summary>
		/// On init we grab our Character component
		/// </summary>
		public override void Initialization()
		{
			_potentialTargets = new List<Transform>();
			_character = this.gameObject.GetComponentInParent<Character>();
			_orientation2D = _character?.FindAbility<CharacterOrientation2D>();
			_collider = this.gameObject.GetComponentInParent<Collider2D>();
			_gizmoColor.a = 0.25f;
			_init = true;
			_results = new Collider2D[OverlapMaximum];
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
			
			if (_orientation2D != null)
			{
				_facingDirection = _orientation2D.IsFacingRight ? Vector2.right : Vector2.left;
				_raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
				_raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;
			}
			else
			{
				_raycastOrigin = transform.position +  DetectionOriginOffset;
			}
            
			int numberOfResults = Physics2D.OverlapCircleNonAlloc(_raycastOrigin, Radius, _results, TargetLayer);     
			// if there are no targets around, we exit
			if (numberOfResults == 0)
			{
				_lastReturnValue = false;
				return false;
			}
            
			// we go through each collider found
			int min = Mathf.Min(OverlapMaximum, numberOfResults);
			for (int i = 0; i < min; i++)
			{
				if (_results[i] == null)
				{
					continue;
				}
                
				if (!CanTargetSelf)
				{
					if ((_results[i].gameObject == _brain.Owner) || (_results[i].transform.IsChildOf(this.transform)))
					{
						continue;
					}    
				}
                
				_potentialTargets.Add(_results[i].gameObject.transform);
			}
            
			// we check if there's a target in the list
			if (_potentialTargets.Count == 0)
			{
				_lastReturnValue = false;
				return false;
			}
            
			// we sort our targets by distance
			_potentialTargets.Sort(delegate(Transform a, Transform b)
			{
				if (a == null || b == null)
				{
					return 0;
				}
                
				return Vector2.Distance(this.transform.position,a.transform.position)
					.CompareTo(
						Vector2.Distance(this.transform.position,b.transform.position) );
			});
            
			if (!ObstacleDetection && _potentialTargets[0] != null)
			{
				_brain.Target = _potentialTargets[0].gameObject.transform;
				return true;
			}
            
			// we return the first unobscured target
			foreach (Transform t in _potentialTargets)
			{
				_boxcastDirection = (Vector2)(t.gameObject.MMGetComponentNoAlloc<Collider2D>().bounds.center - _collider.bounds.center);
                
				if (ObstaclesDetectionMode == ObstaclesDetectionModes.Boxcast)
				{
					_hit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, _boxcastDirection.normalized, _boxcastDirection.magnitude, ObstacleMask);    
				}
				else
				{
					_hit = MMDebug.RayCast(_collider.bounds.center, _boxcastDirection, _boxcastDirection.magnitude,
						ObstacleMask, Color.yellow, true);
				}
                
				if (!_hit)
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
			_raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
			_raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

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