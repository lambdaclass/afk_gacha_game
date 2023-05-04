using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This action will make the character pathfind its way back to its last patrol point
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionPathfinderToPatrol3D")]
	//[RequireComponent(typeof(AIActionMovePatrol3D))]
	//[RequireComponent(typeof(CharacterPathfinder3D))]
	//[RequireComponent(typeof(CharacterMovement))]
	public class AIActionPathfinderToPatrol3D : AIAction
	{
		protected CharacterMovement _characterMovement;
		protected CharacterPathfinder3D _characterPathfinder3D;
		protected Transform _backToPatrolTransform;
		protected AIActionMovePatrol3D _aiActionMovePatrol3D;
		
		static Transform _BackToPatrolBeacons;
		public static Transform BackToPatrolBeaconsRoot
		{
			get
			{
				if (_BackToPatrolBeacons != null)
				{
					return _BackToPatrolBeacons;
				}
				GameObject newRoot = new GameObject(nameof(AIActionPathfinderToPatrol3D)+"_BackToPatrolBeacons");
				_BackToPatrolBeacons = newRoot.transform;
				return _BackToPatrolBeacons;
			}
		}

		/// <summary>
		/// On init we grab our CharacterMovement ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
			_characterPathfinder3D = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterPathfinder3D>();
			_aiActionMovePatrol3D = this.gameObject.GetComponent<AIActionMovePatrol3D>();

			GameObject backToPatrolBeacon = new GameObject();
			backToPatrolBeacon.name = this.gameObject.name + "BackToPatrolBeacon";
			_backToPatrolTransform = backToPatrolBeacon.transform;			
			_backToPatrolTransform.parent = BackToPatrolBeaconsRoot;
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
			if (_aiActionMovePatrol3D == null)
			{
				return;
			}

			_backToPatrolTransform.position = _aiActionMovePatrol3D.LastReachedPatrolPoint;
			_characterPathfinder3D.SetNewDestination(_backToPatrolTransform);
			_brain.Target = _backToPatrolTransform;
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