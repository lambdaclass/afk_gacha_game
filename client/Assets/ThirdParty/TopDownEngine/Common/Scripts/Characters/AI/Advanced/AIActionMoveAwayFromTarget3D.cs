using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Requires a CharacterMovement ability. Makes the character move away from the target, up to the specified MaximumDistance. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveAwayFromTarget3D")]
	//[RequireComponent(typeof(CharacterMovement))]
	public class AIActionMoveAwayFromTarget3D : AIAction
	{
		/// the maximum distance from the target this Character can reach.
		[Tooltip("the maximum distance from the target this Character can reach.")]
		public float MaximumDistance = 10f;

		protected Vector3 _directionToTarget;
		protected CharacterMovement _characterMovement;
		protected Vector2 _movementVector;

		/// <summary>
		/// On init we grab our CharacterMovement ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
		}

		/// <summary>
		/// On PerformAction we move
		/// </summary>
		public override void PerformAction()
		{
			Move();
		}

		/// <summary>
		/// Moves the character towards the target if needed
		/// </summary>
		protected virtual void Move()
		{
			if (_brain.Target == null)
			{
				return;
			}
            
			_directionToTarget = _brain.Target.position - this.transform.position;
			_movementVector.x = _directionToTarget.x;
			_movementVector.y = _directionToTarget.z;
			_characterMovement.SetMovement(-_movementVector);


			if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) > MaximumDistance)
			{
				_characterMovement.SetHorizontalMovement(0f);
			}

			if (Mathf.Abs(this.transform.position.z - _brain.Target.position.z) > MaximumDistance)
			{
				_characterMovement.SetVerticalMovement(0f);
			}
		}

		/// <summary>
		/// On exit state we stop our movement
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();

			_characterMovement?.SetHorizontalMovement(0f);
			_characterMovement?.SetVerticalMovement(0f);
		}
	}
}