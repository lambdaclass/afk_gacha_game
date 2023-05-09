using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This is a replacement InputManager if you prefer using Unity's InputSystem over the legacy one.
    /// Note that it's not the default solution in the engine at the moment, because older versions of Unity don't support it, 
    /// and most people still prefer not using it
    /// You can see an example of how to set it up in the MinimalScene3D_InputSystem demo scene
    /// </summary>
    public class InputSystemManager : InputManager
    {
        /// a set of input actions to use to read input on
        public TopDownEngineInputActions InputActions;
        /// the position of the mouse
        public override Vector2 MousePosition => Mouse.current.position.ReadValue();

        protected override void Awake()
        {
            base.Awake();
            InputActions = new TopDownEngineInputActions();
        }
        
        /// <summary>
        /// On init we register to all our actions
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            InputActions.PlayerControls.PrimaryMovement.performed += context => _primaryMovement = ApplyCameraRotation(context.ReadValue<Vector2>());
            InputActions.PlayerControls.SecondaryMovement.performed += context => _secondaryMovement = ApplyCameraRotation(context.ReadValue<Vector2>());
            InputActions.PlayerControls.CameraRotation.performed += context => _cameraRotationInput = context.ReadValue<float>();

            InputActions.PlayerControls.Jump.performed += context => { BindButton(context, JumpButton); };
            InputActions.PlayerControls.Run.performed += context => { BindButton(context, RunButton); };
            InputActions.PlayerControls.Dash.performed += context => { BindButton(context, DashButton); };
            InputActions.PlayerControls.Crouch.performed += context => { BindButton(context, CrouchButton); };
            InputActions.PlayerControls.Shoot.performed += context => { BindButton(context, ShootButton); };
            InputActions.PlayerControls.SecondaryShoot.performed += context => { BindButton(context, SecondaryShootButton); };
            InputActions.PlayerControls.Interact.performed += context => { BindButton(context, InteractButton); };
            InputActions.PlayerControls.Reload.performed += context => { BindButton(context, ReloadButton); };
            InputActions.PlayerControls.Pause.performed += context => { BindButton(context, PauseButton); };
            InputActions.PlayerControls.SwitchWeapon.performed += context => { BindButton(context, SwitchWeaponButton); };
            InputActions.PlayerControls.SwitchCharacter.performed += context => { BindButton(context, SwitchCharacterButton); };
            InputActions.PlayerControls.TimeControl.performed += context => { BindButton(context, TimeControlButton); };
        }

        /// <summary>
        /// Changes the state of our button based on the input value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="imButton"></param>
        protected virtual void BindButton(InputAction.CallbackContext context, MMInput.IMButton imButton)
        {
            var control = context.control;

            if (control is ButtonControl button)
            {
                if (button.wasPressedThisFrame)
                {
                    imButton.State.ChangeState(MMInput.ButtonStates.ButtonDown);
                }
                if (button.wasReleasedThisFrame)
                {
                    imButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
                }
            }
        }

        protected override void GetInputButtons()
        {
            // useless now
        }

        public override void SetMovement()
        {
            //do nothing
        }

        public override void SetSecondaryMovement()
        {
            //do nothing
        }

        protected override void SetShootAxis()
        {
            //do nothing
        }
        
        protected override void SetCameraRotationAxis()
        {
            // do nothing
        }

        /// <summary>
        /// On enable we enable our input actions
        /// </summary>
        protected virtual void OnEnable()
        {
            InputActions.Enable();
        }

        /// <summary>
        /// On disable we disable our input actions
        /// </summary>
        protected virtual void OnDisable()
        {
            InputActions.Disable();
        }
    }
}