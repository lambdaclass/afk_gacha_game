using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This ability will allow a character to jump in 3D
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Jump 3D")]
	public class CharacterJump3D : CharacterAbility 
	{
		[Header("Jump Settings")]
		/// whether or not the jump should be proportional to press (if yes, releasing the button will stop the jump)
		[Tooltip("whether or not the jump should be proportional to press (if yes, releasing the button will stop the jump)")]
		public bool JumpProportionalToPress = true;
		/// the minimum amount of time after the jump's start before releasing the jump button has any effect
		[Tooltip("the minimum amount of time after the jump's start before releasing the jump button has any effect")]
		public float MinimumPressTime = 0.4f;
		/// the force to apply to the jump, the higher the jump, the faster the jump
		[Tooltip("the force to apply to the jump, the higher the jump, the faster the jump")]
		public float JumpForce = 800f;
		/// the height the jump should have
		[Tooltip("the height the jump should have")]
		public float JumpHeight = 4f;

		[Header("Slopes")]
		/// whether or not the character can jump if standing on a slope too steep to walk on
		[Tooltip("whether or not the character can jump if standing on a slope too steep to walk on")]
		public bool CanJumpOnTooSteepSlopes = true;
		/// whether or not standing on a slope too steep to walk on should reset jump counters 
		[Tooltip("whether or not standing on a slope too steep to walk on should reset jump counters")]
		public bool ResetJumpsOnTooSteepSlopes = false;
        
		[Header("Number of Jumps")]
		/// the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)
		[Tooltip("the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)")]
		public int NumberOfJumps = 1;
		/// the number of jumps left to the character
		[MMReadOnly]
		[Tooltip("the number of jumps left to the character")]
		public int NumberOfJumpsLeft = 0;

		[Header("Feedbacks")]
		/// the feedback to play when the jump starts
		[Tooltip("the feedback to play when the jump starts")]
		public MMFeedbacks JumpStartFeedback;
		/// the feedback to play when the jump stops
		[Tooltip("the feedback to play when the jump stops")]
		public MMFeedbacks JumpStopFeedback;

		protected bool _doubleJumping;
		protected Vector3 _jumpForce;
		protected Vector3 _jumpOrigin;
		protected CharacterButtonActivation _characterButtonActivation;
		protected CharacterCrouch _characterCrouch;
		protected bool _jumpStopped = false;
		protected float _jumpStartedAt = 0f;
		protected bool _buttonReleased = false;
		protected int _initialNumberOfJumps;

		protected const string _jumpingAnimationParameterName = "Jumping";
		protected const string _doubleJumpingAnimationParameterName = "DoubleJumping";
		protected const string _hitTheGroundAnimationParameterName = "HitTheGround";
		protected int _jumpingAnimationParameter;
		protected int _doubleJumpingAnimationParameter;
		protected int _hitTheGroundAnimationParameter;

		/// <summary>
		/// On init we grab other components
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization ();
			ResetNumberOfJumps();
			_characterButtonActivation = _character?.FindAbility<CharacterButtonActivation> ();
			_characterCrouch = _character?.FindAbility<CharacterCrouch> ();
			JumpStartFeedback?.Initialization(this.gameObject);
			JumpStopFeedback?.Initialization(this.gameObject);
			_initialNumberOfJumps = NumberOfJumps;
		}

		/// <summary>
		/// Watches for input and triggers a jump if needed
		/// </summary>
		protected override void HandleInput()
		{
			base.HandleInput();
			// if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}
			if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				JumpStart();
			}
			if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{               
				_buttonReleased = true;                               
			}
		}

		/// <summary>
		/// On process ability, we check if we should stop the jump
		/// </summary>
		public override void ProcessAbility()
		{
			if (_controller.JustGotGrounded)
			{
				ResetNumberOfJumps();
			}

			// if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}

			if (!_jumpStopped
			    &&
			    ((_movement.CurrentState == CharacterStates.MovementStates.Idle)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Walking)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Running)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Pushing)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Falling)
			    ))
			{
				JumpStop();
			}

			if (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
			{
				if (_buttonReleased 
				    && !_jumpStopped
				    && JumpProportionalToPress 
				    && (Time.time - _jumpStartedAt > MinimumPressTime))
				{
					JumpStop();
				}
	            
				if (!_jumpStopped)
				{
					if ((this.transform.position.y - _jumpOrigin.y > JumpHeight)
					    || CeilingTest())
					{
						JumpStop();
						_controller3D.Grounded = _controller3D.IsGroundedTest();
						if (_controller.Grounded)
						{
							ResetNumberOfJumps();  
						}
					}
					else
					{
						_jumpForce = Vector3.up * JumpForce * Time.deltaTime;
						_controller.AddForce(_jumpForce);
					}
				}
			}

			if (!ResetJumpsOnTooSteepSlopes && _controller3D.ExitedTooSteepSlopeThisFrame && _controller3D.Grounded)
			{
				ResetNumberOfJumps();
			}
		}

		/// <summary>
		/// Returns true if a ceiling's found above the character, false otherwise
		/// </summary>
		protected virtual bool CeilingTest()
		{
			bool returnValue = _controller3D.CollidingAbove();
			return returnValue;
		}

		/// <summary>
		/// On jump start, we change our state to jumping
		/// </summary>
		public virtual void JumpStart()
		{
			if (!EvaluateJumpConditions())
			{
				return;
			}

			// we decrease the number of jumps left
			NumberOfJumpsLeft = NumberOfJumpsLeft - 1;

			_movement.ChangeState(CharacterStates.MovementStates.Jumping);	
			MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Jump);
			JumpStartFeedback?.PlayFeedbacks(this.transform.position);
			_jumpOrigin = this.transform.position;
			_jumpStopped = false;
			_jumpStartedAt = Time.time;
			_controller.Grounded = false;
			_controller.GravityActive = false;
			_buttonReleased = false;

			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();
			PlayAbilityStartFeedbacks();
		}

		/// <summary>
		/// Stops the jump
		/// </summary>
		public virtual void JumpStop()
		{
			_controller.GravityActive = true;
			if (_controller.Velocity.y > 0)
			{
				_controller.Velocity.y = 0f;
			}
			_jumpStopped = true;
			_buttonReleased = false;
			PlayAbilityStopSfx();
			StopAbilityUsedSfx();
			StopStartFeedbacks();
			PlayAbilityStopFeedbacks();
			JumpStopFeedback?.PlayFeedbacks(this.transform.position);
		}

		/// <summary>
		/// Resets the number of jumps.
		/// </summary>
		public virtual void ResetNumberOfJumps()
		{
			bool shouldResetJumps = true;

			if (!ResetJumpsOnTooSteepSlopes)
			{
				if (_controller3D.TooSteep())
				{
					shouldResetJumps = false;
				}
			}

			if (shouldResetJumps)
			{
				NumberOfJumpsLeft = NumberOfJumps;
			}
		}

		/// <summary>
		/// Evaluates the jump conditions and returns true if a jump can be performed
		/// </summary>
		/// <returns></returns>
		protected virtual bool EvaluateJumpConditions()
		{
			if (!AbilityAuthorized)
			{
				return false;
			}
			if (_characterButtonActivation != null)
			{
				if (_characterButtonActivation.AbilityAuthorized
				    && _characterButtonActivation.InButtonActivatedZone
				    && _characterButtonActivation.PreventJumpInButtonActivatedZone)
				{
					return false;
				}
			}

			if (!CanJumpOnTooSteepSlopes)
			{
				if (_controller3D.TooSteep())
				{
					return false;
				}
			}

			if (_characterCrouch != null)
			{
				if (_characterCrouch.InATunnel)
				{
					return false;
				}
			}

			if (CeilingTest())
			{
				return false;
			}

			if (NumberOfJumpsLeft <= 0)
			{
				return false;
			}

			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_jumpingAnimationParameterName, AnimatorControllerParameterType.Bool, out _jumpingAnimationParameter);
			RegisterAnimatorParameter (_doubleJumpingAnimationParameterName, AnimatorControllerParameterType.Bool, out _doubleJumpingAnimationParameter);
			RegisterAnimatorParameter (_hitTheGroundAnimationParameterName, AnimatorControllerParameterType.Bool, out _hitTheGroundAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, sends Jumping states to the Character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _jumpingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Jumping),_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _doubleJumpingAnimationParameter, _doubleJumping,_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool (_animator, _hitTheGroundAnimationParameter, _controller.JustGotGrounded, _character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}