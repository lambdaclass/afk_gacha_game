using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Do not use this class directly, use TopDownController2D for 2D characters, or TopDownController3D for 3D characters
	/// Both of these classes inherit from this one
	/// </summary>
	public class TopDownController : TopDownMonoBehaviour 
	{
		[Header("Gravity")]
		/// the current gravity to apply to our character (positive goes down, negative goes up, higher value, higher acceleration)
		[Tooltip("the current gravity to apply to our character (positive goes down, negative goes up, higher value, higher acceleration)")]
		public float Gravity = 40f;
		/// whether or not the gravity is currently being applied to this character
		[Tooltip("whether or not the gravity is currently being applied to this character")]
		public bool GravityActive = true;

		[Header("General Raycasts")]
		/// by default, the length of the raycasts used to get back to normal size will be auto generated based on your character's normal/standing height, but here you can specify a different value
		[Tooltip("by default, the length of the raycasts used to get back to normal size will be auto generated based on your character's normal/standing height, but here you can specify a different value")]
		public float CrouchedRaycastLengthMultiplier = 1f;
		/// if this is true, extra raycasts will be cast on all 4 sides to detect obstacles and feed the CollidingWithCardinalObstacle bool, only useful when working with grid movement, or if you need that info for some reason
		[Tooltip("if this is true, extra raycasts will be cast on all 4 sides to detect obstacles and feed the CollidingWithCardinalObstacle bool, only useful when working with grid movement, or if you need that info for some reason")]
		public bool PerformCardinalObstacleRaycastDetection = false;

		/// the current speed of the character
		[MMReadOnly]
		[Tooltip("the current speed of the character")]
		public Vector3 Speed;
		/// the current velocity
		[MMReadOnly]
		[Tooltip("the current velocity in units/second")]
		public Vector3 Velocity;
		/// the velocity of the character last frame
		[MMReadOnly]
		[Tooltip("the velocity of the character last frame")]
		public Vector3 VelocityLastFrame;
		/// the current acceleration
		[MMReadOnly]
		[Tooltip("the current acceleration")]
		public Vector3 Acceleration;
		/// whether or not the character is grounded
		[MMReadOnly]
		[Tooltip("whether or not the character is grounded")]
		public bool Grounded;
		/// whether or not the character got grounded this frame
		[MMReadOnly]
		[Tooltip("whether or not the character got grounded this frame")]
		public bool JustGotGrounded;
		/// the current movement of the character
		[MMReadOnly]
		[Tooltip("the current movement of the character")]
		public Vector3 CurrentMovement;
		/// the direction the character is going in
		[MMReadOnly]
		[Tooltip("the direction the character is going in")]
		public Vector3 CurrentDirection;
		/// the current friction
		[MMReadOnly]
		[Tooltip("the current friction")]
		public float Friction;
		/// the current added force, to be added to the character's movement
		[MMReadOnly]
		[Tooltip("the current added force, to be added to the character's movement")]
		public Vector3 AddedForce;
		/// whether or not the character is in free movement mode or not
		[MMReadOnly]
		[Tooltip("whether or not the character is in free movement mode or not")]
		public bool FreeMovement = true;
        
		/// the collider's center coordinates
		public virtual Vector3 ColliderCenter { get { return Vector3.zero; }  }
		/// the collider's bottom coordinates
		public virtual Vector3 ColliderBottom { get { return Vector3.zero; }  }
		/// the collider's top coordinates
		public virtual Vector3 ColliderTop { get { return Vector3.zero; }  }
		/// the object (if any) below our character
		public GameObject ObjectBelow { get; set; }
		/// the surface modifier object below our character (if any)
		public SurfaceModifier SurfaceModifierBelow { get; set; }
		public virtual Vector3 AppliedImpact { get { return _impact; } }
		/// whether or not the character is on a moving platform
		public virtual bool OnAMovingPlatform { get; set; }
		/// the speed of the moving platform
		public virtual Vector3 MovingPlatformSpeed { get; set; }

		// the obstacle left to this controller (only updated if DetectObstacles is called)
		public GameObject DetectedObstacleLeft { get; set; }
		// the obstacle right to this controller (only updated if DetectObstacles is called)
		public GameObject DetectedObstacleRight { get; set; }
		// the obstacle up to this controller (only updated if DetectObstacles is called)
		public GameObject DetectedObstacleUp { get; set; }
		// the obstacle down to this controller (only updated if DetectObstacles is called)
		public GameObject DetectedObstacleDown { get; set; }
		// true if an obstacle was detected in any of the cardinal directions
		public bool CollidingWithCardinalObstacle { get; set; }

		protected Vector3 _positionLastFrame;
		protected Vector3 _speedComputation;
		protected bool _groundedLastFrame;
		protected Vector3 _impact;		
		protected const float _smallValue=0.0001f;

		/// <summary>
		/// On awake, we initialize our current direction
		/// </summary>
		protected virtual void Awake()
		{			
			CurrentDirection = transform.forward;
		}

		/// <summary>
		/// On update, we check if we're grounded, and determine the direction
		/// </summary>
		protected virtual void Update()
		{
			CheckIfGrounded ();
			HandleFriction ();
			DetermineDirection ();
		}

		/// <summary>
		/// Computes the speed
		/// </summary>
		protected virtual void ComputeSpeed ()
		{
			if (Time.deltaTime != 0f)
			{
				Speed = (this.transform.position - _positionLastFrame) / Time.deltaTime;
			}			
			// we round the speed to 2 decimals
			Speed.x = Mathf.Round(Speed.x * 100f) / 100f;
			Speed.y = Mathf.Round(Speed.y * 100f) / 100f;
			Speed.z = Mathf.Round(Speed.z * 100f) / 100f;
			_positionLastFrame = this.transform.position;
		}

		/// <summary>
		/// Determines the controller's current direction
		/// </summary>
		protected virtual void DetermineDirection()
		{
			
		}

		/// <summary>
		/// Performs obstacle detection "manually"
		/// </summary>
		/// <param name="distance"></param>
		/// <param name="offset"></param>
		public virtual void DetectObstacles(float distance, Vector3 offset)
		{

		}

		/// <summary>
		/// Called at FixedUpdate
		/// </summary>
		protected virtual void FixedUpdate()
		{

		}

		/// <summary>
		/// On LateUpdate, computes the speed of the agent
		/// </summary>
		protected virtual void LateUpdate()
		{
		}

		/// <summary>
		/// Checks if the character is grounded
		/// </summary>
		protected virtual void CheckIfGrounded()
		{
			JustGotGrounded = (!_groundedLastFrame && Grounded);
			_groundedLastFrame = Grounded;
		}
        
		/// <summary>
		/// Use this to apply an impact to a controller, moving it in the specified direction at the specified force
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="force"></param>
		public virtual void Impact(Vector3 direction, float force)
		{

		}

		/// <summary>
		/// Sets gravity active or inactive
		/// </summary>
		/// <param name="status"></param>
		public virtual void SetGravityActive(bool status)
		{
			GravityActive = status;
		}

		/// <summary>
		/// Adds the specified force to the controller
		/// </summary>
		/// <param name="movement"></param>
		public virtual void AddForce(Vector3 movement)
		{

		}

		/// <summary>
		/// Sets the current movement of the controller to the specified Vector3
		/// </summary>
		/// <param name="movement"></param>
		public virtual void SetMovement(Vector3 movement)
		{

		}

		/// <summary>
		/// Moves the controller to the specified position (in world space)
		/// </summary>
		/// <param name="newPosition"></param>
		public virtual void MovePosition(Vector3 newPosition)
		{
			
		}

		/// <summary>
		/// Resizes the controller's collider
		/// </summary>
		/// <param name="newHeight"></param>
		public virtual void ResizeColliderHeight(float newHeight, bool translateCenter = false)
		{

		}

		/// <summary>
		/// Resets the controller's collider size
		/// </summary>
		public virtual void ResetColliderSize()
		{

		}

		/// <summary>
		/// Returns true if the controller's collider can go back to original size without hitting an obstacle, false otherwise
		/// </summary>
		/// <returns></returns>
		public virtual bool CanGoBackToOriginalSize()
		{
			return true;
		}

		/// <summary>
		/// Turns the controller's collisions on
		/// </summary>
		public virtual void CollisionsOn()
		{

		}

		/// <summary>
		/// Turns the controller's collisions off
		/// </summary>
		public virtual void CollisionsOff()
		{

		}

		/// <summary>
		/// Sets the controller's rigidbody to Kinematic (or not kinematic)
		/// </summary>
		/// <param name="state"></param>
		public virtual void SetKinematic(bool state)
		{

		}

		/// <summary>
		/// Handles friction collisions
		/// </summary>
		protected virtual void HandleFriction()
		{

		}

		/// <summary>
		/// Resets all values for this controller
		/// </summary>
		public virtual void Reset()
		{
			_impact = Vector3.zero;
			GravityActive = true;
			Speed = Vector3.zero;
			Velocity = Vector3.zero;
			VelocityLastFrame = Vector3.zero;
			Acceleration = Vector3.zero;
			Grounded = true;
			JustGotGrounded = false;
			CurrentMovement = Vector3.zero;
			CurrentDirection = Vector3.zero;
			AddedForce = Vector3.zero;
		}
	}
}