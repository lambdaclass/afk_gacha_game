using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This action is used to force your character to switch to another weapon. Just drag a weapon prefab into its NewWeapon slot and you're good to go.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionChangeWeapon")]
	//[RequireComponent(typeof(CharacterHandleWeapon))]
	public class AIActionChangeWeapon : AIAction
	{
		/// the new weapon to equip when that action is performed
		[Tooltip("The new weapon to equip when that action is performed")]
		public Weapon NewWeapon;

		protected CharacterHandleWeapon _characterHandleWeapon;
		protected int _change = 0;

		/// <summary>
		/// On init we grab our CharacterHandleWeapon ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterHandleWeapon = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
		}

		/// <summary>
		/// On PerformAction we change weapon
		/// </summary>
		public override void PerformAction()
		{
			ChangeWeapon();
		}

		/// <summary>
		/// Performs the weapon change
		/// </summary>
		protected virtual void ChangeWeapon()
		{
			if (_change < 1)
			{
				if (NewWeapon == null)
				{
					_characterHandleWeapon.ChangeWeapon(NewWeapon, "");
				}
				else
				{
					_characterHandleWeapon.ChangeWeapon(NewWeapon, NewWeapon.name);
				}
                
				_change++;
			}
		}

		/// <summary>
		/// Resets our counter
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			_change = 0;
		}
	}
}