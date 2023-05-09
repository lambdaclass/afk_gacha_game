using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using Random = UnityEngine.Random;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to an object (usually a platform but it could be anything really) to have it run on an appearing/disappearing loop, like the appearing platforms in Megaman for example.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Appear and Disappear")]
	public class AppearDisappear : TopDownMonoBehaviour
	{
		/// the possible states this object can be in
		public enum AppearDisappearStates { Visible, Hidden, VisibleToHidden, HiddenToVisible }
		/// the possible start modes (automatic will start on Start, PlayerContact when an object with the Player tag collides with the object, and Script lets you trigger that manually
		public enum StartModes { Automatic, PlayerContact, Script }

		public enum CyclingModes { Forever, Limited}
        
		[Header("Settings")]

		/// whether the object is active right now or not
		[Tooltip("whether the object is active right now or not")]
		public bool Active = true;
		/// the initial state (visible or hidden) the object should start in
		[Tooltip("the initial state (visible or hidden) the object should start in")]
		public AppearDisappearStates InitialState;
		/// how the object should be activated
		[Tooltip("how the object should be activated")]
		public StartModes StartMode = StartModes.Automatic;
		/// how the object should cycle states (forever, a limited amount of times, or never)
		[Tooltip("how the object should cycle states (forever, a limited amount of times, or never)")]
		public CyclingModes CyclingMode = CyclingModes.Forever;
		/// the number of cycles this object can go through before it stops (only used if CyclingMode is Limited)
		[Tooltip("the number of cycles this object can go through before it stops (only used if CyclingMode is Limited)")]
		[MMEnumCondition("CyclingMode", (int)CyclingModes.Limited)]
		public int CyclesAmount = 1;


		[Header("Timing")]

		/// the initial offset to apply to the object's first state change (in seconds)
		[MMVector("Min", "Max")]
		[Tooltip("the initial offset to apply to the object's first state change (in seconds)")]
		public Vector2 InitialOffset = new Vector2(0f, 0f);
		/// the min and max duration of the visible state (in seconds)
		[MMVector("Min", "Max")]
		[Tooltip("the min and max duration of the visible state (in seconds)")]
		public Vector2 VisibleDuration = new Vector2(1f, 1f);
		/// the min and max duration of the hidden state (in seconds)
		[MMVector("Min", "Max")]
		[Tooltip("the min and max duration of the hidden state (in seconds)")]
		public Vector2 HiddenDuration = new Vector2(1f, 1f);
		/// the min and max duration of the visible to hidden state (in seconds)
		[MMVector("Min", "Max")]
		[Tooltip("the min and max duration of the visible to hidden state (in seconds)")]
		public Vector2 VisibleToHiddenDuration = new Vector2(1f, 1f);
		/// the min and max duration of the hidden to visible state (in seconds)
		[MMVector("Min", "Max")]
		[Tooltip("the min and max duration of the hidden to visible state (in seconds)")]
		public Vector2 HiddenToVisibleDuration = new Vector2(1f, 1f);
                
		[Header("Feedbacks")]

		/// the feedback to trigger when reaching the visible state
		[Tooltip("the feedback to trigger when reaching the visible state")]
		public MMFeedbacks VisibleFeedback;
		/// the feedback to trigger when reaching the visible to hidden state
		[Tooltip("the feedback to trigger when reaching the visible to hidden state")]
		public MMFeedbacks VisibleToHiddenFeedback;
		/// the feedback to trigger when reaching the hidden state
		[Tooltip("the feedback to trigger when reaching the hidden state")]
		public MMFeedbacks HiddenFeedback;
		/// the feedback to trigger when reaching the hidden to visible state
		[Tooltip("the feedback to trigger when reaching the hidden to visible state")]
		public MMFeedbacks HiddenToVisibleFeedback;

		[Header("Bindings")]
		/// the animator to update
		[Tooltip("the animator to update")]
		public Animator TargetAnimator;
		/// the game object to show/hide
		[Tooltip("the game object to show/hide")]
		public GameObject TargetModel;
		/// whether or not the object should update its animator (set at the same level) when changing state
		[Tooltip("whether or not the object should update its animator (set at the same level) when changing state")]
		public bool UpdateAnimator = true;
		/// whether or not the object should update its Collider or Collider2D (set at the same level) when changing state
		[Tooltip("whether or not the object should update its Collider or Collider2D (set at the same level) when changing state")]
		public bool EnableDisableCollider = true;
		/// whether or not the object should hide/show a model when changing state
		[Tooltip("whether or not the object should hide/show a model when changing state")]
		public bool ShowHideModel = false;

		[Header("Trigger Area")]

		/// the area used to detect the presence of a character
		[Tooltip("the area used to detect the presence of a character")]
		public CharacterDetector TriggerArea;
		/// whether or not we should prevent this component from appearing when a character is in the area
		[Tooltip("whether or not we should prevent this component from appearing when a character is in the area")]
		public bool PreventAppearWhenCharacterInArea = true;
		/// whether or not we should prevent this component from disappearing when a character is in the area
		[Tooltip("whether or not we should prevent this component from disappearing when a character is in the area")]
		public bool PreventDisappearWhenCharacterInArea = false;

		[Header("Debug")]

		/// the current state this object is in
		[MMReadOnly]
		[Tooltip("the current state this object is in")]
		public AppearDisappearStates _currentState;
		/// the state this object will be in next
		[MMReadOnly]
		[Tooltip("the state this object will be in next")]
		public AppearDisappearStates _nextState;
		/// the last time this object changed state
		[MMReadOnly]
		[Tooltip("the last time this object changed state")]
		public float _lastStateChangedAt = 0f;
		[MMReadOnly]
		[Tooltip("the last time this object changed state")]
		public int _cyclesLeft;

		protected const string _animationParameter = "Visible";

		protected float _visibleDuration;
		protected float _hiddenDuration;
		protected float _visibleToHiddenDuration;
		protected float _hiddenToVisibleDuration;

		protected float _nextChangeIn;
		protected MMFeedbacks _nextFeedback;

		protected Collider _collider;
		protected Collider2D _collider2D;

		protected bool _characterInTriggerArea = false;

		/// <summary>
		/// On start we initialize our object
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// On Init, we set our active state, grab components, and determine our next state
		/// </summary>
		protected virtual void Initialization()
		{
			_currentState = InitialState;
			_lastStateChangedAt = Time.time;
			_cyclesLeft = CyclesAmount;

			Active = (StartMode == StartModes.Automatic);

			if (_currentState == AppearDisappearStates.HiddenToVisible) { _currentState = AppearDisappearStates.Visible; }
			if (_currentState == AppearDisappearStates.VisibleToHidden) { _currentState = AppearDisappearStates.Hidden; }

			if (TargetAnimator == null)
			{
				TargetAnimator = this.gameObject.GetComponent<Animator>();	
			}
			
			_collider = this.gameObject.GetComponent<Collider>();
			_collider2D = this.gameObject.GetComponent<Collider2D>();

			RandomizeDurations();

			_visibleDuration += Random.Range(InitialOffset.x, InitialOffset.y);
			_hiddenDuration += Random.Range(InitialOffset.x, InitialOffset.y);

			UpdateBoundComponents(_currentState == AppearDisappearStates.Visible);

			DetermineNextState();
		}

		/// <summary>
		/// Activates or disables the appearing/disappearing behaviour
		/// </summary>
		/// <param name="status"></param>
		public virtual void Activate(bool status)
		{
			Active = status;
		}
        
		/// <summary>
		/// On Update we process our state machine
		/// </summary>
		protected virtual void Update()
		{
			ProcessTriggerArea();
			ProcessStateMachine();
		}

		protected virtual void ProcessTriggerArea()
		{
			_characterInTriggerArea = false;
			if (TriggerArea == null)
			{
				return;
			}
			_characterInTriggerArea = TriggerArea.CharacterInArea;
		}

		/// <summary>
		/// Changes the state to the next if time requires it
		/// </summary>
		protected virtual void ProcessStateMachine()
		{
			if (!Active)
			{
				return;
			}

			if (Time.time - _lastStateChangedAt > _nextChangeIn)
			{
				ChangeState();
			}
		}

		/// <summary>
		/// Determines the next state this object should be in
		/// </summary>
		protected virtual void DetermineNextState()
		{
			switch (_currentState)
			{
				case AppearDisappearStates.Visible:
					_nextChangeIn = _visibleDuration;
					_nextState = AppearDisappearStates.VisibleToHidden;
					_nextFeedback = VisibleToHiddenFeedback;
					break;
				case AppearDisappearStates.Hidden:
					_nextChangeIn = _hiddenDuration;
					_nextState = AppearDisappearStates.HiddenToVisible;
					_nextFeedback = HiddenToVisibleFeedback;
					break;
				case AppearDisappearStates.HiddenToVisible:
					_nextChangeIn = _hiddenToVisibleDuration;
					_nextState = AppearDisappearStates.Visible;
					_nextFeedback = VisibleFeedback;
					break;
				case AppearDisappearStates.VisibleToHidden:
					_nextChangeIn = _visibleToHiddenDuration;
					_nextState = AppearDisappearStates.Hidden;
					_nextFeedback = HiddenFeedback;
					break;
			}
		}

		/// <summary>
		/// Changes the state for the next in line
		/// </summary>
		public virtual void ChangeState()
		{
			if (((_nextState == AppearDisappearStates.HiddenToVisible) || (_nextState == AppearDisappearStates.Visible))
			    && _characterInTriggerArea
			    && PreventAppearWhenCharacterInArea)
			{
				return;
			}

			if (((_nextState == AppearDisappearStates.VisibleToHidden) || (_nextState == AppearDisappearStates.Hidden))
			    && _characterInTriggerArea
			    && PreventDisappearWhenCharacterInArea)
			{
				return;
			}

			_lastStateChangedAt = Time.time;
			_currentState = _nextState;
			_nextFeedback?.PlayFeedbacks();
			RandomizeDurations();

			if (_currentState == AppearDisappearStates.Hidden)
			{
				UpdateBoundComponents(false);   
			}

			if (_currentState == AppearDisappearStates.Visible)
			{
				UpdateBoundComponents(true);
			}
            
			DetermineNextState();

			if (CyclingMode == CyclingModes.Limited)
			{
				if (_currentState == AppearDisappearStates.Hidden || _currentState == AppearDisappearStates.Visible)
				{
					_cyclesLeft--;
					if (_cyclesLeft <= 0)
					{
						Active = false;
					}
				}
			}
		}

		/// <summary>
		/// Updates animator, collider and renderer according to the visible state
		/// </summary>
		/// <param name="visible"></param>
		protected virtual void UpdateBoundComponents(bool visible)
		{
			if (UpdateAnimator && (TargetAnimator != null))
			{
				TargetAnimator.SetBool(_animationParameter, visible);
			}
			if (EnableDisableCollider)
			{
				if (_collider != null)
				{
					_collider.enabled = visible;
				}
				if (_collider2D != null)
				{
					_collider2D.enabled = visible;
				}
			}
			if (ShowHideModel && (TargetModel != null))
			{
				TargetModel.SetActive(visible);
			}
		}

		/// <summary>
		/// Randomizes the durations of each state based on the mins and maxs set in the inpector
		/// </summary>
		protected virtual void RandomizeDurations()
		{
			_visibleDuration = Random.Range(VisibleDuration.x, VisibleDuration.y);
			_hiddenDuration = Random.Range(HiddenDuration.x, HiddenDuration.y);
			_visibleToHiddenDuration = Random.Range(VisibleToHiddenDuration.x, VisibleToHiddenDuration.y);
			_hiddenToVisibleDuration = Random.Range(HiddenToVisibleDuration.x, HiddenToVisibleDuration.y);
		}

		/// <summary>
		/// When colliding with another object, we check if it's a player, and if it is and we are supposed to start on player contact, we enable our object
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerEnter2D(Collider2D collider)
		{
			if (StartMode != StartModes.PlayerContact)
			{
				return;
			}

			if (collider.CompareTag("Player"))
			{
				_lastStateChangedAt = Time.time;
				Activate(true);
			}
		}

		public virtual void ResetCycling()
		{
			_cyclesLeft = CyclesAmount;
		}
	}
}