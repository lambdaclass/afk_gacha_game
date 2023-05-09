using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{	
	[RequireComponent(typeof(Weapon))]
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Ammo")]
	public class WeaponAmmo : TopDownMonoBehaviour, MMEventListener<MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates>>, MMEventListener<MMInventoryEvent>, MMEventListener<MMGameEvent>
	{
		[Header("Ammo")]
		
		/// the ID of this ammo, to be matched on the ammo display if you use one
		[Tooltip("the ID of this ammo, to be matched on the ammo display if you use one")]
		public string AmmoID;
		/// the name of the inventory where the system should look for ammo
		[Tooltip("the name of the inventory where the system should look for ammo")]
		public string AmmoInventoryName = "MainInventory";
		/// the theoretical maximum of ammo
		[Tooltip("the theoretical maximum of ammo")]
		public int MaxAmmo = 100;
		/// if this is true, everytime you equip this weapon, it'll auto fill with ammo
		[Tooltip("if this is true, everytime you equip this weapon, it'll auto fill with ammo")]
		public bool ShouldLoadOnStart = true;

		/// if this is true, everytime you equip this weapon, it'll auto fill with ammo
		[Tooltip("if this is true, everytime you equip this weapon, it'll auto fill with ammo")]
		public bool ShouldEmptyOnSave = true;

		/// the current amount of ammo available in the inventory
		[MMReadOnly]
		[Tooltip("the current amount of ammo available in the inventory")]
		public int CurrentAmmoAvailable;
		/// the inventory where ammo for this weapon is stored
		public Inventory AmmoInventory { get; set; }

		protected Weapon _weapon;
		protected InventoryItem _ammoItem;
		protected bool _emptied = false;

		/// <summary>
		/// On start, we grab the ammo inventory if we can find it
		/// </summary>
		protected virtual void Start()
		{
			_weapon = GetComponent<Weapon> ();
			Inventory[] inventories = FindObjectsOfType<Inventory>();
			foreach (Inventory inventory in inventories)
			{
				if (inventory.PlayerID != _weapon.Owner.PlayerID)
				{
					continue;
				}
				if ((AmmoInventory == null) && (inventory.name == AmmoInventoryName))
				{
					AmmoInventory = inventory;
				}
			}
			if (ShouldLoadOnStart)
			{
				LoadOnStart ();	
			}
		}

		/// <summary>
		/// Loads our weapon with ammo
		/// </summary>
		protected virtual void LoadOnStart()
		{
			FillWeaponWithAmmo ();
		}

		/// <summary>
		/// Updates the CurrentAmmoAvailable counter
		/// </summary>
		protected virtual void RefreshCurrentAmmoAvailable()
		{
			CurrentAmmoAvailable = AmmoInventory.GetQuantity (AmmoID);
		}

		/// <summary>
		/// Returns true if this weapon has enough ammo to fire, false otherwise
		/// </summary>
		/// <returns></returns>
		public virtual bool EnoughAmmoToFire()
		{
			if (AmmoInventory == null)
			{
				Debug.LogWarning (this.name + " couldn't find the associated inventory. Is there one present in the scene? It should be named '" + AmmoInventoryName + "'.");
				return false;
			}

			RefreshCurrentAmmoAvailable ();

			if (_weapon.MagazineBased)
			{
				if (_weapon.CurrentAmmoLoaded >= _weapon.AmmoConsumedPerShot)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (CurrentAmmoAvailable >= _weapon.AmmoConsumedPerShot)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Consumes ammo based on the amount of ammo to consume per shot
		/// </summary>
		protected virtual void ConsumeAmmo()
		{
			if (_weapon.MagazineBased)
			{
				_weapon.CurrentAmmoLoaded = _weapon.CurrentAmmoLoaded - _weapon.AmmoConsumedPerShot;
			}
			else
			{
				for (int i = 0; i < _weapon.AmmoConsumedPerShot; i++)
				{
					AmmoInventory.UseItem (AmmoID);	
					CurrentAmmoAvailable--;
				}	
			}

			if (CurrentAmmoAvailable < _weapon.AmmoConsumedPerShot)
			{
				if (_weapon.AutoDestroyWhenEmpty)
				{
					StartCoroutine(_weapon.WeaponDestruction());
				}
			}
		}

		/// <summary>
		/// Fills the weapon with ammo
		/// </summary>
		public virtual void FillWeaponWithAmmo()
		{
			if (AmmoInventory != null)
			{
				RefreshCurrentAmmoAvailable ();
			}

			if (_ammoItem == null)
			{
				List<int> list = AmmoInventory.InventoryContains(AmmoID);
				if (list.Count > 0)
				{
					_ammoItem = AmmoInventory.Content[list[list.Count - 1]].Copy();
				}
			}

			if (_weapon.MagazineBased)
			{
				int counter = 0;
				int stock = CurrentAmmoAvailable - _weapon.CurrentAmmoLoaded;
                
				for (int i = _weapon.CurrentAmmoLoaded; i < _weapon.MagazineSize; i++)
				{
					if (stock > 0) 
					{
						stock--;
						counter++;
						
						AmmoInventory.UseItem (AmmoID);	
					}									
				}
				_weapon.CurrentAmmoLoaded += counter;
			}
			
			RefreshCurrentAmmoAvailable();
		}
        
		/// <summary>
		/// Empties the weapon's magazine and puts the ammo back in the inventory
		/// </summary>
		public virtual void EmptyMagazine()
		{
			if (AmmoInventory != null)
			{
				RefreshCurrentAmmoAvailable ();
			}

			if ((_ammoItem == null) || (AmmoInventory == null))
			{
				return;
			}

			if (_emptied)
			{
				return;
			}

			if (_weapon.MagazineBased)
			{
				int stock = _weapon.CurrentAmmoLoaded;
				int counter = 0;
                
				for (int i = 0; i < stock; i++)
				{
					AmmoInventory.AddItem(_ammoItem, 1);
					counter++;
				}
				_weapon.CurrentAmmoLoaded -= counter;

				if (AmmoInventory.Persistent)
				{
					AmmoInventory.SaveInventory();
				}
			}
			RefreshCurrentAmmoAvailable();
			_emptied = true;
		}

		/// <summary>
		/// When getting weapon events, we either consume ammo or refill it
		/// </summary>
		/// <param name="weaponEvent"></param>
		public virtual void OnMMEvent(MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates> weaponEvent)
		{
			// if this event doesn't concern us, we do nothing and exit
			if (weaponEvent.Target != this.gameObject)
			{
				return;
			}

			switch (weaponEvent.NewState)
			{
				case MoreMountains.TopDownEngine.Weapon.WeaponStates.WeaponUse:
					ConsumeAmmo ();
					break;

				case MoreMountains.TopDownEngine.Weapon.WeaponStates.WeaponReloadStop:
					FillWeaponWithAmmo();
					break;
			}
		}

		/// <summary>
		/// Grabs inventory events and refreshes ammo if needed
		/// </summary>
		/// <param name="inventoryEvent"></param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			switch (inventoryEvent.InventoryEventType)
			{
				case MMInventoryEventType.Pick:
					if (inventoryEvent.EventItem.ItemClass == ItemClasses.Ammo)
					{
						StartCoroutine(DelayedRefreshCurrentAmmoAvailable());
					}
					break;				
			}
		}

		protected IEnumerator DelayedRefreshCurrentAmmoAvailable()
		{
			yield return null;
			RefreshCurrentAmmoAvailable ();
		}

		/// <summary>
		/// Grabs inventory events and refreshes ammo if needed
		/// </summary>
		/// <param name="inventoryEvent"></param>
		public virtual void OnMMEvent(MMGameEvent gameEvent)
		{
			switch (gameEvent.EventName)
			{
				case "Save":
					if (ShouldEmptyOnSave)
					{
						EmptyMagazine();    
					}
					break;				
			}
		}

		protected void OnDestroy()
		{
			// on destroy we put our ammo back in the inventory
			EmptyMagazine();
		}

		/// <summary>
		/// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates>>();
			this.MMEventStartListening<MMInventoryEvent> ();
			this.MMEventStartListening<MMGameEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates>>();
			this.MMEventStopListening<MMInventoryEvent> ();
			this.MMEventStartListening<MMGameEvent>();
		}
	}
}