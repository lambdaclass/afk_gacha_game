using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An abstract class, meant to be extended for 2D and 3D specifics, handling the basics of auto aim. 
	/// Extended components should be placed on a weapon with an aim component
	/// </summary>
	[RequireComponent(typeof(Weapon))]
	public abstract class WeaponAutoAim : TopDownMonoBehaviour
	{
		[Header("Layer Masks")]
		/// the layermask on which to look for aim targets
		[Tooltip("the layermask on which to look for aim targets")]
		public LayerMask TargetsMask;
		/// the layermask on which to look for obstacles
		[Tooltip("the layermask on which to look for obstacles")]
		public LayerMask ObstacleMask = LayerManager.ObstaclesLayerMask;

		[Header("Scan for Targets")]
		/// the radius (in units) around the character within which to search for targets
		[Tooltip("the radius (in units) around the character within which to search for targets")]
		public float ScanRadius = 15f;
		/// the size of the boxcast that will be performed to verify line of fire
		[Tooltip("the size of the boxcast that will be performed to verify line of fire")]
		public Vector2 LineOfFireBoxcastSize = new Vector2(0.1f, 0.1f);
		/// the duration (in seconds) between 2 scans for targets
		[Tooltip("the duration (in seconds) between 2 scans for targets")]
		public float DurationBetweenScans = 1f;
		/// an offset to apply to the weapon's position for scan 
		[Tooltip("an offset to apply to the weapon's position for scan ")]
		public Vector3 DetectionOriginOffset = Vector3.zero;
		/// if this is true, auto aim scan will only acquire new targets if the owner is in the idle state 
		[Tooltip("if this is true, auto aim scan will only acquire new targets if the owner is in the idle state")]
		public bool OnlyAcquireTargetsIfOwnerIsIdle = false;
		
		[Header("Weapon Rotation")]
		/// the rotation mode to apply when a target is found
		[Tooltip("the rotation mode to apply when a target is found")]
		public WeaponAim.RotationModes RotationMode;
		/// if this is true, the auto aim direction will also be passed as the last non null direction, so the weapon will keep aiming in that direction should the target be lost
		[Tooltip("if this is true, the auto aim direction will also be passed as the last non null direction, so the weapon will keep aiming in that direction should the target be lost")]
		public bool ApplyAutoAimAsLastDirection = true;
        
		[Header("Camera Target")]
		/// whether or not this component should take control of the camera target when a camera is found
		[Tooltip("whether or not this component should take control of the camera target when a camera is found")]
		public bool MoveCameraTarget = true;
		/// the normalized distance (between 0 and 1) at which the camera target should be, on a line going from the weapon owner (0) to the auto aim target (1) 
		[Tooltip("the normalized distance (between 0 and 1) at which the camera target should be, on a line going from the weapon owner (0) to the auto aim target (1)")]
		[Range(0f, 1f)]
		public float CameraTargetDistance = 0.5f;
		/// the maximum distance from the weapon owner at which the camera target can be
		[Tooltip("the maximum distance from the weapon owner at which the camera target can be")]
		[MMCondition("MoveCameraTarget", true)]
		public float CameraTargetMaxDistance = 10f;
		/// the speed at which to move the camera target
		[Tooltip("the speed at which to move the camera target")]
		[MMCondition("MoveCameraTarget", true)]
		public float CameraTargetSpeed = 5f;

		[Header("Aim Marker")]
		/// An AimMarker prefab to use to show where this auto aim weapon is aiming
		[Tooltip("An AimMarker prefab to use to show where this auto aim weapon is aiming")]
		public AimMarker AimMarkerPrefab;
		/// if this is true, the aim marker will be removed when the weapon gets destroyed
		[Tooltip("if this is true, the aim marker will be removed when the weapon gets destroyed")]
		public bool DestroyAimMarkerOnWeaponDestroy = true;

		[Header("Feedback")]
		/// A feedback to play when a target is found and we didn't have one already
		[Tooltip("A feedback to play when a target is found and we didn't have one already")]
		public MMFeedbacks FirstTargetFoundFeedback;
		/// a feedback to play when we already had a target and just found a new one
		[Tooltip("a feedback to play when we already had a target and just found a new one")]
		public MMFeedbacks NewTargetFoundFeedback;
		/// a feedback to play when no more targets are found, and we just lost our last target
		[Tooltip("a feedback to play when no more targets are found, and we just lost our last target")]
		public MMFeedbacks NoMoreTargetsFeedback;

		[Header("Debug")]
		/// the current target of the auto aim module
		[Tooltip("the current target of the auto aim module")]
		[MMReadOnly]
		public Transform Target;
		/// whether or not to draw a debug sphere around the weapon to show its aim radius
		[Tooltip("whether or not to draw a debug sphere around the weapon to show its aim radius")]
		public bool DrawDebugRadius = true;
        
		protected float _lastScanTimestamp = 0f;
		protected WeaponAim _weaponAim;
		protected WeaponAim.AimControls _originalAimControl;
		protected WeaponAim.RotationModes _originalRotationMode;
		protected Vector3 _raycastOrigin;
		protected Weapon _weapon;
		protected bool _originalMoveCameraTarget;
		protected Transform _targetLastFrame;
		protected AimMarker _aimMarker;

		/// <summary>
		/// On Awake we initialize our component
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// On init we grab our WeaponAim
		/// </summary>
		protected virtual void Initialization()
		{
			_weaponAim = this.gameObject.GetComponent<WeaponAim>();
			_weapon = this.gameObject.GetComponent<Weapon>();
			_isOwnerNull = _weapon.Owner == null;
			if (_weaponAim == null)
			{
				Debug.LogWarning(this.name + " : the WeaponAutoAim on this object requires that you add either a WeaponAim2D or WeaponAim3D component to your weapon.");
				return;
			}

			_originalAimControl = _weaponAim.AimControl;
			_originalRotationMode = _weaponAim.RotationMode;
			_originalMoveCameraTarget = _weaponAim.MoveCameraTargetTowardsReticle;

			FirstTargetFoundFeedback?.Initialization(this.gameObject);
			NewTargetFoundFeedback?.Initialization(this.gameObject);
			NoMoreTargetsFeedback?.Initialization(this.gameObject);

			if (AimMarkerPrefab != null)
			{
				_aimMarker = Instantiate(AimMarkerPrefab);
				_aimMarker.name = this.gameObject.name + "_AimMarker";
				_aimMarker.Disable();
			}
		}

		/// <summary>
		/// On Update, we setup our ray origin, scan periodically and set aim if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_weaponAim == null)
			{
				return;
			}

			DetermineRaycastOrigin();
			ScanIfNeeded();
			HandleTarget();
			HandleMoveCameraTarget();
			HandleTargetChange();
			_targetLastFrame = Target;
		}

		/// <summary>
		/// A method used to compute the origin of the detection casts
		/// </summary>
		protected abstract void DetermineRaycastOrigin();

		/// <summary>
		/// This method should define how the scan for targets is performed
		/// </summary>
		/// <returns></returns>
		protected abstract bool ScanForTargets();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual bool CanAcquireNewTargets() 
		{
			if (OnlyAcquireTargetsIfOwnerIsIdle && !_isOwnerNull)
			{
				if (_weapon.Owner.MovementState.CurrentState != CharacterStates.MovementStates.Idle)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Sends aim coordinates to the weapon aim component
		/// </summary>
		protected abstract void SetAim();

		/// <summary>
		/// Moves the camera target towards the auto aim target if needed
		/// </summary>
		protected Vector3 _newCamTargetPosition;
		protected Vector3 _newCamTargetDirection;
		protected bool _isOwnerNull;

		/// <summary>
		/// Checks for target changes and triggers the appropriate methods if needed
		/// </summary>
		protected virtual void HandleTargetChange()
		{
			if (Target == _targetLastFrame)
			{
				return;
			}

			if (_aimMarker != null)
			{
				_aimMarker.SetTarget(Target);
			}

			if (Target == null)
			{
				NoMoreTargets();
				return;
			}

			if (_targetLastFrame == null)
			{
				FirstTargetFound();
				return;
			}

			if ((_targetLastFrame != null) && (Target != null))
			{
				NewTargetFound();
			}
		}

		/// <summary>
		/// When no more targets are found, and we just lost one, we play a dedicated feedback
		/// </summary>
		protected virtual void NoMoreTargets()
		{
			NoMoreTargetsFeedback?.PlayFeedbacks();
		}

		/// <summary>
		/// When a new target is found and we didn't have one already, we play a dedicated feedback
		/// </summary>
		protected virtual void FirstTargetFound()
		{
			FirstTargetFoundFeedback?.PlayFeedbacks();
		}

		/// <summary>
		/// When a new target is found, and we previously had another, we play a dedicated feedback
		/// </summary>
		protected virtual void NewTargetFound()
		{
			NewTargetFoundFeedback?.PlayFeedbacks();
		}

		/// <summary>
		/// Moves the camera target if needed
		/// </summary>
		protected virtual void HandleMoveCameraTarget()
		{
			if (!MoveCameraTarget || (Target == null) || (_isOwnerNull))
			{
				return;
			}
            
			_newCamTargetPosition = Vector3.Lerp(_weapon.Owner.transform.position, Target.transform.position, CameraTargetDistance);
			_newCamTargetDirection = _newCamTargetPosition - this.transform.position;
            
			if (_newCamTargetDirection.magnitude > CameraTargetMaxDistance)
			{
				_newCamTargetDirection = _newCamTargetDirection.normalized * CameraTargetMaxDistance;
			}

			_newCamTargetPosition = this.transform.position + _newCamTargetDirection;

			_newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position,
				_newCamTargetPosition,
				Time.deltaTime * CameraTargetSpeed);

			_weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
		}

		/// <summary>
		/// Performs a periodic scan
		/// </summary>
		protected virtual void ScanIfNeeded()
		{
			if (Time.time - _lastScanTimestamp > DurationBetweenScans)
			{
				ScanForTargets();
				_lastScanTimestamp = Time.time;
			}
		}

		/// <summary>
		/// Sets aim if needed, otherwise reverts to the previous aim control mode
		/// </summary>
		protected virtual void HandleTarget()
		{
			if (Target == null)
			{
				_weaponAim.AimControl = _originalAimControl;
				_weaponAim.RotationMode = _originalRotationMode;
				_weaponAim.MoveCameraTargetTowardsReticle = _originalMoveCameraTarget;
			}
			else
			{
				_weaponAim.AimControl = WeaponAim.AimControls.Script;
				_weaponAim.RotationMode = RotationMode;
				if (MoveCameraTarget)
				{
					_weaponAim.MoveCameraTargetTowardsReticle = false;
				}
				SetAim();
			}
		}

		/// <summary>
		/// Draws a sphere around the weapon to show its auto aim radius
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			if (DrawDebugRadius)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(_raycastOrigin, ScanRadius);
			}
		}

		/// <summary>
		/// On Disable, we hide our aim marker if needed
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_aimMarker != null)
			{
				_aimMarker.Disable();
			}
		}

		protected void OnDestroy()
		{
			if (DestroyAimMarkerOnWeaponDestroy && (_aimMarker != null))
			{
				Destroy(_aimMarker.gameObject);
			}
		}
	}
}