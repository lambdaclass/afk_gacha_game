using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An item picker that instantiates an effect and plays a sound on pick
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/InventoryPickableItem")]
	public class InventoryPickableItem : ItemPicker 
	{
		/// The effect to instantiate when the coin is hit
		[Tooltip("The effect to instantiate when the coin is hit")]
		public GameObject Effect;
		/// The sound effect to play when the object gets picked
		[Tooltip("The sound effect to play when the object gets picked")]
		public AudioClip PickSfx;

		protected override void PickSuccess()
		{
			base.PickSuccess ();
			Effects ();
		}

		/// <summary>
		/// Triggers the various pick effects
		/// </summary>
		protected virtual void Effects()
		{
			if (!Application.isPlaying)
			{
				return;
			}				
			else
			{
				if (PickSfx!=null) 
				{	
					MMSoundManagerSoundPlayEvent.Trigger(PickSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);
				}

				if (Effect != null)
				{
					// adds an instance of the effect at the coin's position
					Instantiate(Effect, transform.position, transform.rotation);				
				}	
			}
		}
	}
}