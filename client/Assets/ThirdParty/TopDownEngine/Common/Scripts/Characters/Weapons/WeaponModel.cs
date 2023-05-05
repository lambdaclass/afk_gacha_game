using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used to force a model to aim at a Weapon's target
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Model")]
	public class WeaponModel : TopDownMonoBehaviour
	{
		[Header("Model")]
		/// a unique ID that will be used to hide / show this model when the corresponding weapon gets equipped
		[Tooltip("a unique ID that will be used to hide / show this model when the corresponding weapon gets equipped")]
		public string WeaponID = "WeaponID";
		/// a GameObject to show/hide for this model, usually nested right below the logic level of the WeaponModel
		[Tooltip("a GameObject to show/hide for this model, usually nested right below the logic level of the WeaponModel")]
		public GameObject TargetModel;

		[Header("Aim")]
		/// if this is true, the model will aim at the parent weapon's target
		[Tooltip("if this is true, the model will aim at the parent weapon's target")]
		public bool AimWeaponModelAtTarget = true;
		/// if this is true, the model's aim will be vertically locked (no up/down aiming)
		[Tooltip("if this is true, the model's aim will be vertically locked (no up/down aiming)")]
		public bool LockVerticalRotation = true;

		[Header("Animator")]
		/// whether or not to add the target animator to the real weapon's animator list
		[Tooltip("whether or not to add the target animator to the real weapon's animator list")]
		public bool AddAnimator = false;
		/// the animator to send weapon animation parameters to
		[Tooltip("the animator to send weapon animation parameters to")]
		public Animator TargetAnimator;

		[Header("SpawnTransform")]
		/// whether or not to override the weapon use transform
		[Tooltip("whether or not to override the weapon use transform")]
		public bool OverrideWeaponUseTransform = false;
		/// a transform to use as the spawn point for weapon use (if null, only offset will be considered, otherwise the transform without offset)
		[Tooltip("a transform to use as the spawn point for weapon use (if null, only offset will be considered, otherwise the transform without offset)")]
		public Transform WeaponUseTransform;

		[Header("IK")]
		/// whether or not to use IK with this model
		[Tooltip("whether or not to use IK with this model")]
		public bool UseIK = false;
		/// the transform to which the character's left hand should be attached to
		[Tooltip("the transform to which the character's left hand should be attached to")]
		public Transform LeftHandHandle;
		/// the transform to which the character's right hand should be attached to
		[Tooltip("the transform to which the character's right hand should be attached to")]
		public Transform RightHandHandle;

		[Header("Feedbacks")]
		/// if this is true, the model's feedbacks will replace the original weapon's feedbacks
		[Tooltip("if this is true, the model's feedbacks will replace the original weapon's feedbacks")]
		public bool BindFeedbacks = true;
		/// the feedback to play when the weapon starts being used
		[Tooltip("the feedback to play when the weapon starts being used")]
		public MMFeedbacks WeaponStartMMFeedback;
		/// the feedback to play while the weapon is in use
		[Tooltip("the feedback to play while the weapon is in use")]
		public MMFeedbacks WeaponUsedMMFeedback;
		/// the feedback to play when the weapon stops being used
		[Tooltip("the feedback to play when the weapon stops being used")]
		public MMFeedbacks WeaponStopMMFeedback;
		/// the feedback to play when the weapon gets reloaded
		[Tooltip("the feedback to play when the weapon gets reloaded")]
		public MMFeedbacks WeaponReloadMMFeedback;
		/// the feedback to play when the weapon gets reloaded
		[Tooltip("the feedback to play when the weapon gets reloaded")]
		public MMFeedbacks WeaponReloadNeededMMFeedback;

		protected CharacterHandleWeapon _handleWeapon;
		protected WeaponAim _weaponAim;
		protected Vector3 _rotationDirection;

		protected virtual void Awake()
		{
			Hide();
		}

		/// <summary>
		/// On Start we grab our CharacterHandleWeapon component
		/// </summary>
		protected virtual void Start()
		{
			_handleWeapon = this.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
		}

		/// <summary>
		/// Aims the weapon model at the target
		/// </summary>
		protected virtual void Update()
		{
			if (!AimWeaponModelAtTarget)
			{
				return;
			}

			if (_weaponAim == null)
			{
				if (_handleWeapon.CurrentWeapon != null)
				{
					_weaponAim = _handleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
				}                
			}
			else
			{
				_rotationDirection = _weaponAim.CurrentAim.normalized;
				if (LockVerticalRotation)
				{
					_rotationDirection.y = 0;
				}
				this.transform.LookAt(_weaponAim.transform.position + 10f * _rotationDirection);
			}
		}

		public virtual void Show()
		{
			TargetModel.SetActive(true);
		}

		public virtual void Hide()
		{
			TargetModel.SetActive(false);
		}
	}
}