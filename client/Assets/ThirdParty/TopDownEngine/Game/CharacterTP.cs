using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/CharacterTP")]
    public class CharacterTP : CharacterAbility
    {
        protected override void Initialization()
        {
            base.Initialization();
        }

        /// <summary>
        /// Every frame, we check if we're crouched and if we still should be
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        protected virtual void GoThroughWalls(bool status)
        {
            if (status == true)
            {
                _controller.CollisionsOff();
            }
            else
            {
                _controller.CollisionsOn();
            }
        }

        /// <summary>
        /// Called at the start of the ability's cycle, this is where you'll check for input
        /// </summary>
        protected override void HandleInput()
        {
            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                GoThroughWalls(true);
            }
            if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
            {
                GoThroughWalls(false);
            }
        }
    }
}
