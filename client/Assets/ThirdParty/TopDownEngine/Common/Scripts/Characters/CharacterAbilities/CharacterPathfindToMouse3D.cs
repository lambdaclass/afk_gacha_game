using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This ability, used on a Player character, will let you click on the ground and have the character move to the click's position
	/// You'll find a demo of this ability on the LoftSuspendersMouseDriven demo character. You can drag it in the Loft3D demo scene's LevelManager's PlayerPrefabs slot to give it a try.
	/// For AIs, look at the MousePathfinderAI3D script instead, and its demo in the MinimalPathfinding3D demo scene
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Pathfind To Mouse")]
	[RequireComponent(typeof(CharacterPathfinder3D))]
	public class CharacterPathfindToMouse3D : CharacterAbility
	{
		[Header("Mouse")]
		/// the index of the mouse button to read input on
		[Tooltip("the index of the mouse button to read input on")]
		public int MouseButtonIndex = 1;
        
		[Header("OnClick")] 
		/// a feedback to play at the position of the click
		[Tooltip("a feedback to play at the position of the click")]
		public MMFeedbacks OnClickFeedbacks;

		/// if this is true, a click or tap on a UI element will block the click and won't cause the character to move
		[Tooltip("if this is true, a click or tap on a UI element will block the click and won't cause the character to move")]
		public bool UIShouldBlockInput = true;
        
		public GameObject Destination { get; set; }

		protected CharacterPathfinder3D _characterPathfinder3D;
		protected Plane _playerPlane;
		protected bool _destinationSet = false;
		protected Camera _mainCamera;

		/// <summary>
		/// On awake we create a plane to catch our ray
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_mainCamera = Camera.main;
			_characterPathfinder3D = this.gameObject.GetComponent<CharacterPathfinder3D>();
			_character.FindAbility<CharacterMovement>().ScriptDrivenInput = true;
            
			OnClickFeedbacks?.Initialization();
			_playerPlane = new Plane(Vector3.up, Vector3.zero);
			if (Destination == null)
			{
				Destination = new GameObject();
				Destination.name = this.name + "PathfindToMouseDestination";
			}
		}
        
		/// <summary>
		/// Every frame we make sure we shouldn't be exiting our run state
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			if (!AbilityAuthorized)
			{
				return;
			}
			DetectMouse();
		}

		/// <summary>
		/// If the mouse is clicked, we cast a ray and if that ray hits the plane we make it the pathfinding target
		/// </summary>
		protected virtual void DetectMouse()
		{
			bool testUI = false;

			if (UIShouldBlockInput)
			{
				testUI = MMGUI.PointOrTouchBlockedByUI();
			}
            
			if (Input.GetMouseButtonDown(MouseButtonIndex) && !testUI)
			{
				Ray ray = _mainCamera.ScreenPointToRay(InputManager.Instance.MousePosition);
				Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
				float distance;
				if (_playerPlane.Raycast(ray, out distance))
				{
					Vector3 target = ray.GetPoint(distance);
					Destination.transform.position = target;
					_destinationSet = true;
					_characterPathfinder3D.SetNewDestination(Destination.transform);
					OnClickFeedbacks?.PlayFeedbacks(Destination.transform.position);
				}
			}
		}
	}
}