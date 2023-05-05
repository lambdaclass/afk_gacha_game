using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionPathfinderToTarget3D")]
	//[RequireComponent(typeof(CharacterMovement))]
	//[RequireComponent(typeof(CharacterPathfinder3D))]
	public class AIActionPathfinderToTarget3D : AIAction
	{
		public float MinimumDelayBeforeUpdatingTarget = 0.3f;
		
		protected CharacterMovement _characterMovement;
		protected CharacterPathfinder3D _characterPathfinder3D;
		protected float _lastSetNewDestinationAt = -Single.MaxValue;

		/// <summary>
		/// On init we grab our CharacterMovement ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
			_characterPathfinder3D = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterPathfinder3D>();
			if (_characterPathfinder3D == null)
			{
				Debug.LogWarning(this.name + " : the AIActionPathfinderToTarget3D AI Action requires the CharacterPathfinder3D ability");
			}
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
			if (Time.time - _lastSetNewDestinationAt < MinimumDelayBeforeUpdatingTarget)
			{
				return;
			}

			_lastSetNewDestinationAt = Time.time;
			
			if (_brain.Target == null)
			{
				_characterPathfinder3D.SetNewDestination(null);
				return;
			}
			else
			{
				_characterPathfinder3D.SetNewDestination(_brain.Target.transform);
			}
		}

		/// <summary>
		/// On exit state we stop our movement
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();
            
			_characterPathfinder3D?.SetNewDestination(null);
			_characterMovement?.SetHorizontalMovement(0f);
			_characterMovement?.SetVerticalMovement(0f);
		}
	}
}