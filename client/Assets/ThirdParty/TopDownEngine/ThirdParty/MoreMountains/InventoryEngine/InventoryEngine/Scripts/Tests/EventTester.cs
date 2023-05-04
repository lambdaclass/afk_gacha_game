using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// This class shows examples of how you can listen to MMInventoryEvents, from any class
/// </summary>
public class EventTester : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
	/// <summary>
	/// When we catch a MMInventoryEvent, we filter on its type and display info about the item used
	/// </summary>
	/// <param name="inventoryEvent"></param>
	public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
	{
		if (inventoryEvent.InventoryEventType == MMInventoryEventType.ItemUsed)
		{
			MMDebug.DebugLogTime("item used");
			MMDebug.DebugLogTime("ItemID : "+inventoryEvent.EventItem.ItemID);
			MMDebug.DebugLogTime("Item name : "+inventoryEvent.EventItem.ItemName);
		}
	}
    
	/// <summary>
	/// On enable we start listening for MMInventoryEvents 
	/// </summary>
	protected virtual void OnEnable()
	{
		this.MMEventStartListening<MMInventoryEvent>();
	}
    
	/// <summary>
	/// On disable we stop listening for MMInventoryEvents 
	/// </summary>
	protected virtual void OnDisable()
	{
		this.MMEventStopListening<MMInventoryEvent>();
	}
}