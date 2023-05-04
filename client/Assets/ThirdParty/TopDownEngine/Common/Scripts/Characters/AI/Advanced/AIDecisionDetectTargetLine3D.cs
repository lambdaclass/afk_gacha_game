using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This Decision will return true if a target is found in a ray or boxcast in the specified direction
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectTargetLine3D")]
	public class AIDecisionDetectTargetLine3D : AIDecision
	{
		/// the possible detection methods
		public enum DetectMethods { Ray, WideRay }
		/// the detection method
		[Tooltip("the selected detection method : ray is a single ray, wide ray is more expensive but also more accurate")]
		public DetectMethods DetectMethod = DetectMethods.Ray;
		/// the width of the ray to cast (if we're in WideRay mode only
		[Tooltip("the width of the ray to cast (if we're in WideRay mode only")]
		public float RayWidth = 1f;
		/// the distance up to which we'll cast our rays
		[Tooltip("the distance up to which we'll cast our rays")]
		public float DetectionDistance = 10f;
		/// the offset to apply to the ray(s)
		[Tooltip("the offset to apply to the ray(s)")]
		public Vector3 DetectionOriginOffset = new Vector3(0,0,0);
		/// the layer(s) on which we want to search a target on
		[Tooltip("the layer(s) on which we want to search a target on")]
		public LayerMask TargetLayer;
		/// the layer(s) on which obstacles are set. Obstacles will block the ray
		[Tooltip("the layer(s) on which obstacles are set. Obstacles will block the ray")]
		public LayerMask ObstaclesLayer = LayerManager.ObstaclesLayerMask;
		/// a transform to use as the rotation reference for detection raycasts. If you have a rotating model for example, you'll want to set it as your reference transform here.
		[Tooltip("a transform to use as the rotation reference for detection raycasts. If you have a rotating model for example, you'll want to set it as your reference transform here.")]
		public Transform ReferenceTransform;
		/// if this is true, this decision will force the weapon to aim in the detection direction
		[Tooltip("if this is true, this decision will force the weapon to aim in the detection direction")]
		public bool ForceAimToDetectionDirection = false;
		/// if this is true, this decision will set the AI Brain's Target to null if no target is found
		[Tooltip("if this is true, this decision will set the AI Brain's Target to null if no target is found")]
		public bool SetTargetToNullIfNoneIsFound = true;

		protected Vector3 _direction;
		protected float _distanceToTarget;
		protected Vector3 _raycastOrigin;
		protected Character _character;
		protected Color _gizmosColor = Color.yellow;
		protected Vector3 _gizmoCenter;
		protected Vector3 _gizmoSize;
		protected bool _init = false;
		protected CharacterHandleWeapon _characterHandleWeapon;

		/// <summary>
		/// On Init we grab our character
		/// </summary>
		public override void Initialization()
		{
			_character = this.gameObject.GetComponentInParent<Character>();
			_characterHandleWeapon = _character.FindAbility<CharacterHandleWeapon>();
			_gizmosColor.a = 0.25f;
			_init = true;
			if (ReferenceTransform == null)
			{
				ReferenceTransform = this.transform;
			}
		}

		/// <summary>
		/// On Decide we look for a target
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return DetectTarget();
		}

		/// <summary>
		/// Returns true if a target is found by the ray
		/// </summary>
		/// <returns></returns>
		protected virtual bool DetectTarget()
		{
			bool hit = false;
			_distanceToTarget = 0;
			Transform target = null;
			RaycastHit raycast;

			_direction = ReferenceTransform.forward;
            
			// we cast a ray to the left of the agent to check for a Player
			_raycastOrigin = ReferenceTransform.position + DetectionOriginOffset ;

			if (DetectMethod == DetectMethods.Ray)
			{
				raycast = MMDebug.Raycast3D(_raycastOrigin, _direction, DetectionDistance, TargetLayer, MMColors.Gold, true);
                
			}
			else
			{
				hit = Physics.BoxCast(_raycastOrigin, Vector3.one * (RayWidth * 0.5f), _direction, out raycast, ReferenceTransform.rotation, DetectionDistance, TargetLayer);
			}
                
			if (raycast.collider != null)
			{
				hit = true;
				_distanceToTarget = Vector3.Distance(_raycastOrigin, raycast.point);
				target = raycast.collider.gameObject.transform;
			}

			if (hit)
			{
				// we make sure there isn't an obstacle in between
				float distance = Vector3.Distance(target.transform.position, _raycastOrigin);
				RaycastHit raycastObstacle = MMDebug.Raycast3D(_raycastOrigin, (target.transform.position - _raycastOrigin).normalized, distance, ObstaclesLayer, Color.gray, true);
                
				if ((raycastObstacle.collider != null) && (_distanceToTarget > raycastObstacle.distance))
				{
					if (SetTargetToNullIfNoneIsFound) { _brain.Target = null; }
					return false;
				}
				else
				{
					// if there's no obstacle, we store our target and return true
					_brain.Target = target;
					return true;
				}
			}

			ForceDirection();
            
			if (SetTargetToNullIfNoneIsFound) { _brain.Target = null; }
			return false;           
		}

		/// <summary>
		/// Forces the weapon to aim in the selected direction if needed
		/// </summary>
		protected virtual void ForceDirection()
		{
			if (!ForceAimToDetectionDirection)
			{
				return;
			}
			if (_characterHandleWeapon == null)
			{
				return;
			}
			if (_characterHandleWeapon.CurrentWeapon == null)
			{
				return;
			}
			_characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim3D>()?.SetCurrentAim(ReferenceTransform.forward);
		}
        
		/// <summary>
		/// Draws ray gizmos
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			if (DetectMethod != DetectMethods.WideRay)
			{
				return;
			}

			Gizmos.color = _gizmosColor;
            

            
			_gizmoCenter = DetectionOriginOffset + Vector3.forward * DetectionDistance / 2f;
			_gizmoSize.x = RayWidth;
			_gizmoSize.y = RayWidth;
			_gizmoSize.z = DetectionDistance;
            
			Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			if (ReferenceTransform != null)
			{
				Gizmos.matrix = ReferenceTransform.localToWorldMatrix;
			}
            
            
			Gizmos.DrawCube(_gizmoCenter, _gizmoSize);
            
            
		}
        
        
	}
}