using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this script to a platform and it'll fall down when walked upon by a playable character
	/// Add an AutoRespawn component to your platform and it'll get reset when your character dies
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Falling Platform 3D")]
	public class FallingPlatform3D : TopDownMonoBehaviour 
	{
		/// the possible states for the platform
		public enum FallingPlatformStates { Idle, Shaking, Falling }

		/// the platform's current state
		[MMReadOnly]
		[Tooltip("the platform's current state")]
		public FallingPlatformStates State;

		/// if this is true, the platform will fall inevitably once touched
		[Tooltip("if this is true, the platform will fall inevitably once touched")]
		public bool InevitableFall = false;
		/// the time (in seconds) before the fall of the platform
		[Tooltip("the time (in seconds) before the fall of the platform")]
		public float TimeBeforeFall = 2f;
		/// if this is true, the object's rigidbody will be turned non kinematic when falling. Only works in 3D.
		[Tooltip("if this is true, the object's rigidbody will be turned non kinematic when falling. Only works in 3D.")]
		public bool UsePhysics = true;
		/// the speed at which the platforms falls
		[Tooltip("the speed at which the platforms falls")]
		public float NonPhysicsFallSpeed = 2f;

		// private stuff
		protected Animator _animator;
		protected Vector2 _newPosition;
		protected Bounds _bounds;
		protected Collider _collider;
		protected Vector3 _initialPosition;
		protected float _timer;
		protected float _platformTopY;
		protected Rigidbody _rigidbody;
		protected bool _contact = false;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// Grabs components and saves initial position and timer
		/// </summary>
		protected virtual void Initialization()
		{
			// we get the animator
			State = FallingPlatformStates.Idle;
			_animator = GetComponent<Animator>();
			_collider = GetComponent<Collider> ();
			_bounds=LevelManager.Instance.LevelBounds;
			_initialPosition = this.transform.position;
			_timer = TimeBeforeFall;
			_rigidbody = GetComponent<Rigidbody> ();

		}

		/// <summary>
		/// This is called every frame.
		/// </summary>
		protected virtual void FixedUpdate()
		{		
			// we send our various states to the animator.		
			UpdateAnimator ();	

			if (_contact)
			{
				_timer -= Time.deltaTime;
			}

			if (_timer < 0)
			{
				State = FallingPlatformStates.Falling;
				if (UsePhysics)
				{
					_rigidbody.isKinematic = false;
				}
				else
				{
					_newPosition = new Vector2(0,-NonPhysicsFallSpeed*Time.deltaTime);
					transform.Translate(_newPosition,Space.World);

					if (transform.position.y < _bounds.min.y)
					{
						DisableFallingPlatform ();
					}	
				}
			}
		}

		/// <summary>
		/// Disables the falling platform. We're not destroying it, so we can revive it on respawn
		/// </summary>
		protected virtual void DisableFallingPlatform()
		{
			this.gameObject.SetActive (false);					
			this.transform.position = _initialPosition;		
			_timer = TimeBeforeFall;
			State = FallingPlatformStates.Idle;
		}

		/// <summary>
		/// Updates the block's animator.
		/// </summary>
		protected virtual void UpdateAnimator()
		{				
			if (_animator!=null)
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, "Shaking", (State == FallingPlatformStates.Shaking));	
			}
		}

		/// <summary>
		/// Triggered when a TopDownController touches the platform
		/// </summary>
		/// <param name="controller">The TopDown controller that collides with the platform.</param>		
		public virtual void OnTriggerStay(Collider collider)
		{
			TopDownController controller = collider.gameObject.MMGetComponentNoAlloc<TopDownController>();
			if (controller==null)
			{
				return;
			}

			if (State == FallingPlatformStates.Falling)
			{
				return;
			}

			if (TimeBeforeFall>0)
			{
				_contact = true;
				State = FallingPlatformStates.Shaking;
			}	
			else
			{
				if (!InevitableFall)
				{
					_contact = false;
					State = FallingPlatformStates.Idle;
				}
			}
		}
		/// <summary>
		/// Triggered when a TopDownController exits the platform
		/// </summary>
		/// <param name="controller">The TopDown controller that collides with the platform.</param>
		protected virtual void OnTriggerExit(Collider collider)
		{
			if (InevitableFall)
			{
				return;
			}

			TopDownController controller = collider.gameObject.GetComponent<TopDownController>();
			if (controller==null)
				return;

			_contact = false;
			if (State == FallingPlatformStates.Shaking)
			{
				State = FallingPlatformStates.Idle;
			}
		}

		/// <summary>
		/// On Revive we restore this platform's state
		/// </summary>
		protected virtual void OnRevive()
		{
			this.transform.position = _initialPosition;		
			_timer = TimeBeforeFall;
			State = FallingPlatformStates.Idle;

		}
	}
}