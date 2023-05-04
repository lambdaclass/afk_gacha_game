using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This is a replacement InputManager if you prefer using Unity's InputSystem over the legacy one, in a multiplayer context.
    /// Note that it's not the default solution in the engine at the moment, because older versions of Unity don't support it, 
    /// and most people still prefer not using it
    /// You can see an example of how to set it up in the MinimalScene3D_InputSystem_Multiplayer demo scene
    /// </summary>
    public class InputSystemManagerEventsBased : InputManager
    {   
        /// the position of the mouse
        public override Vector2 MousePosition => Mouse.current.position.ReadValue();
        
        public void OnJump(InputAction.CallbackContext context) { BindButton(context, JumpButton); }
        public void OnPrimaryMovement(InputAction.CallbackContext context) { _primaryMovement = ApplyCameraRotation(context.ReadValue<Vector2>());  }
        public void OnSecondaryMovement(InputAction.CallbackContext context) { _secondaryMovement = ApplyCameraRotation(context.ReadValue<Vector2>()); }
        public void OnRun(InputAction.CallbackContext context) { BindButton(context, RunButton); }
        public void OnDash(InputAction.CallbackContext context) { BindButton(context, DashButton); }
        public void OnCrouch(InputAction.CallbackContext context) { BindButton(context, CrouchButton); }
        public void OnShoot(InputAction.CallbackContext context) { BindButton(context, ShootButton); }
        public void OnSecondaryShoot(InputAction.CallbackContext context) { BindButton(context, SecondaryShootButton); }
        public void OnInteract(InputAction.CallbackContext context) { BindButton(context, InteractButton); }
        public void OnReload(InputAction.CallbackContext context) { BindButton(context, ReloadButton); }
        public void OnPause(InputAction.CallbackContext context) { BindButton(context, PauseButton); }
        public void OnSwitchWeapon(InputAction.CallbackContext context) { BindButton(context, SwitchWeaponButton); }
        public void OnSwitchCharacter(InputAction.CallbackContext context) { BindButton(context, SwitchCharacterButton); }
        public void OnTimeControl(InputAction.CallbackContext context) { BindButton(context, TimeControlButton); }

        /// <summary>
        /// Changes the state of our button based on the input value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="imButton"></param>
        protected virtual void BindButton(InputAction.CallbackContext context, MMInput.IMButton imButton)
        {
            if (!context.performed)
            {
                return;
            }

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
        
        protected override void SetCameraRotationAxis()
        {
            // do nothing
        }

        protected override void SetShootAxis()
        {
            //do nothing
        }
    }
}


