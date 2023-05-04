using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionShoot3D")]
	//[RequireComponent(typeof(CharacterOrientation3D))]
	//[RequireComponent(typeof(CharacterHandleWeapon))]
	public class AIActionShoot3D : AIAction
	{
		public enum AimOrigins { Transform, SpawnPosition }
        
		[Header("Binding")] 
		/// the CharacterHandleWeapon ability this AI action should pilot. If left blank, the system will grab the first one it finds.
		[Tooltip("the CharacterHandleWeapon ability this AI action should pilot. If left blank, the system will grab the first one it finds.")]
		public CharacterHandleWeapon TargetHandleWeaponAbility;
        
		[Header("Behaviour")]
		/// if true the Character will aim at the target when shooting
		[Tooltip("if true the Character will aim at the target when shooting")]
		public bool AimAtTarget = true;
		/// the point to consider as the aim origin
		[Tooltip("the point to consider as the aim origin")]
		public AimOrigins AimOrigin = AimOrigins.Transform;
		/// an offset to apply to the aim (useful to aim at the head/torso/etc automatically)
		[Tooltip("an offset to apply to the aim (useful to aim at the head/torso/etc automatically)")]
		public Vector3 ShootOffset;
		/// if this is set to true, vertical aim will be locked to remain horizontal
		[Tooltip("if this is set to true, vertical aim will be locked to remain horizontal")]
		public bool LockVerticalAim = false;

		protected CharacterOrientation3D _orientation3D;
		protected Character _character;
		protected WeaponAim _weaponAim;
		protected ProjectileWeapon _projectileWeapon;
		protected Vector3 _weaponAimDirection;
		protected int _numberOfShoots = 0;
		protected bool _shooting = false;
		protected Weapon _targetWeapon;

		/// <summary>
		/// On init we grab our CharacterHandleWeapon ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_character = GetComponentInParent<Character>();
			_orientation3D = _character?.FindAbility<CharacterOrientation3D>();
			if (TargetHandleWeaponAbility == null)
			{
				TargetHandleWeaponAbility = _character?.FindAbility<CharacterHandleWeapon>();
			}
		}

		/// <summary>
		/// On PerformAction we face and aim if needed, and we shoot
		/// </summary>
		public override void PerformAction()
		{
			MakeChangesToTheWeapon();
			TestAimAtTarget();
			Shoot();
		}

		/// <summary>
		/// Makes changes to the weapon to ensure it works ok with AI scripts
		/// </summary>
		protected virtual void MakeChangesToTheWeapon()
		{
			if (TargetHandleWeaponAbility.CurrentWeapon != null)
			{
				TargetHandleWeaponAbility.CurrentWeapon.TimeBetweenUsesReleaseInterruption = true;
			}
		}

		/// <summary>
		/// Sets the current aim if needed
		/// </summary>
		protected virtual void Update()
		{
			if (TargetHandleWeaponAbility.CurrentWeapon != null)
			{
				if (_weaponAim != null)
				{
					if (_shooting)
					{
						if (LockVerticalAim)
						{
							_weaponAimDirection.y = 0;
						}

						if (AimAtTarget)
						{
							_weaponAim.SetCurrentAim(_weaponAimDirection);    
						}
					}
				}
			}
		}
        
		/// <summary>
		/// Aims at the target if required
		/// </summary>
		protected virtual void TestAimAtTarget()
		{
			if (!AimAtTarget || (_brain.Target == null))
			{
				return;
			}

			if (TargetHandleWeaponAbility.CurrentWeapon != null)
			{
				if (_weaponAim == null)
				{
					_weaponAim = TargetHandleWeaponAbility.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
				}

				if (_weaponAim != null)
				{
					if (_projectileWeapon != null)
					{
						if (AimOrigin == AimOrigins.Transform)
						{
							_weaponAimDirection = _brain.Target.position + ShootOffset - _character.transform.position;   
						}
						else if (AimOrigin == AimOrigins.SpawnPosition)
						{
							_projectileWeapon.DetermineSpawnPosition();
							_weaponAimDirection = _brain.Target.position + ShootOffset - _projectileWeapon.SpawnPosition;    
						}
					}
					else
					{
						_weaponAimDirection = _brain.Target.position + ShootOffset - _character.transform.position;
					}                    
				}                
			}
		}

		/// <summary>
		/// Activates the weapon
		/// </summary>
		protected virtual void Shoot()
		{
			if (_numberOfShoots < 1)
			{
				_targetWeapon = TargetHandleWeaponAbility.CurrentWeapon;
				TargetHandleWeaponAbility.ShootStart();
				_numberOfShoots++;
			}

			if ((_targetWeapon == null) || (TargetHandleWeaponAbility.CurrentWeapon != _targetWeapon))
			{
				OnEnterState();
			}
		}

		/// <summary>
		/// When entering the state we reset our shoot counter and grab our weapon
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			_numberOfShoots = 0;
			_shooting = true;
			_weaponAim = TargetHandleWeaponAbility.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
			_projectileWeapon = TargetHandleWeaponAbility.CurrentWeapon.gameObject.MMGetComponentNoAlloc<ProjectileWeapon>();
		}

		/// <summary>
		/// When exiting the state we make sure we're not shooting anymore
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();
			if (TargetHandleWeaponAbility != null)
			{
				TargetHandleWeaponAbility.ForceStop();    
			}
			_shooting = false;
		}
	}
}