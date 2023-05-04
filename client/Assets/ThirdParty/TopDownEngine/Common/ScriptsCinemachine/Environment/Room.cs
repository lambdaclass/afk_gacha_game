using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
#if MM_CINEMACHINE
using Cinemachine;
#endif
using UnityEngine.Events;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class lets you define the boundaries of rooms in your level.
	/// Rooms are useful if you want to cut your level into portions (think Super Metroid or Hollow Knight for example).
	/// These rooms will require their own virtual camera, and a confiner to define their size. 
	/// Note that the confiner is different from the collider that defines the room.
	/// You can see an example of rooms in action in the KoalaRooms demo scene.
	/// </summary>
	public class Room : TopDownMonoBehaviour, MMEventListener<TopDownEngineEvent>
	{
		public enum Modes { TwoD, ThreeD }

		/// the collider for this room
		public Vector3 RoomColliderCenter
		{
			get
			{
				if (_roomCollider2D != null)
				{
					return _roomCollider2D.bounds.center;
				}
				else
				{
					return _roomCollider.bounds.center;
				}
			}
		}
        
		public Vector3 RoomColliderSize
		{
			get
			{
				if (_roomCollider2D != null)
				{
					return _roomCollider2D.bounds.size;
				}
				else
				{
					return _roomCollider.bounds.size;
				}
			}
		}

		public Bounds RoomBounds
		{
			get
			{
				if (_roomCollider2D != null)
				{
					return _roomCollider2D.bounds;
				}
				else
				{
					return _roomCollider.bounds;
				}
			}
		}

		[Header("Mode")]
		/// whether this room is intended to work in 2D or 3D mode
		[Tooltip("whether this room is intended to work in 2D or 3D mode")]
		public Modes Mode = Modes.TwoD;

		#if MM_CINEMACHINE
		[Header("Camera")]
		/// the virtual camera associated to this room
		[Tooltip("the virtual camera associated to this room")]
		public CinemachineVirtualCamera VirtualCamera;
		/// the confiner for this room, that will constrain the virtual camera, usually placed on a child object of the Room
		[Tooltip("the confiner for this room, that will constrain the virtual camera, usually placed on a child object of the Room")]
		public BoxCollider Confiner;
		/// the confiner component of the virtual camera
		[Tooltip("the confiner component of the virtual camera")]
		public CinemachineConfiner CinemachineCameraConfiner;
		#endif
		/// whether or not the confiner should be auto resized on start to match the camera's size and ratio
		[Tooltip("whether or not the confiner should be auto resized on start to match the camera's size and ratio")]
		public bool ResizeConfinerAutomatically = true;
		/// whether or not this Room should look at the level's start position and declare itself the current room on start or not
		[Tooltip("whether or not this Room should look at the level's start position and declare itself the current room on start or not")]
		public bool AutoDetectFirstRoomOnStart = true;
		/// the depth of the room (used to resize the z value of the confiner
		[MMEnumCondition("Mode", (int)Modes.TwoD)]
		[Tooltip("the depth of the room (used to resize the z value of the confiner")]
		public float RoomDepth = 100f;

		[Header("State")]
		/// whether this room is the current room or not
		[Tooltip("whether this room is the current room or not")]
		public bool CurrentRoom = false;
		/// whether this room has already been visited or not
		[Tooltip("whether this room has already been visited or not")]
		public bool RoomVisited = false;

		[Header("Actions")]
		/// the event to trigger when the player enters the room for the first time
		[Tooltip("the event to trigger when the player enters the room for the first time")]
		public UnityEvent OnPlayerEntersRoomForTheFirstTime;
		/// the event to trigger everytime the player enters the room
		[Tooltip("the event to trigger everytime the player enters the room")]
		public UnityEvent OnPlayerEntersRoom;
		/// the event to trigger everytime the player exits the room
		[Tooltip("the event to trigger everytime the player exits the room")]
		public UnityEvent OnPlayerExitsRoom;

		[Header("Activation")]
		/// a list of gameobjects to enable when entering the room, and disable when exiting it
		[Tooltip("a list of gameobjects to enable when entering the room, and disable when exiting it")]
		public List<GameObject> ActivationList;

		protected Collider _roomCollider;
		protected Collider2D _roomCollider2D;
		protected Camera _mainCamera;
		protected Vector2 _cameraSize;
		protected bool _initialized = false;

		/// <summary>
		/// On Awake we reset our camera's priority
		/// </summary>
		protected virtual void Awake()
		{
			#if MM_CINEMACHINE
			if (VirtualCamera != null)
			{
				VirtualCamera.Priority = 0;	
			}
			#endif
		}
        
		/// <summary>
		/// On Start we initialize our room
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// Grabs our Room collider, our main camera, and starts the confiner resize
		/// </summary>
		protected virtual void Initialization()
		{
			if (_initialized)
			{
				return;
			}
			_roomCollider = this.gameObject.GetComponent<Collider>();
			_roomCollider2D = this.gameObject.GetComponent<Collider2D>();
			_mainCamera = Camera.main;          
			StartCoroutine(ResizeConfiner());
			_initialized = true;
		}

		/// <summary>
		/// Resizes the confiner 
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator ResizeConfiner()
		{
			#if MM_CINEMACHINE
			if ((VirtualCamera == null) || (Confiner == null) || !ResizeConfinerAutomatically)
			{
				yield break;
			}

			// we wait two more frame for Unity's pixel perfect camera component to be ready because apparently sending events is not a thing.
			yield return null;
			yield return null;

			Confiner.transform.position = RoomColliderCenter;
			Vector3 size = RoomColliderSize;

			switch (Mode)
			{
				case Modes.TwoD:
					size.z = RoomDepth;
					Confiner.size = size;
					_cameraSize.y = 2 * _mainCamera.orthographicSize;
					_cameraSize.x = _cameraSize.y * _mainCamera.aspect;

					Vector3 newSize = Confiner.size;

					if (Confiner.size.x < _cameraSize.x)
					{
						newSize.x = _cameraSize.x;
					}
					if (Confiner.size.y < _cameraSize.y)
					{
						newSize.y = _cameraSize.y;
					}

					Confiner.size = newSize;
					break;
				case Modes.ThreeD:
					Confiner.size = size;
					break;
			}
            
			CinemachineCameraConfiner.InvalidatePathCache();

			//HandleLevelStartDetection();
			#else
            yield return null;
			#endif
		}   
        
		/// <summary>
		/// Looks for the level start position and if it's inside the room, makes this room the current one
		/// </summary>
		protected virtual void HandleLevelStartDetection()
		{
			#if MM_CINEMACHINE
			if (!_initialized)
			{
				Initialization();
			}

			if (AutoDetectFirstRoomOnStart)
			{
				if (LevelManager.HasInstance)
				{
					if (RoomBounds.Contains(LevelManager.Instance.Players[0].transform.position))
					{
						MMCameraEvent.Trigger(MMCameraEventTypes.ResetPriorities);
						MMCinemachineBrainEvent.Trigger(MMCinemachineBrainEventTypes.ChangeBlendDuration, 0f);

						MMSpriteMaskEvent.Trigger(MMSpriteMaskEvent.MMSpriteMaskEventTypes.MoveToNewPosition,
							RoomColliderCenter,
							RoomColliderSize,
							0f, MMTween.MMTweenCurve.LinearTween);

						PlayerEntersRoom();
						if (VirtualCamera != null)
						{
							VirtualCamera.Priority = 10;
							VirtualCamera.enabled = true;	
						}
					}
					else
					{
						if (VirtualCamera != null)
						{
							VirtualCamera.Priority = 0;
							VirtualCamera.enabled = false;	
						}
					}
				}
			}
			#endif
		}

		/// <summary>
		/// Call this to let the room know a player entered
		/// </summary>
		public virtual void PlayerEntersRoom()
		{
			CurrentRoom = true;
			if (RoomVisited)
			{
				OnPlayerEntersRoom?.Invoke();
			}
			else
			{
				RoomVisited = true;
				OnPlayerEntersRoomForTheFirstTime?.Invoke();
			}  
			foreach(GameObject go in ActivationList)
			{
				go.SetActive(true);
			}
		}

		/// <summary>
		/// Call this to let this room know a player exited
		/// </summary>
		public virtual void PlayerExitsRoom()
		{
			CurrentRoom = false;
			OnPlayerExitsRoom?.Invoke();
			foreach (GameObject go in ActivationList)
			{
				go.SetActive(false);
			}
		}

		/// <summary>
		/// When we get a respawn event, we ask for a camera reposition
		/// </summary>
		/// <param name="topDownEngineEvent"></param>
		public virtual void OnMMEvent(TopDownEngineEvent topDownEngineEvent)
		{
			if ((topDownEngineEvent.EventType == TopDownEngineEventTypes.RespawnComplete)
			    || (topDownEngineEvent.EventType == TopDownEngineEventTypes.LevelStart))
			{
				HandleLevelStartDetection();
			}
		}

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<TopDownEngineEvent>();
		}

		/// <summary>
		/// On enable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<TopDownEngineEvent>();
		}
	}
}