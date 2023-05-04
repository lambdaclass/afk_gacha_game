using UnityEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to an empty component in your scene, and it'll allow you to swap characters in your scene when pressing the SwapButton (P, by default)
	/// Each character in your scene will need to have a CharacterSwap class on it, and the corresponding PlayerID.
	/// You can see an example of such a setup in the MinimalCharacterSwap demo scene
	/// </summary>
	[AddComponentMenu("TopDown Engine/Managers/CharacterSwapManager")]
	public class CharacterSwapManager : MMSingleton<CharacterSwapManager>, MMEventListener<TopDownEngineEvent>
	{
		[Header("Character Swap")]
		#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			/// the button to use to go up
			public Key SwapKey = Key.P;
		#else
		/// the name of the axis to use to catch input and trigger a swap on press
		[Tooltip("the name of the axis to use to catch input and trigger a swap on press")]
		public string SwapButtonName = "Player1_SwapCharacter";
		#endif
		/// the PlayerID set on the Characters you want to swap between
		[Tooltip("the PlayerID set on the Characters you want to swap between")]
		public string PlayerID = "Player1";

		protected CharacterSwap[] _characterSwapArray;
		protected List<CharacterSwap> _characterSwapList;
		protected TopDownEngineEvent _swapEvent = new TopDownEngineEvent(TopDownEngineEventTypes.CharacterSwap, null);

		/// <summary>
		/// Grabs all CharacterSwap equipped characters in the scene and stores them in a list, sorted by Order
		/// </summary>
		public virtual void UpdateList()
		{
			_characterSwapArray = FindObjectsOfType<CharacterSwap>();
			_characterSwapList = new List<CharacterSwap>();

			// stores the array into the list if the PlayerID matches
			for (int i = 0; i < _characterSwapArray.Length; i++)
			{
				if (_characterSwapArray[i].PlayerID == PlayerID)
				{
					_characterSwapList.Add(_characterSwapArray[i]);
				}
			}

			if (_characterSwapList.Count == 0)
			{
				return;
			}

			// sorts the list by order
			_characterSwapList.Sort(SortSwapsByOrder);
		}

		/// <summary>
		/// Static method to compare two CharacterSwaps
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static int SortSwapsByOrder(CharacterSwap a, CharacterSwap b)
		{
			return a.Order.CompareTo(b.Order);
		}

		/// <summary>
		/// On Update, we watch for input
		/// </summary>
		protected virtual void Update()
		{
			HandleInput();
		}

		/// <summary>
		/// If the user presses the Swap button, we swap characters
		/// </summary>
		protected virtual void HandleInput()
		{
			#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
			if (Input.GetButtonDown(SwapButtonName))
			{
				SwapCharacter();
			}
			#else
			if (Keyboard.current[SwapKey].wasPressedThisFrame)
			{
				SwapCharacter();
			}
			#endif
		}

		/// <summary>
		/// Changes the current character to the next one in line
		/// </summary>
		public virtual void SwapCharacter()
		{
			if (_characterSwapList.Count < 2)
			{
				return;
			}

			int newIndex = -1;

			for (int i = 0; i < _characterSwapList.Count; i++)
			{
				if (_characterSwapList[i].Current())
				{
					_characterSwapList[i].ResetCharacterSwap();
					newIndex = i + 1;
				}
			}

			if (newIndex >= _characterSwapList.Count)
			{
				newIndex = 0;
			}
			_characterSwapList[newIndex].SwapToThisCharacter();

			LevelManager.Instance.Players[0] = _characterSwapList[newIndex].gameObject.GetComponentInParent<Character>();
			MMEventManager.TriggerEvent(_swapEvent);
			MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
		}

		/// <summary>
		/// On Level Start, we initialize our list
		/// </summary>
		/// <param name="eventType"></param>
		public void OnMMEvent(TopDownEngineEvent engineEvent)
		{
			switch (engineEvent.EventType)
			{
				case TopDownEngineEventTypes.LevelStart:
					UpdateList();
					break;
			}
		}
        
		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<TopDownEngineEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<TopDownEngineEvent>();
		}
	}
}