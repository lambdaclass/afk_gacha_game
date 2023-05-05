using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionRotateTowardsTarget3D")]
	//[RequireComponent(typeof(CharacterOrientation3D))]
	public class AIActionRotateTowardsTarget3D : AIAction
	{
		[Header("Lock Rotation")]
		/// whether or not to lock the X rotation. If set to false, the model will rotate on the x axis, to aim up or down 
		[Tooltip("whether or not to lock the X rotation. If set to false, the model will rotate on the x axis, to aim up or down")]
		public bool LockRotationX = false;

		protected CharacterOrientation3D _characterOrientation3D;
		protected Vector3 _targetPosition;
		protected bool _originalForcedRotation;

		/// <summary>
		/// On init we grab our CharacterOrientation3D ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterOrientation3D = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterOrientation3D>();
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
			_characterOrientation3D.ForcedRotationDirection = (_targetPosition - this.transform.position).normalized;
		}

		/// <summary>
		/// On enter state we reset our flag
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			if (_characterOrientation3D == null)
			{
				_characterOrientation3D = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterOrientation3D>();
			}
			if (_characterOrientation3D != null)
			{
				_originalForcedRotation = _characterOrientation3D.ForcedRotation;
				_characterOrientation3D.ForcedRotation = true;
			}
		}

		/// <summary>
		/// On enter state we reset our flag
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();
			if (_characterOrientation3D != null)
			{
				_characterOrientation3D.ForcedRotation = _originalForcedRotation;
			}
		}
	}
}