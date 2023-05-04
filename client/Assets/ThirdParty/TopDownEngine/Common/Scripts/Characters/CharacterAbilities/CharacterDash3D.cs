using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this ability to a 3D character and it'll be able to dash (cover the specified distance in the specified time)
	///
	/// Animation parameters :
	/// Dashing : true if the character is currently dashing
	/// DashStarted : true when the dash starts
	/// DashingDirectionX : the x component of the dash direction, normalized
	/// DashingDirectionY : the y component of the dash direction, normalized
	/// DashingDirectionZ : the z component of the dash direction, normalized
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Dash 3D")]
	public class CharacterDash3D : CharacterAbility
	{
		/// the possible dash modes (fixed = always the same direction)
		public enum DashModes { Fixed, MainMovement, SecondaryMovement, MousePosition, ModelDirection }
		/// the possible spaces the dash should happen in, either in world coordinates or local ones
		public enum DashSpaces { World, Local }

		/// the current dash mode
		[Tooltip("the current dash mode (fixed : always the same direction, MainMovement : usually your left stick, SecondaryMovement : usually your right stick, MousePosition : the cursor's position")]
		public DashModes DashMode = DashModes.MainMovement;

		[Header("Dash")]
		/// the space in which the dash should happen, either local or world
		public DashSpaces DashSpace = DashSpaces.World;
		/// the direction of the dash, relative to the character
		[Tooltip("the direction of the dash, relative to the character")]
		public Vector3 DashDirection = Vector3.forward;
		/// the distance to cover
		[Tooltip("the distance to cover")]
		public float DashDistance = 10f;
		/// the duration of the dash
		[Tooltip("the duration of the dash, in seconds")]
		public float DashDuration = 0.5f;
		/// the curve to apply to the dash's acceleration
		[Tooltip("the curve to apply to the dash's acceleration")]
		public AnimationCurve DashCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		/// if this is true, dash will be allowed while jumping, otherwise it'll be ignored
		[Tooltip("if this is true, dash will be allowed while jumping, otherwise it'll be ignored")]
		public bool AllowDashWhenJumping = false;

		[Header("Cooldown")]
		/// this ability's cooldown
		[Tooltip("this ability's cooldown")]
		public MMCooldown Cooldown;
        
		[Header("Damage")] 
		/// if this is true, this character won't receive any damage while a dash is in progress
		[Tooltip("if this is true, this character won't receive any damage while a dash is in progress")]
		public bool InvincibleWhileDashing = false; 

		[Header("Feedbacks")]
		/// the feedbacks to play when dashing
		[Tooltip("the feedbacks to play when dashing")]
		public MMFeedbacks DashFeedback;

		protected bool _dashing;
		protected bool _dashStartedThisFrame;
		protected float _dashTimer;
		protected Vector3 _dashOrigin;
		protected Vector3 _dashDestination;
		protected Vector3 _newPosition;
		protected Vector3 _oldPosition;
		protected Vector3 _dashAngle = Vector3.zero;
		protected Vector3 _inputDirection;
		protected Vector3 _dashAnimParameterDirection;
		protected Plane _playerPlane;
		protected Camera _mainCamera;
		protected const string _dashingAnimationParameterName = "Dashing";
		protected const string _dashStartedAnimationParameterName = "DashStarted";
		protected const string _dashingDirectionXAnimationParameterName = "DashingDirectionX";
		protected const string _dashingDirectionYAnimationParameterName = "DashingDirectionY";
		protected const string _dashingDirectionZAnimationParameterName = "DashingDirectionZ";
		protected int _dashingAnimationParameter;
		protected int _dashStartedAnimationParameter;
		protected int _dashingDirectionXAnimationParameter;
		protected int _dashingDirectionYAnimationParameter;
		protected int _dashingDirectionZAnimationParameter;
		protected CharacterOrientation3D _characterOrientation3D;
        
		/// <summary>
		/// On init we initialize our cooldown and feedback
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_playerPlane = new Plane(Vector3.up, Vector3.zero);
			_characterOrientation3D = _character.FindAbility<CharacterOrientation3D>();
			_mainCamera = Camera.main;
			Cooldown.Initialization();
			DashFeedback?.Initialization(this.gameObject);
		}

		/// <summary>
		/// Watches for input and starts a dash if needed
		/// </summary>
		protected override void HandleInput()
		{
			base.HandleInput();
			if (!AbilityAuthorized
			    || (!Cooldown.Ready())
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}

			if (!AllowDashWhenJumping && (_movement.CurrentState == CharacterStates.MovementStates.Jumping))
			{
				return;
			}

			if (_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				DashStart();
			}
		}

		/// <summary>
		/// Starts a dash
		/// </summary>
		public virtual void DashStart()
		{
			if (!Cooldown.Ready())
			{
				return;
			}
			Cooldown.Start();

			_movement.ChangeState(CharacterStates.MovementStates.Dashing);
			_dashing = true;
			_dashTimer = 0f;
			_dashOrigin = this.transform.position;
			_controller.FreeMovement = false;
			_controller3D.DetachFromMovingPlatform();
			DashFeedback?.PlayFeedbacks(this.transform.position);
			PlayAbilityStartFeedbacks();
			_dashStartedThisFrame = true;

			if (InvincibleWhileDashing)
			{
				_health.DamageDisabled();
			}

			HandleDashMode();
		}

		protected virtual void HandleDashMode()
		{
			float angle  = 0f;
			switch (DashMode)
			{
				case DashModes.MainMovement:
					angle = Vector3.SignedAngle(this.transform.forward, _controller.CurrentDirection.normalized, Vector3.up);
					_dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
					_dashAngle.y = angle;
					_dashDestination = MMMaths.RotatePointAroundPivot(_dashDestination, this.transform.position, _dashAngle);
					break;

				case DashModes.Fixed:
					_dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
					break;

				case DashModes.SecondaryMovement:
					_inputDirection = _character.LinkedInputManager.SecondaryMovement;
					_inputDirection.z = _inputDirection.y;
					_inputDirection.y = 0;

					angle = Vector3.SignedAngle(this.transform.forward, _inputDirection.normalized, Vector3.up);
					_dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
					_dashAngle.y = angle;
					_dashDestination = MMMaths.RotatePointAroundPivot(_dashDestination, this.transform.position, _dashAngle);

					_controller.CurrentDirection = (_dashDestination - this.transform.position).normalized;
					break;
				
				case DashModes.ModelDirection:
					_dashDestination = this.transform.position + _characterOrientation3D.ModelDirection.normalized * DashDistance;
					break;

				case DashModes.MousePosition:
					Ray ray = _mainCamera.ScreenPointToRay(InputManager.Instance.MousePosition);
					Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
					float distance;
					_playerPlane.SetNormalAndPosition(_playerPlane.normal, this.transform.position);
					if (_playerPlane.Raycast(ray, out distance))
					{
						_inputDirection = ray.GetPoint(distance);
					}

					angle = Vector3.SignedAngle(this.transform.forward, (_inputDirection - this.transform.position).normalized, Vector3.up);
					_dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
					_dashAngle.y = angle;
					_dashDestination = MMMaths.RotatePointAroundPivot(_dashDestination, this.transform.position, _dashAngle);

					_controller.CurrentDirection = (_dashDestination - this.transform.position).normalized;
					break;
			}
		}

		/// <summary>
		/// Stops the dash
		/// </summary>
		public virtual void DashStop()
		{
			Cooldown.Stop();
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
			_dashing = false;
			_controller.FreeMovement = true;
			DashFeedback?.StopFeedbacks();
			StopStartFeedbacks();
			PlayAbilityStopFeedbacks();
			if (InvincibleWhileDashing)
			{
				_health.DamageEnabled();
			}
		}

		/// <summary>
		/// On process ability, we move our character if we're currently dashing
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			Cooldown.Update();

			if (_dashing)
			{
				if (_dashTimer < DashDuration)
				{
					_dashAnimParameterDirection = (_dashDestination - _dashOrigin).normalized;
					if (DashSpace == DashSpaces.World)
					{
						_newPosition = Vector3.Lerp(_dashOrigin, _dashDestination, DashCurve.Evaluate(_dashTimer / DashDuration));	
						_dashTimer += Time.deltaTime;
						_controller.MovePosition(_newPosition);
					}
					else
					{
						_oldPosition = _dashTimer == 0 ? _dashOrigin : _newPosition;
						_newPosition = Vector3.Lerp(_dashOrigin, _dashDestination, DashCurve.Evaluate(_dashTimer / DashDuration));
						_dashTimer += Time.deltaTime;
						_controller.MovePosition(this.transform.position + _newPosition - _oldPosition);
					}
				}
				else
				{
					DashStop();                   
				}
			}
		}
        
		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_dashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashingAnimationParameter);
			RegisterAnimatorParameter(_dashStartedAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashStartedAnimationParameter);
			RegisterAnimatorParameter(_dashingDirectionXAnimationParameterName, AnimatorControllerParameterType.Float, out _dashingDirectionXAnimationParameter);
			RegisterAnimatorParameter(_dashingDirectionYAnimationParameterName, AnimatorControllerParameterType.Float, out _dashingDirectionYAnimationParameter);
			RegisterAnimatorParameter(_dashingDirectionZAnimationParameterName, AnimatorControllerParameterType.Float, out _dashingDirectionZAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashStartedAnimationParameter, _dashStartedThisFrame, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _dashingDirectionXAnimationParameter, _dashAnimParameterDirection.x, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _dashingDirectionYAnimationParameter, _dashAnimParameterDirection.y, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _dashingDirectionZAnimationParameter, _dashAnimParameterDirection.z, _character._animatorParameters, _character.RunAnimatorSanityChecks);

			_dashStartedThisFrame = false;
		}
	}
}