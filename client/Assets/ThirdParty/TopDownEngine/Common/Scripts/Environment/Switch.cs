using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Switches can be used to trigger actions based on their current state (on or off). Useful to open doors, chests, portals, bridges...
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Switch")]
	public class Switch : TopDownMonoBehaviour
	{
		[Header("Bindings")]
		/// a SpriteReplace to represent the switch knob when it's on
		[Tooltip("a SpriteReplace to represent the switch knob when it's on")]
		public Animator SwitchAnimator;
     
		/// the possible states of the switch
		public enum SwitchStates { On, Off }
		/// the current state of the switch
		public SwitchStates CurrentSwitchState { get; set; }

		[Header("Switch")]

		/// the state the switch should start in
		[Tooltip("the state the switch should start in")]
		public SwitchStates InitialState = SwitchStates.Off;

		[Header("Events")]

		/// the methods to call when the switch is turned on
		[Tooltip("the methods to call when the switch is turned on")]
		public UnityEvent SwitchOn;
		/// the methods to call when the switch is turned off
		[Tooltip("the methods to call when the switch is turned off")]
		public UnityEvent SwitchOff;
		/// the methods to call when the switch is toggled
		[Tooltip("the methods to call when the switch is toggled")]
		public UnityEvent SwitchToggle;

		[Header("Feedbacks")]

		/// a feedback to play when the switch is toggled on
		[Tooltip("a feedback to play when the switch is toggled on")]
		public MMFeedbacks SwitchOnFeedback;
		/// a feedback to play when the switch is turned off
		[Tooltip("a feedback to play when the switch is turned off")]
		public MMFeedbacks SwitchOffFeedback;
		/// a feedback to play when the switch changes state
		[Tooltip("a feedback to play when the switch changes state")]
		public MMFeedbacks ToggleFeedback;

		[MMInspectorButton("TurnSwitchOn")]
		/// a test button to turn the switch on
		public bool SwitchOnButton;
		[MMInspectorButton("TurnSwitchOff")]
		/// a test button to turn the switch off
		public bool SwitchOffButton;
		[MMInspectorButton("ToggleSwitch")]
		/// a test button to change the switch's state
		public bool ToggleSwitchButton;

		/// <summary>
		/// On init, we set our current switch state
		/// </summary>
		protected virtual void Start()
		{
			CurrentSwitchState = InitialState;
			SwitchOffFeedback?.Initialization(this.gameObject);
			SwitchOnFeedback?.Initialization(this.gameObject);
			ToggleFeedback?.Initialization(this.gameObject);
		}

		/// <summary>
		/// Turns the switch on
		/// </summary>
		public virtual void TurnSwitchOn()
		{
			CurrentSwitchState = SwitchStates.On;
			if (SwitchOn != null) { SwitchOn.Invoke(); }
			if (SwitchToggle != null) { SwitchToggle.Invoke(); }
			SwitchOnFeedback?.PlayFeedbacks(this.transform.position);
		}

		/// <summary>
		/// Turns the switch off
		/// </summary>
		public virtual void TurnSwitchOff()
		{
			CurrentSwitchState = SwitchStates.Off;
			if (SwitchOff != null) { SwitchOff.Invoke(); }
			if (SwitchToggle != null) { SwitchToggle.Invoke(); }
			SwitchOffFeedback?.PlayFeedbacks(this.transform.position);
		}
        
		/// <summary>
		/// Use this method to go from one state to the other
		/// </summary>
		public virtual void ToggleSwitch()
		{
			if (CurrentSwitchState == SwitchStates.Off)
			{
				CurrentSwitchState = SwitchStates.On;
				if (SwitchOn != null) { SwitchOn.Invoke(); }
				if (SwitchToggle != null) { SwitchToggle.Invoke(); }
				SwitchOnFeedback?.PlayFeedbacks(this.transform.position);
			}
			else
			{
				CurrentSwitchState = SwitchStates.Off;
				if (SwitchOff != null) { SwitchOff.Invoke(); }
				if (SwitchToggle != null) { SwitchToggle.Invoke(); }
				SwitchOffFeedback?.PlayFeedbacks(this.transform.position);
			}
			ToggleFeedback?.PlayFeedbacks(this.transform.position);
		}

		/// <summary>
		/// On Update, we update our switch's animator
		/// </summary>
		protected virtual void Update()
		{
			if (SwitchAnimator != null)
			{
				SwitchAnimator.SetBool("SwitchOn", (CurrentSwitchState == SwitchStates.On));
			}            
		}
	}
}