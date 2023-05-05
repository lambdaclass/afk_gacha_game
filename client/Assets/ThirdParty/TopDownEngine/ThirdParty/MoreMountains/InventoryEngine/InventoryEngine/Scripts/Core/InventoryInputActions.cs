using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
	/// <summary>
	/// This class lets you bind keys to specific slots in a target inventory, and associate an action to execute when that key is pressed.
	/// A typical use case would be a weapon bar, where pressing 1 equips a gun, pressing 2 equips a shotgun, etc.
	/// Coincidentally, that's what the PixelRogueWeaponBar demo scene demonstrates.
	/// </summary>
	public class InventoryInputActions : MonoBehaviour
	{
		/// <summary>
		/// A class used to store slot / key / action bindings 
		/// </summary>
		[System.Serializable]
		public class InventoryInputActionsBindings
		{
			/// the slot in the target inventory to bind an action to 
			public int SlotIndex = 0;
			/// the key that should trigger the action
			public KeyCode InputBinding = KeyCode.Alpha0;
			/// an alt key that will also trigger the action
			public KeyCode AltInputBinding = KeyCode.None;
			/// the action to trigger when pressing the input
			public InventoryInputActions.Actions Action = InventoryInputActions.Actions.Equip;
			/// whether or not this action should be triggered
			public bool Active = true;
		}
        
		/// <summary>
		/// The possible actions that can be caused when activating input
		/// </summary>
		public enum Actions
		{
			Equip,
			Use,
			Drop,
			Unequip
		}
        
		/// the name of the inventory to pilot with these bindings
		public string TargetInventoryName = "MainInventory";
		/// the unique ID of the Player associated to this component
		public string PlayerID = "Player1";
		/// a list of bindings to go through when looking for input
		public List<InventoryInputActionsBindings> InputBindings;

		protected Inventory _targetInventory = null;

		/// <summary>
		/// Returns the target inventory of this component
		/// </summary>
		public Inventory TargetInventory
		{
			get
			{
				if (TargetInventoryName == null)
				{
					return null;
				}

				if (_targetInventory == null)
				{
					_targetInventory = Inventory.FindInventory(TargetInventoryName, PlayerID);
				}

				return _targetInventory;
			}
		}

		/// <summary>
		/// On Start we initialize our inventory reference
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// Makes sure we have a target inventory
		/// </summary>
		protected virtual void Initialization()
		{
			if (TargetInventoryName == "")
			{
				Debug.LogError("The " + this.name +
				               " Inventory Input Actions component doesn't have a TargetInventoryName set. You need to set one from its inspector, matching an Inventory's name.");
				return;
			}

			if (TargetInventory == null)
			{
				Debug.LogError("The " + this.name +
				               " Inventory Input Actions component couldn't find a TargetInventory. You either need to create an inventory with a matching inventory name (" +
				               TargetInventoryName + "), or set that TargetInventoryName to one that exists.");
				return;
			}
		}

		/// <summary>
		/// On Update we look for input
		/// </summary>
		protected virtual void Update()
		{
			DetectInput();
		}

		/// <summary>
		/// Every frame we look for input for each of our bindings
		/// </summary>
		protected virtual void DetectInput()
		{
			foreach (InventoryInputActionsBindings binding in InputBindings)
			{
				if (binding == null)
				{
					continue;
				}
				if (!binding.Active)
				{
					continue;
				}
				if (Input.GetKeyDown(binding.InputBinding) || Input.GetKeyDown(binding.AltInputBinding))
				{
					ExecuteAction(binding);
				}
			}
		}

		/// <summary>
		/// Executes the corresponding action for the specified binding
		/// </summary>
		/// <param name="binding"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		protected virtual void ExecuteAction(InventoryInputActionsBindings binding)
		{
			if (binding.SlotIndex > _targetInventory.Content.Length)
			{
				return;
			}
			if (_targetInventory.Content[binding.SlotIndex] == null)
			{
				return;
			}

			switch (binding.Action)
			{
				case Actions.Equip:
					MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, _targetInventory.name, _targetInventory.Content[binding.SlotIndex], 0, binding.SlotIndex, _targetInventory.PlayerID);
					break;
				case Actions.Use:
					MMInventoryEvent.Trigger(MMInventoryEventType.UseRequest, null, _targetInventory.name, _targetInventory.Content[binding.SlotIndex], 0, binding.SlotIndex, _targetInventory.PlayerID);
					break;
				case Actions.Drop:
					MMInventoryEvent.Trigger(MMInventoryEventType.Drop, null, _targetInventory.name, _targetInventory.Content[binding.SlotIndex], 0, binding.SlotIndex, _targetInventory.PlayerID);
					break;
				case Actions.Unequip:
					MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null, _targetInventory.name, _targetInventory.Content[binding.SlotIndex], 0, binding.SlotIndex, _targetInventory.PlayerID);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}