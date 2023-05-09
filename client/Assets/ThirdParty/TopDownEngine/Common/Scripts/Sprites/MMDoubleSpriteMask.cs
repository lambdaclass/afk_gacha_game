using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
	/// <summary>
	/// This setup uses two sprite masks, bound in the inspector, to enable one and then disable the other to mask specific parts of a level
	/// </summary>
	public class MMDoubleSpriteMask : MonoBehaviour, MMEventListener<MMSpriteMaskEvent>
	{
		[Header("Masks")]

		/// the first sprite mask
		[Tooltip("the first sprite mask")]
		public MMSpriteMask Mask1;
		/// the second sprite mask
		[Tooltip("the second sprite mask")]
		public MMSpriteMask Mask2;

		protected MMSpriteMask _currentMask;
		protected MMSpriteMask _dormantMask;

		/// <summary>
		/// On awake we initialize our masks
		/// </summary>
		protected virtual void Awake()
		{
			Mask1.gameObject.SetActive(true);
			Mask2.gameObject.SetActive(false);
			_currentMask = Mask1;
			_dormantMask = Mask2;
		}

		/// <summary>
		/// Sets new values for current and dormant masks
		/// </summary>
		protected virtual void SwitchCurrentMask()
		{
			_currentMask = (_currentMask == Mask1) ? Mask2 : Mask1;
			_dormantMask = (_currentMask == Mask1) ? Mask2 : Mask1;
		}

		/// <summary>
		/// A coroutine designed to mask the first mask after having activated and moved the dormant one to the new position
		/// </summary>
		/// <param name="spriteMaskEvent"></param>
		/// <returns></returns>
		protected virtual IEnumerator DoubleMaskCo(MMSpriteMaskEvent spriteMaskEvent)
		{
			_dormantMask.transform.position = spriteMaskEvent.NewPosition;
			_dormantMask.transform.localScale = spriteMaskEvent.NewSize * _dormantMask.ScaleMultiplier;
			_dormantMask.gameObject.SetActive(true);
			yield return new WaitForSeconds(spriteMaskEvent.Duration);
			_currentMask.gameObject.SetActive(false);
			SwitchCurrentMask();
		}

		/// <summary>
		/// When we catch a double mask event, we handle it
		/// </summary>
		/// <param name="spriteMaskEvent"></param>
		public virtual void OnMMEvent(MMSpriteMaskEvent spriteMaskEvent)
		{
			switch (spriteMaskEvent.EventType)
			{
				case MMSpriteMaskEvent.MMSpriteMaskEventTypes.DoubleMask:
					StartCoroutine(DoubleMaskCo(spriteMaskEvent));
					break;
			}
		}

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMSpriteMaskEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMSpriteMaskEvent>();
		}
	}
}