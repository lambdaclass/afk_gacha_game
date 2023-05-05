using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Linq;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// A class meant to be overridden that handles a character's ability. 
	/// </summary>
	//[RequireComponent(typeof(Character))]
	public class CharacterAbility : TopDownMonoBehaviour 
	{
		/// the sound fx to play when the ability starts
		[Tooltip("the sound fx to play when the ability starts")]
		public AudioClip AbilityStartSfx;
		/// the sound fx to play while the ability is running
		[Tooltip("the sound fx to play while the ability is running")]
		public AudioClip AbilityInProgressSfx;
		/// the sound fx to play when the ability stops
		[Tooltip("the sound fx to play when the ability stops")]
		public AudioClip AbilityStopSfx;

		/// the feedbacks to play when the ability starts
		[Tooltip("the feedbacks to play when the ability starts")]
		public MMFeedbacks AbilityStartFeedbacks;
		/// the feedbacks to play when the ability stops
		[Tooltip("the feedbacks to play when the ability stops")]
		public MMFeedbacks AbilityStopFeedbacks;
                
		[Header("Permission")]
		/// if true, this ability can perform as usual, if not, it'll be ignored. You can use this to unlock abilities over time for example
		[Tooltip("if true, this ability can perform as usual, if not, it'll be ignored. You can use this to unlock abilities over time for example")]
		public bool AbilityPermitted = true;
        
		/// an array containing all the blocking movement states. If the Character is in one of these states and tries to trigger this ability, it won't be permitted. Useful to prevent this ability from being used while Idle or Swimming, for example.
		[Tooltip("an array containing all the blocking movement states. If the Character is in one of these states and tries to trigger this ability, it won't be permitted. Useful to prevent this ability from being used while Idle or Swimming, for example.")]
		public CharacterStates.MovementStates[] BlockingMovementStates;
		/// an array containing all the blocking condition states. If the Character is in one of these states and tries to trigger this ability, it won't be permitted. Useful to prevent this ability from being used while dead, for example.
		[Tooltip("an array containing all the blocking condition states. If the Character is in one of these states and tries to trigger this ability, it won't be permitted. Useful to prevent this ability from being used while dead, for example.")]
		public CharacterStates.CharacterConditions[] BlockingConditionStates;
		/// an array containing all the blocking weapon states. If one of the character's weapons is in one of these states and yet the character tries to trigger this ability, it won't be permitted. Useful to prevent this ability from being used while attacking, for example.
		[Tooltip("an array containing all the blocking weapon states. If one of the character's weapons is in one of these states and yet the character tries to trigger this ability, it won't be permitted. Useful to prevent this ability from being used while attacking, for example.")]
		public Weapon.WeaponStates[] BlockingWeaponStates;

		public virtual bool AbilityAuthorized
		{
			get
			{
				if (_character != null)
				{
					if ((BlockingMovementStates != null) && (BlockingMovementStates.Length > 0))
					{
						for (int i = 0; i < BlockingMovementStates.Length; i++)
						{
							if (BlockingMovementStates[i] == (_character.MovementState.CurrentState))
							{
								return false;
							}    
						}
					}

					if ((BlockingConditionStates != null) && (BlockingConditionStates.Length > 0))
					{
						for (int i = 0; i < BlockingConditionStates.Length; i++)
						{
							if (BlockingConditionStates[i] == (_character.ConditionState.CurrentState))
							{
								return false;
							}    
						}
					}
					
					if ((BlockingWeaponStates != null) && (BlockingWeaponStates.Length > 0))
					{
						for (int i = 0; i < BlockingWeaponStates.Length; i++)
						{
							foreach (CharacterHandleWeapon handleWeapon in _handleWeaponList)
							{
								if (handleWeapon.CurrentWeapon != null)
								{
									if (BlockingWeaponStates[i] == (handleWeapon.CurrentWeapon.WeaponState.CurrentState))
									{
										return false;
									}
								}
							}
						}
					}
				}
				return AbilityPermitted;
			}
		}
        
		/// whether or not this ability has been initialized
		public bool AbilityInitialized { get { return _abilityInitialized; } }
		
		public delegate void AbilityEvent();
		public AbilityEvent OnAbilityStart;
		public AbilityEvent OnAbilityStop;
        
		protected Character _character;
		protected TopDownController _controller;
		protected TopDownController2D _controller2D;
		protected TopDownController3D _controller3D;
		protected GameObject _model;
		protected Health _health;
		protected CharacterMovement _characterMovement;
		protected InputManager _inputManager;
		protected Animator _animator = null;
		protected CharacterStates _state;
		protected SpriteRenderer _spriteRenderer;
		protected MMStateMachine<CharacterStates.MovementStates> _movement;
		protected MMStateMachine<CharacterStates.CharacterConditions> _condition;
		protected AudioSource _abilityInProgressSfx;
		protected bool _abilityInitialized = false;
		protected float _verticalInput;
		protected float _horizontalInput;
		protected bool _startFeedbackIsPlaying = false;
		protected List<CharacterHandleWeapon> _handleWeaponList;

		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public virtual string HelpBoxText() { return ""; }

		/// <summary>
		/// On awake we proceed to pre initializing our ability
		/// </summary>
		protected virtual void Awake()
		{
			PreInitialization ();
		}

		/// <summary>
		/// On Start(), we call the ability's intialization
		/// </summary>
		protected virtual void Start () 
		{
			Initialization();
		}

		/// <summary>
		/// A method you can override to have an initialization before the actual initialization
		/// </summary>
		protected virtual void PreInitialization()
		{
			_character = this.gameObject.GetComponentInParent<Character>();
			BindAnimator();
		}

		/// <summary>
		/// Gets and stores components for further use
		/// </summary>
		protected virtual void Initialization()
		{
			BindAnimator();
			_controller = this.gameObject.GetComponentInParent<TopDownController>();
			_controller2D = this.gameObject.GetComponentInParent<TopDownController2D>();
			_controller3D = this.gameObject.GetComponentInParent<TopDownController3D>();
			_model = _character.CharacterModel;
			_characterMovement = _character?.FindAbility<CharacterMovement>();
			_spriteRenderer = this.gameObject.GetComponentInParent<SpriteRenderer>();
			_health = _character.CharacterHealth;
			_handleWeaponList = _character?.FindAbilities<CharacterHandleWeapon>();
			_inputManager = _character.LinkedInputManager;
			_state = _character.CharacterState;
			_movement = _character.MovementState;
			_condition = _character.ConditionState;
			_abilityInitialized = true;
		}

		/// <summary>
		/// Call this any time you want to force this ability to initialize (again)
		/// </summary>
		public virtual void ForceInitialization()
		{
			Initialization();
		}

		/// <summary>
		/// Binds the animator from the character and initializes the animator parameters
		/// </summary>
		protected virtual void BindAnimator()
		{
			if (_character._animator == null)
			{
				_character.AssignAnimator();
			}

			_animator = _character._animator;

			if (_animator != null)
			{
				InitializeAnimatorParameters();
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected virtual void InitializeAnimatorParameters()
		{

		}

		/// <summary>
		/// Internal method to check if an input manager is present or not
		/// </summary>
		protected virtual void InternalHandleInput()
		{
			if (_inputManager == null) { return; }
			_horizontalInput = _inputManager.PrimaryMovement.x;
			_verticalInput = _inputManager.PrimaryMovement.y;
			HandleInput();
		}

		/// <summary>
		/// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls methods if conditions are met
		/// </summary>
		protected virtual void HandleInput()
		{

		}

		/// <summary>
		/// Resets all input for this ability. Can be overridden for ability specific directives
		/// </summary>
		public virtual void ResetInput()
		{
			_horizontalInput = 0f;
			_verticalInput = 0f;
		}


		/// <summary>
		/// The first of the 3 passes you can have in your ability. Think of it as EarlyUpdate() if it existed
		/// </summary>
		public virtual void EarlyProcessAbility()
		{
			InternalHandleInput();
		}

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public virtual void ProcessAbility()
		{
			
		}

		/// <summary>
		/// The last of the 3 passes you can have in your ability. Think of it as LateUpdate()
		/// </summary>
		public virtual void LateProcessAbility()
		{
			
		}

		/// <summary>
		/// Override this to send parameters to the character's animator. This is called once per cycle, by the Character class, after Early, normal and Late process().
		/// </summary>
		public virtual void UpdateAnimator()
		{

		}

		/// <summary>
		/// Changes the status of the ability's permission
		/// </summary>
		/// <param name="abilityPermitted">If set to <c>true</c> ability permitted.</param>
		public virtual void PermitAbility(bool abilityPermitted)
		{
			AbilityPermitted = abilityPermitted;
		}

		/// <summary>
		/// Override this to specify what should happen in this ability when the character flips
		/// </summary>
		public virtual void Flip()
		{
			
		}

		/// <summary>
		/// Override this to reset this ability's parameters. It'll be automatically called when the character gets killed, in anticipation for its respawn.
		/// </summary>
		public virtual void ResetAbility()
		{
			
		}

		/// <summary>
		/// Changes the reference to the input manager with the one set in parameters
		/// </summary>
		/// <param name="newInputManager"></param>
		public virtual void SetInputManager(InputManager newInputManager)
		{
			_inputManager = newInputManager;
		}

		/// <summary>
		/// Plays the ability start sound effect
		/// </summary>
		public virtual void PlayAbilityStartSfx()
		{
			if (AbilityStartSfx!=null)
			{
				AudioSource tmp = new AudioSource();
				MMSoundManagerSoundPlayEvent.Trigger(AbilityStartSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);	
			}
		}	

		/// <summary>
		/// Plays the ability used sound effect
		/// </summary>
		public virtual void PlayAbilityUsedSfx()
		{
			if (AbilityInProgressSfx != null) 
			{	
				if (_abilityInProgressSfx == null)
				{
					_abilityInProgressSfx = MMSoundManagerSoundPlayEvent.Trigger(AbilityInProgressSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position, true);
				}
			}
		}	

		/// <summary>
		/// Stops the ability used sound effect
		/// </summary>
		public virtual void StopAbilityUsedSfx()
		{
			if (_abilityInProgressSfx != null)
			{
				MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Free, 0, _abilityInProgressSfx);
				_abilityInProgressSfx = null;
			}
		}	

		/// <summary>
		/// Plays the ability stop sound effect
		/// </summary>
		public virtual void PlayAbilityStopSfx()
		{
			if (AbilityStopSfx!=null) 
			{	
				MMSoundManagerSoundPlayEvent.Trigger(AbilityStopSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);
			}
		}

		/// <summary>
		/// Plays the ability start sound effect
		/// </summary>
		public virtual void PlayAbilityStartFeedbacks()
		{
			AbilityStartFeedbacks?.PlayFeedbacks(this.transform.position);
			_startFeedbackIsPlaying = true;
			OnAbilityStart?.Invoke();
		}

		/// <summary>
		/// Stops the ability used sound effect
		/// </summary>
		public virtual void StopStartFeedbacks()
		{
			AbilityStartFeedbacks?.StopFeedbacks();
			_startFeedbackIsPlaying = false;
		}

		/// <summary>
		/// Plays the ability stop sound effect
		/// </summary>
		public virtual void PlayAbilityStopFeedbacks()
		{
			AbilityStopFeedbacks?.PlayFeedbacks();
			OnAbilityStop?.Invoke();
		}

		/// <summary>
		/// Registers a new animator parameter to the list
		/// </summary>
		/// <param name="parameterName">Parameter name.</param>
		/// <param name="parameterType">Parameter type.</param>
		protected virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType, out int parameter)
		{
			parameter = Animator.StringToHash(parameterName);

			if (_animator == null) 
			{
				return;
			}
			if (_animator.MMHasParameterOfType(parameterName, parameterType))
			{
				if (_character != null)
				{
					_character._animatorParameters.Add(parameter);	
				}
			}
		}

		/// <summary>
		/// Override this to describe what should happen to this ability when the character respawns
		/// </summary>
		protected virtual void OnRespawn()
		{
		}

		/// <summary>
		/// Override this to describe what should happen to this ability when the character respawns
		/// </summary>
		protected virtual void OnDeath()
		{
			StopAbilityUsedSfx ();
			StopStartFeedbacks();
		}

		/// <summary>
		/// Override this to describe what should happen to this ability when the character takes a hit
		/// </summary>
		protected virtual void OnHit()
		{

		}

		/// <summary>
		/// On enable, we bind our respawn delegate
		/// </summary>
		protected virtual void OnEnable()
		{
			if (_health == null)
			{
				_health = this.gameObject.GetComponentInParent<Character>().CharacterHealth;
			}

			if (_health == null)
			{
				_health = this.gameObject.GetComponentInParent<Health>();
			}

			if (_health != null)
			{
				_health.OnRevive += OnRespawn;
				_health.OnDeath += OnDeath;
				_health.OnHit += OnHit;
			}
		}

		/// <summary>
		/// On disable, we unbind our respawn delegate
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				_health.OnRevive -= OnRespawn;
				_health.OnDeath -= OnDeath;
				_health.OnHit -= OnHit;
			}	
		}
	}
}