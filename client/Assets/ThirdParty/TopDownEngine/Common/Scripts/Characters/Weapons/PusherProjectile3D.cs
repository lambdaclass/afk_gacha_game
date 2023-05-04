using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this component to a projectile (in 3D) and it'll be able to push stuff (opening doors for example)
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Pusher Projectile 3D")]
	public class PusherProjectile3D : TopDownMonoBehaviour
	{
		/// the amount of force to apply when colliding
		[Tooltip("the amount of force to apply when colliding")]
		public float PushPower = 10f;
		/// an offset to apply on the projectile's forward to account for super high speeds. This will affect the position the force is applied at. Usually 0 will be fine
		[Tooltip("an offset to apply on the projectile's forward to account for super high speeds. This will affect the position the force is applied at. Usually 0 will be fine")]
		public float PositionOffset = 0f;

		protected Rigidbody _pushedRigidbody;
		protected Projectile _projectile;
		protected Vector3 _pushDirection;

		/// <summary>
		/// On Awake we grab our projectile component
		/// </summary>
		protected virtual void Awake()
		{
			_projectile = this.gameObject.GetComponent<Projectile>();
		}

		/// <summary>
		/// When our projectile collides with something, if it has a rigidbody, we apply the specified force to it
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter(Collider collider)
		{
			_pushedRigidbody = collider.attachedRigidbody;

			if ((_pushedRigidbody == null) || (_pushedRigidbody.isKinematic))
			{
				return;
			}

			_pushDirection.x = _projectile.Direction.x;
			_pushDirection.y = 0;
			_pushDirection.z = _projectile.Direction.z;

			_pushedRigidbody.AddForceAtPosition(_pushDirection.normalized * PushPower, this.transform.position + this.transform.forward * PositionOffset);
		}		
	}
}