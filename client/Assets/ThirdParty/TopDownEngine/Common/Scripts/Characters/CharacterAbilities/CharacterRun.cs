using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to run
	/// Animator parameters : Running
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Run")] 
	public class CharacterRun : CharacterAbility
	{	
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to change speed (defined here) when pressing the run button."; }

		[Header("Speed")]

		/// the speed of the character when it's running
		[Tooltip("the speed of the character when it's running")]
		public float RunSpeed = 16f;

		[Header("AutoRun")]

		/// whether or not run should auto trigger if you move the joystick far enough
		[Tooltip("whether or not run should auto trigger if you move the joystick far enough")]
		public bool AutoRun = false;
		/// the input threshold on the joystick (normalized)
		[Tooltip("the input threshold on the joystick (normalized)")]
		public float AutoRunThreshold = 0.6f;

		protected const string _runningAnimationParameterName = "Running";
		protected int _runningAnimationParameter;
		protected bool _runningStarted = false;

		/// <summary>
		/// At the beginning of each cycle, we check if we've pressed or released the run button
		/// </summary>
		protected override void HandleInput()
		{
			if (AutoRun)
			{
				if (_inputManager.PrimaryMovement.magnitude > AutoRunThreshold)
				{
					_inputManager.RunButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed);
				}
			}

			if (_inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				RunStart();
			}				
			if (_inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{
				RunStop();
			}
			else
			{
				if (AutoRun)
				{
					if (_inputManager.PrimaryMovement.magnitude <= AutoRunThreshold)
					{
						_inputManager.RunButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
						RunStop();
					}
				}
			}          
		}

		/// <summary>
		/// Every frame we make sure we shouldn't be exiting our run state
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			HandleRunningExit();
		}

		/// <summary>
		/// Checks if we should exit our running state
		/// </summary>
		protected virtual void HandleRunningExit()
		{
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			{
				StopAbilityUsedSfx();
			}
			if (_movement.CurrentState == CharacterStates.MovementStates.Running && AbilityInProgressSfx != null && _abilityInProgressSfx == null)
			{
				PlayAbilityUsedSfx();
			}
			// if we're running and not grounded, we change our state to Falling
			if (!_controller.Grounded
			    && (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			    && (_movement.CurrentState == CharacterStates.MovementStates.Running))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Falling);
				StopFeedbacks();
				StopSfx ();
			}
			// if we're not moving fast enough, we go back to idle
			if ((Mathf.Abs(_controller.CurrentMovement.magnitude) < RunSpeed / 10) && (_movement.CurrentState == CharacterStates.MovementStates.Running))
			{
				_movement.ChangeState (CharacterStates.MovementStates.Idle);
				StopFeedbacks();
				StopSfx ();
			}
			if (!_controller.Grounded && _abilityInProgressSfx != null)
			{
				StopFeedbacks();
				StopSfx ();
			}
		}

		/// <summary>
		/// Causes the character to start running.
		/// </summary>
		public virtual void RunStart()
		{		
			if ( !AbilityAuthorized // if the ability is not permitted
			     || (!_controller.Grounded) // or if we're not grounded
			     || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in normal conditions
			     || (_movement.CurrentState != CharacterStates.MovementStates.Walking) ) // or if we're not walking
			{
				// we do nothing and exit
				return;
			}

			// if the player presses the run button and if we're on the ground and not crouching and we can move freely, 
			// then we change the movement speed in the controller's parameters.
			if (_characterMovement != null)
			{
				_characterMovement.MovementSpeed = RunSpeed;
			}

			// if we're not already running, we trigger our sounds
			if (_movement.CurrentState != CharacterStates.MovementStates.Running)
			{
				PlayAbilityStartSfx();
				PlayAbilityUsedSfx();
				PlayAbilityStartFeedbacks();
				_runningStarted = true;
			}

			_movement.ChangeState(CharacterStates.MovementStates.Running);
		}

		/// <summary>
		/// Causes the character to stop running.
		/// </summary>
		public virtual void RunStop()
		{
			if (_runningStarted)
			{
				// if the run button is released, we revert back to the walking speed.
				if ((_characterMovement != null))
				{
					_characterMovement.ResetSpeed();
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
				}
				StopFeedbacks();
				StopSfx();
				_runningStarted = false;
			}            
		}

		/// <summary>
		/// Stops all run feedbacks
		/// </summary>
		protected virtual void StopFeedbacks()
		{
			if (_startFeedbackIsPlaying)
			{
				StopStartFeedbacks();
				PlayAbilityStopFeedbacks();
			}
		}

		/// <summary>
		/// Stops all run sounds
		/// </summary>
		protected virtual void StopSfx()
		{
			StopAbilityUsedSfx();
			PlayAbilityStopSfx();
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_runningAnimationParameterName, AnimatorControllerParameterType.Bool, out _runningAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Running),_character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}