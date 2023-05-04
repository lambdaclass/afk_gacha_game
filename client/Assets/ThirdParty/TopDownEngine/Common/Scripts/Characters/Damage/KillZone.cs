using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this component to an object and it will cause damage to objects that collide with it. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Damage/KillZone")]
	public class KillZone : TopDownMonoBehaviour
	{
		[Header("Targets")]
		[MMInformation("This component will make your object kill objects that collide with it. Here you can define what layers will be killed.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
		// the layers containing the objects that will be damaged by this object
		[Tooltip("the layers containing the objects that will be damaged by this object")]
		public LayerMask TargetLayerMask = LayerManager.PlayerLayerMask;

		protected Health _colliderHealth;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Awake()
		{

		}

		/// <summary>
		/// OnEnable we set the start time to the current timestamp
		/// </summary>
		protected virtual void OnEnable()
		{

		}
        
		/// <summary>
		/// When a collision with the player is triggered, we give damage to the player and knock it back
		/// </summary>
		/// <param name="collider">what's colliding with the object.</param>
		public virtual void OnTriggerStay2D(Collider2D collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// when something enters our zone, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerEnter2D(Collider2D collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// when something stays in the zone, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerStay(Collider collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// When something enters our zone, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerEnter(Collider collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// When colliding, we kill our collider if it's a Health equipped object
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void Colliding(GameObject collider)
		{
			if (!this.isActiveAndEnabled)
			{
				return;
			}

			// if what we're colliding with isn't part of the target layers, we do nothing and exit
			if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
			{
				return;
			}

			_colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

			// if what we're colliding with is damageable
			if (_colliderHealth != null)
			{
				if (_colliderHealth.CurrentHealth > 0)
				{
					_colliderHealth.Kill();
				}                
			}
		}
	}
}