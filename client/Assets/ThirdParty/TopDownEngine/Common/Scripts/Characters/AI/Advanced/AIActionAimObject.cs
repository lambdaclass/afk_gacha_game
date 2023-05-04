using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An AIACtion used to aim any object in the direction of the AI's movement or aim
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionAimObject")]
	public class AIActionAimObject : AIAction
	{
		/// the possible directions we can aim the target object at
		public enum Modes { Movement, WeaponAim }
		/// the axis to aim at the movement or weapon aim direction
		public enum PossibleAxis { Right, Forward }
        
		[Header("Aim Object")] 
		/// an object to aim
		[Tooltip("an object to aim")]
		public GameObject GameObjectToAim;
		/// whether to aim at the AI's movement direction or the weapon aim direction
		[Tooltip("whether to aim at the AI's movement direction or the weapon aim direction")]
		public Modes Mode = Modes.Movement;
		/// the axis to aim at the moment or weapon aim direction (usually right for 2D, forward for 3D)
		[Tooltip("the axis to aim at the moment or weapon aim direction (usually right for 2D, forward for 3D)")]
		public PossibleAxis Axis = PossibleAxis.Right;

		[Header("Interpolation")] 
		/// whether or not to interpolate the rotation
		[Tooltip("whether or not to interpolate the rotation")]
		public bool Interpolate = false;
		/// the rate at which to interpolate the rotation
		[Tooltip("the rate at which to interpolate the rotation")]
		[MMCondition("Interpolate", true)] 
		public float InterpolateRate = 5f;
        
		protected CharacterHandleWeapon _characterHandleWeapon;
		protected WeaponAim _weaponAim;
		protected TopDownController _controller;
		protected Vector3 _newAim;
        
		/// <summary>
		/// On init we grab our components
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterHandleWeapon = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
			_controller = this.gameObject.GetComponentInParent<TopDownController>();
		}

		public override void PerformAction()
		{
			AimObject();
		}

		/// <summary>
		/// Aims the object at either movement or weapon aim if possible
		/// </summary>
		protected virtual void AimObject()
		{
			if (GameObjectToAim == null)
			{
				return;
			}
            
			switch (Mode )
			{
				case Modes.Movement:
					AimAt(_controller.CurrentDirection.normalized);
					break;
				case Modes.WeaponAim:
					if (_weaponAim == null)
					{
						GrabWeaponAim();
					}
					else
					{
						AimAt(_weaponAim.CurrentAim.normalized);    
					}
					break;
			}
		}

		/// <summary>
		/// Rotates the target object, interpolating the rotation if needed
		/// </summary>
		/// <param name="direction"></param>
		protected virtual void AimAt(Vector3 direction)
		{
			if (Interpolate)
			{
				_newAim = MMMaths.Lerp(_newAim, direction, InterpolateRate, Time.deltaTime);
			}
			else
			{
				_newAim = direction;
			}
            
			switch (Axis)
			{
				case PossibleAxis.Forward:
					GameObjectToAim.transform.forward = _newAim;
					break;
				case PossibleAxis.Right:
					GameObjectToAim.transform.right = _newAim;
					break;
			}
		}
        
		/// <summary>
		/// Caches the weapon aim comp
		/// </summary>
		protected virtual void GrabWeaponAim()
		{
			if ((_characterHandleWeapon != null) && (_characterHandleWeapon.CurrentWeapon != null))
			{
				_weaponAim = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
			}            
		}
        
		/// <summary>
		/// On entry we grab the weapon aim and cache it
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			GrabWeaponAim();
		}
	}
}