using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Agnostic version of the InputSystemManager that lets you define a set of input actions to read from
    /// </summary>
    public class InputSystemManagerBase<T> : InputManager where T : IInputActionCollection, new()
    {
        /// a set of input actions to use to read input on
        public T InputActions;
        
        /// the position of the mouse
        public override Vector2 MousePosition => Mouse.current.position.ReadValue();

        protected override void Awake()
        {
            base.Awake();
            InputActions = new T();
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

        protected override void Update()
        {
            // base.Update();
        }
    }
}