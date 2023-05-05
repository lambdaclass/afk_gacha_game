using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class handles the movement of a pathed projectile
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Automation/PathedProjectile")]
	public class PathedProjectile : TopDownMonoBehaviour
	{
		[MMInformation("A GameObject with this component will move towards its target and get destroyed when it reaches it. Here you can define what object to instantiate on impact. Use the Initialize method to set its destination and speed.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// The effect to instantiate when the object gets destroyed
		[Tooltip("The effect to instantiate when the object gets destroyed")]
		public GameObject DestroyEffect;
		/// the destination of the projectile
		[Tooltip("the destination of the projectile")]
		protected Transform _destination;
		/// the movement speed
		[Tooltip("the movement speed")]
		protected float _speed;

		/// <summary>
		/// Initializes the specified destination and speed.
		/// </summary>
		/// <param name="destination">Destination.</param>
		/// <param name="speed">Speed.</param>
		public virtual void Initialize(Transform destination, float speed)
		{
			_destination=destination;
			_speed=speed;
		}

		/// <summary>
		/// Every frame, me move the projectile's position to its destination
		/// </summary>
		protected virtual void Update () 
		{
			transform.position=Vector3.MoveTowards(transform.position,_destination.position,Time.deltaTime * _speed);
			var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
			if(distanceSquared > .01f * .01f)
				return;
			
			if (DestroyEffect!=null)
			{
				Instantiate(DestroyEffect,transform.position,transform.rotation); 
			}
			
			Destroy(gameObject);
		}	
	}
}