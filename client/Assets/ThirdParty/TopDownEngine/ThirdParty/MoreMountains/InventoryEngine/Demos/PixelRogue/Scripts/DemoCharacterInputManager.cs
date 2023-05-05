using UnityEngine;
using MoreMountains.Tools;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace MoreMountains.InventoryEngine
{	
	/// <summary>
	/// A very simple input manager to handle the demo character's input and make it move
	/// </summary>
	public class DemoCharacterInputManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
	{
		/// The character that'll move through the level
		[MMInformation("This component is a very simple input manager that handles the demo character's input and makes it move. If you remove it from the scene your character won't move anymore.", MMInformationAttribute.InformationType.Info,false)]
		public InventoryDemoCharacter DemoCharacter ;
		
		[Header("Input")]
		
		#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
		
		public InputActionProperty LeftInputAction = new InputActionProperty(
			new InputAction(
				name: "IM_Demo_LeftKey",
				type: InputActionType.Button, 
				binding: "Keyboard/leftArrow", 
				interactions: "Press(behavior=2)"));
		
		public InputActionProperty RightInputAction = new InputActionProperty(
			new InputAction(
				name: "IM_Demo_RightKey",
				type: InputActionType.Button, 
				binding: "Keyboard/rightArrow", 
				interactions: "Press(behavior=2)"));
		
		public InputActionProperty UpInputAction = new InputActionProperty(
			new InputAction(
				name: "IM_Demo_UpKey",
				type: InputActionType.Button, 
				binding: "Keyboard/upArrow", 
				interactions: "Press(behavior=2)"));
		
		public InputActionProperty DownInputAction = new InputActionProperty(
			new InputAction(
				name: "IM_Demo_DownKey",
				type: InputActionType.Button, 
				binding: "Keyboard/downArrow", 
				interactions: "Press(behavior=2)"));
		
		#else

		public KeyCode LeftKey = KeyCode.LeftArrow;
		public KeyCode LeftKeyAlt = KeyCode.None;
		public KeyCode RightKey = KeyCode.RightArrow;
		public KeyCode RightKeyAlt = KeyCode.None;
		public KeyCode DownKey = KeyCode.DownArrow;
		public KeyCode DownKeyAlt = KeyCode.None;
		public KeyCode UpKey = KeyCode.UpArrow;
		public KeyCode UpKeyAlt = KeyCode.None;
	    
		#endif
	        
		public string VerticalAxisName = "Vertical";

		protected bool _pause = false;

		/// <summary>
		/// Every frame, we check for input for the inventory, the hotbars and the character
		/// </summary>
		protected virtual void Update ()
		{
			HandleDemoCharacterInput();
		}

		/// <summary>
		/// Handles the demo character movement input.
		/// </summary>
		protected virtual void HandleDemoCharacterInput()
		{
			if (_pause)
			{
				DemoCharacter.SetMovement(0,0);
				return;
			}

			float horizontalMovement = 0f;
			float verticalMovement = 0f;
			
			#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			
			if (LeftInputAction.action.IsPressed()) { horizontalMovement = -1f; }
			if (RightInputAction.action.IsPressed()) { horizontalMovement = 1f; }
			if (DownInputAction.action.IsPressed()) { verticalMovement = -1f; }
			if (UpInputAction.action.IsPressed()) { verticalMovement = 1f; }
			
			#else
				if ( (Input.GetKey(LeftKey)) || (Input.GetKey(LeftKeyAlt)) ) { horizontalMovement = -1f; }
				if ( (Input.GetKey(RightKey)) || (Input.GetKey(RightKeyAlt)) ) { horizontalMovement = 1f; }
				if ( (Input.GetKey(DownKey)) || (Input.GetKey(DownKeyAlt)) ) { verticalMovement = -1f; }
				if ( (Input.GetKey(UpKey)) || (Input.GetKey(UpKeyAlt)) ) { verticalMovement = 1f; }
			#endif
			
			
			
				
			
			DemoCharacter.SetMovement(horizontalMovement,verticalMovement);
		}

		/// <summary>
		/// Catches MMInventoryEvents to detect pauses
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens)
			{
				_pause = true;
			}
			if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
			{
				_pause = false;
			}
		}

		/// <summary>
		/// On Enable, we start listening for MMInventoryEvents
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMInventoryEvent>();
			#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			UpInputAction.action.Enable();
			DownInputAction.action.Enable();
			LeftInputAction.action.Enable();
			RightInputAction.action.Enable();
			#endif
		}

		/// <summary>
		/// On Disable, we stop listening for MMInventoryEvents
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMInventoryEvent>();
			#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			UpInputAction.action.Disable();
			DownInputAction.action.Disable();
			LeftInputAction.action.Disable();
			RightInputAction.action.Disable();
			#endif
		}
	}
}