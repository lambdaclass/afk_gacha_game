using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An event to trigger when a checkpoint is reached
	/// </summary>
	public struct CheckPointEvent
	{
		public int Order;
		public CheckPointEvent(int order)
		{
			Order = order;
		}

		static CheckPointEvent e;
		public static void Trigger(int order)
		{
			e.Order = order;
			MMEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// Checkpoint class. Will make the player respawn at this point if it dies.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Spawn/Checkpoint")]
	public class CheckPoint : TopDownMonoBehaviour 
	{
		[Header("Spawn")]
		[MMInformation("Add this script to a (preferrably empty) GameObject and it'll be added to the level's checkpoint list, allowing you to respawn from there. If you bind it to the LevelManager's starting point, that's where your character will spawn at the start of the level. And here you can decide whether the character should spawn facing left or right.",MMInformationAttribute.InformationType.Info,false)]
		/// the facing direction the character should face when spawning from this checkpoint
		[Tooltip("the facing direction the character should face when spawning from this checkpoint")]
		public Character.FacingDirections FacingDirection = Character.FacingDirections.East ;
		/// whether or not this checkpoint should override any order and assign itself on entry
		[Tooltip("whether or not this checkpoint should override any order and assign itself on entry")]
		public bool ForceAssignation = false;
		/// the order of the checkpoint
		[Tooltip("the order of the checkpoint")]
		public int CheckPointOrder;
        
		protected List<Respawnable> _listeners;

		/// <summary>
		/// Initializes the list of listeners
		/// </summary>
		protected virtual void Awake () 
		{
			_listeners = new List<Respawnable>();
		}
				
		/// <summary>
		/// Spawns the player at the checkpoint.
		/// </summary>
		/// <param name="player">Player.</param>
		public virtual void SpawnPlayer(Character player)
		{
			player.RespawnAt(transform, FacingDirection);
			
			foreach(Respawnable listener in _listeners)
			{
				listener.OnPlayerRespawn(this,player);
			}
		}
		
		/// <summary>
		/// Assigns the Respawnable to this checkpoint
		/// </summary>
		/// <param name="listener"></param>
		public virtual void AssignObjectToCheckPoint (Respawnable listener) 
		{
			_listeners.Add(listener);
		}

		/// <summary>
		/// Describes what happens when something enters the checkpoint
		/// </summary>
		/// <param name="collider">Something colliding with the water.</param>
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			TriggerEnter(collider.gameObject);            
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			TriggerEnter(collider.gameObject);
		}

		protected virtual void TriggerEnter(GameObject collider)
		{
			Character character = collider.GetComponent<Character>();

			if (character == null) { return; }
			if (character.CharacterType != Character.CharacterTypes.Player) { return; }
			if (!LevelManager.HasInstance) { return; }
			LevelManager.Instance.SetCurrentCheckpoint(this);
			CheckPointEvent.Trigger(CheckPointOrder);
		}

		/// <summary>
		/// On DrawGizmos, we draw lines to show the path the object will follow
		/// </summary>
		protected virtual void OnDrawGizmos()
		{	
			#if UNITY_EDITOR

			if (!LevelManager.HasInstance)
			{
				return;
			}

			if (LevelManager.Instance.Checkpoints == null)
			{
				return;
			}

			if (LevelManager.Instance.Checkpoints.Count == 0)
			{
				return;
			}

			for (int i=0; i < LevelManager.Instance.Checkpoints.Count; i++)
			{
				// we draw a line towards the next point in the path
				if ((i+1) < LevelManager.Instance.Checkpoints.Count)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(LevelManager.Instance.Checkpoints[i].transform.position,LevelManager.Instance.Checkpoints[i+1].transform.position);
				}
			}
			#endif
		}
	}
}