using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Melee Weapon")]
	public class MeleeWeapon : Weapon
	{
		/// the possible shapes for the melee weapon's damage area
		public enum MeleeDamageAreaShapes { Rectangle, Circle, Box, Sphere }
		public enum MeleeDamageAreaModes { Generated, Existing }

		[MMInspectorGroup("Damage Area", true, 22)]
		/// the possible modes to handle the damage area. In Generated, the MeleeWeapon will create it, in Existing, you can bind an existing damage area - usually nested under the weapon
		[Tooltip("the possible modes to handle the damage area. In Generated, the MeleeWeapon will create it, in Existing, you can bind an existing damage area - usually nested under the weapon")]
		public MeleeDamageAreaModes MeleeDamageAreaMode = MeleeDamageAreaModes.Generated;
		/// the shape of the damage area (rectangle or circle)
		[Tooltip("the shape of the damage area (rectangle or circle)")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Generated)]
		public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Rectangle;
		/// the offset to apply to the damage area (from the weapon's attachment position
		[Tooltip("the offset to apply to the damage area (from the weapon's attachment position")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Generated)]
		public Vector3 AreaOffset = new Vector3(1, 0);
		/// the size of the damage area
		[Tooltip("the size of the damage area")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Generated)]
		public Vector3 AreaSize = new Vector3(1, 1);
		/// the trigger filters this melee weapon should apply damage on (by default, it'll apply damage on everything, but you can change this to only apply when targets enter the area, for example)
		[Tooltip("the trigger filters this melee weapon should apply damage on (by default, it'll apply damage on everything, but you can change this to only apply when targets enter the area, for example)")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Generated)]
		public DamageOnTouch.TriggerAndCollisionMask TriggerFilter = DamageOnTouch.AllowedTriggerCallbacks;
		/// the feedback to play when hitting a Damageable
		[Tooltip("the feedback to play when hitting a Damageable")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Generated)]
		public MMFeedbacks HitDamageableFeedback;
		/// the feedback to play when hitting a non Damageable
		[Tooltip("the feedback to play when hitting a non Damageable")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Generated)]
		public MMFeedbacks HitNonDamageableFeedback;
		/// an existing damage area to activate/handle as the weapon is used
		[Tooltip("an existing damage area to activate/handle as the weapon is used")]
		[MMEnumCondition("MeleeDamageAreaMode", (int)MeleeDamageAreaModes.Existing)]
		public DamageOnTouch ExistingDamageArea;

		[MMInspectorGroup("Damage Area Timing", true, 23)]
        
		/// the initial delay to apply before triggering the damage area
		[Tooltip("the initial delay to apply before triggering the damage area")]
		public float InitialDelay = 0f;
		/// the duration during which the damage area is active
		[Tooltip("the duration during which the damage area is active")]
		public float ActiveDuration = 1f;

		[MMInspectorGroup("Damage Caused", true, 24)]

		/// the layers that will be damaged by this object
		[Tooltip("the layers that will be damaged by this object")]
		public LayerMask TargetLayerMask;
		/// The min amount of health to remove from the player's health
		[FormerlySerializedAs("DamageCaused")] 
		[Tooltip("The min amount of health to remove from the player's health")]
		public float MinDamageCaused = 10f;
		/// The max amount of health to remove from the player's health
		[FormerlySerializedAs("DamageCaused")] 
		[Tooltip("The max amount of health to remove from the player's health")]
		public float MaxDamageCaused = 10f;
		/// the kind of knockback to apply
		[Tooltip("the kind of knockback to apply")]
		public DamageOnTouch.KnockbackStyles Knockback;
		/// The force to apply to the object that gets damaged
		[Tooltip("The force to apply to the object that gets damaged")]
		public Vector3 KnockbackForce = new Vector3(10, 2, 0);
		/// The duration of the invincibility frames after the hit (in seconds)
		[Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
		public float InvincibilityDuration = 0.5f;
		/// if this is true, the owner can be damaged by its own weapon's damage area (usually false)
		[Tooltip("if this is true, the owner can be damaged by its own weapon's damage area (usually false)")]
		public bool CanDamageOwner = false;

		protected Collider _damageAreaCollider;
		protected Collider2D _damageAreaCollider2D;
		protected bool _attackInProgress = false;
		protected Color _gizmosColor;
		protected Vector3 _gizmoSize;
		protected CircleCollider2D _circleCollider2D;
		protected BoxCollider2D _boxCollider2D;
		protected BoxCollider _boxCollider;
		protected SphereCollider _sphereCollider;
		protected Vector3 _gizmoOffset;
		protected DamageOnTouch _damageOnTouch;
		protected GameObject _damageArea;

		/// <summary>
		/// Initialization
		/// </summary>
		public override void Initialization()
		{
			base.Initialization();

			if (_damageArea == null)
			{
				CreateDamageArea();
				DisableDamageArea();
			}
			if (Owner != null)
			{
				_damageOnTouch.Owner = Owner.gameObject;
			}            
		}

		/// <summary>
		/// Creates the damage area.
		/// </summary>
		protected virtual void CreateDamageArea()
		{
			if ((MeleeDamageAreaMode == MeleeDamageAreaModes.Existing) && (ExistingDamageArea != null))
			{
				_damageArea = ExistingDamageArea.gameObject;
				_damageAreaCollider = _damageArea.gameObject.GetComponent<Collider>();
				_damageAreaCollider2D = _damageArea.gameObject.GetComponent<Collider2D>();
				_damageOnTouch = ExistingDamageArea;
				return;
			}
			
			_damageArea = new GameObject();
			_damageArea.name = this.name + "DamageArea";
			_damageArea.transform.position = this.transform.position;
			_damageArea.transform.rotation = this.transform.rotation;
			_damageArea.transform.SetParent(this.transform);
			_damageArea.transform.localScale = Vector3.one;
			_damageArea.layer = this.gameObject.layer;
            
			if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
			{
				_boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
				_boxCollider2D.offset = AreaOffset;
				_boxCollider2D.size = AreaSize;
				_damageAreaCollider2D = _boxCollider2D;
				_damageAreaCollider2D.isTrigger = true;
			}
			if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
			{
				_circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
				_circleCollider2D.transform.position = this.transform.position;
				_circleCollider2D.offset = AreaOffset;
				_circleCollider2D.radius = AreaSize.x / 2;
				_damageAreaCollider2D = _circleCollider2D;
				_damageAreaCollider2D.isTrigger = true;
			}

			if ((DamageAreaShape == MeleeDamageAreaShapes.Rectangle) || (DamageAreaShape == MeleeDamageAreaShapes.Circle))
			{
				Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
				rigidBody.isKinematic = true;
				rigidBody.sleepMode = RigidbodySleepMode2D.NeverSleep;
			}            

			if (DamageAreaShape == MeleeDamageAreaShapes.Box)
			{
				_boxCollider = _damageArea.AddComponent<BoxCollider>();
				_boxCollider.center = AreaOffset;
				_boxCollider.size = AreaSize;
				_damageAreaCollider = _boxCollider;
				_damageAreaCollider.isTrigger = true;
			}
			if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
			{
				_sphereCollider = _damageArea.AddComponent<SphereCollider>();
				_sphereCollider.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
				_sphereCollider.radius = AreaSize.x / 2;
				_damageAreaCollider = _sphereCollider;
				_damageAreaCollider.isTrigger = true;
			}

			if ((DamageAreaShape == MeleeDamageAreaShapes.Box) || (DamageAreaShape == MeleeDamageAreaShapes.Sphere))
			{
				Rigidbody rigidBody = _damageArea.AddComponent<Rigidbody>();
				rigidBody.isKinematic = true;
			}

			_damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
			_damageOnTouch.SetGizmoSize(AreaSize);
			_damageOnTouch.SetGizmoOffset(AreaOffset);
			_damageOnTouch.TargetLayerMask = TargetLayerMask;
			_damageOnTouch.MinDamageCaused = MinDamageCaused;
			_damageOnTouch.MaxDamageCaused = MaxDamageCaused;
			_damageOnTouch.DamageDirectionMode = DamageOnTouch.DamageDirections.BasedOnOwnerPosition;
			_damageOnTouch.DamageCausedKnockbackType = Knockback;
			_damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
			_damageOnTouch.InvincibilityDuration = InvincibilityDuration;
			_damageOnTouch.HitDamageableFeedback = HitDamageableFeedback;
			_damageOnTouch.HitNonDamageableFeedback = HitNonDamageableFeedback;
			_damageOnTouch.TriggerFilter = TriggerFilter;
            
			if (!CanDamageOwner && (Owner != null))
			{
				_damageOnTouch.IgnoreGameObject(Owner.gameObject);    
			}
		}

		/// <summary>
		/// When the weapon is used, we trigger our attack routine
		/// </summary>
		public override void WeaponUse()
		{
			base.WeaponUse();
			StartCoroutine(MeleeWeaponAttack());
		}

		/// <summary>
		/// Triggers an attack, turning the damage area on and then off
		/// </summary>
		/// <returns>The weapon attack.</returns>
		protected virtual IEnumerator MeleeWeaponAttack()
		{
			if (_attackInProgress) { yield break; }

			_attackInProgress = true;
			yield return new WaitForSeconds(InitialDelay);
			EnableDamageArea();
			yield return new WaitForSeconds(ActiveDuration);
			DisableDamageArea();
			_attackInProgress = false;
		}

		/// <summary>
		/// Enables the damage area.
		/// </summary>
		protected virtual void EnableDamageArea()
		{
			if (_damageAreaCollider2D != null)
			{
				_damageAreaCollider2D.enabled = true;
			}
			if (_damageAreaCollider != null)
			{
				_damageAreaCollider.enabled = true;
			}
		}


		/// <summary>
		/// Disables the damage area.
		/// </summary>
		protected virtual void DisableDamageArea()
		{
			if (_damageAreaCollider2D != null)
			{
				_damageAreaCollider2D.enabled = false;
			}
			if (_damageAreaCollider != null)
			{
				_damageAreaCollider.enabled = false;
			}
		}

		/// <summary>
		/// When selected, we draw a bunch of gizmos
		/// </summary>
		protected virtual void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying)
			{
				DrawGizmos();
			}            
		}

		protected virtual void DrawGizmos()
		{
			if (MeleeDamageAreaMode == MeleeDamageAreaModes.Existing)
			{
				return;
			}
			
			if (DamageAreaShape == MeleeDamageAreaShapes.Box)
			{
				Gizmos.DrawWireCube(this.transform.position + AreaOffset, AreaSize);
			}

			if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
			{
				Gizmos.DrawWireSphere(this.transform.position + AreaOffset, AreaSize.x / 2);
			}

			if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
			{
				MMDebug.DrawGizmoRectangle(this.transform.position + AreaOffset, AreaSize, Color.red);
			}

			if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
			{
				Gizmos.DrawWireSphere(this.transform.position + AreaOffset, AreaSize.x / 2);
			}
		}

		/// <summary>
		/// On disable we set our flag to false
		/// </summary>
		protected virtual void OnDisable()
		{
			_attackInProgress = false;
		}
	}
}