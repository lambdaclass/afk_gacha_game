using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Coin manager
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/ExplodudesBombPicker")]
	public class ExplodudesBombPicker : PickableItem
	{
		[Header("Explodudes Bomb Picker")]
		/// The amount of points to add when collected
		[Tooltip("The amount of points to add when collected")]
		public int BombsToAdd = 1;

		protected CharacterHandleWeapon _characterHandleWeapon;
		protected ExplodudesWeapon _explodudesWeapon;

		/// <summary>
		/// Triggered when something collides with the coin
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker) 
		{
			_characterHandleWeapon = picker.MMGetComponentNoAlloc<CharacterHandleWeapon>();
			if (_characterHandleWeapon == null)
			{
				return;
			}
			_explodudesWeapon = _characterHandleWeapon.CurrentWeapon.GetComponent<ExplodudesWeapon>();
			if (_explodudesWeapon == null)
			{
				return;
			}
			_explodudesWeapon.MaximumAmountOfBombsAtOnce += BombsToAdd;
			_explodudesWeapon.RemainingBombs += BombsToAdd;
		}
	}
}