using UnityEngine;
using MoreMountains.Tools;
using System;

namespace MoreMountains.InventoryEngine
{
	[Serializable]
	public class InventoryItemDisplayProperties
	{
		public bool DisplayEquipUseButton = true;
		public bool DisplayMoveButton = true;
		public bool DisplayDropButton = true;
		public bool DisplayEquipButton = true;
		public bool DisplayUseButton = true;
		public bool DisplayUnequipButton = true;
	}
	
	[Serializable]
	/// <summary>
	/// Base class for inventory items, meant to be extended.
	/// Will handle base properties and drop spawn
	/// </summary>
	public class InventoryItem : ScriptableObject 
	{
		[Header("ID and Target")]
		/// the (unique) ID of the item
		[Tooltip("the (unique) ID of the item")]
		public string ItemID;
		/// the inventory name into which this item will be stored
		[Tooltip("the inventory name into which this item will be stored")]
		public string TargetInventoryName = "MainInventory";
		/// if this is true, the item won't be added anywhere's there's room in the inventory, but instead at the specified TargetIndex slot
		[Tooltip("if this is true, the item won't be added anywhere's there's room in the inventory, but instead at the specified TargetIndex slot")]
		public bool ForceSlotIndex = false;
		/// if ForceSlotIndex is true, this is the index at which the item will be added in the target inventory
		[Tooltip("if ForceSlotIndex is true, this is the index at which the item will be added in the target inventory")]
		[MMCondition("ForceSlotIndex", true)]
		public int TargetIndex = 0;

		[Header("Permissions")]
		/// whether or not this item can be "used" (via the Use method) - important, this is only the INITIAL state of this object, IsUsable is to be used anytime after that
		[Tooltip("whether or not this item can be 'used' (via the Use method) - important, this is only the INITIAL state of this object, IsUsable is to be used anytime after that")]
		public bool Usable = false;
		/// if this is true, calling Use on that object will consume one unit of it
		[Tooltip("if this is true, calling Use on that object will consume one unit of it")]
		[MMCondition("Usable", true)] 
		public bool Consumable = true;
		/// if this item is consumable, determines how many will be consumed per use (usually one)
		[Tooltip("if this item is consumable, determines how many will be consumed per use (usually one)")]
		[MMCondition("Consumable", true)] 
		public int ConsumeQuantity = 1;
		/// whether or not this item can be equipped - important, this is only the INITIAL state of this object, IsEquippable is to be used anytime after that
		[Tooltip("whether or not this item can be equipped - important, this is only the INITIAL state of this object, IsEquippable is to be used anytime after that")]
		public bool Equippable = false;
		/// whether or not this item can be equipped if its target inventory is full
		[Tooltip("whether or not this item can be equipped if its target inventory is full")]
		[MMCondition("Equippable", true)]
		public bool EquippableIfInventoryIsFull = true;
		/// if this is true, this item will be removed from its original inventory when equipped, and moved to its EquipmentInventory
		[Tooltip("if this is true, this item will be removed from its original inventory when equipped, and moved to its EquipmentInventory")]
		[MMCondition("Equippable", true)]
		public bool MoveWhenEquipped = true;
		
		/// if this is true, this item can be dropped
		[Tooltip("if this is true, this item can be dropped")]
		public bool Droppable = true;
		/// if this is true, objects can be moved
		[Tooltip("if this is true, objects can be moved")]
		public bool CanMoveObject=true;
		/// if this is true, objects can be swapped with another object
		[Tooltip("if this is true, objects can be swapped with another object")]
		public bool CanSwapObject=true;
		/// a set of properties defining whether or not to show inventory action buttons when that item is selected 
		[Tooltip("a set of properties defining whether or not to show inventory action buttons when that item is selected")]
		public InventoryItemDisplayProperties DisplayProperties;
		
		/// whether or not this object can be used
		public virtual bool IsUsable {  get { return Usable;  } }
		/// whether or not this object can be equipped
		public virtual bool IsEquippable { get { return Equippable; } }

		[HideInInspector]
		/// the base quantity of this item
		public int Quantity = 1;

		[Header("Basic info")]
		/// the name of the item - will be displayed in the details panel
		[Tooltip("the name of the item - will be displayed in the details panel")]
		public string ItemName;
		/// the item's short description to display in the details panel
		[TextArea]
		[Tooltip("the item's short description to display in the details panel")]
		public string ShortDescription;
		[TextArea]
		/// the item's long description to display in the details panel
		[Tooltip("the item's long description to display in the details panel")]
		public string Description;

		[Header("Image")]
		/// the icon that will be shown on the inventory's slot
		[Tooltip("the icon that will be shown on the inventory's slot")]
		public Sprite Icon;

		[Header("Prefab Drop")]
		/// the prefab to instantiate when the item is dropped
		[Tooltip("the prefab to instantiate when the item is dropped")]
		public GameObject Prefab;
		/// if this is true, the quantity of the object will be forced to PrefabDropQuantity when dropped
		[Tooltip("if this is true, the quantity of the object will be forced to PrefabDropQuantity when dropped")]
		public bool ForcePrefabDropQuantity = false;
		/// the quantity to force on the spawned item if ForcePrefabDropQuantity is true
		[Tooltip("the quantity to force on the spawned item if ForcePrefabDropQuantity is true")]
		[MMCondition("ForcePrefabDropQuantity", true)]
		public int PrefabDropQuantity = 1;
		/// the minimal distance at which the object should be spawned when dropped
		[Tooltip("the minimal distance at which the object should be spawned when dropped")]
		public MMSpawnAroundProperties DropProperties;

		[Header("Inventory Properties")]
		/// If this object can be stacked (multiple instances in a single inventory slot), you can specify here the maximum size of that stack.
		[Tooltip("If this object can be stacked (multiple instances in a single inventory slot), you can specify here the maximum size of that stack.")]
		public int MaximumStack = 1;
		/// the class of the item
		[Tooltip("the class of the item")]
		public ItemClasses ItemClass;

		[Header("Equippable")]
		/// If this item is equippable, you can set here its target inventory name (for example ArmorInventory). Of course you'll need an inventory with a matching name in your scene.
		[Tooltip("If this item is equippable, you can set here its target inventory name (for example ArmorInventory). Of course you'll need an inventory with a matching name in your scene.")]
		public string TargetEquipmentInventoryName;
		/// the sound the item should play when equipped (optional)
		[Tooltip("the sound the item should play when equipped (optional)")]
		public AudioClip EquippedSound;

		[Header("Usable")]
		/// If this item can be used, you can set here a sound to play when it gets used, if you don't a default sound will be played.
		[Tooltip("If this item can be used, you can set here a sound to play when it gets used, if you don't a default sound will be played.")]
		public AudioClip UsedSound;

		[Header("Sounds")]
		/// the sound the item should play when moved (optional)
		[Tooltip("the sound the item should play when moved (optional)")]
		public AudioClip MovedSound;
		/// the sound the item should play when dropped (optional)
		[Tooltip("the sound the item should play when dropped (optional)")]
		public AudioClip DroppedSound;
		/// if this is set to false, default sounds won't be used and no sound will be played
		[Tooltip("if this is set to false, default sounds won't be used and no sound will be played")]
		public bool UseDefaultSoundsIfNull = true;

		protected Inventory _targetInventory = null;
		protected Inventory _targetEquipmentInventory = null;

		/// <summary>
		/// Gets the target inventory.
		/// </summary>
		/// <value>The target inventory.</value>
		public virtual Inventory TargetInventory(string playerID)
		{ 
			if (TargetInventoryName == null)
			{
				return null;
			}
			_targetInventory = Inventory.FindInventory(TargetInventoryName, playerID);
			return _targetInventory;
		}

		/// <summary>
		/// Gets the target equipment inventory.
		/// </summary>
		/// <value>The target equipment inventory.</value>
		public virtual Inventory TargetEquipmentInventory(string playerID)
		{ 
			if (TargetEquipmentInventoryName == null)
			{
				return null;
			}
			_targetEquipmentInventory = Inventory.FindInventory(TargetEquipmentInventoryName, playerID);
			return _targetEquipmentInventory;
		}

		/// <summary>
		/// Determines if an item is null or not
		/// </summary>
		/// <returns><c>true</c> if is null the specified item; otherwise, <c>false</c>.</returns>
		/// <param name="item">Item.</param>
		public static bool IsNull(InventoryItem item)
		{
			if (item==null)
			{
				return true;
			}
			if (item.ItemID==null)
			{
				return true;
			}
			if (item.ItemID=="")
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Copies an item into a new one
		/// </summary>
		public virtual InventoryItem Copy()
		{
			string name = this.name;
			InventoryItem clone = UnityEngine.Object.Instantiate(this) as InventoryItem;
			clone.name = name;
			return clone;
		}

		/// <summary>
		/// Spawns the associated prefab
		/// </summary>
		public virtual void SpawnPrefab(string playerID)
		{
			if (TargetInventory(playerID) != null)
			{
				// if there's a prefab set for the item at this slot, we instantiate it at the specified offset
				if (Prefab!=null && TargetInventory(playerID).TargetTransform!=null)
				{
					GameObject droppedObject=(GameObject)Instantiate(Prefab);
					if (droppedObject.GetComponent<ItemPicker>()!=null)
					{
						if (ForcePrefabDropQuantity)
						{
							droppedObject.GetComponent<ItemPicker>().Quantity = PrefabDropQuantity;
							droppedObject.GetComponent<ItemPicker>().RemainingQuantity = PrefabDropQuantity;	
						}
					}

					MMSpawnAround.ApplySpawnAroundProperties(droppedObject, DropProperties,
						TargetInventory(playerID).TargetTransform.position);
				}
			}
		}

		/// <summary>
		/// What happens when the object is picked - override this to add your own behaviors
		/// </summary>
		public virtual bool Pick(string playerID) { return true; }

		/// <summary>
		/// What happens when the object is used - override this to add your own behaviors
		/// </summary>
		public virtual bool Use(string playerID) { return true; }

		/// <summary>
		/// What happens when the object is equipped - override this to add your own behaviors
		/// </summary>
		public virtual bool Equip(string playerID) { return true; }

		/// <summary>
		/// What happens when the object is unequipped (called when dropped) - override this to add your own behaviors
		/// </summary>
		public virtual bool UnEquip(string playerID) { return true; }

		/// <summary>
		/// What happens when the object gets swapped for another object
		/// </summary>
		public virtual void Swap(string playerID) {}

		/// <summary>
		/// What happens when the object is dropped - override this to add your own behaviors
		/// </summary>
		public virtual bool Drop(string playerID) { return true; }
	}
}