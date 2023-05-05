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
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Aim 2D")]
	public class WeaponAim2D : WeaponAim
	{
		protected Vector2 _inputMovement;
		protected bool _hasOrientation2D = false;
		protected bool _facingRightLastFrame;

		/// <summary>
		/// On init, we grab our camera and init our last non null movement
		/// </summary>
		protected override void Initialization()
		{
			if (_initialized)
			{
				return;
			}
			base.Initialization();
            
			if (_weapon.Owner?.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterOrientation2D>() != null)
			{
				_hasOrientation2D = true;
				switch (_weapon.Owner?.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterOrientation2D>().CurrentFacingDirection)
				{
					case Character.FacingDirections.East:
						_lastNonNullMovement = Vector2.right;
						break;
					case Character.FacingDirections.North:
						_lastNonNullMovement = Vector2.up;
						break;
					case Character.FacingDirections.West:
						_lastNonNullMovement = Vector2.left;
						break;
					case Character.FacingDirections.South:
						_lastNonNullMovement = Vector2.down;
						break;
				}
			}
		}

		/// <summary>
		/// the current angle the weapon is aiming at, adjusted to compensate for the current orientation of the character
		/// </summary>
		public override float CurrentAngleRelative
		{
			get
			{
				if (_weapon != null)
				{
					if (_weapon.Owner != null)
					{
						if (_hasOrientation2D)
						{
							if (_weapon.Owner.Orientation2D.IsFacingRight)
							{
								return CurrentAngle;
							}
							else
							{
								return -CurrentAngle;
							}                        
						}
					}
				}
				return 0;
			}
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

					if (_weapon.Owner.LinkedInputManager.PrimaryMovement.magnitude > MinimumMagnitude)
					{
						GetPrimaryMovementAim();
					}

					break;

				case AimControls.SecondaryMovement:
					if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
					{
						return;
					}

					if (_weapon.Owner.LinkedInputManager.SecondaryMovement.magnitude > MinimumMagnitude)
					{
						GetSecondaryMovementAim();
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
					_currentAimAbsolute = _weapon.Owner.CameraDirection;
					if (_hasOrientation2D)
					{
						_currentAim = (_weapon.Owner.Orientation2D.IsFacingRight) ? _currentAim : -_currentAim;    
					}
					_direction = -(transform.position - _currentAim);
					break;
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
			_lastNonNullMovement.y = newAim.y;
		}

		/// <summary>
		/// Resets the aim
		/// </summary>
		public virtual void GetOffAim()
		{
			_currentAim = Vector2.right;
			_currentAimAbsolute = Vector2.right;
			_direction = Vector2.right;
		}

		/// <summary>
		/// Computes the current aim and direction based on the current aim provided by script
		/// </summary>
		public virtual void GetScriptAim()
		{
			_currentAimAbsolute = _currentAim;
			if (_hasOrientation2D)
			{
				_currentAim = (_weapon.Owner.Orientation2D.IsFacingRight) ? _currentAim : -_currentAim;    
			}
			_direction = -(transform.position - _currentAim);
		}

		/// <summary>
		/// Grabs primary input (left stick, by default) movement
		/// </summary>
		public virtual void GetPrimaryMovementAim()
		{
			if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
			{
				return;
			}

			if (_lastNonNullMovement == Vector2.zero)
			{
				_lastNonNullMovement = _weapon.Owner.LinkedInputManager.LastNonNullPrimaryMovement;
			}

			_inputMovement = _weapon.Owner.LinkedInputManager.PrimaryMovement;

			TestLastMovement();


			_currentAimAbsolute = _inputMovement;

			if (_hasOrientation2D)
			{
				if (_weapon.Owner.Orientation2D.IsFacingRight)
				{
					_currentAim = _inputMovement;
					_direction = transform.position + _currentAim;
				}
				else
				{
					_currentAim = -_inputMovement;
					_direction = -(transform.position - _currentAim);
				}    
			}
			else
			{
				_currentAim = _inputMovement;
				_direction = transform.position + _currentAim;
			}

			StoreLastMovement();
		}

		/// <summary>
		/// Grabs secondary movement (right stick by default)
		/// </summary>
		public virtual void GetSecondaryMovementAim()
		{
			if ((_weapon.Owner == null) || (_weapon.Owner.LinkedInputManager == null))
			{
				return;
			}

			if (_lastNonNullMovement == Vector2.zero)
			{
				_lastNonNullMovement = _weapon.Owner.LinkedInputManager.LastNonNullSecondaryMovement;
			}

			_inputMovement = _weapon.Owner.LinkedInputManager.SecondaryMovement;
			TestLastMovement();

			_currentAimAbsolute = _inputMovement;

			if (_hasOrientation2D)
			{
				if (_weapon.Owner.Orientation2D.IsFacingRight)
				{
					_currentAim = _inputMovement;
					_direction = transform.position + _currentAim;
				}
				else
				{
					_currentAim = -_inputMovement;
					_direction = -(transform.position - _currentAim);
				}    
			}
			else
			{
				_currentAim = _inputMovement;
				_direction = transform.position + _currentAim;
			}
            
			StoreLastMovement();
		}

		/// <summary>
		/// Grabs mouse aim
		/// </summary>
		public virtual void GetMouseAim()
		{
			_mousePosition = InputManager.Instance.MousePosition;
			
			_mousePosition.z = 10;

			_direction = _mainCamera.ScreenToWorldPoint(_mousePosition);
			_direction.z = _weapon.Owner.transform.position.z;

			_reticlePosition = _direction;

			_currentAimAbsolute = _direction - _weapon.transform.position;

			_currentAim = _direction - _weapon.transform.position;
            
			if (_hasOrientation2D)
			{
				if (_weapon.Owner.Orientation2D.IsFacingRight)
				{
					_currentAim = _direction - _weapon.transform.position;
					_currentAimAbsolute = _currentAim;
				}
				else
				{
					_currentAim = _weapon.transform.position - _direction;
				}
			}            
		}

		/// <summary>
		/// Grabs the last movement stored value if necessary
		/// </summary>
		protected virtual void TestLastMovement()
		{
			if (RotationMode == RotationModes.Strict2Directions)
			{
				_inputMovement.x = Mathf.Abs(_inputMovement.x) > 0 ? _inputMovement.x : _lastNonNullMovement.x;
				_inputMovement.y = Mathf.Abs(_inputMovement.y) > 0 ? _inputMovement.y : _lastNonNullMovement.y;
			}            
			else
			{
				_inputMovement = _inputMovement.magnitude > 0 ? _inputMovement : _lastNonNullMovement;
			}
		}

		/// <summary>
		/// Stores the movement value for use next frame
		/// </summary>
		protected virtual void StoreLastMovement()
		{
			if (RotationMode == RotationModes.Strict2Directions)
			{
				_lastNonNullMovement.x = Mathf.Abs(_inputMovement.x) > 0 ? _inputMovement.x : _lastNonNullMovement.x;
				_lastNonNullMovement.y = Mathf.Abs(_inputMovement.y) > 0 ? _inputMovement.y : _lastNonNullMovement.y;
			}
			else
			{
				_lastNonNullMovement = _inputMovement.magnitude > 0 ? _inputMovement : _lastNonNullMovement;
			}
		}

		/// <summary>
		/// Every frame, we compute the aim direction and rotate the weapon accordingly
		/// </summary>
		protected override void Update()
		{
			HideMousePointer();
			HideReticle();
			if (GameManager.Instance.Paused)
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
		}

		/// <summary>
		/// Determines the weapon rotation based on the current aim direction
		/// </summary>
		protected override void DetermineWeaponRotation()
		{
			if (_currentAim != Vector3.zero)
			{
				if (_direction != Vector3.zero)
				{
					CurrentAngle = Mathf.Atan2(_currentAim.y, _currentAim.x) * Mathf.Rad2Deg;
					CurrentAngleAbsolute = Mathf.Atan2(_currentAimAbsolute.y, _currentAimAbsolute.x) * Mathf.Rad2Deg;
					if (RotationMode == RotationModes.Strict4Directions || RotationMode == RotationModes.Strict8Directions)
					{
						CurrentAngle = MMMaths.RoundToClosest(CurrentAngle, _possibleAngleValues);
					}
					if (RotationMode == RotationModes.Strict2Directions)
					{
						CurrentAngle = 0f;
					}

					// we add our additional angle
					CurrentAngle += _additionalAngle;

					bool flip = false;
					// we clamp the angle to the min/max values set in the inspector
					if (_hasOrientation2D)
					{
						if (_weapon.Owner.Orientation2D.IsFacingRight)
						{
							CurrentAngle = Mathf.Clamp(CurrentAngle, MinimumAngle, MaximumAngle);
						}
						else
						{
							CurrentAngle = Mathf.Clamp(CurrentAngle, -MaximumAngle, -MinimumAngle);
						}
						flip = _facingRightLastFrame != _weapon.Owner.Orientation2D.IsFacingRight;
						_facingRightLastFrame = _weapon.Owner.Orientation2D.IsFacingRight;
					}
					else
					{
						CurrentAngle = Mathf.Clamp(CurrentAngle, MinimumAngle, MaximumAngle);
					}
                    
					_lookRotation = Quaternion.Euler(CurrentAngle * Vector3.forward);
					RotateWeapon(_lookRotation, flip);
				}
			}
			else
			{
				CurrentAngle = 0f;
				RotateWeapon(_initialRotation);
			}
			MMDebug.DebugDrawArrow(this.transform.position, _currentAimAbsolute.normalized, Color.green);
		}
        
		/// <summary>
		/// If a reticle has been set, instantiates the reticle and positions it
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
						_reticle.transform.localPosition = ReticleDistance * Vector3.right;
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
					_reticle.transform.position = _reticlePosition;
				}                
			}
		}

		/// <summary>
		/// Moves the camera target
		/// </summary>
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
					if (_newCamTargetDirection.magnitude > CameraTargetMaxDistance)
					{
						_newCamTargetDirection = _newCamTargetDirection.normalized * CameraTargetMaxDistance;
					}

					_newCamTargetPosition = this.transform.position + _newCamTargetDirection;

					_newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position,
						Vector3.Lerp(this.transform.position, _newCamTargetPosition, CameraTargetOffset),
						Time.deltaTime * CameraTargetSpeed);

					_weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
				}
				else
				{
					_newCamTargetPosition = this.transform.position + _currentAimAbsolute.normalized * CameraTargetMaxDistance;
					_newCamTargetDirection = _newCamTargetPosition - this.transform.position;
		            
					_newCamTargetPosition = this.transform.position + _newCamTargetDirection;

					_newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position, Vector3.Lerp(this.transform.position, _newCamTargetPosition, CameraTargetOffset), Time.deltaTime * CameraTargetSpeed);

					_weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
                    
				}
			}
		}
	}
}