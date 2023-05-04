using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class is used in the Deadline demos to mark collectibles and disable them if they've been collected in a past visit of the level
	/// </summary>
	public class DeadlineCollectible : TopDownMonoBehaviour
	{
		public string CollectibleName = "";
		
		/// <summary>
		/// On Start we disable our game object if needed
		/// </summary>
		protected virtual void Start()
		{
			DisableIfAlreadyCollected ();
		}

		/// <summary>
		/// Call this to collect this collectible and keep track of it in the future
		/// </summary>
		public virtual void Collect()
		{
			DeadlineProgressManager.Instance.FindCollectible (CollectibleName);
		}

		/// <summary>
		/// Disables the game object if it's already been collected in the past.
		/// </summary>
		protected virtual void DisableIfAlreadyCollected ()
		{
			if (DeadlineProgressManager.Instance.FoundCollectibles == null)
			{
				return;
			}
			foreach (string collectible in DeadlineProgressManager.Instance.FoundCollectibles)
			{
				if (collectible == this.CollectibleName)
				{
					Disable ();
				}
			}
		}

		/// <summary>
		/// Disable this game object.
		/// </summary>
		protected virtual void Disable()
		{
			this.gameObject.SetActive (false);
		}
	    
	}
}
