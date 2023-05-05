using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Serialization;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveTowardsTarget2D")]
	//[RequireComponent(typeof(CharacterMovement))]
	public class AIActionMoveTowardsTarget2D : AIAction
	{
		/// if this is true, movement will be constrained to not overstep a certain distance to the target on the x axis
		[Tooltip("if this is true, movement will be constrained to not overstep a certain distance to the target on the x axis")]
		public bool UseMinimumXDistance = true;
		/// the minimum distance from the target this Character can reach on the x axis.
		[FormerlySerializedAs("MinimumDistance")] [Tooltip("the minimum distance from the target this Character can reach on the x axis.")]
		public float MinimumXDistance = 1f;
		
		protected Vector2 _direction;
		protected CharacterMovement _characterMovement;
		protected int _numberOfJumps = 0;

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

			if (UseMinimumXDistance)
			{
				if (this.transform.position.x < _brain.Target.position.x)
				{
					_characterMovement.SetHorizontalMovement(1f);
				}
				else
				{
					_characterMovement.SetHorizontalMovement(-1f);
				}

				if (this.transform.position.y < _brain.Target.position.y)
				{
					_characterMovement.SetVerticalMovement(1f);
				}
				else
				{
					_characterMovement.SetVerticalMovement(-1f);
				}
            
				if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) < MinimumXDistance)
				{
					_characterMovement.SetHorizontalMovement(0f);
				}

				if (Mathf.Abs(this.transform.position.y - _brain.Target.position.y) < MinimumXDistance)
				{
					_characterMovement.SetVerticalMovement(0f);
				}
			}
			else
			{
				_direction = (_brain.Target.position - this.transform.position).normalized;
				_characterMovement.SetMovement(_direction);
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