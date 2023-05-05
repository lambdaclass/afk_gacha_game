using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
	/// <summary>
	/// This class shows a simple example of a storage box, that will display a target inventory when the Player character is on top of it, and close it when the character exits the zone.
	/// </summary>
	public class InventoryDemoStorageBox : MonoBehaviour
	{
		public CanvasGroup TargetCanvasGroup;

		public virtual void OpenStorage(string playerID)
		{
			TargetCanvasGroup.alpha = 1;
		}

		public virtual void CloseStorage(string playerID)
		{
			TargetCanvasGroup.alpha = 0;
		}
		
		public virtual void OnTriggerEnter(Collider collider)
		{
			OnEnter(collider.gameObject);
		}

		public virtual void OnTriggerEnter2D (Collider2D collider) 
		{
			OnEnter(collider.gameObject);
		}

		protected virtual void OnEnter(GameObject collider)
		{
			// if what's colliding with the picker ain't a Player, we do nothing and exit
			if (!collider.CompareTag("Player"))
			{
				return;
			}

			string playerID = "Player1";
			InventoryCharacterIdentifier identifier = collider.GetComponent<InventoryCharacterIdentifier>();
			if (identifier != null)
			{
				playerID = identifier.PlayerID;
			}

			OpenStorage(playerID);
		}

		public void OnTriggerExit(Collider collider)
		{
			OnExit(collider.gameObject);
		}

		public virtual void OnTriggerExit2D (Collider2D collider) 
		{
			OnExit(collider.gameObject);
		}
		
		protected virtual void OnExit(GameObject collider)
		{
			// if what's colliding with the picker ain't a Player, we do nothing and exit
			if (!collider.CompareTag("Player"))
			{
				return;
			}

			string playerID = "Player1";
			InventoryCharacterIdentifier identifier = collider.GetComponent<InventoryCharacterIdentifier>();
			if (identifier != null)
			{
				playerID = identifier.PlayerID;
			}

			CloseStorage(playerID);
		}
	}	
}