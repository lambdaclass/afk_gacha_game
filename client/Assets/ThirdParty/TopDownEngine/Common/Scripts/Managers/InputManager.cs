using UnityEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// This persistent singleton handles the inputs and sends commands to the player.
	/// IMPORTANT : this script's Execution Order MUST be -100.
	/// You can define a script's execution order by clicking on the script's file and then clicking on the Execution Order button at the bottom right of the script's inspector.
	/// See https://docs.unity3d.com/Manual/class-ScriptExecution.html for more details
	/// </summary>
	[AddComponentMenu("TopDown Engine/Managers/Input Manager")]
	public class InputManager : MMSingleton<InputManager>
	{
		[Header("Settings")]
		/// set this to false to prevent the InputManager from reading input
		[Tooltip("set this to false to prevent the InputManager from reading input")]
		public bool InputDetectionActive = true;
		/// if this is true, button states will be reset on focus loss - when clicking outside the player window on PC, for example
		[Tooltip("if this is true, button states will be reset on focus loss - when clicking outside the player window on PC, for example")]
		public bool ResetButtonStatesOnFocusLoss = true;
        
		[Header("Player binding")]
		[MMInformation("The first thing you need to set on your InputManager is the PlayerID. This ID will be used to bind the input manager to your character(s). You'll want to go with Player1, Player2, Player3 or Player4.",MMInformationAttribute.InformationType.Info,false)]
		/// a string identifying the target player(s). You'll need to set this exact same string on your Character, and set its type to Player
		[Tooltip("a string identifying the target player(s). You'll need to set this exact same string on your Character, and set its type to Player")]
		public string PlayerID = "Player1";
		/// the possible modes for this input manager
		public enum InputForcedModes { None, Mobile, Desktop }
		/// the possible kinds of control used for movement
		public enum MovementControls { Joystick, Arrows }
		
		[Header("Mobile controls")]
		[MMInformation("If you check Auto Mobile Detection, the engine will automatically switch to mobile controls when your build target is Android or iOS. You can also force mobile or desktop (keyboard, gamepad) controls using the dropdown below.\nNote that if you don't need mobile controls and/or GUI this component can also work on its own, just put it on an empty GameObject instead.",MMInformationAttribute.InformationType.Info,false)]
		/// if this is set to true, the InputManager will try to detect what mode it should be in, based on the current target device
		[Tooltip("if this is set to true, the InputManager will try to detect what mode it should be in, based on the current target device")]
		public bool AutoMobileDetection = true;
		/// use this to force desktop (keyboard, pad) or mobile (touch) mode
		[Tooltip("use this to force desktop (keyboard, pad) or mobile (touch) mode")]
		public InputForcedModes InputForcedMode;
		/// if this is true, the weapon mode will be forced to the selected WeaponForcedMode
		[Tooltip("if this is true, the weapon mode will be forced to the selected WeaponForcedMode")]
		public bool ForceWeaponMode = false;
		/// use this to force a control mode for weapons
		[MMCondition("ForceWeaponMode", true)]
		[Tooltip("use this to force a control mode for weapons")]
		public WeaponAim.AimControls WeaponForcedMode;
		/// if this is true, mobile controls will be hidden in editor mode, regardless of the current build target or the forced mode
		[Tooltip("if this is true, mobile controls will be hidden in editor mode, regardless of the current build target or the forced mode")]
		public bool HideMobileControlsInEditor = false;
		/// use this to specify whether you want to use the default joystick or arrows to move your character
		[Tooltip("use this to specify whether you want to use the default joystick or arrows to move your character")]
		public MovementControls MovementControl = MovementControls.Joystick;
		/// if this is true, we're currently in mobile mode
		public bool IsMobile { get; protected set; }

		[Header("Movement settings")]
		[MMInformation("Turn SmoothMovement on to have inertia in your controls (meaning there'll be a small delay between a press/release of a direction and your character moving/stopping). You can also define here the horizontal and vertical thresholds.",MMInformationAttribute.InformationType.Info,false)]
		/// If set to true, acceleration / deceleration will take place when moving / stopping
		[Tooltip("If set to true, acceleration / deceleration will take place when moving / stopping")]
		public bool SmoothMovement=true;
		/// the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)
		[Tooltip("the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)")]
		public Vector2 Threshold = new Vector2(0.1f, 0.4f);

		[Header("Camera Rotation")]
		[MMInformation("Here you can decide whether or not camera rotation should impact your input. That can be useful in, for example, a 3D isometric game, if you want 'up' to mean some other direction than Vector3.up/forward.", MMInformationAttribute.InformationType.Info, false)]
		/// if this is true, any directional input coming into this input manager will be rotated to align with the current camera orientation
		[Tooltip("if this is true, any directional input coming into this input manager will be rotated to align with the current camera orientation")]
		public bool RotateInputBasedOnCameraDirection = false;
        
		/// the jump button, used for jumps and validation
		public MMInput.IMButton JumpButton { get; protected set; }
		/// the run button
		public MMInput.IMButton RunButton { get; protected set; }
		/// the dash button
		public MMInput.IMButton DashButton { get; protected set; }
		/// the crouch button
		public MMInput.IMButton CrouchButton { get; protected set; }
		/// the shoot button
		public MMInput.IMButton ShootButton { get; protected set; }
		/// the activate button, used for interactions with zones
		public MMInput.IMButton InteractButton { get; protected set; }
		/// the shoot button
		public MMInput.IMButton SecondaryShootButton { get; protected set; }
		/// the reload button
		public MMInput.IMButton ReloadButton { get; protected set; }
		/// the pause button
		public MMInput.IMButton PauseButton { get; protected set; }
		/// the time control button
		public MMInput.IMButton TimeControlButton { get; protected set; }
		/// the button used to switch character (either via model or prefab switch)
		public MMInput.IMButton SwitchCharacterButton { get; protected set; }
		/// the switch weapon button
		public MMInput.IMButton SwitchWeaponButton { get; protected set; }
		/// the shoot axis, used as a button (non analogic)
		public MMInput.ButtonStates ShootAxis { get; protected set; }
		/// the shoot axis, used as a button (non analogic)
		public MMInput.ButtonStates SecondaryShootAxis { get; protected set; }
		/// the primary movement value (used to move the character around)
		public Vector2 PrimaryMovement { get { return _primaryMovement; } }
		/// the secondary movement (usually the right stick on a gamepad), used to aim
		public Vector2 SecondaryMovement { get { return _secondaryMovement; } }
		/// the primary movement value (used to move the character around)
		public Vector2 LastNonNullPrimaryMovement { get; set; }
		/// the secondary movement (usually the right stick on a gamepad), used to aim
		public Vector2 LastNonNullSecondaryMovement { get; set; }
		/// the camera rotation axis input value
		public float CameraRotationInput { get { return _cameraRotationInput; } }
		/// the current camera angle
		public float CameraAngle { get { return _cameraAngle; } }
		/// the position of the mouse
		public virtual Vector2 MousePosition => Input.mousePosition;

		protected Camera _targetCamera;
		protected bool _camera3D;
		protected float _cameraAngle;
		protected List<MMInput.IMButton> ButtonList;
		protected Vector2 _primaryMovement = Vector2.zero;
		protected Vector2 _secondaryMovement = Vector2.zero;
		protected float _cameraRotationInput = 0f;
		protected string _axisHorizontal;
		protected string _axisVertical;
		protected string _axisSecondaryHorizontal;
		protected string _axisSecondaryVertical;
		protected string _axisShoot;
		protected string _axisShootSecondary;
		protected string _axisCamera;
		
		/// <summary>
		/// On Awake we run our pre-initialization
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			PreInitialization();
		}

		/// <summary>
		/// On Start we look for what mode to use, and initialize our axis and buttons
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// Initializes buttons and axis
		/// </summary>
		protected virtual void PreInitialization()
		{
			InitializeButtons();
			InitializeAxis();
		}
		
		/// <summary>
		/// On init we auto detect control schemes
		/// </summary>
		protected virtual void Initialization()
		{
			ControlsModeDetection();
		}

		/// <summary>
		/// Turns mobile controls on or off depending on what's been defined in the inspector, and what target device we're on
		/// </summary>
		public virtual void ControlsModeDetection()
		{
			if (GUIManager.HasInstance) { GUIManager.Instance.SetMobileControlsActive(false); }
			IsMobile=false;
			if (AutoMobileDetection)
			{
				#if UNITY_ANDROID || UNITY_IPHONE
					if (GUIManager.HasInstance) { GUIManager.Instance.SetMobileControlsActive(true,MovementControl); }
					IsMobile = true;
				#endif
			}
			if (InputForcedMode==InputForcedModes.Mobile)
			{
				if (GUIManager.HasInstance) { GUIManager.Instance.SetMobileControlsActive(true, MovementControl); }
				IsMobile = true;
			}
			if (InputForcedMode==InputForcedModes.Desktop)
			{
				if (GUIManager.HasInstance) { GUIManager.Instance.SetMobileControlsActive(false); }
				IsMobile = false;					
			}
			if (HideMobileControlsInEditor)
			{
				#if UNITY_EDITOR
				if (GUIManager.HasInstance) { GUIManager.Instance.SetMobileControlsActive(false); }
				IsMobile = false;	
				#endif
			}
		}

		/// <summary>
		/// Initializes the buttons. If you want to add more buttons, make sure to register them here.
		/// </summary>
		protected virtual void InitializeButtons()
		{
			ButtonList = new List<MMInput.IMButton> ();
			ButtonList.Add(JumpButton = new MMInput.IMButton (PlayerID, "Jump", JumpButtonDown, JumpButtonPressed, JumpButtonUp));
			ButtonList.Add(RunButton  = new MMInput.IMButton (PlayerID, "Run", RunButtonDown, RunButtonPressed, RunButtonUp));
			ButtonList.Add(InteractButton = new MMInput.IMButton(PlayerID, "Interact", InteractButtonDown, InteractButtonPressed, InteractButtonUp));
			ButtonList.Add(DashButton  = new MMInput.IMButton (PlayerID, "Dash", DashButtonDown, DashButtonPressed, DashButtonUp));
			ButtonList.Add(CrouchButton  = new MMInput.IMButton (PlayerID, "Crouch", CrouchButtonDown, CrouchButtonPressed, CrouchButtonUp));
			ButtonList.Add(SecondaryShootButton = new MMInput.IMButton(PlayerID, "SecondaryShoot", SecondaryShootButtonDown, SecondaryShootButtonPressed, SecondaryShootButtonUp));
			ButtonList.Add(ShootButton = new MMInput.IMButton (PlayerID, "Shoot", ShootButtonDown, ShootButtonPressed, ShootButtonUp)); 
			ButtonList.Add(ReloadButton = new MMInput.IMButton (PlayerID, "Reload", ReloadButtonDown, ReloadButtonPressed, ReloadButtonUp));
			ButtonList.Add(SwitchWeaponButton = new MMInput.IMButton (PlayerID, "SwitchWeapon", SwitchWeaponButtonDown, SwitchWeaponButtonPressed, SwitchWeaponButtonUp));
			ButtonList.Add(PauseButton = new MMInput.IMButton(PlayerID, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp));
			ButtonList.Add(TimeControlButton = new MMInput.IMButton(PlayerID, "TimeControl", TimeControlButtonDown, TimeControlButtonPressed, TimeControlButtonUp));
			ButtonList.Add(SwitchCharacterButton = new MMInput.IMButton(PlayerID, "SwitchCharacter", SwitchCharacterButtonDown, SwitchCharacterButtonPressed, SwitchCharacterButtonUp));
		}

		/// <summary>
		/// Initializes the axis strings.
		/// </summary>
		protected virtual void InitializeAxis()
		{
			_axisHorizontal = PlayerID+"_Horizontal";
			_axisVertical = PlayerID+"_Vertical";
			_axisSecondaryHorizontal = PlayerID+"_SecondaryHorizontal";
			_axisSecondaryVertical = PlayerID+"_SecondaryVertical";
			_axisShoot = PlayerID+"_ShootAxis";
			_axisShootSecondary = PlayerID + "_SecondaryShootAxis";
			_axisCamera = PlayerID + "_CameraRotationAxis";
		}

		/// <summary>
		/// On LateUpdate, we process our button states
		/// </summary>
		protected virtual void LateUpdate()
		{
			ProcessButtonStates();
		}

		/// <summary>
		/// At update, we check the various commands and update our values and states accordingly.
		/// </summary>
		protected virtual void Update()
		{		
			if (!IsMobile && InputDetectionActive)
			{	
				SetMovement();	
				SetSecondaryMovement ();
				SetShootAxis ();
				SetCameraRotationAxis();
				GetInputButtons ();
				GetLastNonNullValues();
			}									
		}

		/// <summary>
		/// Gets the last non null values for both primary and secondary axis
		/// </summary>
		protected virtual void GetLastNonNullValues()
		{
			if (_primaryMovement.magnitude > Threshold.x)
			{
				LastNonNullPrimaryMovement = _primaryMovement;
			}
			if (_secondaryMovement.magnitude > Threshold.x)
			{
				LastNonNullSecondaryMovement = _secondaryMovement;
			}
		}

		/// <summary>
		/// If we're not on mobile, watches for input changes, and updates our buttons states accordingly
		/// </summary>
		protected virtual void GetInputButtons()
		{
			foreach(MMInput.IMButton button in ButtonList)
			{
				if (Input.GetButton(button.ButtonID))
				{
					button.TriggerButtonPressed ();
				}
				if (Input.GetButtonDown(button.ButtonID))
				{
					button.TriggerButtonDown ();
				}
				if (Input.GetButtonUp(button.ButtonID))
				{
					button.TriggerButtonUp ();
				}
			}
		}

		/// <summary>
		/// Called at LateUpdate(), this method processes the button states of all registered buttons
		/// </summary>
		public virtual void ProcessButtonStates()
		{
			// for each button, if we were at ButtonDown this frame, we go to ButtonPressed. If we were at ButtonUp, we're now Off
			foreach (MMInput.IMButton button in ButtonList)
			{
				if (button.State.CurrentState == MMInput.ButtonStates.ButtonDown)
				{
					button.State.ChangeState(MMInput.ButtonStates.ButtonPressed);				
				}	
				if (button.State.CurrentState == MMInput.ButtonStates.ButtonUp)
				{
					button.State.ChangeState(MMInput.ButtonStates.Off);				
				}	
			}
		}

		/// <summary>
		/// Called every frame, if not on mobile, gets primary movement values from input
		/// </summary>
		public virtual void SetMovement()
		{
			if (!IsMobile && InputDetectionActive)
			{
				if (SmoothMovement)
				{
					_primaryMovement.x = Input.GetAxis(_axisHorizontal);
					_primaryMovement.y = Input.GetAxis(_axisVertical);		
				}
				else
				{
					_primaryMovement.x = Input.GetAxisRaw(_axisHorizontal);
					_primaryMovement.y = Input.GetAxisRaw(_axisVertical);
				}
				_primaryMovement = ApplyCameraRotation(_primaryMovement);
			}
		}

		/// <summary>
		/// Called every frame, if not on mobile, gets secondary movement values from input
		/// </summary>
		public virtual void SetSecondaryMovement()
		{
			if (!IsMobile && InputDetectionActive)
			{
				if (SmoothMovement)
				{
					_secondaryMovement.x = Input.GetAxis(_axisSecondaryHorizontal);
					_secondaryMovement.y = Input.GetAxis(_axisSecondaryVertical);		
				}
				else
				{
					_secondaryMovement.x = Input.GetAxisRaw(_axisSecondaryHorizontal);
					_secondaryMovement.y = Input.GetAxisRaw(_axisSecondaryVertical);
				}
				_secondaryMovement = ApplyCameraRotation(_secondaryMovement);
			}
		}

		/// <summary>
		/// Called every frame, if not on mobile, gets shoot axis values from input
		/// </summary>
		protected virtual void SetShootAxis()
		{
			if (!IsMobile && InputDetectionActive)
			{
				ShootAxis = MMInput.ProcessAxisAsButton (_axisShoot, Threshold.y, ShootAxis);
				SecondaryShootAxis = MMInput.ProcessAxisAsButton(_axisShootSecondary, Threshold.y, SecondaryShootAxis, MMInput.AxisTypes.Positive);
			}
		}

		/// <summary>
		/// Grabs camera rotation input and stores it
		/// </summary>
		protected virtual void SetCameraRotationAxis()
		{
			if (!IsMobile)
			{
				_cameraRotationInput = Input.GetAxis(_axisCamera);	
			}
		}

		/// <summary>
		/// If you're using a touch joystick, bind your main joystick to this method
		/// </summary>
		/// <param name="movement">Movement.</param>
		public virtual void SetMovement(Vector2 movement)
		{
			if (IsMobile && InputDetectionActive)
			{
				_primaryMovement.x = movement.x;
				_primaryMovement.y = movement.y;
			}
			_primaryMovement = ApplyCameraRotation(_primaryMovement);
		}

		/// <summary>
		/// If you're using a touch joystick, bind your secondary joystick to this method
		/// </summary>
		/// <param name="movement">Movement.</param>
		public virtual void SetSecondaryMovement(Vector2 movement)
		{
			if (IsMobile && InputDetectionActive)
			{
				_secondaryMovement.x = movement.x;
				_secondaryMovement.y = movement.y;
			}
			_secondaryMovement = ApplyCameraRotation(_secondaryMovement);
		}

		/// <summary>
		/// If you're using touch arrows, bind your left/right arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetHorizontalMovement(float horizontalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_primaryMovement.x = horizontalInput;
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your secondary down/up arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetVerticalMovement(float verticalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_primaryMovement.y = verticalInput;
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your secondary left/right arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetSecondaryHorizontalMovement(float horizontalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_secondaryMovement.x = horizontalInput;
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your down/up arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetSecondaryVerticalMovement(float verticalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_secondaryMovement.y = verticalInput;
			}
		}

		/// <summary>
		/// Sets an associated camera, used to rotate input based on camera position
		/// </summary>
		/// <param name="targetCamera"></param>
		/// <param name="rotationAxis"></param>
		public virtual void SetCamera(Camera targetCamera, bool camera3D)
		{
			_targetCamera = targetCamera;
			_camera3D = camera3D;
		}

		/// <summary>
		/// Sets the current camera rotation input, which you'll want to keep between -1 (left) and 1 (right), 0 being no rotation
		/// </summary>
		/// <param name="newValue"></param>
		public virtual void SetCameraRotationInput(float newValue)
		{
			_cameraRotationInput = newValue;
		}

		/// <summary>
		/// Rotates input based on camera orientation
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public virtual Vector2 ApplyCameraRotation(Vector2 input)
		{
			if (RotateInputBasedOnCameraDirection)
			{
				if (_camera3D)
				{
					_cameraAngle = _targetCamera.transform.localEulerAngles.y;
					return MMMaths.RotateVector2(input, -_cameraAngle);
				}
				else
				{
					_cameraAngle = _targetCamera.transform.localEulerAngles.z;
					return MMMaths.RotateVector2(input, _cameraAngle);
				}
			}
			else
			{
				return input;
			}
		}

		/// <summary>
		/// If we lose focus, we reset the states of all buttons
		/// </summary>
		/// <param name="hasFocus"></param>
		protected void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus && ResetButtonStatesOnFocusLoss && (ButtonList != null))
			{
				foreach (MMInput.IMButton button in ButtonList)
				{
					button.State.ChangeState(MMInput.ButtonStates.ButtonUp);
				}
			}
		}

		public virtual void JumpButtonDown()		{ JumpButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void JumpButtonPressed()		{ JumpButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void JumpButtonUp()			{ JumpButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void DashButtonDown()		{ DashButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void DashButtonPressed()		{ DashButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void DashButtonUp()			{ DashButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void CrouchButtonDown()		{ CrouchButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void CrouchButtonPressed()	{ CrouchButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void CrouchButtonUp()		{ CrouchButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void RunButtonDown()			{ RunButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void RunButtonPressed()		{ RunButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void RunButtonUp()			{ RunButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void ReloadButtonDown()		{ ReloadButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void ReloadButtonPressed()	{ ReloadButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void ReloadButtonUp()		{ ReloadButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void InteractButtonDown() { InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
		public virtual void InteractButtonPressed() { InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
		public virtual void InteractButtonUp() { InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

		public virtual void ShootButtonDown()		{ ShootButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void ShootButtonPressed()	{ ShootButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void ShootButtonUp()			{ ShootButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void SecondaryShootButtonDown() { SecondaryShootButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
		public virtual void SecondaryShootButtonPressed() { SecondaryShootButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
		public virtual void SecondaryShootButtonUp() { SecondaryShootButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

		public virtual void PauseButtonDown() { PauseButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
		public virtual void PauseButtonPressed() { PauseButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
		public virtual void PauseButtonUp() { PauseButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

		public virtual void TimeControlButtonDown() { TimeControlButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
		public virtual void TimeControlButtonPressed() { TimeControlButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
		public virtual void TimeControlButtonUp() { TimeControlButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

		public virtual void SwitchWeaponButtonDown()		{ SwitchWeaponButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void SwitchWeaponButtonPressed()		{ SwitchWeaponButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void SwitchWeaponButtonUp()			{ SwitchWeaponButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void SwitchCharacterButtonDown() { SwitchCharacterButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
		public virtual void SwitchCharacterButtonPressed() { SwitchCharacterButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
		public virtual void SwitchCharacterButtonUp() { SwitchCharacterButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }
	}
}