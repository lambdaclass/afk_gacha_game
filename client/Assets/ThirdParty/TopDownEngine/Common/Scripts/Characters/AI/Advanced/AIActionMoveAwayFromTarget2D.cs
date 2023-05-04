using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Requires a CharacterMovement ability. Makes the character move away from the target. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveAwayFromTarget2D")]
	//[RequireComponent(typeof(CharacterMovement))]
	public class AIActionMoveAwayFromTarget2D : AIAction
	{
		/// the maximum distance away from the target this Character can reach.
		[Tooltip("the maximum distance away from the target this Character can reach.")]
		public float MaximumDistance = 5f;

		protected CharacterMovement _characterMovement;

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
            
			if (this.transform.position.x < _brain.Target.position.x)
			{
				_characterMovement.SetHorizontalMovement(-1f);
			}
			else
			{
				_characterMovement.SetHorizontalMovement(1f);
			}

			if (this.transform.position.y < _brain.Target.position.y)
			{
				_characterMovement.SetVerticalMovement(-1f);
			}
			else
			{
				_characterMovement.SetVerticalMovement(1f);
			}
            
			if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) > MaximumDistance)
			{
				_characterMovement.SetHorizontalMovement(0f);
			}

			if (Mathf.Abs(this.transform.position.y - _brain.Target.position.y) > MaximumDistance)
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