using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Aims the weapon at the current movement when not shooting
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionAimWeaponAtMovement")]
	//[RequireComponent(typeof(CharacterHandleWeapon))]
	public class AIActionAimWeaponAtMovement : AIAction
	{
		protected TopDownController _controller;
		protected CharacterHandleWeapon _characterHandleWeapon;
		protected WeaponAim _weaponAim;
		protected AIActionShoot2D _aiActionShoot2D;
		protected AIActionShoot3D _aiActionShoot3D;
		protected Vector3 _weaponAimDirection;

		/// <summary>
		/// On init we grab our components
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterHandleWeapon = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
			_aiActionShoot2D = this.gameObject.GetComponent<AIActionShoot2D>();
			_aiActionShoot3D = this.gameObject.GetComponent<AIActionShoot3D>();
			_controller = this.gameObject.GetComponentInParent<TopDownController>();
		}
        
		/// <summary>
		/// if we're not shooting, we aim at our current movement
		/// </summary>
		public override void PerformAction()
		{
			if (!Shooting())
			{
				_weaponAimDirection = _controller.CurrentDirection;
				if (_weaponAim == null)
				{
					GrabWeaponAim();
				}
				if (_weaponAim == null)
				{
					return;
				}
				UpdateAim();
			}
		}
		void UpdateAim()
		{
			_weaponAimDirection = _controller.CurrentDirection;
			if (_weaponAim != null)
			{
				_weaponAim.SetCurrentAim(_weaponAimDirection);
			}
		}

		/// <summary>
		/// Returns true if shooting, returns false otherwise
		/// </summary>
		/// <returns></returns>
		protected bool Shooting()
		{
			if (_aiActionShoot2D != null)
			{
				return _aiActionShoot2D.ActionInProgress;
			}
			if (_aiActionShoot3D != null)
			{
				return _aiActionShoot3D.ActionInProgress;
			}
			return false;
		}

		protected virtual void GrabWeaponAim()
		{
			if ((_characterHandleWeapon != null) && (_characterHandleWeapon.CurrentWeapon != null))
			{
				_weaponAim = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
			}            
		}

		/// <summary>
		/// When entering the state we grab our weapon
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			GrabWeaponAim();
			enabled = true;
		}
		public override void OnExitState()
		{
			base.OnExitState();
			enabled = false;
		}
		private void Update()
		{
			if (!Shooting())
			{
				UpdateAim();
			}
		}
	}
}