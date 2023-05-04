using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this ability to a character and it'll rotate or flip to face the direction of movement or the weapon's, or both, or none
	/// Only add this ability to a 2D character
	/// </summary>
	[MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Orientation 2D")]
	public class CharacterOrientation2D : CharacterAbility
	{
		/// the possible facing modes
		public enum FacingModes { None, MovementDirection, WeaponDirection, Both }
		public enum FacingBases { WeaponAngle, MousePositionX, SceneReticlePositionX }
		/// the facing mode for this character
		public FacingModes FacingMode = FacingModes.None;

		[MMEnumCondition("FacingMode", (int)FacingModes.WeaponDirection, (int)FacingModes.Both)]
		public FacingBases FacingBase = FacingBases.WeaponAngle;
        
		[MMInformation("You can also decide if the character must automatically flip when going backwards or not. Additionnally, if you're not using sprites, you can define here how the character's model's localscale will be affected by flipping. By default it flips on the x axis, but you can change that to fit your model.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]

		[Header("Horizontal Flip")]

		/// whether we should flip the model's scale when the character changes direction or not		
		[Tooltip("whether we should flip the model's scale when the character changes direction or not	")]
		public bool ModelShouldFlip = false;
		/// the scale value to apply to the model when facing left
		[MMCondition("ModelShouldFlip", true)]
		[Tooltip("the scale value to apply to the model when facing left")]
		public Vector3 ModelFlipValueLeft = new Vector3(-1, 1, 1);
		/// the scale value to apply to the model when facing right
		[MMCondition("ModelShouldFlip", true)]
		[Tooltip("the scale value to apply to the model when facing right")]
		public Vector3 ModelFlipValueRight = new Vector3(1, 1, 1);
		/// whether we should rotate the model on direction change or not		
		[Tooltip("whether we should rotate the model on direction change or not")]
		public bool ModelShouldRotate;
		/// the rotation to apply to the model when it changes direction		
		[MMCondition("ModelShouldRotate", true)]
		[Tooltip("the rotation to apply to the model when it changes direction")]
		public Vector3 ModelRotationValueLeft = new Vector3(0f, 180f, 0f);
		/// the rotation to apply to the model when it changes direction		
		[MMCondition("ModelShouldRotate", true)]
		[Tooltip("the rotation to apply to the model when it changes direction")]
		public Vector3 ModelRotationValueRight = new Vector3(0f, 0f, 0f);
		/// the speed at which to rotate the model when changing direction, 0f means instant rotation		
		[MMCondition("ModelShouldRotate", true)]
		[Tooltip("the speed at which to rotate the model when changing direction, 0f means instant rotation	")]
		public float ModelRotationSpeed = 0f;
        
		[Header("Direction")]

		/// true if the player is facing right
		[MMInformation("It's usually good practice to build all your characters facing right. If that's not the case of this character, select Left instead.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
		[Tooltip("true if the player is facing right")]
		public Character.FacingDirections InitialFacingDirection = Character.FacingDirections.East;
		/// the threshold at which movement is considered
		[Tooltip("the threshold at which movement is considered")]
		public float AbsoluteThresholdMovement = 0.5f;
		/// the threshold at which weapon gets considered
		[Tooltip("the threshold at which weapon gets considered")]
		public float AbsoluteThresholdWeapon = 0.5f;
		/// the direction this character is currently facing
		[MMReadOnly]
		[Tooltip("the direction this character is currently facing")]
		public Character.FacingDirections CurrentFacingDirection = Character.FacingDirections.East;
		/// whether or not this character is facing right
		[MMReadOnly]
		[Tooltip("whether or not this character is facing right")]
		public bool IsFacingRight = true;

		protected Vector3 _targetModelRotation;
		protected CharacterHandleWeapon _characterHandleWeapon;
		protected Vector3 _lastRegisteredVelocity;
		protected Vector3 _rotationDirection;
		protected Vector3 _lastMovement = Vector3.zero;
		protected Vector3 _lastAim = Vector3.zero;
		protected float _lastNonNullXMovement;        
		protected float _lastNonNullXInput;
		protected int _direction;
		protected int _directionLastFrame = 0;
		protected float _horizontalDirection;
		protected float _verticalDirection;

		protected const string _facingDirectionAnimationParameterName = "FacingDirection2D";
		protected const string _horizontalDirectionAnimationParameterName = "HorizontalDirection";
		protected const string _verticalDirectionAnimationParameterName = "VerticalDirection";
		protected int _horizontalDirectionAnimationParameter;
		protected int _verticalDirectionAnimationParameter;
		protected const string _horizontalSpeedAnimationParameterName = "HorizontalSpeed";
		protected const string _verticalSpeedAnimationParameterName = "VerticalSpeed";
		protected int _horizontalSpeedAnimationParameter;
		protected int _verticalSpeedAnimationParameter;
		protected int _facingDirectionAnimationParameter;
		protected float _lastDirectionX;
		protected float _lastDirectionY;
		protected bool _initialized = false;
		protected float _directionFloat;
        
		/// <summary>
		/// On awake we init our facing direction and grab components
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			_characterHandleWeapon = this.gameObject.GetComponentInParent<CharacterHandleWeapon>();
		}

		/// <summary>
		/// On Start we reset our CurrentDirection
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			if (_controller == null)
			{
				_controller = this.gameObject.GetComponentInParent<TopDownController>();
			}
			_controller.CurrentDirection = Vector3.zero;
			_initialized = true;
			if (InitialFacingDirection == Character.FacingDirections.West)
			{
				IsFacingRight = false;
				_direction = -1;
			}
			else
			{
				IsFacingRight = true;
				_direction = 1;
			}
			Face(InitialFacingDirection);
			_directionLastFrame = 0;
			CurrentFacingDirection = InitialFacingDirection;
			switch(InitialFacingDirection)
			{
				case Character.FacingDirections.East:
					_lastDirectionX = 1f;
					_lastDirectionY = 0f;
					break;
				case Character.FacingDirections.West:
					_lastDirectionX = -1f;
					_lastDirectionY = 0f;
					break;
				case Character.FacingDirections.North:
					_lastDirectionX = 0f;
					_lastDirectionY = 1f;
					break;
				case Character.FacingDirections.South:
					_lastDirectionX = 0f;
					_lastDirectionY = -1f;
					break;
			}
		}

		/// <summary>
		/// On process ability, we flip to face the direction set in settings
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			{
				return;
			}

			if (!AbilityAuthorized)
			{
				return;
			}

			DetermineFacingDirection();
			FlipToFaceMovementDirection();
			FlipToFaceWeaponDirection();
			ApplyModelRotation();
			FlipAbilities();

			_directionLastFrame = _direction;
			_lastNonNullXMovement = (Mathf.Abs(_controller.CurrentDirection.x) > 0) ? _controller.CurrentDirection.x : _lastNonNullXMovement;
			if (_inputManager != null)
			{
				_lastNonNullXInput = (Mathf.Abs(_inputManager.PrimaryMovement.x) > _inputManager.Threshold.x) ? _inputManager.PrimaryMovement.x : _lastNonNullXInput;
			}
		}

		protected virtual void FixedUpdate()
		{
			ComputeRelativeSpeeds();
		}

		protected virtual void DetermineFacingDirection()
		{
			if (_controller.CurrentDirection == Vector3.zero)
			{
				ApplyCurrentDirection();
			}

			if (_controller.CurrentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
			{
				if (Mathf.Abs(_controller.CurrentDirection.y) > Mathf.Abs(_controller.CurrentDirection.x))
				{
					CurrentFacingDirection = (_controller.CurrentDirection.y > 0) ? Character.FacingDirections.North : Character.FacingDirections.South;
				}
				else
				{
					CurrentFacingDirection = (_controller.CurrentDirection.x > 0) ? Character.FacingDirections.East : Character.FacingDirections.West;
				}
				_horizontalDirection = Mathf.Abs(_controller.CurrentDirection.x) >= AbsoluteThresholdMovement ? _controller.CurrentDirection.x : 0f;
				_verticalDirection = Mathf.Abs(_controller.CurrentDirection.y) >= AbsoluteThresholdMovement ? _controller.CurrentDirection.y : 0f;
			}
			else
			{
				_horizontalDirection = _lastDirectionX;
				_verticalDirection = _lastDirectionY;
			}
            
			switch (CurrentFacingDirection)
			{
				case Character.FacingDirections.West:
					_directionFloat = 0f;
					break;
				case Character.FacingDirections.North:
					_directionFloat = 1f;
					break;
				case Character.FacingDirections.East:
					_directionFloat = 2f;
					break;
				case Character.FacingDirections.South:
					_directionFloat = 3f;
					break;
			}
            
			_lastDirectionX = _horizontalDirection;
			_lastDirectionY = _verticalDirection;
		}

		/// <summary>
		/// Applies the current direction to the controller
		/// </summary>
		protected virtual void ApplyCurrentDirection()
		{
			if (!_initialized)
			{
				Initialization();
			}
            
			switch (CurrentFacingDirection)
			{
				case Character.FacingDirections.East:
					_controller.CurrentDirection = Vector3.right;
					break;
				case Character.FacingDirections.West:
					_controller.CurrentDirection = Vector3.left;
					break;
				case Character.FacingDirections.North:
					_controller.CurrentDirection = Vector3.up;
					break;
				case Character.FacingDirections.South:
					_controller.CurrentDirection = Vector3.down;
					break;
			}
		}
        
		/// <summary>
		/// If the model should rotate, we modify its rotation 
		/// </summary>
		protected virtual void ApplyModelRotation()
		{
			if (!ModelShouldRotate)
			{
				return;
			}

			if (ModelRotationSpeed > 0f)
			{
				_character.CharacterModel.transform.localEulerAngles = Vector3.Lerp(_character.CharacterModel.transform.localEulerAngles, _targetModelRotation, Time.deltaTime * ModelRotationSpeed);
			}
			else
			{
				_character.CharacterModel.transform.localEulerAngles = _targetModelRotation;
			}
		}

		/// <summary>
		/// Flips the object to face direction
		/// </summary>
		protected virtual void FlipToFaceMovementDirection()
		{
			// if we're not supposed to face our direction, we do nothing and exit
			if ((FacingMode != FacingModes.MovementDirection) && (FacingMode != FacingModes.Both)) { return; }
            
			if (_controller.CurrentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
			{
				float checkedDirection = (Mathf.Abs(_controller.CurrentDirection.normalized.x) > 0) ? _controller.CurrentDirection.normalized.x : _lastNonNullXMovement;
                
				if (checkedDirection >= 0)
				{
					FaceDirection(1);
				}
				else
				{
					FaceDirection(-1);
				}
			}                
		}

		/// <summary>
		/// Flips the character to face the current weapon direction
		/// </summary>
		protected virtual void FlipToFaceWeaponDirection()
		{
			if (_characterHandleWeapon == null)
			{
				return;
			}
			// if we're not supposed to face our direction, we do nothing and exit
			if ((FacingMode != FacingModes.WeaponDirection) && (FacingMode != FacingModes.Both)) { return; }
            
			if (_characterHandleWeapon.WeaponAimComponent != null)
			{
				switch (FacingBase)
				{
					case FacingBases.WeaponAngle:
						float weaponAngle = _characterHandleWeapon.WeaponAimComponent.CurrentAngleAbsolute;
                
						if ((weaponAngle > 90) || (weaponAngle < -90))
						{
							FaceDirection(-1);
						}
						else
						{
							FaceDirection(1);
						}	
						break;
					case FacingBases.MousePositionX:
						if (_characterHandleWeapon.WeaponAimComponent.GetMousePosition().x < this.transform.position.x)
						{
							FaceDirection(-1);
						}
						else
						{
							FaceDirection(1);
						}	
						break;
					case FacingBases.SceneReticlePositionX:
						if (_characterHandleWeapon.WeaponAimComponent.GetReticlePosition().x < this.transform.position.x) 
						{
							FaceDirection(-1);
						}
						else
						{
							FaceDirection(1);
						}	
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				

				_horizontalDirection = _characterHandleWeapon.WeaponAimComponent.CurrentAimAbsolute.normalized.x;
				_verticalDirection = _characterHandleWeapon.WeaponAimComponent.CurrentAimAbsolute.normalized.y;
			}            
		}
        
		/// <summary>
		/// Defines the CurrentFacingDirection
		/// </summary>
		/// <param name="direction"></param>
		public virtual void Face(Character.FacingDirections direction)
		{
			CurrentFacingDirection = direction;
			ApplyCurrentDirection();
			if (direction == Character.FacingDirections.West)
			{
				FaceDirection(-1);
			}
			if (direction == Character.FacingDirections.East)
			{
				FaceDirection(1);
			}
		}

		/// <summary>
		/// Flips the character and its dependencies (jetpack for example) horizontally
		/// </summary>
		public virtual void FaceDirection(int direction)
		{
			if (ModelShouldFlip)
			{
				FlipModel(direction);
			}

			if (ModelShouldRotate)
			{
				RotateModel(direction);
			}

			_direction = direction;
			IsFacingRight = _direction == 1;
		}

		/// <summary>
		/// Rotates the model in the specified direction
		/// </summary>
		/// <param name="direction"></param>
		protected virtual void RotateModel(int direction)
		{
			if (_character.CharacterModel != null)
			{
				_targetModelRotation = (direction == 1) ? ModelRotationValueRight : ModelRotationValueLeft;
				_targetModelRotation.x = _targetModelRotation.x % 360;
				_targetModelRotation.y = _targetModelRotation.y % 360;
				_targetModelRotation.z = _targetModelRotation.z % 360;
			}
		}
        
		/// <summary>
		/// Flips the model only, no impact on weapons or attachments
		/// </summary>
		public virtual void FlipModel(int direction)
		{
			if (_character.CharacterModel != null)
			{
				_character.CharacterModel.transform.localScale = (direction == 1) ? ModelFlipValueRight : ModelFlipValueLeft;
			}
			else
			{
				_spriteRenderer.flipX = (direction == -1);
			}
		}

		/// <summary>
		/// Sends a flip event on all other abilities
		/// </summary>
		protected virtual void FlipAbilities()
		{
			if ((_directionLastFrame != 0) && (_directionLastFrame != _direction))
			{
				_character.FlipAllAbilities();
			}
		}

		protected Vector3 _positionLastFrame;
		protected Vector3 _newSpeed;

		/// <summary>
		/// Computes the relative speeds
		/// </summary>
		protected virtual void ComputeRelativeSpeeds()
		{
			if (Time.deltaTime != 0f)
			{
				_newSpeed = (this.transform.position - _positionLastFrame) / Time.deltaTime;
			}            
			_positionLastFrame = this.transform.position;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_horizontalDirectionAnimationParameterName, AnimatorControllerParameterType.Float, out _horizontalDirectionAnimationParameter);
			RegisterAnimatorParameter(_verticalDirectionAnimationParameterName, AnimatorControllerParameterType.Float, out _verticalDirectionAnimationParameter);

			RegisterAnimatorParameter(_horizontalSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _horizontalSpeedAnimationParameter);
			RegisterAnimatorParameter(_verticalSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _verticalSpeedAnimationParameter);
			RegisterAnimatorParameter(_facingDirectionAnimationParameterName, AnimatorControllerParameterType.Float, out _facingDirectionAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, sends Jumping states to the Character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _horizontalDirectionAnimationParameter, _horizontalDirection, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _verticalDirectionAnimationParameter, _verticalDirection, _character._animatorParameters, _character.RunAnimatorSanityChecks);

			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _horizontalSpeedAnimationParameter, _newSpeed.x, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _verticalSpeedAnimationParameter, _newSpeed.y, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _facingDirectionAnimationParameter, _directionFloat, _character._animatorParameters);      
		}

	}
}