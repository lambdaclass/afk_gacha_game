using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
#if MM_CINEMACHINE
using Cinemachine;
#endif

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class that handles camera follow for Cinemachine powered cameras
	/// </summary>
	public class CinemachineCameraController : TopDownMonoBehaviour, MMEventListener<MMCameraEvent>, MMEventListener<TopDownEngineEvent>
	{
		/// True if the camera should follow the player
		public bool FollowsPlayer { get; set; }
		/// Whether or not this camera should follow a player
		[Tooltip("Whether or not this camera should follow a player")]
		public bool FollowsAPlayer = true;
		/// Whether to confine this camera to the level bounds, as defined in the LevelManager
		[Tooltip("Whether to confine this camera to the level bounds, as defined in the LevelManager")]
		public bool ConfineCameraToLevelBounds = true;
		/// If this is true, this confiner will listen to set confiner events
		[Tooltip("If this is true, this confiner will listen to set confiner events")]
		public bool ListenToSetConfinerEvents = true;
		[MMReadOnly]
		/// the target character this camera should follow
		[Tooltip("the target character this camera should follow")]
		public Character TargetCharacter;

		#if MM_CINEMACHINE
		protected CinemachineVirtualCamera _virtualCamera;
		protected CinemachineConfiner _confiner;
		#endif

		/// <summary>
		/// On Awake we grab our components
		/// </summary>
		protected virtual void Awake()
		{
			#if MM_CINEMACHINE
			_virtualCamera = GetComponent<CinemachineVirtualCamera>();
			_confiner = GetComponent<CinemachineConfiner>();
			#endif
		}

		/// <summary>
		/// On Start we assign our bounding volume
		/// </summary>
		protected virtual void Start()
		{
			#if MM_CINEMACHINE
			if ((_confiner != null) && ConfineCameraToLevelBounds && LevelManager.HasInstance)
			{
				_confiner.m_BoundingVolume = LevelManager.Instance.BoundsCollider;
			}
			#endif
		}

		public virtual void SetTarget(Character character)
		{
			TargetCharacter = character;
		}

		/// <summary>
		/// Starts following the LevelManager's main player
		/// </summary>
		public virtual void StartFollowing()
		{
			if (!FollowsAPlayer) { return; }
			FollowsPlayer = true;
			#if MM_CINEMACHINE
			_virtualCamera.Follow = TargetCharacter.CameraTarget.transform;
			_virtualCamera.enabled = true;
			#endif
		}

		/// <summary>
		/// Stops following any target
		/// </summary>
		public virtual void StopFollowing()
		{
			if (!FollowsAPlayer) { return; }
			FollowsPlayer = false;
			#if MM_CINEMACHINE
			_virtualCamera.Follow = null;
			_virtualCamera.enabled = false;
			#endif
		}

		public virtual void OnMMEvent(MMCameraEvent cameraEvent)
		{
			#if MM_CINEMACHINE
			switch (cameraEvent.EventType)
			{
				case MMCameraEventTypes.SetTargetCharacter:
					SetTarget(cameraEvent.TargetCharacter);
					break;

				case MMCameraEventTypes.SetConfiner:                    
					if (_confiner != null && ListenToSetConfinerEvents)
					{
						_confiner.m_BoundingVolume = cameraEvent.Bounds;
					}
					break;

				case MMCameraEventTypes.StartFollowing:
					if (cameraEvent.TargetCharacter != null)
					{
						if (cameraEvent.TargetCharacter != TargetCharacter)
						{
							return;
						}
					}
					StartFollowing();
					break;

				case MMCameraEventTypes.StopFollowing:
					if (cameraEvent.TargetCharacter != null)
					{
						if (cameraEvent.TargetCharacter != TargetCharacter)
						{
							return;
						}
					}
					StopFollowing();
					break;

				case MMCameraEventTypes.RefreshPosition:
					StartCoroutine(RefreshPosition());
					break;

				case MMCameraEventTypes.ResetPriorities:
					_virtualCamera.Priority = 0;
					break;
			}
			#endif
		}

		protected virtual IEnumerator RefreshPosition()
		{
			#if MM_CINEMACHINE
			_virtualCamera.enabled = false;
			#endif
			yield return null;
			StartFollowing();
		}

		public virtual void OnMMEvent(TopDownEngineEvent topdownEngineEvent)
		{
			if (topdownEngineEvent.EventType == TopDownEngineEventTypes.CharacterSwitch)
			{
				SetTarget(LevelManager.Instance.Players[0]);
				StartFollowing();
			}

			if (topdownEngineEvent.EventType == TopDownEngineEventTypes.CharacterSwap)
			{
				SetTarget(LevelManager.Instance.Players[0]);
				MMCameraEvent.Trigger(MMCameraEventTypes.RefreshPosition);
			}
		}

		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMCameraEvent>();
			this.MMEventStartListening<TopDownEngineEvent>();
		}

		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMCameraEvent>();
			this.MMEventStopListening<TopDownEngineEvent>();
		}
	}
}