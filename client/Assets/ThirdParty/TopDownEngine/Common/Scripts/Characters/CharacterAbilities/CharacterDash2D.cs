using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this ability to a character and it'll be able to dash in 2D, covering a certain distance in a certain duration
	///
	/// Animation parameters :
	/// Dashing : true if the character is currently dashing
	/// DashingDirectionX : the x component of the dash direction, normalized
	/// DashingDirectionY : the y component of the dash direction, normalized
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Dash 2D")]
	public class CharacterDash2D : CharacterAbility 
	{
		/// the possible dash modes (fixed : always the same direction)
		public enum DashModes { Fixed, MainMovement, SecondaryMovement, MousePosition }
		/// the possible spaces the dash should happen in, either in world coordinates or local ones
		public enum DashSpaces { World, Local }

		/// the dash mode to apply the dash in
		[Tooltip("the dash mode to apply the dash in")]
		public DashModes DashMode = DashModes.MainMovement;

		[Header("Dash")]
		/// the space in which the dash should happen, either local or world
		public DashSpaces DashSpace = DashSpaces.World;
		/// the dash direction
		[Tooltip("the dash direction")]
		public Vector3 DashDirection = Vector3.forward;
		/// the distance the dash should last for
		[Tooltip("the distance the dash should last for")]
		public float DashDistance = 6f;
		/// the duration of the dash, in seconds
		[Tooltip("the duration of the dash, in seconds")]
		public float DashDuration = 0.2f;
		/// the animation curve to apply to the dash acceleration
		[Tooltip("the animation curve to apply to the dash acceleration")]
		public AnimationCurve DashCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        
		[Header("Cooldown")]

		/// this ability's cooldown
		[Tooltip("this ability's cooldown")]
		public MMCooldown Cooldown;
        
        
		[Header("Damage")] 
		/// if this is true, this character won't receive any damage while a dash is in progress
		[Tooltip("if this is true, this character won't receive any damage while a dash is in progress")]
		public bool InvincibleWhileDashing = false; 

		[Header("Feedback")]
		/// the feedbacks to play when dashing
		[Tooltip("the feedbacks to play when dashing")]
		public MMFeedbacks DashFeedback;

		protected bool _dashing;
		protected float _dashTimer;
		protected Vector3 _dashOrigin;
		protected Vector3 _dashDestination;
		protected Vector3 _newPosition;
		protected Vector3 _oldPosition;
		protected  Vector3 _dashAnimParameterDirection;
		protected Vector3 _dashAngle = Vector3.zero;
		protected Vector3 _inputPosition;
		protected Camera _mainCamera;
		protected const string _dashingAnimationParameterName = "Dashing";
		protected const string _dashingDirectionXAnimationParameterName = "DashingDirectionX";
		protected const string _dashingDirectionYAnimationParameterName = "DashingDirectionY";
		protected int _dashingAnimationParameter;
		protected int _dashingDirectionXAnimationParameter;
		protected int _dashingDirectionYAnimationParameter;

		/// <summary>
		/// On init, we stop our particles, and initialize our dash bar
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization ();
			Cooldown.Initialization();

			_mainCamera = Camera.main;

			if (GUIManager.HasInstance && _character.CharacterType == Character.CharacterTypes.Player)
			{
				GUIManager.Instance.SetDashBar(true, _character.PlayerID);
				UpdateDashBar();
			}
		}

		/// <summary>
		/// Watches for dash inputs
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
			if (_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				DashStart();
			}
		}

		/// <summary>
		/// Initiates the dash
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
			DashFeedback?.PlayFeedbacks(this.transform.position);
			PlayAbilityStartFeedbacks();
            
			if (InvincibleWhileDashing)
			{
				_health.DamageDisabled();
			}

			HandleDashMode();
		}

		protected virtual void HandleDashMode()
		{
			switch (DashMode)
			{
				case DashModes.MainMovement:
					_dashDestination = this.transform.position + _controller.CurrentDirection.normalized * DashDistance;
					break;

				case DashModes.Fixed:
					_dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
					break;

				case DashModes.SecondaryMovement:
					_dashDestination = this.transform.position + (Vector3)_character.LinkedInputManager.SecondaryMovement.normalized * DashDistance;
					break;

				case DashModes.MousePosition:
					_inputPosition = _mainCamera.ScreenToWorldPoint(InputManager.Instance.MousePosition);
					_inputPosition.z = this.transform.position.z;
					_dashDestination = this.transform.position + (_inputPosition - this.transform.position).normalized * DashDistance;
					break;
			}  
		}

		/// <summary>
		/// Stops the dash
		/// </summary>
		public virtual void DashStop()
		{
			DashFeedback?.StopFeedbacks(this.transform.position);

			StopStartFeedbacks();
			PlayAbilityStopFeedbacks();
            
			if (InvincibleWhileDashing)
			{
				_health.DamageEnabled();
			}

			_movement.ChangeState(CharacterStates.MovementStates.Idle);
			_dashing = false;
			_controller.FreeMovement = true;
		}

		/// <summary>
		/// On update, moves the character if needed
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility ();
			Cooldown.Update();
			UpdateDashBar();

			if (_dashing)
			{
				if (_dashTimer < DashDuration)
				{
					_dashAnimParameterDirection = (_dashDestination - _dashOrigin).normalized;
					if (DashSpace == DashSpaces.World)
					{
						_newPosition = Vector3.Lerp (_dashOrigin, _dashDestination, DashCurve.Evaluate (_dashTimer/DashDuration));
						_dashTimer += Time.deltaTime;
						_controller.MovePosition (_newPosition);
					}
					else
					{
						_oldPosition = _dashTimer == 0 ? _dashOrigin : _newPosition;
						_newPosition = Vector3.Lerp (_dashOrigin, _dashDestination, DashCurve.Evaluate (_dashTimer/DashDuration));
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
		/// Updates the GUI jetpack bar.
		/// </summary>
		protected virtual void UpdateDashBar()
		{
			if ((GUIManager.HasInstance) && (_character.CharacterType == Character.CharacterTypes.Player))
			{
				GUIManager.Instance.UpdateDashBars(Cooldown.CurrentDurationLeft, 0f, Cooldown.ConsumptionDuration, _character.PlayerID);
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_dashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashingAnimationParameter);
			RegisterAnimatorParameter(_dashingDirectionXAnimationParameterName, AnimatorControllerParameterType.Float, out _dashingDirectionXAnimationParameter);
			RegisterAnimatorParameter(_dashingDirectionYAnimationParameterName, AnimatorControllerParameterType.Float, out _dashingDirectionYAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing),_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _dashingDirectionXAnimationParameter, _dashAnimParameterDirection.x, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _dashingDirectionYAnimationParameter, _dashAnimParameterDirection.y, _character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}