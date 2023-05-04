using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class will modify the sprite associated with it's sorting order based on the current rotation of the weapon.
	/// Useful to get the weapon get in front or behind your character based on this angle on 2D weapons
	/// </summary>
	[RequireComponent(typeof(WeaponAim2D))]
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Sprite Sorting Order Threshold")]
	public class WeaponSpriteSortingOrderThreshold : TopDownMonoBehaviour
	{
		/// the angle threshold at which to switch the sorting order
		[Tooltip("the angle threshold at which to switch the sorting order")]
		public float Threshold = 0f;
		/// the sorting order to apply when the weapon's rotation is below threshold
		[Tooltip("the sorting order to apply when the weapon's rotation is below threshold")]
		public int BelowThresholdSortingOrder = 1;
		/// the sorting order to apply when the weapon's rotation is above threshold
		[Tooltip("the sorting order to apply when the weapon's rotation is above threshold")]
		public int AboveThresholdSortingOrder = -1;
		/// the sprite whose sorting order we want to modify
		[Tooltip("the sprite whose sorting order we want to modify")]
		public SpriteRenderer Sprite;

		protected WeaponAim2D _weaponAim2D;
        
		/// <summary>
		/// On Awake we grab our weapon aim component
		/// </summary>
		protected virtual void Awake()
		{
			_weaponAim2D = this.gameObject.GetComponent<WeaponAim2D>();
		}

		/// <summary>
		/// On update we change our sorting order based on current weapon angle
		/// </summary>
		protected virtual void Update()
		{
			if ((_weaponAim2D == null) || (Sprite == null)) 
			{
				return;
			}

			Sprite.sortingOrder = (_weaponAim2D.CurrentAngleRelative > Threshold) ? AboveThresholdSortingOrder : BelowThresholdSortingOrder;
		}
	}
}