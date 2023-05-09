using System;
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(CharacterController))]
	[AddComponentMenu("TopDown Engine/Character/Core/TopDown Controller 3D")]

	/// <summary>
	/// A controller, initially adapted from Unity's CharacterMotor.js, to add on top of Unity's native CharacterController, that will handle 
	/// </summary>
	public class TopDownController3D : TopDownController
	{
		/// the possible modes to compute grounded state. Simple should only be used if your ground is even and flat
		public enum GroundedComputationModes { Simple, Advanced }
        
		/// the current input sent to this character
		[MMReadOnly]
		[Tooltip("the current input sent to this character")]
		public Vector3 InputMoveDirection = Vector3.zero;

		/// the different possible update modes
		public enum UpdateModes { Update, FixedUpdate }
		/// the possible ways to transfer velocity on jump
		public enum VelocityTransferOnJump { NoTransfer, InitialVelocity, FloorVelocity, Relative }

		[Header("Settings")]
		/// whether the movement computation should occur at Update or FixedUpdate. FixedUpdate is the recommended choice.
		[Tooltip("whether the movement computation should occur at Update or FixedUpdate. FixedUpdate is the recommended choice.")]
		public UpdateModes UpdateMode = UpdateModes.FixedUpdate;
		/// how the velocity should be affected when jumping from a moving ground
		[Tooltip("how the velocity should be affected when jumping from a moving ground")]
		public VelocityTransferOnJump VelocityTransferMethod = VelocityTransferOnJump.FloorVelocity;
        
		[Header("Raycasts")]
		/// the layer to consider as obstacles (will prevent movement)
		[Tooltip("the layer to consider as obstacles (will prevent movement)")]
		public LayerMask ObstaclesLayerMask = LayerManager.ObstaclesLayerMask;
		/// the length of the raycasts to cast downwards
		[Tooltip("the length of the raycasts to cast downwards")]
		public float GroundedRaycastLength = 5f;
		/// the distance to the ground beyond which the character isn't considered grounded anymore
		[Tooltip("the distance to the ground beyond which the character isn't considered grounded anymore")]
		public float MinimumGroundedDistance = 0.2f;
		/// the selected modes to compute grounded state. Simple should only be used if your ground is even and flat
		[Tooltip("the selected modes to compute grounded state. Simple should only be used if your ground is even and flat")]
		public GroundedComputationModes GroundedComputationMode = GroundedComputationModes.Advanced;
		/// a threshold against which to check when going over steps. Adjust that value if your character has issues going over small steps
		[Tooltip("a threshold against which to check when going over steps. Adjust that value if your character has issues going over small steps")] 
		public float GroundNormalHeightThreshold = 0.2f;
		/// the speed at which external forces get lerped to zero
		[Tooltip("the speed at which external forces get lerped to zero")]
		public float ImpactFalloff = 5f;
        
		[Header("Movement")]

		/// the maximum vertical velocity the character can have while falling
		[Tooltip("the maximum vertical velocity the character can have while falling")]
		public float MaximumFallSpeed = 20.0f;
		/// the factor by which to multiply the speed while walking on a slope. x is the angle, y is the factor
		[Tooltip("the factor by which to multiply the speed while walking on a slope. x is the angle, y is the factor")]
		public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));

		[Header("Steep Surfaces")]
		/// the current surface normal vector
		[Tooltip("the current surface normal vector")]
		[MMReadOnly]
		public Vector3 GroundNormal = Vector3.zero;
		/// whether or not the character should slide while standing on steep surfaces
		[Tooltip("whether or not the character should slide while standing on steep surfaces")]
		public bool SlideOnSteepSurfaces = true;
		/// the speed at which the character should slide
		[Tooltip("the speed at which the character should slide")]
		public float SlidingSpeed = 15f;
		/// the control the player has on the speed while sliding down
		[Tooltip("the control the player has on the speed while sliding down")]
		public float SlidingSpeedControl = 0.4f;
		/// the control the player has on the direction while sliding down
		[Tooltip("the control the player has on the direction while sliding down")]
		public float SlidingDirectionControl = 1f;

		/// returns the center coordinate of the collider
		public override Vector3 ColliderCenter { get { return this.transform.position + _characterController.center; } }
		/// returns the bottom coordinate of the collider
		public override Vector3 ColliderBottom { get { return this.transform.position + _characterController.center + Vector3.down * _characterController.bounds.extents.y; } }
		/// returns the top coordinate of the collider
		public override Vector3 ColliderTop { get { return this.transform.position + _characterController.center + Vector3.up * _characterController.bounds.extents.y; } }
		/// whether or not the character is sliding down a steep slope
		public virtual bool IsSliding() { return (Grounded && SlideOnSteepSurfaces && TooSteep()); }
		/// whether or not the character is colliding above
		public virtual bool CollidingAbove() { return (_collisionFlags & CollisionFlags.CollidedAbove) != 0; }
		/// whether or not the current surface is too steep or not
		public virtual bool TooSteep() { return (GroundNormal.y <= Mathf.Cos(_characterController.slopeLimit * Mathf.Deg2Rad)); }
		/// whether or not the character just entered a slope/ground not too steep this frame
		public virtual bool ExitedTooSteepSlopeThisFrame { get; set; }
		///  whether or not the character is on a moving platform
		public override bool OnAMovingPlatform { get { return ShouldMoveWithPlatformThisFrame(); } }
		/// the speed of the moving platform
		public override Vector3 MovingPlatformSpeed
		{
			get { return _movingPlatformVelocity;  }
		}
        
		protected Transform _transform;
		protected Rigidbody _rigidBody;
		protected Collider _collider;
		protected CharacterController _characterController;
		protected float _originalColliderHeight;
		protected Vector3 _originalColliderCenter;
		protected Vector3 _originalSizeRaycastOrigin;
		protected Vector3 _lastGroundNormal = Vector3.zero;
		protected WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
		protected bool _detached = false;

		// moving platforms
		protected Transform _movingPlatformHitCollider;
		protected Transform _movingPlatformCurrentHitCollider;
		protected Vector3 _movingPlatformCurrentHitColliderLocal;
		protected Vector3 _movingPlatformCurrentGlobalPoint;
		protected Quaternion _movingPlatformLocalRotation;
		protected Quaternion _movingPlatformGlobalRotation;
		protected Matrix4x4 _lastMovingPlatformMatrix;
		protected Vector3 _movingPlatformVelocity;
		protected bool _newMovingPlatform;

		// char movement
		protected CollisionFlags _collisionFlags;
		protected Vector3 _frameVelocity = Vector3.zero;
		protected Vector3 _hitPoint = Vector3.zero;
		protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

		// velocity
		protected Vector3 _newVelocity;
		protected Vector3 _lastHorizontalVelocity;
		protected Vector3 _newHorizontalVelocity;
		protected Vector3 _motion;
		protected Vector3 _idealVelocity;
		protected Vector3 _idealDirection;
		protected Vector3 _horizontalVelocityDelta;
		protected float _stickyOffset = 0f;

		// move position
		protected RaycastHit _movePositionHit;
		protected Vector3 _capsulePoint1;
		protected Vector3 _capsulePoint2;
		protected Vector3 _movePositionDirection;
		protected float _movePositionDistance;

		// collision detection
		protected RaycastHit _cardinalRaycast;
        
		protected float _smallestDistance = Single.MaxValue;
		protected float _longestDistance = Single.MinValue;

		protected RaycastHit _smallestRaycast;
		protected RaycastHit _emptyRaycast = new RaycastHit();
		protected Vector3 _downRaycastsOffset;

		protected Vector3 _moveWithPlatformMoveDistance;
		protected Vector3 _moveWithPlatformGlobalPoint;
		protected Quaternion _moveWithPlatformGlobalRotation;
		protected Quaternion _moveWithPlatformRotationDiff;
		protected RaycastHit _raycastDownHit;
		protected Vector3 _raycastDownDirection = Vector3.down;
		protected RaycastHit _canGoBackHeadCheck;
		protected bool _tooSteepLastFrame;

		/// <summary>
		/// On awake we store our various components for future use
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			_characterController = this.gameObject.GetComponent<CharacterController>();
			_transform = this.transform;
			_rigidBody = this.gameObject.GetComponent<Rigidbody>();
			_collider = this.gameObject.GetComponent<Collider>();
			_originalColliderHeight = _characterController.height;
			_originalColliderCenter = _characterController.center;
		}

		#region Update

		/// <summary>
		/// On late update we apply any impact we have in store, and store our velocity for use next frame
		/// </summary>
		protected override void LateUpdate()
		{
			base.LateUpdate();
			VelocityLastFrame = Velocity;
		}

		/// <summary>
		/// On Update we process our Update computations if UpdateMode is set to Update
		/// </summary>
		protected override void Update()
		{
			base.Update();
			if (UpdateMode == UpdateModes.Update)
			{
				ProcessUpdate();
			}
		}

		/// <summary>
		/// On FixedUpdate we process our Update computations if UpdateMode is set to FixedUpdate
		/// </summary>
		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			ApplyImpact();
			GetMovingPlatformVelocity();
			if (UpdateMode == UpdateModes.FixedUpdate)
			{
				ProcessUpdate();
			}
		}
                
		/// <summary>
		/// Computes the new velocity and moves the character
		/// </summary>
		protected virtual void ProcessUpdate()
		{
			if (_transform == null)
			{
				return;
			}

			if (!FreeMovement)
			{
				return;
			}

			_newVelocity = Velocity;
			_positionLastFrame = _transform.position;

			AddInput();
			AddGravity();
			MoveWithPlatform();
			ComputeVelocityDelta();
			MoveCharacterController();
			DetectNewMovingPlatform();
			ComputeNewVelocity();
			ManualControllerColliderHit();
			HandleGroundContact();
			ComputeSpeed();
		}

		/// <summary>
		/// Determines the new velocity based on the slope we're on and the input 
		/// </summary>
		protected virtual void AddInput()
		{
			if (Grounded && TooSteep())
			{
				_idealVelocity.x = GroundNormal.x;
				_idealVelocity.y = 0;
				_idealVelocity.z = GroundNormal.z;
				_idealVelocity = _idealVelocity.normalized;
				_idealDirection = Vector3.Project(InputMoveDirection, _idealVelocity);
				_idealVelocity = _idealVelocity + (_idealDirection * SlidingSpeedControl) + (InputMoveDirection - _idealDirection) * SlidingDirectionControl;
				_idealVelocity = _idealVelocity * SlidingSpeed;
			}
			else
			{
				_idealVelocity = CurrentMovement;
			}

			if (VelocityTransferMethod == VelocityTransferOnJump.FloorVelocity)
			{
				_idealVelocity += _frameVelocity;
				_idealVelocity.y = 0;
			}

			if (Grounded)
			{
				Vector3 sideways = Vector3.Cross(Vector3.up, _idealVelocity);
				_idealVelocity = Vector3.Cross(sideways, GroundNormal).normalized * _idealVelocity.magnitude;
			}

			_newVelocity = _idealVelocity;
			_newVelocity.y = Grounded ? Mathf.Min(_newVelocity.y, 0) : _newVelocity.y;
		}

		/// <summary>
		/// Adds the gravity to the new velocity and any AddedForce we may have
		/// </summary>
		protected virtual void AddGravity()
		{
			if (GravityActive)
			{
				if (Grounded)
				{
					_newVelocity.y = Mathf.Min(0, _newVelocity.y) - Gravity * Time.deltaTime;
				}
				else
				{
					_newVelocity.y = Velocity.y - Gravity * Time.deltaTime;
					_newVelocity.y = Mathf.Max(_newVelocity.y, -MaximumFallSpeed);
				}
			}
			_newVelocity += AddedForce;
			AddedForce = Vector3.zero;
		}
        
		/// <summary>
		/// Moves and rotates the character controller to follow any moving platform we may be standing on
		/// </summary>
		protected virtual void MoveWithPlatform()
		{
			if (ShouldMoveWithPlatformThisFrame())
			{
				_moveWithPlatformMoveDistance.x = _moveWithPlatformMoveDistance.y = _moveWithPlatformMoveDistance.z = 0f;
				_moveWithPlatformGlobalPoint = _movingPlatformCurrentHitCollider.TransformPoint(_movingPlatformCurrentHitColliderLocal);
				_moveWithPlatformMoveDistance = (_moveWithPlatformGlobalPoint - _movingPlatformCurrentGlobalPoint);
				if (_moveWithPlatformMoveDistance != Vector3.zero)
				{
					_characterController.Move(_moveWithPlatformMoveDistance);
				}
				_moveWithPlatformGlobalRotation = _movingPlatformCurrentHitCollider.rotation * _movingPlatformLocalRotation;
				_moveWithPlatformRotationDiff = _moveWithPlatformGlobalRotation * Quaternion.Inverse(_movingPlatformGlobalRotation);
				float yRotation = _moveWithPlatformRotationDiff.eulerAngles.y;
				if (yRotation != 0)
				{
					_transform.Rotate(0, yRotation, 0);
				}
			}
		}

		/// <summary>
		/// Computes the motion vector to apply to the character controller 
		/// </summary>
		protected virtual void ComputeVelocityDelta()
		{
			_motion = _newVelocity * Time.deltaTime;
			_horizontalVelocityDelta.x = _motion.x;
			_horizontalVelocityDelta.y = 0f;
			_horizontalVelocityDelta.z = _motion.z;
			_stickyOffset = Mathf.Max(_characterController.stepOffset, _horizontalVelocityDelta.magnitude);
			if (Grounded)
			{
				_motion -= _stickyOffset * Vector3.up;
			}
		}

		/// <summary>
		/// Moves the character controller by the computed _motion 
		/// </summary>
		protected virtual void MoveCharacterController()
		{
			GroundNormal.x = GroundNormal.y = GroundNormal.z = 0f;

			_collisionFlags = _characterController.Move(_motion); // controller move

			_lastHitPoint = _hitPoint;
			_lastGroundNormal = GroundNormal;
		}

		/// <summary>
		/// Detects any moving platform we may be standing on
		/// </summary>
		protected virtual void DetectNewMovingPlatform()
		{
			if (_movingPlatformCurrentHitCollider != _movingPlatformHitCollider)
			{
				if (_movingPlatformHitCollider != null)
				{
					_movingPlatformCurrentHitCollider = _movingPlatformHitCollider;
					_lastMovingPlatformMatrix = _movingPlatformHitCollider.localToWorldMatrix;
					_newMovingPlatform = true;
				}
			}
		}

		/// <summary>
		/// Determines the new Velocity value based on our position and our position last frame
		/// </summary>
		protected virtual void ComputeNewVelocity()
		{
			Velocity = _newVelocity;
			Acceleration = (Velocity - VelocityLastFrame) / Time.deltaTime;
		}

		/// <summary>
		/// We handle ground contact, velocity transfer and moving platforms
		/// </summary>
		protected virtual void HandleGroundContact()
		{
			Grounded = _characterController.isGrounded;
            
			if (Grounded && !IsGroundedTest())
			{
				Grounded = false;
				if ((VelocityTransferMethod == VelocityTransferOnJump.InitialVelocity ||
				     VelocityTransferMethod == VelocityTransferOnJump.FloorVelocity))
				{
					_frameVelocity = _movingPlatformVelocity;
					Velocity += _movingPlatformVelocity;
				}
			}
			else if (!Grounded && IsGroundedTest())
			{
				if (_detached && Velocity.y <= 0)
				{
					Grounded = true;
					_detached = false;
				}
				SubstractNewPlatformVelocity();
			}

			if (ShouldMoveWithPlatformThisFrame())
			{
				_movingPlatformCurrentHitColliderLocal = _movingPlatformCurrentHitCollider.InverseTransformPoint(_movingPlatformCurrentGlobalPoint);
				_movingPlatformGlobalRotation = _transform.rotation;
				_movingPlatformLocalRotation = Quaternion.Inverse(_movingPlatformCurrentHitCollider.rotation) * _movingPlatformGlobalRotation;
			}

			ExitedTooSteepSlopeThisFrame = (_tooSteepLastFrame && !TooSteep());
            
			_tooSteepLastFrame = TooSteep();
		}

		/// <summary>
		/// Determines the direction based on the current movement
		/// </summary>
		protected override void DetermineDirection()
		{
			if (CurrentMovement.magnitude > 0f)
			{
				CurrentDirection = CurrentMovement.normalized;
			}
		}
        
		/// <summary>
		/// Handles friction with ground surfaces, coming soon
		/// </summary>
		protected override void HandleFriction()
		{
			//TODO - coming soon
		}

		/// <summary>
		/// Forces the controller to detach from the ground
		/// </summary>
		public virtual void DetachFromGround()
		{
			_detached = true;
		}

		public virtual void DetachFromMovingPlatform()
		{
			_movingPlatformVelocity = Vector3.zero;
			_movingPlatformCurrentHitCollider = null;
			_movingPlatformHitCollider = null;
		}

		#endregion

		#region Rigidbody push mechanics

		/// <summary>
		/// This method compensates for the regular OnControllerColliderHit, which unfortunately generates a lot of garbage.
		/// To do so, it casts a ray downwards to get our ground normal, and a ray in the current movement direction to (potentially) push rigidbodies
		/// </summary>
        
		protected virtual void ManualControllerColliderHit()
		{
			HandleAdvancedGroundDetection();
		}

		protected virtual void HandleAdvancedGroundDetection()
		{
			if (GroundedComputationMode != GroundedComputationModes.Advanced)
			{
				return;
			}
            
			_smallestDistance = Single.MaxValue;
			_longestDistance = Single.MinValue;
			_smallestRaycast = _emptyRaycast;

			// we cast 4 rays downwards to get ground normal
			float offset = _characterController.radius;
            
			_downRaycastsOffset.x = 0f;
			_downRaycastsOffset.y = 0f;
			_downRaycastsOffset.z = 0f;
			CastRayDownwards();
			_downRaycastsOffset.x = -offset;
			_downRaycastsOffset.y = offset;
			_downRaycastsOffset.z = 0f;
			CastRayDownwards();
			_downRaycastsOffset.x = 0f;
			_downRaycastsOffset.y = offset;
			_downRaycastsOffset.z = -offset;
			CastRayDownwards();
			_downRaycastsOffset.x = offset;
			_downRaycastsOffset.y = offset;
			_downRaycastsOffset.z = 0f;
			CastRayDownwards();
			_downRaycastsOffset.x = 0f;
			_downRaycastsOffset.y = offset;
			_downRaycastsOffset.z = offset;
			CastRayDownwards();
            
			// we handle our shortest ray
			if (_smallestRaycast.collider != null)
			{
				float adjustedDistance = AdjustDistance(_smallestRaycast.distance);
                
				if (_smallestRaycast.normal.y > 0 && _smallestRaycast.normal.y > GroundNormal.y )
				{
					if ( (Mathf.Abs(_smallestRaycast.point.y - _lastHitPoint.y) < GroundNormalHeightThreshold) && ( (_smallestRaycast.point != _lastHitPoint) || (_lastGroundNormal == Vector3.zero)))
					{
						GroundNormal = _smallestRaycast.normal;
					}
					else
					{
						GroundNormal = _lastGroundNormal;
					}
                    
					_movingPlatformHitCollider = _smallestRaycast.collider.transform;
					_hitPoint = _smallestRaycast.point;
					_frameVelocity.x = _frameVelocity.y = _frameVelocity.z = 0f;
				}    
			} 
		}
        
		/// <summary>
		/// Casts a ray downwards and adjusts distances if needed
		/// </summary>
		protected virtual void CastRayDownwards()
		{
			if (_smallestDistance <= MinimumGroundedDistance)
			{
				return;
			}
            
			Physics.Raycast(this._transform.position + _characterController.center + _downRaycastsOffset, _raycastDownDirection, out _raycastDownHit, 
				_characterController.height/2f + GroundedRaycastLength, ObstaclesLayerMask);
            
			if (_raycastDownHit.collider != null)
			{
				float adjustedDistance = AdjustDistance(_raycastDownHit.distance);
                
				if (adjustedDistance < _smallestDistance) { _smallestDistance = adjustedDistance; _smallestRaycast = _raycastDownHit; }
				if (adjustedDistance > _longestDistance) { _longestDistance = adjustedDistance; }
			}
		}

		/// <summary>
		/// Returns the real distance between the extremity of the character and the ground
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		protected float AdjustDistance(float distance)
		{
			float adjustedDistance = distance - _characterController.height / 2f -
			                         _characterController.skinWidth;
			return adjustedDistance;
		}

		protected Vector3 _onTriggerEnterPushbackDirection;
        
		/// <summary>
		/// When triggering with something else, we check if it's a moving platform and we push ourselves if needed
		/// </summary>
		/// <param name="other"></param>
		protected virtual void OnTriggerEnter(Collider other)
		{
			// on trigger enter, if we're colliding with a moving platform, we push ourselves in the opposite direction
			if (other.gameObject.MMGetComponentNoAlloc<MovingPlatform3D>() != null)
			{
				if (this.transform.position.y < other.transform.position.y)
				{
					_onTriggerEnterPushbackDirection = (this.transform.position - other.transform.position).normalized;
					this.Impact(_onTriggerEnterPushbackDirection.normalized, other.gameObject.MMGetComponentNoAlloc<MovingPlatform3D>().PushForce);
				}
			}
		}

		#endregion

		#region Moving Platforms
        
		/// <summary>
		/// Gets the current moving platform's velocity
		/// </summary>
		protected virtual void GetMovingPlatformVelocity()
		{
			if (_movingPlatformCurrentHitCollider != null)
			{
				if (!_newMovingPlatform && (Time.deltaTime != 0f))
				{
					_movingPlatformVelocity = (
						_movingPlatformCurrentHitCollider.localToWorldMatrix.MultiplyPoint3x4(_movingPlatformCurrentHitColliderLocal)
						- _lastMovingPlatformMatrix.MultiplyPoint3x4(_movingPlatformCurrentHitColliderLocal)
					) / Time.deltaTime;
				}
				_lastMovingPlatformMatrix = _movingPlatformCurrentHitCollider.localToWorldMatrix;
				_newMovingPlatform = false;
			}
			else
			{
				_movingPlatformVelocity = Vector3.zero;
			}
		}

		/// <summary>
		/// Computes the relative velocity
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator SubstractNewPlatformVelocity()
		{
			if ((VelocityTransferMethod == VelocityTransferOnJump.InitialVelocity ||
			     VelocityTransferMethod == VelocityTransferOnJump.FloorVelocity))
			{
				if (_newMovingPlatform)
				{
					Transform platform = _movingPlatformCurrentHitCollider;
					yield return _waitForFixedUpdate;
					if (Grounded && platform == _movingPlatformCurrentHitCollider)
					{
						yield return 1;
					}
				}
				Velocity -= _movingPlatformVelocity;
			}
		}

		/// <summary>
		/// Whether or not our character should move with the moving platform this frame
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldMoveWithPlatformThisFrame()
		{
			return (
				(Grounded || VelocityTransferMethod == VelocityTransferOnJump.Relative)
				&& _movingPlatformCurrentHitCollider != null
			);
		}

		#endregion

		#region Collider Resizing
        
		/// <summary>
		/// Determines whether this instance can go back to original size.
		/// </summary>
		/// <returns><c>true</c> if this instance can go back to original size; otherwise, <c>false</c>.</returns>
		public override bool CanGoBackToOriginalSize()
		{
			// if we're already at original size, we return true
			if (_collider.bounds.size.y == _originalColliderHeight)
			{
				return true;
			}
			float headCheckDistance = _originalColliderHeight * transform.localScale.y * CrouchedRaycastLengthMultiplier;

			// we cast two rays above our character to check for obstacles. If we didn't hit anything, we can go back to original size, otherwise we can't
			_originalSizeRaycastOrigin = ColliderTop + transform.up * _smallValue;

			_canGoBackHeadCheck = MMDebug.Raycast3D(_originalSizeRaycastOrigin, transform.up, headCheckDistance, ObstaclesLayerMask, Color.cyan, true);
			if (_canGoBackHeadCheck.collider != null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Resizes the collider to the new size set in parameters
		/// </summary>
		/// <param name="newSize">New size.</param>
		public override void ResizeColliderHeight(float newHeight, bool translateCenter = false)
		{
			float newYOffset = _originalColliderCenter.y - (_originalColliderHeight - newHeight) / 2;
			_characterController.height = newHeight;
			_characterController.center = ((_originalColliderHeight - newHeight) / 2) * Vector3.up;

			if (translateCenter)
			{
				this.transform.Translate((newYOffset / 2f) * Vector3.up);	
			}
		}

		/// <summary>
		/// Returns the collider to its initial size
		/// </summary>
		public override void ResetColliderSize()
		{
			_characterController.height = _originalColliderHeight;
			_characterController.center = _originalColliderCenter;
		}

		#endregion

		#region Grounded Tests
        
		/// <summary>
		/// Whether or not the character is grounded
		/// </summary>
		/// <returns></returns>
		public virtual bool IsGroundedTest()
		{
			bool grounded = false;
			if (GroundedComputationMode == GroundedComputationModes.Advanced)
			{
				if (_smallestDistance <= MinimumGroundedDistance)
				{
					grounded = (GroundNormal.y > 0.01) ;
				}    
			}
			else
			{
				grounded = true;
				GroundNormal.x = 0;
				GroundNormal.y = 1;
				GroundNormal.z = 0;
			}
            
			return grounded;
		}

		/// <summary>
		/// Grounded check
		/// </summary>
		protected override void CheckIfGrounded()
		{
			JustGotGrounded = (!_groundedLastFrame && Grounded);
			_groundedLastFrame = Grounded;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Enables the collider
		/// </summary>
		public override void CollisionsOn()
		{
			_collider.enabled = true;
		}

		/// <summary>
		/// Disables collider
		/// </summary>
		public override void CollisionsOff()
		{
			_collider.enabled = false;
		}

		/// <summary>
		/// Performs a cardinal collision check and stores collision objects informations
		/// </summary>
		/// <param name="distance"></param>
		/// <param name="offset"></param>
		public override void DetectObstacles(float distance, Vector3 offset)
		{
			if (!PerformCardinalObstacleRaycastDetection)
			{
				return;
			}
            
			CollidingWithCardinalObstacle = false;
			// right
			_cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.right, distance, ObstaclesLayerMask, Color.yellow, true);
			if (_cardinalRaycast.collider != null) { DetectedObstacleRight = _cardinalRaycast.collider.gameObject; CollidingWithCardinalObstacle = true; } else { DetectedObstacleRight = null; }
			// left
			_cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.left, distance, ObstaclesLayerMask, Color.yellow, true);
			if (_cardinalRaycast.collider != null) { DetectedObstacleLeft = _cardinalRaycast.collider.gameObject; CollidingWithCardinalObstacle = true; } else { DetectedObstacleLeft = null; }
			// up
			_cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.forward, distance, ObstaclesLayerMask, Color.yellow, true);
			if (_cardinalRaycast.collider != null) { DetectedObstacleUp = _cardinalRaycast.collider.gameObject; CollidingWithCardinalObstacle = true; } else { DetectedObstacleUp = null; }
			// down
			_cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.back, distance, ObstaclesLayerMask, Color.yellow, true);
			if (_cardinalRaycast.collider != null) { DetectedObstacleDown = _cardinalRaycast.collider.gameObject; CollidingWithCardinalObstacle = true; } else { DetectedObstacleDown = null; }
		}

		/// <summary>
		/// Applies the stored impact to the character
		/// </summary>
		protected virtual void ApplyImpact()
		{
			if (_impact.magnitude > 0.2f)
			{
				_characterController.Move(_impact * Time.deltaTime);
			}
			_impact = Vector3.Lerp(_impact, Vector3.zero, ImpactFalloff * Time.deltaTime);
		}

		/// <summary>
		/// Adds a force of the specified direction and magnitude to the character
		/// </summary>
		/// <param name="movement"></param>
		public override void AddForce(Vector3 movement)
		{
			AddedForce += movement;
		}

		/// <summary>
		/// Applies an impact to the character of the specified direction and force
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="force"></param>
		public override void Impact(Vector3 direction, float force)
		{
			direction = direction.normalized;
			if (direction.y < 0) { direction.y = -direction.y; }
			_impact += direction.normalized * force;
		}

		/// <summary>
		/// Sets the character's current input direction and magnitude
		/// </summary>
		/// <param name="movement"></param>
		public override void SetMovement(Vector3 movement)
		{
			CurrentMovement = movement;

			Vector3 directionVector;
			directionVector = movement;
			if (directionVector != Vector3.zero)
			{
				float directionLength = directionVector.magnitude;
				directionVector = directionVector / directionLength;
				directionLength = Mathf.Min(1, directionLength);
				directionLength = directionLength * directionLength;
				directionVector = directionVector * directionLength;
			}
			InputMoveDirection = transform.rotation * directionVector;
		}

		/// <summary>
		/// Turns this character's rigidbody kinematic or not
		/// </summary>
		/// <param name="state"></param>
		public override void SetKinematic(bool state)
		{
			_rigidBody.isKinematic = state;
		}

		/// <summary>
		/// Moves this character to the specified position while trying to avoid obstacles
		/// </summary>
		/// <param name="newPosition"></param>
		public override void MovePosition(Vector3 newPosition)
		{
            
			_movePositionDirection = (newPosition - this.transform.position);
			_movePositionDistance = Vector3.Distance(this.transform.position, newPosition) ;

			_capsulePoint1 =    this.transform.position 
			                    + _characterController.center 
			                    - (Vector3.up * _characterController.height / 2f) 
			                    + Vector3.up * _characterController.skinWidth 
			                    + Vector3.up * _characterController.radius;
			_capsulePoint2 =    this.transform.position
			                    + _characterController.center
			                    + (Vector3.up * _characterController.height / 2f)
			                    - Vector3.up * _characterController.skinWidth
			                    - Vector3.up * _characterController.radius;

			if (!Physics.CapsuleCast(_capsulePoint1, _capsulePoint2, _characterController.radius, _movePositionDirection, out _movePositionHit, _movePositionDistance, ObstaclesLayerMask))
			{
				this.transform.position = newPosition;
			}
		}

		/// <summary>
		/// On reset, we reset vectors and transforms
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			_idealDirection = Vector3.zero;
			_idealVelocity = Vector3.zero;
			_newVelocity = Vector3.zero;
			_movingPlatformVelocity = Vector3.zero;
			_movingPlatformCurrentHitCollider = null;
			_movingPlatformHitCollider = null;
			_lastGroundNormal = Vector3.zero;
			_detached = false;
			_frameVelocity = Vector3.zero;
			_hitPoint = Vector3.zero;
			_lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
		}

		#endregion
        
	}
}