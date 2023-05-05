using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used in the Koala demo to emit particles if a spawner is active and stop them otherwise
	/// </summary>
	public class DungeonPortal : TopDownMonoBehaviour
	{
		/// the particles to play while the portal is spawning
		[Tooltip("the particles to play while the portal is spawning")]
		public ParticleSystem SpawnParticles;

		protected TimedSpawner _timedSpawner;
        
		/// <summary>
		/// On Awake we grab our timed spawner
		/// </summary>
		protected virtual void Awake()
		{
			_timedSpawner = this.gameObject.GetComponent<TimedSpawner>();
		}

		/// <summary>
		/// On Update, we stop or play our particles if needed
		/// </summary>
		protected virtual void Update()
		{
			if ((!_timedSpawner.CanSpawn) && (SpawnParticles.isPlaying))
			{
				SpawnParticles.Stop();
			}
			if ((_timedSpawner.CanSpawn) && (!SpawnParticles.isPlaying))
			{
				SpawnParticles.Play();
			}
		}
	}
}