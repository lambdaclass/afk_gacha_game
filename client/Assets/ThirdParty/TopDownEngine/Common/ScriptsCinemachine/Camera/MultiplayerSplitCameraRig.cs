using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class handles a X-players split screen setup
	/// </summary>
	public class MultiplayerSplitCameraRig : TopDownMonoBehaviour, MMEventListener<MMGameEvent>
	{
		[Header("Multiplayer Split Camera Rig")]
		/// the list of camera controllers to bind to level manager players on load
		[Tooltip("the list of camera controllers to bind to level manager players on load")]
		public List<CinemachineCameraController> CameraControllers;

		/// <summary>
		/// Binds each camera controller to its target
		/// </summary>
		protected virtual void BindCameras()
		{
			int i = 0;
			foreach (Character character in LevelManager.Instance.Players)
			{
				CameraControllers[i].TargetCharacter = character;
				CameraControllers[i].FollowsAPlayer = true;
				CameraControllers[i].StartFollowing();
				i++;
			}
		}

		/// <summary>
		/// When the cameras are ready to be bound (we're being told so by the LevelManager usually), we bind them
		/// </summary>
		/// <param name="gameEvent"></param>
		public virtual void OnMMEvent(MMGameEvent gameEvent)
		{
			if (gameEvent.EventName == "CameraBound")
			{
				BindCameras();
			}
		}

		/// <summary>
		/// On enable we start listening for game events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMGameEvent>();
		}

		/// <summary>
		/// On disable we stop listening for game events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMGameEvent>();
		}
	}
}