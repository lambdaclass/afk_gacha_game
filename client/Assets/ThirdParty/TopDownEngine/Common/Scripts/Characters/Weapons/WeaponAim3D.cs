using UnityEngine;
using MoreMountains.Tools;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace MoreMountains.TopDownEngine
{	
	[RequireComponent(typeof(Weapon))]
	/// <summary>
	/// Add this component to a Weapon and you'll be able to aim it (meaning you'll rotate it)
	/// Supported control modes are mouse, primary movement (you aim wherever you direct your character) and secondary movement (using a secondary axis, separate from the movement).
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Aim 3D")]
	public class WeaponAim3D : WeaponAim
	{
		[Header("3D")] 
		/// if this is true, aim will be unrestricted to angles, and will aim freely in all 3 axis, useful when dealing with AI and elevation
		[Tooltip("if this is true, aim will be unrestricted to angles, and will aim freely in all 3 axis, useful when dealing with AI and elevation")]
		public bool Unrestricted3DAim = false;
	    
		[Header("Reticle and slopes")]
		/// whether or not the reticle should move vertically to stay above slopes
		[MMEnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
		[Tooltip("whether or not the reticle should move vertically to stay above slopes")]
		public bool ReticleMovesWithSlopes = false;
		/// the layers the reticle should consider as obstacles to move on
		[MMEnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
		[Tooltip("the layers the reticle should consider as obstacles")]
		public LayerMask ReticleObstacleMask = LayerManager.ObstaclesLayerMask;
		/// the maximum slope elevation for the reticle
		[MMEnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
		[Tooltip("the maximum slope elevation for the reticle")]
		public float MaximumSlopeElevation = 50f;
		/// if this is true, the aim system will try to compensate when aim direction is null (for example when you haven't set any primary input yet)
		[Tooltip("if this is true, the aim system will try to compensate when aim direction is null (for example when you haven't set any primary input yet)")]
		public bool AvoidNullAim = true;

		protected Vector2 _inputMovement;
		protected Vector3 _slopeTargetPosition;
		protected Vector3 _weaponAimCurrentAim;

		protected override void Initialization()
		{
			if (_initialized)
			{
				return;
			}
			base.Initialization();
			_mainCamera = Camera.main;
		}

		protected virtual void Reset()
		{
			ReticleObstacleMask = LayerMask.NameToLayer("Ground");
		}

		/// <summary>
		/// Computes the current aim direction
		/// </summary>
		protected override void GetCurrentAim()
		{
			if (_weapon.Owner == null)
			{
				return;
			}

			if ((_weapon.Owner.LinkedInputManager == null) && (_weapon.Owner.CharacterType == Character.CharacterTypes.Player))
			{
				return;
			}

			if ((_weapon.Owner.ConditionState.CurrentState != CharacterStates.CharacterConditions.Normal) &&
			    (_weapon.Owner.ConditionState.CurrentState != CharacterStates.CharacterConditions.ControlledMovement))
			{
				return;
			}

			AutoDetectWeaponMode();             

			switch (AimControl)
			{
				case AimControls.Off:
					if (_weapon.Owner == null) { return; }
					GetOffAim();
					break;

				case AimControls.Script:
					GetScriptAim();
					break;

				case AimControls.PrimaryMovement:
					if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
					{
						return;
					}
					GetPrimaryMovementAim();
					break;

				case AimControls.SecondaryMovement:
					if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
					{
						return;
					}
					GetSecondaryMovementAim();
					break;

				case AimControls.SecondaryThenPrimaryMovement:
					if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
					{
						return;
					}

					if (_weapon.Owner.LinkedInputManager.SecondaryMovement.magnitude > MinimumMagnitude)
					{
						GetSecondaryMovementAim();
					}
					else
					{
						GetPrimaryMovementAim();
					}

					if (_currentAim == Vector3.zero)
					{
						_currentAim = _weapon.Owner.transform.forward;
						_weaponAimCurrentAim = _currentAim;
						_direction = transform.position + _currentAim;
					}                    

					break;

				case AimControls.PrimaryThenSecondaryMovement:
					if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
					{
						return;
					}

					if (_weapon.Owner.LinkedInputManager.PrimaryMovement.magnitude > MinimumMagnitude)
					{
						GetPrimaryMovementAim();
					}
					else
					{
						GetSecondaryMovementAim();
					}
					break;

				case AimControls.Mouse:
					if (_weapon.Owner == null)
					{
						return;
					}
					GetMouseAim();					
					break;

				case AimControls.CharacterRotateCameraDirection:
					if (_weapon.Owner == null)
					{
						return;
					}
					_currentAim = _weapon.Owner.CameraDirection;
					_weaponAimCurrentAim = _currentAim;
					_direction = transform.position + _currentAim;
					break;
			}
			
			if (AvoidNullAim && (_currentAim == Vector3.zero))
			{
				GetOffAim();
			}
		}

		public virtual void GetOffAim()
		{
			_currentAim = Vector3.right;
			_weaponAimCurrentAim = _currentAim;
			_direction = Vector3.right;
		}

		public virtual void GetPrimaryMovementAim()
		{
			if (_lastNonNullMovement == Vector2.zero)
			{
				_lastNonNullMovement = _weapon.Owner.LinkedInputManager.LastNonNullPrimaryMovement;
			}

			_inputMovement = _weapon.Owner.LinkedInputManager.PrimaryMovement;
			_inputMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;

			_currentAim.x = _inputMovement.x;
			_currentAim.y = 0f;
			_currentAim.z = _inputMovement.y;
			_weaponAimCurrentAim = _currentAim;
			_direction = transform.position + _currentAim;

			_lastNonNullMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;
		}

		public virtual void GetSecondaryMovementAim()
		{
			if (_lastNonNullMovement == Vector2.zero)
			{
				_lastNonNullMovement = _weapon.Owner.LinkedInputManager.LastNonNullSecondaryMovement;
			}

			_inputMovement = _weapon.Owner.LinkedInputManager.SecondaryMovement;
			_inputMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;

			_currentAim.x = _inputMovement.x;
			_currentAim.y = 0f;
			_currentAim.z = _inputMovement.y;
			_weaponAimCurrentAim = _currentAim;
			_direction = transform.position + _currentAim;

			_lastNonNullMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;
		}

		public virtual void GetScriptAim()
		{
			_direction = -(transform.position - _currentAim);
			_weaponAimCurrentAim = _currentAim;
		}

		public virtual void GetMouseAim()
		{
			_mousePosition = InputManager.Instance.MousePosition;
			
			Ray ray = _mainCamera.ScreenPointToRay(_mousePosition);
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
			float distance;
			if (_playerPlane.Raycast(ray, out distance))
			{
				Vector3 target = ray.GetPoint(distance);
				_direction = target;
			}
            
			_reticlePosition = _direction;

			if (Vector3.Distance(_direction, transform.position) < MouseDeadZoneRadius)
			{
				_direction = _lastMousePosition;
			}
			else
			{
				_lastMousePosition = _direction;
			}

			_direction.y = transform.position.y;
			_currentAim = _direction - _weapon.Owner.transform.position;
			_weaponAimCurrentAim = _direction - this.transform.position;
		}

		/// <summary>
		/// Every frame, we compute the aim direction and rotate the weapon accordingly
		/// </summary>
		protected override void Update()
		{
			HideMousePointer();
			HideReticle();
			if (GameManager.HasInstance && GameManager.Instance.Paused)
			{
				return;
			}
			GetCurrentAim();
			DetermineWeaponRotation();
		}

		/// <summary>
		/// At fixed update we move the target and reticle
		/// </summary>
		protected virtual void FixedUpdate()
		{
			if (GameManager.Instance.Paused)
			{
				return;
			}
			MoveTarget();
			MoveReticle();
			UpdatePlane();
		}

		protected virtual void UpdatePlane()
		{
			_playerPlane.SetNormalAndPosition (Vector3.up, this.transform.position);
		}

		/// <summary>
		/// Determines the weapon rotation based on the current aim direction
		/// </summary>
		protected override void DetermineWeaponRotation()
		{
			if (ReticleMovesWithSlopes)
			{
				if (Vector3.Distance(_slopeTargetPosition, this.transform.position) < MouseDeadZoneRadius)
				{
					return;
				}
				AimAt(_slopeTargetPosition);

				if (_weaponAimCurrentAim != Vector3.zero)
				{
					if (_direction != Vector3.zero)
					{
						CurrentAngle = Mathf.Atan2 (_weaponAimCurrentAim.z, _weaponAimCurrentAim.x) * Mathf.Rad2Deg;
						CurrentAngleAbsolute = Mathf.Atan2(_weaponAimCurrentAim.y, _weaponAimCurrentAim.x) * Mathf.Rad2Deg;
						if (RotationMode == RotationModes.Strict4Directions || RotationMode == RotationModes.Strict8Directions)
						{
							CurrentAngle = MMMaths.RoundToClosest (CurrentAngle, _possibleAngleValues);
						}
						CurrentAngle += _additionalAngle;
						CurrentAngle = Mathf.Clamp (CurrentAngle, MinimumAngle, MaximumAngle);	
						CurrentAngle = -CurrentAngle + 90f;
						_lookRotation = Quaternion.Euler (CurrentAngle * Vector3.up);
					}
				}

				return;
			}

			if (Unrestricted3DAim)
			{
				AimAt(this.transform.position + _weaponAimCurrentAim);
				return;
			}

			if (_weaponAimCurrentAim != Vector3.zero)
			{
				if (_direction != Vector3.zero)
				{
					CurrentAngle = Mathf.Atan2 (_weaponAimCurrentAim.z, _weaponAimCurrentAim.x) * Mathf.Rad2Deg;
					CurrentAngleAbsolute = Mathf.Atan2(_weaponAimCurrentAim.y, _weaponAimCurrentAim.x) * Mathf.Rad2Deg;
					if (RotationMode == RotationModes.Strict4Directions || RotationMode == RotationModes.Strict8Directions)
					{
						CurrentAngle = MMMaths.RoundToClosest (CurrentAngle, _possibleAngleValues);
					}

					// we add our additional angle
					CurrentAngle += _additionalAngle;

					// we clamp the angle to the min/max values set in the inspector

					CurrentAngle = Mathf.Clamp (CurrentAngle, MinimumAngle, MaximumAngle);	
					CurrentAngle = -CurrentAngle + 90f;

					_lookRotation = Quaternion.Euler (CurrentAngle * Vector3.up);
                    
					RotateWeapon(_lookRotation);
				}
			}
			else
			{
				CurrentAngle = 0f;
				RotateWeapon(_initialRotation);	
			}
		}

		protected override void AimAt(Vector3 target)
		{
			base.AimAt(target);

			_aimAtDirection = target - transform.position;
			_aimAtQuaternion = Quaternion.LookRotation(_aimAtDirection, Vector3.up);
			if (WeaponRotationSpeed == 0f)
			{
				transform.rotation = _aimAtQuaternion;
			}
			else
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, _aimAtQuaternion, WeaponRotationSpeed * Time.deltaTime);	
			}
		}
        
		/// <summary>
		/// Aims the weapon towards a new point
		/// </summary>
		/// <param name="newAim">New aim.</param>
		public override void SetCurrentAim(Vector3 newAim, bool setAimAsLastNonNullMovement = false)
		{
			base.SetCurrentAim(newAim, setAimAsLastNonNullMovement);
	        
			_lastNonNullMovement.x = newAim.x;
			_lastNonNullMovement.y = newAim.z;
		}

		/// <summary>
		/// Initializes the reticle based on the settings defined in the inspector
		/// </summary>
		protected override void InitializeReticle()
		{
			if (_weapon.Owner == null) { return; }
			if (Reticle == null) { return; }
			if (ReticleType == ReticleTypes.None) { return; }

			if (_reticle != null)
			{
				Destroy(_reticle);
			}

			if (ReticleType == ReticleTypes.Scene)
			{
				_reticle = (GameObject)Instantiate(Reticle);

				if (!ReticleAtMousePosition)
				{
					if (_weapon.Owner != null)
					{
						_reticle.transform.SetParent(_weapon.transform);
						_reticle.transform.localPosition = ReticleDistance * Vector3.forward;
					}
				}
			}

			if (ReticleType == ReticleTypes.UI)
			{
				_reticle = (GameObject)Instantiate(Reticle);
				_reticle.transform.SetParent(GUIManager.Instance.MainCanvas.transform);
				_reticle.transform.localScale = Vector3.one;
				if (_reticle.gameObject.MMGetComponentNoAlloc<MMUIFollowMouse>() != null)
				{
					_reticle.gameObject.MMGetComponentNoAlloc<MMUIFollowMouse>().TargetCanvas = GUIManager.Instance.MainCanvas;
				}
			}
		}

		/// <summary>
		/// Every frame, moves the reticle if it's been told to follow the pointer
		/// </summary>
		protected override void MoveReticle()
		{		
			if (ReticleType == ReticleTypes.None) { return; }
			if (_reticle == null) { return; }
			if (_weapon.Owner.ConditionState.CurrentState == CharacterStates.CharacterConditions.Paused) { return; }

			if (ReticleType == ReticleTypes.Scene)
			{
				// if we're not supposed to rotate the reticle, we force its rotation, otherwise we apply the current look rotation
				if (!RotateReticle)
				{
					_reticle.transform.rotation = Quaternion.identity;
				}
				else
				{
					if (ReticleAtMousePosition)
					{
						_reticle.transform.rotation = _lookRotation;
					}
				}

				// if we're in follow mouse mode and the current control scheme is mouse, we move the reticle to the mouse's position
				if (ReticleAtMousePosition && AimControl == AimControls.Mouse)
				{
					_reticle.transform.position = MMMaths.Lerp(_reticle.transform.position, _reticlePosition, 0.3f, Time.deltaTime);
				}
			}
			_reticlePosition = _reticle.transform.position;
            
			if (ReticleMovesWithSlopes)
			{
				// we cast a ray from above
				RaycastHit groundCheck = MMDebug.Raycast3D(_reticlePosition + Vector3.up * MaximumSlopeElevation / 2f, Vector3.down, MaximumSlopeElevation, ReticleObstacleMask, Color.cyan, true);
				if (groundCheck.collider != null)
				{
					_reticlePosition.y = groundCheck.point.y + ReticleHeight;
					_reticle.transform.position = _reticlePosition;

					_slopeTargetPosition = groundCheck.point + Vector3.up * ReticleHeight;
				}
				else
				{
					_slopeTargetPosition = _reticle.transform.position;
				}
			}
		}

		protected override void MoveTarget()
		{
			if (_weapon.Owner == null)
			{
				return;
			}
	        
			if (MoveCameraTargetTowardsReticle)
			{
				if (ReticleType != ReticleTypes.None)
				{
					_newCamTargetPosition = _reticlePosition;
					_newCamTargetDirection = _newCamTargetPosition - this.transform.position;
					if (_newCamTargetDirection.sqrMagnitude > (CameraTargetMaxDistance*CameraTargetMaxDistance))
					{
						_newCamTargetDirection = _newCamTargetDirection.normalized * CameraTargetMaxDistance;
					}
					_newCamTargetPosition = this.transform.position + _newCamTargetDirection;

					_newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position, Vector3.Lerp(this.transform.position, _newCamTargetPosition, CameraTargetOffset), Time.deltaTime * CameraTargetSpeed);

					_weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
				}
				else
				{
					_newCamTargetPosition = this.transform.position + CurrentAim.normalized * CameraTargetMaxDistance;
					_newCamTargetDirection = _newCamTargetPosition - this.transform.position;
		            
					_newCamTargetPosition = this.transform.position + _newCamTargetDirection;

					_newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position, Vector3.Lerp(this.transform.position, _newCamTargetPosition, CameraTargetOffset), Time.deltaTime * CameraTargetSpeed);

					_weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
				}
			}
		}
	}
}