using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A generic level manager meant to handle multiplayer scenes (specifically spawn and camera modes
	/// It's recommended to extend it to implement your own specific gameplay rules
	/// </summary>
	[AddComponentMenu("TopDown Engine/Managers/MultiplayerLevelManager")]
	public class MultiplayerLevelManager : LevelManager
	{
		[Header("Multiplayer spawn")]
		/// the list of checkpoints (in order) to use to spawn characters
		[Tooltip("the list of checkpoints (in order) to use to spawn characters")]
		public List<CheckPoint> SpawnPoints;
		/// the types of cameras to choose from
		public enum CameraModes { Split, Group }

		[Header("Cameras")]
		/// the selected camera mode (either group, all targets in one screen, or split screen)
		[Tooltip("the selected camera mode (either group, all targets in one screen, or split screen)")]
		public CameraModes CameraMode = CameraModes.Split;
		/// the group camera rig
		[Tooltip("the group camera rig")]
		public GameObject GroupCameraRig;
		/// the split camera rig
		[Tooltip("the split camera rig")]
		public GameObject SplitCameraRig;

		[Header("GUI Manager")]
		/// the multiplayer GUI Manager
		[Tooltip("the multiplayer GUI Manager")]
		public MultiplayerGUIManager MPGUIManager;

		/// <summary>
		/// On awake we handle our different camera modes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			HandleCameraModes();
		}

		/// <summary>
		/// Sets up the scene to match the selected camera mode
		/// </summary>
		protected virtual void HandleCameraModes()
		{
			if (CameraMode == CameraModes.Split)
			{
				if (GroupCameraRig != null) { GroupCameraRig.SetActive(false); }
				if (SplitCameraRig != null) { SplitCameraRig.SetActive(true); }
				if (MPGUIManager != null)
				{
					MPGUIManager.SplitHUD?.SetActive(true);
					MPGUIManager.GroupHUD?.SetActive(false);
					MPGUIManager.SplittersGUI?.SetActive(true);
				}
			}
			if (CameraMode == CameraModes.Group)
			{
				if (GroupCameraRig != null) { GroupCameraRig?.SetActive(true); }
				if (SplitCameraRig != null) { SplitCameraRig?.SetActive(false); }
				if (MPGUIManager != null)
				{
					MPGUIManager.SplitHUD?.SetActive(false);
					MPGUIManager.GroupHUD?.SetActive(true);
					MPGUIManager.SplittersGUI?.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Spawns all characters at the specified spawn points
		/// </summary>
		protected override void SpawnMultipleCharacters()
		{
			for (int i = 0; i < Players.Count; i++)
			{
				SpawnPoints[i].SpawnPlayer(Players[i]);
				if (AutoAttributePlayerIDs)
				{
					Players[i].SetPlayerID("Player" + (i + 1));
				}                
			}
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.SpawnComplete, null);
		}

		/// <summary>
		/// Kills the specified player 
		/// </summary>
		public override void PlayerDead(Character playerCharacter)
		{
			if (playerCharacter == null)
			{
				return;
			}
			Health characterHealth = playerCharacter.GetComponent<Health>();
			if (characterHealth == null)
			{
				return;
			}
			else
			{
				OnPlayerDeath(playerCharacter);
			}
		}
        
		/// <summary>
		/// Override this to specify what happens when a player dies
		/// </summary>
		/// <param name="playerCharacter"></param>
		protected virtual void OnPlayerDeath(Character playerCharacter)
		{

		}
	}
}