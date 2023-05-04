using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Extend this class to activate something when a button is pressed in a certain zone
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Button Activated")]
	public class ButtonActivated : TopDownMonoBehaviour 
	{
		public enum ButtonActivatedRequirements { Character, ButtonActivator, Either, None }
		public enum InputTypes { Default, Button, Key }

		[Header("Requirements")]
		[MMInformation("Here you can specify what is needed for something to interact with this zone. Does it require the ButtonActivation character ability? Can it only be interacted with by the Player? ", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
		/// if this is true, objects with a ButtonActivator class will be able to interact with this zone
		[Tooltip("if this is true, objects with a ButtonActivator class will be able to interact with this zone")]
		public ButtonActivatedRequirements ButtonActivatedRequirement = ButtonActivatedRequirements.Either;
		/// if this is true, this can only be activated by player Characters
		[Tooltip("if this is true, this can only be activated by player Characters")]
		public bool RequiresPlayerType = true;
		/// if this is true, this zone can only be activated if the character has the required ability
		[Tooltip("if this is true, this zone can only be activated if the character has the required ability")]
		public bool RequiresButtonActivationAbility = true;
        
		[Header("Activation Conditions")]

		[MMInformation("Here you can specific how that zone is interacted with. You can have it auto activate, activate only when grounded, or prevent its activation altogether.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// if this is false, the zone won't be activable 
		[Tooltip("if this is false, the zone won't be activable ")]
		public bool Activable = true;
		/// if true, the zone will activate whether the button is pressed or not
		[Tooltip("if true, the zone will activate whether the button is pressed or not")]
		public bool AutoActivation = false;
		/// the delay, in seconds, during which the character has to be within the zone to activate it
		[MMCondition("AutoActivation", true)]
		[Tooltip("the delay, in seconds, during which the character has to be within the zone to activate it")]
		public float AutoActivationDelay = 0f;
		/// if this is true, exiting the zone will reset the auto activation delay
		[MMCondition("AutoActivation", true)]
		[Tooltip("if this is true, exiting the zone will reset the auto activation delay")]
		public bool AutoActivationDelayResetsOnExit = true;
		/// if this is set to false, the zone won't be activable while not grounded
		[Tooltip("if this is set to false, the zone won't be activable while not grounded")]
		public bool CanOnlyActivateIfGrounded = false;
		/// Set this to true if you want the CharacterBehaviorState to be notified of the player's entry into the zone.
		[Tooltip("Set this to true if you want the CharacterBehaviorState to be notified of the player's entry into the zone.")]
		public bool ShouldUpdateState = true;
		/// if this is true, enter won't be retriggered if another object enters, and exit will only be triggered when the last object exits
		[Tooltip("if this is true, enter won't be retriggered if another object enters, and exit will only be triggered when the last object exits")]
		public bool OnlyOneActivationAtOnce = true;
		/// a layermask with all the layers that can interact with this specific button activated zone
		[Tooltip("a layermask with all the layers that can interact with this specific button activated zone")]
		public LayerMask TargetLayerMask = ~0;

		[Header("Number of Activations")]

		[MMInformation("You can decide to have that zone be interactable forever, or just a limited number of times, and can specify a delay between uses (in seconds).",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// if this is set to false, your number of activations will be MaxNumberOfActivations
		[Tooltip("if this is set to false, your number of activations will be MaxNumberOfActivations")]
		public bool UnlimitedActivations = true;
		/// the number of times the zone can be interacted with
		[Tooltip("the number of times the zone can be interacted with")]
		public int MaxNumberOfActivations = 0;
		/// the delay (in seconds) after an activation during which the zone can't be activated
		[Tooltip("the delay (in seconds) after an activation during which the zone can't be activated")]
		public float DelayBetweenUses = 0f;
		/// if this is true, the zone will disable itself (forever or until you manually reactivate it) after its last use
		[Tooltip("if this is true, the zone will disable itself (forever or until you manually reactivate it) after its last use")]
		public bool DisableAfterUse = false;

		[Header("Input")]

		/// the selected input type (default, button or key)
		[Tooltip("the selected input type (default, button or key)")]
		public InputTypes InputType = InputTypes.Default;
		#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			/// the input action to use for this button activated object
			public InputActionProperty InputSystemAction = new InputActionProperty(
				new InputAction(
					name: "ButtonActivatedAction",
					type: InputActionType.Button, 
					binding: "Keyboard/space", 
					interactions: "Press(behavior=2)"));
		#else
			/// the selected button string used to activate this zone
			[MMEnumCondition("InputType", (int)InputTypes.Button)]
			[Tooltip("the selected button string used to activate this zone")]
			public string InputButton = "Interact";
			/// the key used to activate this zone
			[MMEnumCondition("InputType", (int)InputTypes.Key)]
			[Tooltip("the key used to activate this zone")]
			public KeyCode InputKey = KeyCode.Space;
		#endif

		[Header("Animation")]

		/// an (absolutely optional) animation parameter that can be triggered on the character when activating the zone	
		[Tooltip("an (absolutely optional) animation parameter that can be triggered on the character when activating the zone	")]
		public string AnimationTriggerParameterName;

		[Header("Visual Prompt")]

		[MMInformation("You can have this zone show a visual prompt to indicate to the player that it's interactable.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
		/// if this is true, a prompt will be shown if setup properly
		[Tooltip("if this is true, a prompt will be shown if setup properly")]
		public bool UseVisualPrompt = true;
		/// the gameobject to instantiate to present the prompt
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the gameobject to instantiate to present the prompt")]
		public ButtonPrompt ButtonPromptPrefab;
		/// the text to display in the button prompt
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the text to display in the button prompt")]
		public string ButtonPromptText = "A";
		/// the text to display in the button prompt
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the text to display in the button prompt")]
		public Color ButtonPromptColor = MMColors.LawnGreen;
		/// the color for the prompt's text
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the color for the prompt's text")]
		public Color ButtonPromptTextColor = MMColors.White;
		/// If true, the "buttonA" prompt will always be shown, whether the player is in the zone or not.
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("If true, the 'buttonA' prompt will always be shown, whether the player is in the zone or not.")]
		public bool AlwaysShowPrompt = true;
		/// If true, the "buttonA" prompt will be shown when a player is colliding with the zone
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("If true, the 'buttonA' prompt will be shown when a player is colliding with the zone")]
		public bool ShowPromptWhenColliding = true;
		/// If true, the prompt will hide after use
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("If true, the prompt will hide after use")]
		public bool HidePromptAfterUse = false;
		/// the position of the actual buttonA prompt relative to the object's center
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the position of the actual buttonA prompt relative to the object's center")]
		public Vector3 PromptRelativePosition = Vector3.zero;
		/// the rotation of the actual buttonA prompt 
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the rotation of the actual buttonA prompt ")]
		public Vector3 PromptRotation = Vector3.zero;

		[Header("Feedbacks")]
		/// a feedback to play when the zone gets activated
		[Tooltip("a feedback to play when the zone gets activated")]
		public MMFeedbacks ActivationFeedback;
		/// a feedback to play when the zone tries to get activated but can't
		[Tooltip("a feedback to play when the zone tries to get activated but can't")]
		public MMFeedbacks DeniedFeedback;
		/// a feedback to play when the zone gets entered	
		[Tooltip("a feedback to play when the zone gets entered	")]
		public MMFeedbacks EnterFeedback;
		/// a feedback to play when the zone gets exited	
		[Tooltip("a feedback to play when the zone gets exited	")]
		public MMFeedbacks ExitFeedback;

		[Header("Actions")]
		/// a UnityEvent to trigger when this zone gets activated
		[Tooltip("a UnityEvent to trigger when this zone gets activated")]
		public UnityEvent OnActivation;
		/// a UnityEvent to trigger when this zone gets exited
		[Tooltip("a UnityEvent to trigger when this zone gets exited")]
		public UnityEvent OnExit;
		/// a UnityEvent to trigger when a character is within the zone
		[Tooltip("a UnityEvent to trigger when a character is within the zone")]
		public UnityEvent OnStay;

		protected Animator _buttonPromptAnimator;
		protected ButtonPrompt _buttonPrompt;
		protected Collider _collider;
		protected Collider2D _collider2D;
		protected bool _promptHiddenForever = false;
		protected CharacterButtonActivation _characterButtonActivation;
		protected int _numberOfActivationsLeft;
		protected float _lastActivationTimestamp;
		protected List<GameObject> _collidingObjects;
		protected Character _currentCharacter;
		protected bool _staying = false;
		protected Coroutine _autoActivationCoroutine;
        
		public bool AutoActivationInProgress { get; set; }
		public float AutoActivationStartedAt { get; set; }
		public bool InputActionPerformed
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
					return InputSystemAction.action.WasPressedThisFrame();
				#else
					return false;
				#endif
			}
		}

		/// <summary>
		/// On Enable, we initialize our ButtonActivated zone
		/// </summary>
		protected virtual void OnEnable()
		{
			Initialization ();
		}

		/// <summary>
		/// Grabs components and shows prompt if needed
		/// </summary>
		public virtual void Initialization()
		{
			_collider = this.gameObject.GetComponent<Collider>();
			_collider2D = this.gameObject.GetComponent<Collider2D>();
			_numberOfActivationsLeft = MaxNumberOfActivations;
			_collidingObjects = new List<GameObject>();

			ActivationFeedback?.Initialization(this.gameObject);
			DeniedFeedback?.Initialization(this.gameObject);
			EnterFeedback?.Initialization(this.gameObject);
			ExitFeedback?.Initialization(this.gameObject);

			if (AlwaysShowPrompt)
			{
				ShowPrompt();
			}
			
			#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
				InputSystemAction.action.Enable();
			#endif
		}
		
		/// <summary>
		/// On disable we disable our input action if needed
		/// </summary>
		protected virtual void OnDisable()
		{
			#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
				InputSystemAction.action.Disable();
			#endif
		}

		protected virtual IEnumerator TriggerButtonActionCo()
		{
			if (AutoActivationDelay <= 0f)
			{
				TriggerButtonAction();
				yield break;
			}
			else
			{
				AutoActivationInProgress = true;
				AutoActivationStartedAt = Time.time;
				yield return MMCoroutine.WaitFor(AutoActivationDelay);
				AutoActivationInProgress = false;
				TriggerButtonAction();
				yield break;
			}
		}

		/// <summary>
		/// When the input button is pressed, we check whether or not the zone can be activated, and if yes, trigger ZoneActivated
		/// </summary>
		public virtual void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				PromptError();
				return;
			}

			_staying = true;
			ActivateZone();
		}

		public virtual void TriggerExitAction(GameObject collider)
		{
			_staying = false;
			if (OnExit != null)
			{
				OnExit.Invoke();
			}
		}

		/// <summary>
		/// Makes the zone activable
		/// </summary>
		public virtual void MakeActivable()
		{
			Activable = true;
		}

		/// <summary>
		/// Makes the zone unactivable
		/// </summary>
		public virtual void MakeUnactivable()
		{
			Activable = false;
		}

		/// <summary>
		/// Makes the zone activable if it wasn't, unactivable if it was activable.
		/// </summary>
		public virtual void ToggleActivable()
		{
			Activable = !Activable;
		}

		protected virtual void Update()
		{
			if (_staying && (OnStay != null))
			{
				OnStay.Invoke();
			}
		}

		/// <summary>
		/// Activates the zone
		/// </summary>
		protected virtual void ActivateZone()
		{
			if (OnActivation != null)
			{
				OnActivation.Invoke();
			}

			_lastActivationTimestamp = Time.time;

			ActivationFeedback?.PlayFeedbacks(this.transform.position);

			if (HidePromptAfterUse)
			{
				_promptHiddenForever = true;
				HidePrompt();	
			}	
			_numberOfActivationsLeft--;

			if (DisableAfterUse && (_numberOfActivationsLeft <= 0))
			{
				DisableZone();
			}
		}

		/// <summary>
		/// Triggers an error 
		/// </summary>
		public virtual void PromptError()
		{
			if (_buttonPromptAnimator != null)
			{
				_buttonPromptAnimator.SetTrigger("Error");
			}
			DeniedFeedback?.PlayFeedbacks(this.transform.position);
		}

		/// <summary>
		/// Shows the button A prompt.
		/// </summary>
		public virtual void ShowPrompt()
		{
			if (!UseVisualPrompt || _promptHiddenForever || (ButtonPromptPrefab == null))
			{
				return;
			}
            
			// we add a blinking A prompt to the top of the zone
			if (_buttonPrompt == null)
			{
				_buttonPrompt = (ButtonPrompt)Instantiate(ButtonPromptPrefab);
				_buttonPrompt.Initialization();
				_buttonPromptAnimator = _buttonPrompt.gameObject.MMGetComponentNoAlloc<Animator>();
			}
			
			if (_collider != null)
			{
				_buttonPrompt.transform.position = _collider.bounds.center + PromptRelativePosition;
			}
			if (_collider2D != null)
			{
				_buttonPrompt.transform.position = _collider2D.bounds.center + PromptRelativePosition;
			}
			_buttonPrompt.transform.parent = transform;
			_buttonPrompt.transform.localEulerAngles = PromptRotation;
			_buttonPrompt.SetText(ButtonPromptText);
			_buttonPrompt.SetBackgroundColor(ButtonPromptColor);
			_buttonPrompt.SetTextColor(ButtonPromptTextColor);
			_buttonPrompt.Show();
		}

		/// <summary>
		/// Hides the button A prompt.
		/// </summary>
		public virtual void HidePrompt()
		{
			if (_buttonPrompt != null)
			{
				_buttonPrompt.Hide();
			}
		}

		/// <summary>
		/// Disables the button activated zone
		/// </summary>
		public virtual void DisableZone()
		{
			Activable = false;
            
			if (_collider != null)
			{
				_collider.enabled = false;
			}

			if (_collider2D != null)
			{
				_collider2D.enabled = false;
			}
	            
			if (ShouldUpdateState && (_characterButtonActivation != null))
			{
				_characterButtonActivation.InButtonActivatedZone = false;
				_characterButtonActivation.ButtonActivatedZone = null;
			}
		}

		/// <summary>
		/// Enables the button activated zone
		/// </summary>
		public virtual void EnableZone()
		{
			Activable = true;
            
			if (_collider != null)
			{
				_collider.enabled = true;
			}

			if (_collider2D != null)
			{
				_collider2D.enabled = true;
			}
		}

		/// <summary>
		/// Handles enter collision with 2D triggers
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected virtual void OnTriggerEnter2D (Collider2D collidingObject)
		{
			TriggerEnter (collidingObject.gameObject);
		}
		/// <summary>
		/// Handles enter collision with 2D triggers
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected virtual void OnTriggerExit2D (Collider2D collidingObject)
		{
			TriggerExit (collidingObject.gameObject);
		}
		/// <summary>
		/// Handles enter collision with 2D triggers
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected virtual void OnTriggerEnter (Collider collidingObject)
		{
			TriggerEnter (collidingObject.gameObject);
		}
		/// <summary>
		/// Handles enter collision with 2D triggers
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected virtual void OnTriggerExit (Collider collidingObject)
		{
			TriggerExit (collidingObject.gameObject);
		}
        
		/// <summary>
		/// Triggered when something collides with the button activated zone
		/// </summary>
		/// <param name="collider">Something colliding with the water.</param>
		protected virtual void TriggerEnter(GameObject collider)
		{            
			if (!CheckConditions(collider))
			{
				return;
			}

			// if we can only activate this zone when grounded, we check if we have a controller and if it's not grounded,
			// we do nothing and exit
			if (CanOnlyActivateIfGrounded)
			{
				if (collider != null)
				{
					TopDownController controller = collider.gameObject.MMGetComponentNoAlloc<TopDownController>();
					if (controller != null)
					{
						if (!controller.Grounded)
						{
							return;
						}
					}
				}
			}

			// at this point the object is colliding and authorized, we add it to our list
			_collidingObjects.Add(collider.gameObject);
			if (!TestForLastObject(collider))
			{
				return;
			}
            
			EnterFeedback?.PlayFeedbacks(this.transform.position);

			if (ShouldUpdateState)
			{
				_characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterButtonActivation>();
				if (_characterButtonActivation != null)
				{
					_characterButtonActivation.InButtonActivatedZone = true;
					_characterButtonActivation.ButtonActivatedZone = this;
					_characterButtonActivation.InButtonAutoActivatedZone = AutoActivation;
				}
			}

			if (AutoActivation)
			{
				_autoActivationCoroutine = StartCoroutine(TriggerButtonActionCo());
			}	

			// if we're not already showing the prompt and if the zone can be activated, we show it
			if (ShowPromptWhenColliding)
			{
				ShowPrompt();	
			}
		}

		/// <summary>
		/// Triggered when something exits the water
		/// </summary>
		/// <param name="collider">Something colliding with the dialogue zone.</param>
		protected virtual void TriggerExit(GameObject collider)
		{
			if (!CheckConditions(collider))
			{
				return;
			}

			_collidingObjects.Remove(collider.gameObject);
			if (!TestForLastObject(collider))
			{
				return;
			}
            
			AutoActivationInProgress = false;
			if (_autoActivationCoroutine != null)
			{
				StopCoroutine(_autoActivationCoroutine);
			}

			if (ShouldUpdateState)
			{
				_characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterButtonActivation>();
				if (_characterButtonActivation != null)
				{
					_characterButtonActivation.InButtonActivatedZone=false;
					_characterButtonActivation.ButtonActivatedZone=null;		
				}
			}

			ExitFeedback?.PlayFeedbacks(this.transform.position);

			if ((_buttonPrompt!=null) && !AlwaysShowPrompt)
			{
				HidePrompt();	
			}

			TriggerExitAction(collider);
		}

		/// <summary>
		/// Tests if the object exiting our zone is the last remaining one
		/// </summary>
		/// <param name="collider"></param>
		/// <returns></returns>
		protected virtual bool TestForLastObject(GameObject collider)
		{
			if (OnlyOneActivationAtOnce)
			{
				if (_collidingObjects.Count > 0)
				{
					bool lastObject = true;
					foreach (GameObject obj in _collidingObjects)
					{
						if ((obj != null) && (obj != collider))
						{
							lastObject = false;
						}
					}
					return lastObject;
				}                    
			}
			return true;            
		}

		/// <summary>
		/// Checks the remaining number of uses and eventual delay between uses and returns true if the zone can be activated.
		/// </summary>
		/// <returns><c>true</c>, if number of uses was checked, <c>false</c> otherwise.</returns>
		public virtual bool CheckNumberOfUses()
		{
			if (!Activable)
			{
				return false;
			}

			if (Time.time - _lastActivationTimestamp < DelayBetweenUses)
			{
				return false;
			}

			if (UnlimitedActivations)
			{
				return true;
			}

			if (_numberOfActivationsLeft == 0)
			{
				return false;
			}

			if (_numberOfActivationsLeft > 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether or not this zone should be activated
		/// </summary>
		/// <returns><c>true</c>, if conditions was checked, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="characterButtonActivation">Character button activation.</param>
		protected virtual bool CheckConditions(GameObject collider)
		{
			if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
			{
				return false;
			}
			
			Character character = collider.gameObject.MMGetComponentNoAlloc<Character>();

			switch (ButtonActivatedRequirement)
			{
				case ButtonActivatedRequirements.Character:
					if (character == null)
					{
						return false;
					}
					break;

				case ButtonActivatedRequirements.ButtonActivator:
					if (collider.gameObject.MMGetComponentNoAlloc<ButtonActivator>() == null)
					{
						return false;
					}
					break;

				case ButtonActivatedRequirements.Either:
					if ((character == null) && (collider.gameObject.MMGetComponentNoAlloc<ButtonActivator>() == null))
					{
						return false;
					}
					break;
			}

			if (RequiresPlayerType)
			{
				if (character == null)
				{
					return false;
				}
				if (character.CharacterType != Character.CharacterTypes.Player)
				{
					return false;
				}
			}

			if (RequiresButtonActivationAbility)
			{
				CharacterButtonActivation characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterButtonActivation>();
				// we check that the object colliding with the water is actually a TopDown controller and a character
				if (characterButtonActivation==null)
				{
					return false;	
				}					
			}

			return true;
		}
	}
}