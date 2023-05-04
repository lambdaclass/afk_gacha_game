using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
	/// <summary>
	/// This test class lets you play with the Inventory API.
	/// You can see it in action in the PixelRogueRoom2 demo scene
	/// </summary>
	public class InventoryTester : MonoBehaviour
	{
		[Header("Add item")] 
		/// an item to add when pressing the AddItemTest button 
		public InventoryItem AddItem;
		/// the quantity of the item to add
		public int AddItemQuantity;
		/// the inventory to which to add the item
		public Inventory AddItemInventory;
		/// a test button
		[MMInspectorButton("AddItemTest")]
		public bool AddItemTestButton;
        
		[Header("Add item at")] 
		/// the inventory item to add at a specific index
		public InventoryItem AddItemAtItem;
		/// the quantity of item to add
		public int AddItemAtQuantity;
		/// the index at which the item should be added
		public int AddItemAtIndex;
		/// the inventory to add items to
		public Inventory AddItemAtInventory;
		/// a test button
		[MMInspectorButton("AddItemAtTest")] 
		public bool AddItemAtTestButton;

		[Header("Move Item")] 
		/// the index from which to move items
		public int MoveItemOrigin;
		/// the index to which we want to move the origin index's item
		public int MoveItemDestination;
		/// the inventory in which to move items
		public Inventory MoveItemInventory;
		/// a test button
		[MMInspectorButton("MoveItemTest")] 
		public bool MoveItemTestButton;

		[Header("Move Item To Inventory")] 
		/// the index from which to move items
		public int MoveItemToInventoryOriginIndex;
		/// the index to which we want to move the origin index's item
		public int MoveItemToInventoryDestinationIndex = -1;
		/// the inventory in which to move an item from
		public Inventory MoveItemToOriginInventory;
		/// the inventory in which to move an item to
		public Inventory MoveItemToDestinationInventory;
		/// a test button
		[MMInspectorButton("MoveItemToInventory")] 
		public bool MoveItemToInventoryTestButton;

		[Header("Remove Item")] 
		/// the index at which to remove items
		public int RemoveItemIndex;
		/// the quantity to remove at the specified index
		public int RemoveItemQuantity;
		/// the inventory from which to remove an item
		public Inventory RemoveItemInventory;
		/// a test button
		[MMInspectorButton("RemoveItemTest")] 
		public bool RemoveItemTestButton;

		[Header("Empty Inventory")]
		/// the inventory to empty
		public Inventory EmptyTargetInventory;
		/// a test button
		[MMInspectorButton("EmptyInventoryTest")] 
		public bool EmptyInventoryTestButton;

		/// <summary>
		/// Test method to add an item
		/// </summary>
		protected virtual void AddItemTest()
		{
			AddItemInventory.AddItem(AddItem, AddItemQuantity);
		}
        
		/// <summary>
		/// Test method to add an item at the specified index 
		/// </summary>
		protected virtual void AddItemAtTest()
		{
			AddItemAtInventory.AddItemAt(AddItemAtItem, AddItemAtQuantity, AddItemAtIndex);
		}
        
		/// <summary>
		/// Test method to move an item from index A to B
		/// </summary>
		protected virtual void MoveItemTest()
		{
			MoveItemInventory.MoveItem(MoveItemOrigin, MoveItemDestination);
		}
        
		/// <summary>
		/// Test method to move an item from index A to B
		/// </summary>
		protected virtual void MoveItemToInventory()
		{
			MoveItemToOriginInventory.MoveItemToInventory(MoveItemToInventoryOriginIndex, MoveItemToDestinationInventory, MoveItemToInventoryDestinationIndex);
		}
        
		/// <summary>
		/// Test method to remove an item at the specified index 
		/// </summary>
		protected virtual void RemoveItemTest()
		{
			RemoveItemInventory.RemoveItem(RemoveItemIndex, RemoveItemQuantity);
		}
        
		/// <summary>
		/// Test method to empty the target inventory 
		/// </summary>
		protected virtual void EmptyInventoryTest()
		{
			EmptyTargetInventory.EmptyInventory();
		}
	}
}