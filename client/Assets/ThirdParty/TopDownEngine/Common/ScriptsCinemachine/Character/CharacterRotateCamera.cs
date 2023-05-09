using System.Collections;
using UnityEngine;
#if MM_CINEMACHINE
using Cinemachine;
#endif
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An ability that will let the Character rotate its associated camera, using the PlayerID_CameraRotationAxis input axis
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Rotate Camera")]
	public class CharacterRotateCamera : CharacterAbility
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "An ability that will let the Character rotate its associated camera, using the PlayerID_CameraRotationAxis input axis"; }

		[Header("Rotation axis")]
		/// the space in which to rotate the camera (usually world)
		[Tooltip("the space in which to rotate the camera (usually world)")]
		public Space RotationSpace = Space.World;
		/// the camera's forward vector, usually 0,0,1
		[Tooltip("the camera's forward vector, usually 0,0,1")]
		public Vector3 RotationForward = Vector3.forward;
		/// the axis on which to rotate the camera (usually 0,1,0 in 3D, 0,0,1 in 2D)
		[Tooltip("the axis on which to rotate the camera (usually 0,1,0 in 3D, 0,0,1 in 2D)")]
		public Vector3 RotationAxis = Vector3.up;

		[Header("Camera Speed")]
		/// the speed at which the camera should rotate
		[Tooltip("the speed at which the camera should rotate")]
		public float CameraRotationSpeed = 3f;
		/// the speed at which the camera should interpolate towards its target position
		[Tooltip("the speed at which the camera should interpolate towards its target position")]
		public float CameraInterpolationSpeed = 0.2f;

		[Header("Input Manager")] 
		/// if this is false, this ability won't read input
		[Tooltip("if this is false, this ability won't read input")]
		public bool InputAuthorized = true;
		/// whether or not this ability should make changes on the InputManager to set it in camera driven input mode
		[Tooltip("whether or not this ability should make changes on the InputManager to set it in camera driven input mode")]
		public bool AutoSetupInputManager = true;

		protected float _requestedCameraAngle = 0f;
		protected Camera _mainCamera;
		#if MM_CINEMACHINE
		protected CinemachineBrain _brain;
		protected CinemachineVirtualCamera _virtualCamera;
		#endif
		protected float _targetRotationAngle;
		protected Vector3 _cameraDirection;
		protected float _cameraDirectionAngle;

		/// <summary>
		/// On init we grab our camera and setup our input manager if needed
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_mainCamera = Camera.main;
			StartCoroutine(DelayedInitialization());
			if (AutoSetupInputManager)
			{
				_inputManager.RotateInputBasedOnCameraDirection = true;
				bool camera3D = (_character.CharacterDimension == Character.CharacterDimensions.Type3D);
				_inputManager.SetCamera(_mainCamera, camera3D);
			}
		}

		/// <summary>
		/// Because Cinemachine only initializes in LateUpdate, and doesn't offer events to know when it'll be ready, we wait a bit for it to be done
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator DelayedInitialization()
		{
			yield return MMCoroutine.WaitForFrames(2);
			GetCurrentCamera();
		}

		/// <summary>
		/// Stores the current camera
		/// </summary>
		protected virtual void GetCurrentCamera()
		{
			#if MM_CINEMACHINE
			_brain = _mainCamera.GetComponent<CinemachineBrain>();
			if (_brain != null)
			{
				_virtualCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;
			}
			#endif
		}

		/// <summary>
		/// If InputAuthorized is false, you can use this method to force a camera angle from another script
		/// </summary>
		/// <param name="newAngle"></param>
		public virtual void SetCameraAngle(float newAngle)
		{
			_requestedCameraAngle = newAngle;
		}

		/// <summary>
		/// Reads input to know the requested rotation angle for the camera
		/// </summary>
		protected override void HandleInput()
		{
			base.HandleInput();
			if (!InputAuthorized)
			{
				return;
			}
			_requestedCameraAngle = _inputManager.CameraRotationInput * CameraRotationSpeed;
		}

		/// <summary>
		/// Every frame we rotate the camera
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			if (!AbilityAuthorized)
			{
				return;
			}
			RotateCamera();
		}

		/// <summary>
		/// Changes the rotation of the camera to match input
		/// </summary>
		protected virtual void RotateCamera()
		{
			_targetRotationAngle = MMMaths.Lerp(_targetRotationAngle, _requestedCameraAngle, CameraInterpolationSpeed, Time.deltaTime);

			#if MM_CINEMACHINE
			if (_virtualCamera != null)
			{
				_virtualCamera.transform.Rotate(RotationAxis, _targetRotationAngle, RotationSpace);
				_cameraDirectionAngle = (_character.CharacterDimension == Character.CharacterDimensions.Type3D) ? _virtualCamera.transform.localEulerAngles.y : _virtualCamera.transform.localEulerAngles.z;

			}
			else  if (_mainCamera != null)
			{
				_mainCamera.transform.Rotate(RotationAxis, _targetRotationAngle, RotationSpace);
				_cameraDirectionAngle = (_character.CharacterDimension == Character.CharacterDimensions.Type3D) ? _mainCamera.transform.localEulerAngles.y : _mainCamera.transform.localEulerAngles.z;
			}
			#endif
			_cameraDirection = Quaternion.AngleAxis(_cameraDirectionAngle, RotationAxis) * RotationForward;
			_character.SetCameraDirection(_cameraDirection);
		}
	}
}