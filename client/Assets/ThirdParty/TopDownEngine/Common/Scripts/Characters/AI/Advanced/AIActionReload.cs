using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An AIACtion used to request a reload on the weapon
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionReload")]
	public class AIActionReload : AIAction
	{
		public bool OnlyReloadOnceInThisSate = true;

		protected CharacterHandleWeapon _characterHandleWeapon;
		protected bool _reloadedOnce = false;

		/// <summary>
		/// On init we grab our components
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterHandleWeapon = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
		}

		/// <summary>
		/// Requests a reload
		/// </summary>
		public override void PerformAction()
		{
			if (OnlyReloadOnceInThisSate && _reloadedOnce)
			{
				return;
			}
			if (_characterHandleWeapon == null)
			{
				return;
			}
			_characterHandleWeapon.Reload();
			_reloadedOnce = true;
		}

		/// <summary>
		/// On enter state we reset our counter
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			_reloadedOnce = false;
		}
	}
}