using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Spawns pathed projectiles
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Automation/PathedProjectileSpawner")]
	public class PathedProjectileSpawner : TopDownMonoBehaviour 
	{
		[MMInformation("A GameObject with this component will spawn projectiles at the specified fire rate.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// the pathed projectile's destination
		[Tooltip("the pathed projectile's destination")]
		public Transform Destination;
		/// the projectiles to spawn
		[Tooltip("the projectiles to spawn")]
		public PathedProjectile Projectile;
		/// the effect to instantiate at each spawn
		[Tooltip("the effect to instantiate at each spawn")]
		public GameObject SpawnEffect;
		/// the speed of the projectiles
		[Tooltip("the speed of the projectiles")]
		public float Speed;
		/// the frequency of the spawns
		[Tooltip("the frequency of the spawns")]
		public float FireRate;
		
		protected float _nextShotInSeconds;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Start () 
		{
			_nextShotInSeconds=FireRate;
		}

		/// <summary>
		/// Every frame, we check if we need to instantiate a new projectile
		/// </summary>
		protected virtual void Update () 
		{
			if((_nextShotInSeconds -= Time.deltaTime)>0)
				return;
				
			_nextShotInSeconds = FireRate;
			var projectile = (PathedProjectile) Instantiate(Projectile, transform.position,transform.rotation);
			projectile.Initialize(Destination,Speed);
			
			if (SpawnEffect!=null)
			{
				Instantiate(SpawnEffect,transform.position,transform.rotation);
			}
		}

		/// <summary>
		/// Debug mode
		/// </summary>
		public virtual void OnDrawGizmos()
		{
			if (Destination==null)
				return;
			
			Gizmos.color=Color.gray;
			Gizmos.DrawLine(transform.position,Destination.position);
		}
	}
}