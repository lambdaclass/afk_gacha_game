using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used to store dialogue lines info
	/// </summary>
	[Serializable]
	public class DialogueElement
	{
		[Multiline]
		public string DialogueLine;
	}

	/// <summary>
	/// Add this class to an empty component. It requires a Collider or Collider2D, set to "is trigger". 
	/// You can then customize the dialogue zone through the inspector.
	/// </summary>
	[AddComponentMenu("TopDown Engine/GUI/Dialogue Zone")]
	public class DialogueZone : ButtonActivated
	{
		[Header("Dialogue Look")]

		/// the prefab to use to display the dialogue
		[Tooltip("the prefab to use to display the dialogue")]
		public DialogueBox DialogueBoxPrefab;
		/// the color of the text background.
		[Tooltip("the color of the text background.")]
		public Color TextBackgroundColor = Color.black;
		/// the color of the text
		[Tooltip("the color of the text")]
		public Color TextColor = Color.white;
		/// the font that should be used to display the text
		[Tooltip("the font that should be used to display the text")]
		public Font TextFont;
		/// the size of the font
		[Tooltip("the size of the font")]
		public int TextSize = 40;
		/// the text alignment in the box used to display the text
		[Tooltip("the text alignment in the box used to display the text")]
		public TextAnchor Alignment = TextAnchor.MiddleCenter;
        
		[Header("Dialogue Speed (in seconds)")]

		/// the duration of the in and out fades
		[Tooltip("the duration of the in and out fades")]
		public float FadeDuration = 0.2f;
		/// the time between two dialogues 
		[Tooltip("the time between two dialogues ")]
		public float TransitionTime = 0.2f;

		[Header("Dialogue Position")]

		/// the distance from the top of the box collider the dialogue box should appear at
		[Tooltip("the distance from the top of the box collider the dialogue box should appear at")]
		public Vector3 Offset = Vector3.zero;
		/// if this is true, the dialogue boxes will follow the zone's position
		[Tooltip("if this is true, the dialogue boxes will follow the zone's position")]
		public bool BoxesFollowZone = false;

		[Header("Player Movement")]

		/// if this is set to true, the character will be able to move while dialogue is in progress
		[Tooltip("if this is set to true, the character will be able to move while dialogue is in progress")]
		public bool CanMoveWhileTalking = true;

		[Header("Press button to go from one message to the next ?")]

		/// whether or not this zone is handled by a button or not
		[Tooltip("whether or not this zone is handled by a button or not")]
		public bool ButtonHandled = true;
		/// duration of the message. only considered if the box is not button handled
		[Header("Only if the dialogue is not button handled :")]
		[Range(1, 100)]
		[Tooltip("The duration for which the message should be displayed, in seconds. only considered if the box is not button handled")]
		public float MessageDuration = 3f;
        
		[Header("Activations")]
		/// true if can be activated more than once
		[Tooltip("true if can be activated more than once")]
		public bool ActivableMoreThanOnce = true;
		/// if the zone is activable more than once, how long should it remain inactive between up times?
		[Range(1, 100)]
		[Tooltip("if the zone is activable more than once, how long should it remain inactive between up times?")]
		public float InactiveTime = 2f;

		/// the dialogue lines
		[Tooltip("the dialogue lines")]
		public DialogueElement[] Dialogue;

		/// private variables
		protected DialogueBox _dialogueBox;
		protected bool _activated = false;
		protected bool _playing = false;
		protected int _currentIndex;
		protected bool _activable = true;
		protected WaitForSeconds _transitionTimeWFS;
		protected WaitForSeconds _messageDurationWFS;
		protected WaitForSeconds _inactiveTimeWFS;

		/// <summary>
		/// Initializes the dialogue zone
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			_currentIndex = 0;
			_transitionTimeWFS = new WaitForSeconds(TransitionTime);
			_messageDurationWFS = new WaitForSeconds(MessageDuration);
			_inactiveTimeWFS = new WaitForSeconds(InactiveTime);
		}

		/// <summary>
		/// When the button is pressed we start the dialogue
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			if (_playing && !ButtonHandled)
			{
				return;
			}
			base.TriggerButtonAction();
			StartDialogue();
		}

		/// <summary>
		/// When triggered, either by button press or simply entering the zone, starts the dialogue
		/// </summary>
		public virtual void StartDialogue()
		{
			// if the dialogue zone has no box collider, we do nothing and exit
			if ((_collider == null) && (_collider2D == null))
			{
				return;
			}

			// if the zone has already been activated and can't be activated more than once.
			if (_activated && !ActivableMoreThanOnce)
			{
				return;
			}

			// if the zone is not activable, we do nothing and exit
			if (!_activable)
			{
				return;
			}

			// if the player can't move while talking, we notify the game manager
			if (!CanMoveWhileTalking)
			{
				LevelManager.Instance.FreezeCharacters();
				if (ShouldUpdateState && (_characterButtonActivation != null))
				{
					_characterButtonActivation.GetComponentInParent<Character>().MovementState.ChangeState(CharacterStates.MovementStates.Idle);
				}
			}

			// if it's not already playing, we'll initialize the dialogue box
			if (!_playing)
			{
				// we instantiate the dialogue box
				_dialogueBox = Instantiate(DialogueBoxPrefab);
				// we set its position
				if (_collider2D != null)
				{
					_dialogueBox.transform.position = _collider2D.bounds.center + Offset;
				}
				if (_collider != null)
				{
					_dialogueBox.transform.position = _collider.bounds.center + Offset;
				}
				// we set the color's and background's colors
				_dialogueBox.ChangeColor(TextBackgroundColor, TextColor);
				// if it's a button handled dialogue, we turn the A prompt on
				_dialogueBox.ButtonActive(ButtonHandled);

				// if font settings have been specified, we set them
				if (BoxesFollowZone)
				{
					_dialogueBox.transform.SetParent(this.gameObject.transform);
				}
				if (TextFont != null)
				{
					_dialogueBox.DialogueText.font = TextFont;
				}
				if (TextSize != 0)
				{
					_dialogueBox.DialogueText.fontSize = TextSize;
				}
				_dialogueBox.DialogueText.alignment = Alignment;

				// the dialogue is now playing
				_playing = true;
			}
			// we start the next dialogue
			StartCoroutine(PlayNextDialogue());
		}

		/// <summary>
		/// Turns collider on or off
		/// </summary>
		/// <param name="status"></param>
		protected virtual void EnableCollider(bool status)
		{
			if (_collider2D != null)
			{
				_collider2D.enabled = status;
			}
			if (_collider != null)
			{
				_collider.enabled = status;
			}
		}

		/// <summary>
		/// Plays the next dialogue in the queue
		/// </summary>
		protected virtual IEnumerator PlayNextDialogue()
		{
			// we check that the dialogue box still exists
			if (_dialogueBox == null)
			{
				yield break;
			}
			// if this is not the first message
			if (_currentIndex != 0)
			{
				// we turn the message off
				_dialogueBox.FadeOut(FadeDuration);
				// we wait for the specified transition time before playing the next dialogue
				yield return _transitionTimeWFS;
			}
			// if we've reached the last dialogue line, we exit
			if (_currentIndex >= Dialogue.Length)
			{
				_currentIndex = 0;
				Destroy(_dialogueBox.gameObject);
				EnableCollider(false);
				// we set activated to true as the dialogue zone has now been turned on		
				_activated = true;
				// we let the player move again
				if (!CanMoveWhileTalking)
				{
					LevelManager.Instance.UnFreezeCharacters();
				}
				if ((_characterButtonActivation != null))
				{
					_characterButtonActivation.InButtonActivatedZone = false;
					_characterButtonActivation.ButtonActivatedZone = null;
				}
				// we turn the zone inactive for a while
				if (ActivableMoreThanOnce)
				{
					_activable = false;
					_playing = false;
					StartCoroutine(Reactivate());
				}
				else
				{
					gameObject.SetActive(false);
				}
				yield break;
			}

			// we check that the dialogue box still exists
			if (_dialogueBox.DialogueText != null)
			{
				// every dialogue box starts with it fading in
				_dialogueBox.FadeIn(FadeDuration);
				// then we set the box's text with the current dialogue
				_dialogueBox.DialogueText.text = Dialogue[_currentIndex].DialogueLine;
			}

			_currentIndex++;

			// if the zone is not button handled, we start a coroutine to autoplay the next dialogue
			if (!ButtonHandled)
			{
				StartCoroutine(AutoNextDialogue());
			}
		}

		/// <summary>
		/// Automatically goes to the next dialogue line
		/// </summary>
		/// <returns>The next dialogue.</returns>
		protected virtual IEnumerator AutoNextDialogue()
		{
			// we wait for the duration of the message
			yield return _messageDurationWFS;
			StartCoroutine(PlayNextDialogue());
		}

		/// <summary>
		/// Reactivate the dialogue zone
		/// </summary>
		protected virtual IEnumerator Reactivate()
		{
			yield return _inactiveTimeWFS;
			EnableCollider(true);
			_activable = true;
			_playing = false;
			_currentIndex = 0;
			_promptHiddenForever = false;

			if (AlwaysShowPrompt)
			{
				ShowPrompt();
			}
		}
	}
}