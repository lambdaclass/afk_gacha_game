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
        public void ExecuteAoeAttack(Vector2 aoePosition)
        {
            //print("ability at: " + aoePosition);
            //Load the prefab
            GameObject instance = Instantiate(Resources.Load("AoeAttack", typeof(GameObject))) as GameObject;
            //Set the prefav as a player child
            instance.transform.parent = transform;
            //Set its position to the player position
            instance.transform.position = transform.position;
            //Destroy after showing it
            Destroy(instance, 2.1f);
            //print("Player: " + _character.PlayerID);

            RelativePosition relative_position = new RelativePosition
            {
                X = (long)(-aoePosition.y * 100),
                Y = (long)(aoePosition.x * 100)
            };

            ClientAction action = new ClientAction { Action = Action.AttackAoe, Position = relative_position };
            SocketConnectionManager.Instance.SendAction(action);
        }
    }
}
