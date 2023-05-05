using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
	[System.Serializable]
	public struct AutoPickItem
	{
		public InventoryItem Item;
		public int Quantity;
	}

	/// <summary>
	/// Add this component to a character and it'll be able to control an inventory
	/// Animator parameters : none
	/// </summary>
	[MMHiddenProperties("AbilityStopFeedbacks")]
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Inventory")] 
	public class CharacterInventory : CharacterAbility, MMEventListener<MMInventoryEvent>
	{
		public enum WeaponRotationModes { Normal, AddEmptySlot, AddInitialWeapon }
        
		[Header("Inventories")]
		/// the unique ID of this player as far as the InventoryEngine is concerned. This has to match all its Inventory and InventoryEngine UI components' PlayerID for that player. If you're not going for multiplayer here, just leave Player1.
		[Tooltip("the unique ID of this player as far as the InventoryEngine is concerned. This has to match all its Inventory and InventoryEngine UI components' PlayerID for that player. If you're not going for multiplayer here, just leave Player1.")]
		public string PlayerID = "Player1";
		/// the name of the main inventory for this character
		[Tooltip("the name of the main inventory for this character")]
		public string MainInventoryName;
		/// the name of the inventory where this character stores weapons
		[Tooltip("the name of the inventory where this character stores weapons")]
		public string WeaponInventoryName;
		/// the name of the hotbar inventory for this character
		[Tooltip("the name of the hotbar inventory for this character")]
		public string HotbarInventoryName;
		/// a transform to pass to the inventories, will be passed to the inventories and used as reference for drops. If left empty, this.transform will be used.
		[Tooltip("a transform to pass to the inventories, will be passed to the inventories and used as reference for drops. If left empty, this.transform will be used.")]
		public Transform InventoryTransform;

		[Header("Weapon Rotation")]
		/// the rotation mode for weapons : Normal will cycle through all weapons, AddEmptySlot will return to empty hands, AddOriginalWeapon will cycle back to the original weapon
		[Tooltip("if this is true, will add an empty slot to the weapon rotation")]
		public WeaponRotationModes WeaponRotationMode = WeaponRotationModes.Normal;

		[Header("Auto Pick")]
		/// a list of items to automatically add to this Character's inventories on start
		[Tooltip("a list of items to automatically add to this Character's inventories on start")]
		public AutoPickItem[] AutoPickItems;
		/// if this is true, auto pick items will only be added if the main inventory is empty
		[Tooltip("if this is true, auto pick items will only be added if the main inventory is empty")]
		public bool AutoPickOnlyIfMainInventoryIsEmpty;
		
		[Header("Auto Equip")]
		/// a weapon to auto equip on start
		[Tooltip("a weapon to auto equip on start")]
		public InventoryWeapon AutoEquipWeaponOnStart;
		/// if this is true, auto equip will only occur if the main inventory is empty
		[Tooltip("if this is true, auto equip will only occur if the main inventory is empty")]
		public bool AutoEquipOnlyIfMainInventoryIsEmpty;
		/// the target handle weapon ability - if left empty, will pick the first one it finds
		[Tooltip("the target handle weapon ability - if left empty, will pick the first one it finds")]
		public CharacterHandleWeapon CharacterHandleWeapon;

		public Inventory MainInventory { get; set; }
		public Inventory WeaponInventory { get; set; }
		public Inventory HotbarInventory { get; set; }
		public List<string> AvailableWeaponsIDs => _availableWeaponsIDs;

		protected List<int> _availableWeapons;
		protected List<string> _availableWeaponsIDs;
		protected string _nextWeaponID;
		protected bool _nextFrameWeapon = false;
		protected string _nextFrameWeaponName;
		protected const string _emptySlotWeaponName = "_EmptySlotWeaponName";
		protected const string _initialSlotWeaponName = "_InitialSlotWeaponName";
		protected bool _initialized = false;

		/// <summary>
		/// On init we setup our ability
		/// </summary>
		protected override void Initialization () 
		{
			base.Initialization();
			Setup ();
		}

		/// <summary>
		/// Grabs all inventories, and fills weapon lists
		/// </summary>
		protected virtual void Setup()
		{
			if (InventoryTransform == null)
			{
				InventoryTransform = this.transform;
			}
			GrabInventories ();
			if (CharacterHandleWeapon == null)
			{
				CharacterHandleWeapon = _character?.FindAbility<CharacterHandleWeapon> ();	
			}
			FillAvailableWeaponsLists ();

			if (_initialized)
			{
				return;
			}

			bool mainInventoryEmpty = true;
			if (MainInventory != null)
			{
				mainInventoryEmpty = MainInventory.NumberOfFilledSlots == 0;
			}
			bool canAutoPick = !(AutoPickOnlyIfMainInventoryIsEmpty && !mainInventoryEmpty);
			bool canAutoEquip = !(AutoEquipOnlyIfMainInventoryIsEmpty && !mainInventoryEmpty);
			
			// we auto pick items if needed
			if ((AutoPickItems.Length > 0) && !_initialized && canAutoPick)
			{
				foreach (AutoPickItem item in AutoPickItems)
				{
					MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, item.Item.TargetInventoryName, item.Item, item.Quantity, 0, PlayerID);
				}
			}

			// we auto equip a weapon if needed
			if ((AutoEquipWeaponOnStart != null) && !_initialized && canAutoEquip)
			{
				MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, AutoEquipWeaponOnStart.TargetInventoryName, AutoEquipWeaponOnStart, 1, 0, PlayerID);
				EquipWeapon(AutoEquipWeaponOnStart.ItemID);
			}
			_initialized = true;
		}

		public override void ProcessAbility()
		{
			base.ProcessAbility();
            
			if (_nextFrameWeapon)
			{
				EquipWeapon(_nextFrameWeaponName);
				_nextFrameWeapon = false;
			}
		}

		/// <summary>
		/// Grabs any inventory it can find that matches the names set in the inspector
		/// </summary>
		protected virtual void GrabInventories()
		{
			Inventory[] inventories = FindObjectsOfType<Inventory>();
			foreach (Inventory inventory in inventories)
			{
				if (inventory.PlayerID != PlayerID)
				{
					continue;
				}
				if ((MainInventory == null) && (inventory.name == MainInventoryName))
				{
					MainInventory = inventory;
				}
				if ((WeaponInventory == null) && (inventory.name == WeaponInventoryName))
				{
					WeaponInventory = inventory;
				}
				if ((HotbarInventory == null) && (inventory.name == HotbarInventoryName))
				{
					HotbarInventory = inventory;
				}
			}
			if (MainInventory != null) { MainInventory.SetOwner (this.gameObject); MainInventory.TargetTransform = InventoryTransform;}
			if (WeaponInventory != null) { WeaponInventory.SetOwner (this.gameObject); WeaponInventory.TargetTransform = InventoryTransform;}
			if (HotbarInventory != null) { HotbarInventory.SetOwner (this.gameObject); HotbarInventory.TargetTransform = InventoryTransform;}
		}

		/// <summary>
		/// On handle input, we watch for the switch weapon button, and switch weapon if needed
		/// </summary>
		protected override void HandleInput()
		{
			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}
			if (_inputManager.SwitchWeaponButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				SwitchWeapon ();
			}
		}

		/// <summary>
		/// Fills the weapon list. The weapon list will be used to determine what weapon we can switch to
		/// </summary>
		protected virtual void FillAvailableWeaponsLists()
		{
			_availableWeaponsIDs = new List<string> ();
			if ((CharacterHandleWeapon == null) || (WeaponInventory == null))
			{
				return;
			}
			_availableWeapons = MainInventory.InventoryContains (ItemClasses.Weapon);
			foreach (int index in _availableWeapons)
			{
				_availableWeaponsIDs.Add (MainInventory.Content [index].ItemID);
			}
			if (!InventoryItem.IsNull(WeaponInventory.Content[0]))
			{
				_availableWeaponsIDs.Add (WeaponInventory.Content [0].ItemID);
			}

			_availableWeaponsIDs.Sort ();
		}

		/// <summary>
		/// Determines the name of the next weapon in line
		/// </summary>
		protected virtual void DetermineNextWeaponName ()
		{
			if (InventoryItem.IsNull(WeaponInventory.Content[0]))
			{
				_nextWeaponID = _availableWeaponsIDs [0];
				return;
			}

			if ((_nextWeaponID == _emptySlotWeaponName) || (_nextWeaponID == _initialSlotWeaponName))
			{
				_nextWeaponID = _availableWeaponsIDs[0];
				return;
			}

			for (int i = 0; i < _availableWeaponsIDs.Count; i++)
			{
				if (_availableWeaponsIDs[i] == WeaponInventory.Content[0].ItemID)
				{
					if (i == _availableWeaponsIDs.Count - 1)
					{
						switch (WeaponRotationMode)
						{
							case WeaponRotationModes.AddEmptySlot:
								_nextWeaponID = _emptySlotWeaponName;
								return;
							case WeaponRotationModes.AddInitialWeapon:
								_nextWeaponID = _initialSlotWeaponName;
								return;
						}

						_nextWeaponID = _availableWeaponsIDs [0];
					}
					else
					{
						_nextWeaponID = _availableWeaponsIDs [i+1];
					}
				}
			}
		}

		/// <summary>
		/// Equips the weapon with the name passed in parameters
		/// </summary>
		/// <param name="weaponID"></param>
		public virtual void EquipWeapon(string weaponID)
		{
			if ((weaponID == _emptySlotWeaponName) && (CharacterHandleWeapon != null))
			{
				MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null, WeaponInventoryName, WeaponInventory.Content[0], 0, 0, PlayerID);
				CharacterHandleWeapon.ChangeWeapon(null, _emptySlotWeaponName, false);
				MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, WeaponInventory.name, null, 0, 0, PlayerID);
				return;
			}

			if ((weaponID == _initialSlotWeaponName) && (CharacterHandleWeapon != null))
			{
				MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null, WeaponInventoryName, WeaponInventory.Content[0], 0, 0, PlayerID);
				CharacterHandleWeapon.ChangeWeapon(CharacterHandleWeapon.InitialWeapon, _initialSlotWeaponName, false);
				MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, WeaponInventory.name, null, 0, 0, PlayerID);
				return;
			}
			
			for (int i = 0; i < MainInventory.Content.Length ; i++)
			{
				if (InventoryItem.IsNull(MainInventory.Content[i]))
				{
					continue;
				}
				if (MainInventory.Content[i].ItemID == weaponID)
				{
					MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, MainInventory.name, MainInventory.Content[i], 0, i, PlayerID);
					break;
				}
			}
		}

		/// <summary>
		/// Switches to the next weapon in line
		/// </summary>
		protected virtual void SwitchWeapon()
		{
			// if there's no character handle weapon component, we can't switch weapon, we do nothing and exit
			if ((CharacterHandleWeapon == null) || (WeaponInventory == null))
			{
				return;
			}

			FillAvailableWeaponsLists ();

			// if we only have 0 or 1 weapon, there's nothing to switch, we do nothing and exit
			if (_availableWeaponsIDs.Count <= 0)
			{
				return;
			}

			DetermineNextWeaponName ();
			EquipWeapon (_nextWeaponID);
			PlayAbilityStartFeedbacks();
			PlayAbilityStartSfx();
		}

		/// <summary>
		/// Watches for InventoryLoaded events
		/// When an inventory gets loaded, if it's our WeaponInventory, we check if there's already a weapon equipped, and if yes, we equip it
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryLoaded)
			{
				if (inventoryEvent.TargetInventoryName == WeaponInventoryName)
				{
					this.Setup ();
					if (WeaponInventory != null)
					{
						if (!InventoryItem.IsNull (WeaponInventory.Content [0]))
						{
							CharacterHandleWeapon.Setup ();
							WeaponInventory.Content [0].Equip (PlayerID);
						}
					}
				}
			}
			if (inventoryEvent.InventoryEventType == MMInventoryEventType.Pick)
			{
				bool isSubclass = (inventoryEvent.EventItem.GetType().IsSubclassOf(typeof(InventoryWeapon)));
				bool isClass = (inventoryEvent.EventItem.GetType() == typeof(InventoryWeapon));
				if (isClass || isSubclass)
				{
					InventoryWeapon inventoryWeapon = (InventoryWeapon)inventoryEvent.EventItem;
					switch (inventoryWeapon.AutoEquipMode)
					{
						case InventoryWeapon.AutoEquipModes.NoAutoEquip:
							// we do nothing
							break;

						case InventoryWeapon.AutoEquipModes.AutoEquip:
							_nextFrameWeapon = true;
							_nextFrameWeaponName = inventoryEvent.EventItem.ItemID;
							break;

						case InventoryWeapon.AutoEquipModes.AutoEquipIfEmptyHanded:
							if (CharacterHandleWeapon.CurrentWeapon == null)
							{
								_nextFrameWeapon = true;
								_nextFrameWeaponName = inventoryEvent.EventItem.ItemID;
							}
							break;
					}
				}
			}
		}

		protected override void OnDeath()
		{
			base.OnDeath();
			if (WeaponInventory != null)
			{
				MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null, WeaponInventoryName, WeaponInventory.Content[0], 0, 0, PlayerID);
			}            
		}

		/// <summary>
		/// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			this.MMEventStartListening<MMInventoryEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();
			this.MMEventStopListening<MMInventoryEvent>();
		}
	}
}