using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{	
	[CreateAssetMenu(fileName = "InventoryEngineHealth", menuName = "MoreMountains/TopDownEngine/InventoryEngineHealth", order = 1)]
	[Serializable]
	/// <summary>
	/// Pickable health item
	/// </summary>
	public class InventoryEngineHealth : InventoryItem 
	{
		[Header("Health")]
		[MMInformation("Here you need specify the amount of health gained when using this item.",MMInformationAttribute.InformationType.Info,false)]
		/// the amount of health to add to the player when the item is used
		[Tooltip("the amount of health to add to the player when the item is used")]
		public float HealthBonus;

		/// <summary>
		/// When the item is used, we try to grab our character's Health component, and if it exists, we add our health bonus amount of health
		/// </summary>
		public override bool Use(string playerID)
		{
			base.Use(playerID);

			if (TargetInventory(playerID).Owner == null)
			{
				return false;
			}

			Health characterHealth = TargetInventory(playerID).Owner.GetComponent<Health>();
			if (characterHealth != null)
			{
				characterHealth.ReceiveHealth(HealthBonus,TargetInventory(playerID).gameObject);
				return true;
			}
			else
			{
				return false;
			}
		}

	}
}