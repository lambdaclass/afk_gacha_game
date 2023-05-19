using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine // you might want to use your own namespace here
{
    public class GenericAoeAttack : CharacterAbility
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

        // This is a temporary function but later on it should be in GenericAoeAttack 
        public void ExecuteAoeAttack(Vector2 aoePosition)
        {
            //print("ability at: " + aoePosition);
        }
    }
}
