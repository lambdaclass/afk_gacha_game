using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This AI Action will let an agent with a CharacterRotation2D ability (set to ForcedRotation:true) rotate to face its target
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionRotateTowardsTarget2D")]
	//[RequireComponent(typeof(CharacterRotation2D))]
	public class AIActionRotateTowardsTarget2D : AIAction
	{
		[Header("Lock Rotation")]
		/// whether or not to lock the X rotation. If set to false, the model will rotate on the x axis, to aim up or down 
		[Tooltip("whether or not to lock the X rotation. If set to false, the model will rotate on the x axis, to aim up or down")]
		public bool LockRotationX = false;

		protected CharacterRotation2D _characterRotation2D;
		protected Vector3 _targetPosition;

		/// <summary>
		/// On init we grab our CharacterOrientation3D ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterRotation2D = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterRotation2D>();
		}

		/// <summary>
		/// On PerformAction we move
		/// </summary>
		public override void PerformAction()
		{
			Rotate();
		}

		/// <summary>
		/// Makes the orientation 3D ability rotate towards the brain target
		/// </summary>
		protected virtual void Rotate()
		{
			if (_brain.Target == null)
			{
				return;
			}
			_targetPosition = _brain.Target.transform.position;
			if (LockRotationX)
			{
				_targetPosition.y = this.transform.position.y;
			}
			_characterRotation2D.ForcedRotationDirection = (_targetPosition - this.transform.position).normalized;
		}
	}
}