using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this ability to a Character to have it handle ground movement (walk, and potentially run, crawl, etc) in x and z direction for 3D, x and y for 2D
	/// Animator parameters : Speed (float), Walking (bool)
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Movement")] 
	public class CharacterMovement : CharacterAbility 
	{
		/// the possible rotation modes for the character
		public enum Movements { Free, Strict2DirectionsHorizontal, Strict2DirectionsVertical, Strict4Directions, Strict8Directions }

		/// the current reference movement speed
		public float MovementSpeed { get; set; }
		/// if this is true, movement will be forbidden (as well as flip)
		public bool MovementForbidden { get; set; }

		[Header("Direction")]

		/// whether the character can move freely, in 2D only, in 4 or 8 cardinal directions
		[Tooltip("whether the character can move freely, in 2D only, in 4 or 8 cardinal directions")]
		public Movements Movement = Movements.Free;

		[Header("Settings")]

		/// whether or not movement input is authorized at that time
		[Tooltip("whether or not movement input is authorized at that time")]
		public bool InputAuthorized = true;
		/// whether or not input should be analog
		[Tooltip("whether or not input should be analog")]
		public bool AnalogInput = false;
		/// whether or not input should be set from another script
		[Tooltip("whether or not input should be set from another script")]
		public bool ScriptDrivenInput = false;

		[Header("Speed")]

		/// the speed of the character when it's walking
		[Tooltip("the speed of the character when it's walking")]
		public float WalkSpeed = 6f;
		/// whether or not this component should set the controller's movement
		[Tooltip("whether or not this component should set the controller's movement")]
		public bool ShouldSetMovement = true;
		/// the speed threshold after which the character is not considered idle anymore
		[Tooltip("the speed threshold after which the character is not considered idle anymore")]
		public float IdleThreshold = 0.05f;

		[Header("Acceleration")]

		/// the acceleration to apply to the current speed / 0f : no acceleration, instant full speed
		[Tooltip("the acceleration to apply to the current speed / 0f : no acceleration, instant full speed")]
		public float Acceleration = 10f;
		/// the deceleration to apply to the current speed / 0f : no deceleration, instant stop
		[Tooltip("the deceleration to apply to the current speed / 0f : no deceleration, instant stop")]
		public float Deceleration = 10f;

		/// whether or not to interpolate movement speed
		[Tooltip("whether or not to interpolate movement speed")]
		public bool InterpolateMovementSpeed = false;
		public float MovementSpeedMaxMultiplier { get; set; } = float.MaxValue;
		private float _movementSpeedMultiplier;
		/// the multiplier to apply to the horizontal movement
		public float MovementSpeedMultiplier
		{
			get => Mathf.Min(_movementSpeedMultiplier, MovementSpeedMaxMultiplier);
			set => _movementSpeedMultiplier = value;
		}
		/// the multiplier to apply to the horizontal movement, applied by contextual elements (movement zones, etc)
		public Stack<float> ContextSpeedStack = new Stack<float>();
		public float ContextSpeedMultiplier => ContextSpeedStack.Count > 0 ? ContextSpeedStack.Peek() : 1;

		[Header("Walk Feedback")]
		/// the particles to trigger while walking
		[Tooltip("the particles to trigger while walking")]
		public ParticleSystem[] WalkParticles;

		[Header("Touch The Ground Feedback")]
		/// the particles to trigger when touching the ground
		[Tooltip("the particles to trigger when touching the ground")]
		public ParticleSystem[] TouchTheGroundParticles;
		/// the sfx to trigger when touching the ground
		[Tooltip("the sfx to trigger when touching the ground")]
		public AudioClip[] TouchTheGroundSfx;

		protected float _movementSpeed;
		protected float _horizontalMovement;
		protected float _verticalMovement;
		protected Vector3 _movementVector;
		protected Vector2 _currentInput = Vector2.zero;
		protected Vector2 _normalizedInput;
		protected Vector2 _lerpedInput = Vector2.zero;
		protected float _acceleration = 0f;
		protected bool _walkParticlesPlaying = false;

		protected const string _speedAnimationParameterName = "Speed";
		protected const string _walkingAnimationParameterName = "Walking";
		protected const string _idleAnimationParameterName = "Idle";
		protected int _speedAnimationParameter;
		protected int _walkingAnimationParameter;
		protected int _idleAnimationParameter;

		/// <summary>
		/// On Initialization, we set our movement speed to WalkSpeed.
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization ();
			ResetAbility();
		}

		/// <summary>
		/// Resets character movement states and speeds
		/// </summary>
        public override void ResetAbility()
        {
	        base.ResetAbility();
			MovementSpeed = WalkSpeed;
			if (ContextSpeedStack != null)
			{
				ContextSpeedStack.Clear();
			}
			if ((_movement != null) && (_movement.CurrentState != CharacterStates.MovementStates.FallingDownHole))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}
			MovementSpeedMultiplier = 1f;
			MovementForbidden = false;

			foreach (ParticleSystem system in TouchTheGroundParticles)
			{
				if (system != null)
				{
					system.Stop();
				}				
			}
			foreach (ParticleSystem system in WalkParticles)
			{
				if (system != null)
				{
					system.Stop();
				}				
			}
		} 

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			
			HandleFrozen();
			
			if (!AbilityAuthorized
			    || ((_condition.CurrentState != CharacterStates.CharacterConditions.Normal) && (_condition.CurrentState != CharacterStates.CharacterConditions.ControlledMovement)))
			{
				if (AbilityAuthorized)
				{
					StopAbilityUsedSfx();
				}
				return;
			}
			HandleDirection();
			HandleMovement();
			Feedbacks ();
		}

		/// <summary>
		/// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls
		/// methods if conditions are met
		/// </summary>
		protected override void HandleInput()
		{
			if (ScriptDrivenInput)
			{
				return;
			}

			if (InputAuthorized)
			{
				_horizontalMovement = _horizontalInput;
				_verticalMovement = _verticalInput;
			}
			else
			{
				_horizontalMovement = 0f;
				_verticalMovement = 0f;
			}	
		}

		/// <summary>
		/// Sets the horizontal move value.
		/// </summary>
		/// <param name="value">Horizontal move value, between -1 and 1 - positive : will move to the right, negative : will move left </param>
		public virtual void SetMovement(Vector2 value)
		{
			_horizontalMovement = value.x;
			_verticalMovement = value.y;
		}

		/// <summary>
		/// Sets the horizontal part of the movement
		/// </summary>
		/// <param name="value"></param>
		public virtual void SetHorizontalMovement(float value)
		{
			_horizontalMovement = value;
		}

		/// <summary>
		/// Sets the vertical part of the movement
		/// </summary>
		/// <param name="value"></param>
		public virtual void SetVerticalMovement(float value)
		{
			_verticalMovement = value;
		}
		
		/// <summary>
		/// Applies a movement multiplier for the specified duration
		/// </summary>
		/// <param name="movementMultiplier"></param>
		/// <param name="duration"></param>
		public virtual void ApplyMovementMultiplier(float movementMultiplier, float duration)
		{
			StartCoroutine(ApplyMovementMultiplierCo(movementMultiplier, duration));
		}
		
		/// <summary>
		/// A coroutine used to apply a movement multiplier for a certain duration only
		/// </summary>
		/// <param name="movementMultiplier"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		protected virtual IEnumerator ApplyMovementMultiplierCo(float movementMultiplier, float duration)
		{
			if (_characterMovement == null)
			{
				yield break;
			}
			SetContextSpeedMultiplier(movementMultiplier);
			yield return MMCoroutine.WaitFor(duration);
			ResetContextSpeedMultiplier();
		}

		/// <summary>
		/// Stacks a new context speed multiplier
		/// </summary>
		/// <param name="newMovementSpeedMultiplier"></param>
		public virtual void SetContextSpeedMultiplier(float newMovementSpeedMultiplier)
		{
			ContextSpeedStack.Push(newMovementSpeedMultiplier);
		}

		/// <summary>
		/// Revers the context speed multiplier to its previous value
		/// </summary>
		public virtual void ResetContextSpeedMultiplier()
		{
			if (ContextSpeedStack.Count <= 0)
			{
				return;
			}
			
			ContextSpeedStack.Pop();
		}

		/// <summary>
		/// Modifies player input to account for the selected movement mode
		/// </summary>
		protected virtual void HandleDirection()
		{
			switch (Movement)
			{
				case Movements.Free:
					// do nothing
					break;
				case Movements.Strict2DirectionsHorizontal:
					_verticalMovement = 0f;
					break;
				case Movements.Strict2DirectionsVertical:
					_horizontalMovement = 0f;
					break;
				case Movements.Strict4Directions:
					if (Mathf.Abs(_horizontalMovement) > Mathf.Abs(_verticalMovement))
					{
						_verticalMovement = 0f;
					}
					else
					{
						_horizontalMovement = 0f;
					}
					break;
				case Movements.Strict8Directions:
					_verticalMovement = Mathf.Round(_verticalMovement);
					_horizontalMovement = Mathf.Round(_horizontalMovement);
					break;
			}
		}

		/// <summary>
		/// Called at Update(), handles horizontal movement
		/// </summary>
		protected virtual void HandleMovement()
		{
			// if we're not walking anymore, we stop our walking sound
			if ((_movement.CurrentState != CharacterStates.MovementStates.Walking) && _startFeedbackIsPlaying)
			{
				StopStartFeedbacks();
			}

			// if we're not walking anymore, we stop our walking sound
			if (_movement.CurrentState != CharacterStates.MovementStates.Walking && _abilityInProgressSfx != null)
			{
				StopAbilityUsedSfx();
			}

			if (_movement.CurrentState == CharacterStates.MovementStates.Walking && _abilityInProgressSfx == null)
			{
				PlayAbilityUsedSfx();
			}

			// if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
			if ( !AbilityAuthorized
			     || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) )
			{
				return;				
			}
            
			CheckJustGotGrounded();

			if (MovementForbidden)
			{
				_horizontalMovement = 0f;
				_verticalMovement = 0f;
			}

			// if the character is not grounded, but currently idle or walking, we change its state to Falling
			if (!_controller.Grounded
			    && (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			    && (
				    (_movement.CurrentState == CharacterStates.MovementStates.Walking)
				    || (_movement.CurrentState == CharacterStates.MovementStates.Idle)
			    ))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Falling);
			}

			if (_controller.Grounded && (_movement.CurrentState == CharacterStates.MovementStates.Falling))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}

			if ( _controller.Grounded
			     && (_controller.CurrentMovement.magnitude > IdleThreshold)
			     && ( _movement.CurrentState == CharacterStates.MovementStates.Idle))
			{				
				_movement.ChangeState(CharacterStates.MovementStates.Walking);	
				PlayAbilityStartSfx();	
				PlayAbilityUsedSfx();
				PlayAbilityStartFeedbacks();
			}
            
			// if we're walking and not moving anymore, we go back to the Idle state
			if ((_movement.CurrentState == CharacterStates.MovementStates.Walking) 
			    && (_controller.CurrentMovement.magnitude <= IdleThreshold))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				PlayAbilityStopSfx();
				PlayAbilityStopFeedbacks();
			}

			if (ShouldSetMovement)
			{
				SetMovement ();	
			}
		}

		/// <summary>
		/// Describes what happens when the character is in the frozen state
		/// </summary>
		protected virtual void HandleFrozen()
		{
			if (!AbilityAuthorized)
			{
				return;
			}
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Frozen)
			{
				_horizontalMovement = 0f;
				_verticalMovement = 0f;
				SetMovement();
			}
		}

		/// <summary>
		/// Moves the controller
		/// </summary>
		protected virtual void SetMovement()
		{
			_movementVector = Vector3.zero;
			_currentInput = Vector2.zero;

			_currentInput.x = _horizontalMovement;
			_currentInput.y = _verticalMovement;
            
			_normalizedInput = _currentInput.normalized;

			float interpolationSpeed = 1f;
            
			if ((Acceleration == 0) || (Deceleration == 0))
			{
				_lerpedInput = AnalogInput ? _currentInput : _normalizedInput;
			}
			else
			{
				if (_normalizedInput.magnitude == 0)
				{
					_acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
					_lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
					interpolationSpeed = Deceleration;
				}
				else
				{
					_acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
					_lerpedInput = AnalogInput ? Vector2.ClampMagnitude (_currentInput, _acceleration) : Vector2.ClampMagnitude(_normalizedInput, _acceleration);
					interpolationSpeed = Acceleration;
				}
			}		
			
			_movementVector.x = _lerpedInput.x;
			_movementVector.y = 0f;
			_movementVector.z = _lerpedInput.y;

			if (InterpolateMovementSpeed)
			{
				_movementSpeed = Mathf.Lerp(_movementSpeed, MovementSpeed * ContextSpeedMultiplier * MovementSpeedMultiplier, interpolationSpeed * Time.deltaTime);
			}
			else
			{
				_movementSpeed = MovementSpeed * MovementSpeedMultiplier * ContextSpeedMultiplier;
			}

			_movementVector *= _movementSpeed;

			if (_movementVector.magnitude > MovementSpeed * ContextSpeedMultiplier * MovementSpeedMultiplier)
			{
				_movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed);
			}

			if ((_currentInput.magnitude <= IdleThreshold) && (_controller.CurrentMovement.magnitude < IdleThreshold))
			{
				_movementVector = Vector3.zero;
			}
            
			_controller.SetMovement (_movementVector);
		} 

		/// <summary>
		/// Every frame, checks if we just hit the ground, and if yes, changes the state and triggers a particle effect
		/// </summary>
		protected virtual void CheckJustGotGrounded()
		{
			// if the character just got grounded
			if (_controller.JustGotGrounded)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}
		}

		/// <summary>
		/// Plays particles when walking, and particles and sounds when landing
		/// </summary>
		protected virtual void Feedbacks ()
		{
			if (_controller.Grounded)
			{
				if (_controller.CurrentMovement.magnitude > IdleThreshold)	
				{			
					foreach (ParticleSystem system in WalkParticles)
					{				
						if (!_walkParticlesPlaying && (system != null))
						{
							system.Play();		
						}
						_walkParticlesPlaying = true;
					}	
				}
				else
				{
					foreach (ParticleSystem system in WalkParticles)
					{						
						if (_walkParticlesPlaying && (system != null))
						{
							system.Stop();		
							_walkParticlesPlaying = false;
						}
					}
				}
			}
			else
			{
				foreach (ParticleSystem system in WalkParticles)
				{						
					if (_walkParticlesPlaying && (system != null))
					{
						system.Stop();		
						_walkParticlesPlaying = false;
					}
				}
			}

			if (_controller.JustGotGrounded)
			{
				foreach (ParticleSystem system in TouchTheGroundParticles)
				{
					if (system != null)
					{
						system.Clear();
						system.Play();
					}					
				}
				foreach (AudioClip clip in TouchTheGroundSfx)
				{
					MMSoundManagerSoundPlayEvent.Trigger(clip, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);
				}
			}
		}

		/// <summary>
		/// Resets this character's speed
		/// </summary>
		public virtual void ResetSpeed()
		{
			MovementSpeed = WalkSpeed;
		}

		/// <summary>
		/// On Respawn, resets the speed
		/// </summary>
		protected override void OnRespawn()
		{
			ResetSpeed();
			MovementForbidden = false;
		}

		protected override void OnDeath()
		{
			base.OnDeath();
			DisableWalkParticles();
		}

		/// <summary>
		/// Disables all walk particle systems that may be playing
		/// </summary>
		protected virtual void DisableWalkParticles()
		{
			if (WalkParticles.Length > 0)
			{
				foreach (ParticleSystem walkParticle in WalkParticles)
				{
					if (walkParticle != null)
					{
						walkParticle.Stop();
					}
				}
			}
		}

		/// <summary>
		/// On disable we make sure to turn off anything that could still be playing
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();
			DisableWalkParticles();
			PlayAbilityStopSfx();
			PlayAbilityStopFeedbacks();
			StopAbilityUsedSfx();
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
			RegisterAnimatorParameter (_walkingAnimationParameterName, AnimatorControllerParameterType.Bool, out _walkingAnimationParameter);
			RegisterAnimatorParameter (_idleAnimationParameterName, AnimatorControllerParameterType.Bool, out _idleAnimationParameter);
		}

		/// <summary>
		/// Sends the current speed and the current value of the Walking state to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _speedAnimationParameter, Mathf.Abs(_controller.CurrentMovement.magnitude),_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Walking),_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Idle),_character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}