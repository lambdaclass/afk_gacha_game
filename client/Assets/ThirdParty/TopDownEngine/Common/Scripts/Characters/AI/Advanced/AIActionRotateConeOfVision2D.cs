using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This AIAction will rotate this AI's ConeOfVision2D either towards the AI's movement or its weapon aim direction
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionRotateConeOfVision2D")]
	public class AIActionRotateConeOfVision2D : AIAction
	{
		/// the possible directions we can aim the cone at
		public enum Modes { Movement, WeaponAim }
        
		[Header("Aim")] 
		/// whether to aim at the AI's movement direction or the weapon aim direction
		[Tooltip("whether to aim at the AI's movement direction or the weapon aim direction")]
		public Modes Mode = Modes.Movement;

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
		protected MMConeOfVision2D _coneOfVision2D;
		protected float _angle;
		protected Vector3 _eulerAngles = Vector3.zero;
        
		/// <summary>
		/// On init we grab our components
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterHandleWeapon = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
			_controller = this.gameObject.GetComponentInParent<TopDownController>();
			_coneOfVision2D = this.gameObject.GetComponent<MMConeOfVision2D>();
		}

		public override void PerformAction()
		{
			AimCone();
		}

		/// <summary>
		/// Aims the cone at either movement or weapon aim if possible
		/// </summary>
		protected virtual void AimCone()
		{
			if (_coneOfVision2D == null)
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
		/// Rotates the cone, interpolating the rotation if needed
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

			_angle = MMMaths.AngleBetween(this.transform.right, _newAim);
			_eulerAngles.y = -_angle;
            
			_coneOfVision2D.SetDirectionAndAngles(_newAim, _eulerAngles);
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