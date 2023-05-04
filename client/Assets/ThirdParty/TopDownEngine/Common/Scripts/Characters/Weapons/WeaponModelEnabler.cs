using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;

namespace MoreMountains.TopDownEngine
{
	[Serializable]
	public struct WeaponModelBindings
	{
		public GameObject WeaponModel;
		public int WeaponAnimationID;
	}

	/// <summary>
	/// This class is responsible for enabling/disabling the visual representation of weapons, if they're separated from the actual weapon object
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Model Enabler")]
	public class WeaponModelEnabler : TopDownMonoBehaviour
	{
		/// a list of model bindings. A binding is made of a gameobject, already present on the character, that will act as the visual representation of the weapon, and a name, that has to match the WeaponAnimationID of the actual Weapon
		[Tooltip("a list of model bindings. A binding is made of a gameobject, already present on the character, that will act as the visual representation of the weapon, and a name, that has to match the WeaponAnimationID of the actual Weapon")]
		public WeaponModelBindings[] Bindings;

		protected CharacterHandleWeapon _characterHandleWeapon;

		/// <summary>
		/// On Awake we grab our CharacterHandleWeapon component
		/// </summary>
		protected virtual void Awake()
		{
			_characterHandleWeapon = this.gameObject.GetComponent<CharacterHandleWeapon>();
		}

		/// <summary>
		/// On Update, we enable/disable bound gameobjects based on their name
		/// </summary>
		protected virtual void Update()
		{
			if (Bindings.Length <= 0)
			{
				return;
			}

			if (_characterHandleWeapon == null)
			{
				return;
			}

			if (_characterHandleWeapon.CurrentWeapon == null)
			{
				foreach (WeaponModelBindings binding in Bindings)
				{
					binding.WeaponModel.SetActive(false);
				}
				return;
			}

			foreach (WeaponModelBindings binding in Bindings)
			{
				if (binding.WeaponAnimationID == _characterHandleWeapon.CurrentWeapon.WeaponAnimationID)
				{
					binding.WeaponModel.SetActive(true);
				}
				else
				{
					binding.WeaponModel.SetActive(false);
				}
			}
		}			
	}
}