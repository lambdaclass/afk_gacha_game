using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Use this class for physics based projectiles (meant to be thrown by a ProjectileWeapon)
	/// </summary>
	public class PhysicsProjectile : Projectile
	{
		[Header("Physics")] 
		public float InitialForce = 10f;
		public Vector3 InitialRotation = Vector3.zero;

		public ForceMode InitialForceMode = ForceMode.Impulse;
		public ForceMode2D InitialForceMode2D = ForceMode2D.Impulse;

		public override void Movement()
		{
			//do nothing
		}
		
		public override void SetDirection(Vector3 newDirection, Quaternion newRotation, bool spawnerIsFacingRight = true)
		{
			base.SetDirection(newDirection, newRotation, spawnerIsFacingRight);
			
			this.transform.Rotate(InitialRotation, Space.Self);

			newDirection = this.transform.forward;
			
			if (_rigidBody != null)
			{
				_rigidBody.AddForce(newDirection * InitialForce, InitialForceMode);	
			}
			if (_rigidBody2D != null)
			{
				_rigidBody2D.AddForce(newDirection * InitialForce, InitialForceMode2D);
			}
		}
	}	
}