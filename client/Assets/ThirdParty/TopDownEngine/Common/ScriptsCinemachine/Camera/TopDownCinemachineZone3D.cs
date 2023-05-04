using System.Collections;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a box collider and it'll let you define a zone that, when entered, enables a virtual camera, letting you define sections inside your level easily
	/// </summary>
	public class TopDownCinemachineZone3D : MMCinemachineZone3D
	{
		[Header("Top Down Engine")]
		/// if this is true, the zone will require colliders that want to trigger it to have a Character components of type Player
		[Tooltip("if this is true, the zone will require colliders that want to trigger it to have a Character components of type Player")]
		public bool RequiresPlayerCharacter = true;
		protected CinemachineCameraController _cinemachineCameraController;
		protected Character _character;
        
		/// <summary>
		/// On Awake, adds a camera controller if needed
		/// </summary>
		#if MM_CINEMACHINE
		protected override void Awake()
		{
			base.Awake();
			if (Application.isPlaying)
			{
				_cinemachineCameraController = VirtualCamera.gameObject.MMGetComponentAroundOrAdd<CinemachineCameraController>();
				_cinemachineCameraController.ConfineCameraToLevelBounds = false;    
			}
		}

		/// <summary>
		/// Enables/Disables the camera
		/// </summary>
		/// <param name="state"></param>
		/// <param name="frames"></param>
		/// <returns></returns>
		protected override IEnumerator EnableCamera(bool state, int frames)
		{
			yield return base.EnableCamera(state, frames);
			if (state)
			{
				_cinemachineCameraController.FollowsAPlayer = true;
				_cinemachineCameraController.StartFollowing();
			}
			else
			{
				_cinemachineCameraController.StopFollowing();
				_cinemachineCameraController.FollowsAPlayer = false;
			}
		}
		
		/// <summary>
		/// An extra test you can override to add extra collider conditions
		/// </summary>
		/// <param name="collider"></param>
		/// <returns></returns>
		protected override bool TestCollidingGameObject(GameObject collider)
		{
			if (RequiresPlayerCharacter)
			{
				_character = collider.MMGetComponentNoAlloc<Character>();
				if (_character == null)
				{
					return false;
				}

				if (_character.CharacterType != Character.CharacterTypes.Player)
				{
					return false;
				}
			}
			
			return true;
		}
		#endif
	}    
}