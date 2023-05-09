using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This Action will make the Character patrol along the defined path (see the MMPath inspector for that) until it hits a wall or a hole while following a path.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMovePatrol2D")]
	//[RequireComponent(typeof(MMPath))]
	//[RequireComponent(typeof(Character))]
	//[RequireComponent(typeof(CharacterOrientation2D))]
	//[RequireComponent(typeof(CharacterMovement))]
	public class AIActionMovePatrol2D : AIAction
	{        
		[Header("Obstacle Detection")]
		/// If set to true, the agent will change direction when hitting an obstacle
		[Tooltip("If set to true, the agent will change direction when hitting an obstacle")]
		public bool ChangeDirectionOnObstacle = true;
		/// the layermask to look for obstacles on
		[Tooltip("the layermask to look for obstacles on")]
		public LayerMask ObstaclesLayerMask = LayerManager.ObstaclesLayerMask;
		/// the frequency (in seconds) at which to check for obstacles
		[Tooltip("the frequency (in seconds) at which to check for obstacles")]
		public float ObstaclesCheckFrequency = 1f;
		/// the coordinates of the last patrol point
		public Vector3 LastReachedPatrolPoint { get; set; }

		// private stuff
		protected TopDownController _controller;
		protected Character _character;
		protected CharacterOrientation2D _orientation2D;
		protected CharacterMovement _characterMovement;
		protected Health _health;
		protected Vector2 _direction;
		protected Vector2 _startPosition;
		protected Vector3 _initialScale;
		protected MMPath _mmPath;
		protected float _lastObstacleDetectionTimestamp = 0f;

		protected int _currentIndex = 0;
		protected int _indexLastFrame = -1;
		protected float _waitingDelay = 0f;

		/// <summary>
		/// On init we grab all the components we'll need
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			InitializePatrol();
		}
        
		/// <summary>
		/// On init we grab all the components we'll need
		/// </summary>
		protected virtual void InitializePatrol()
		{
			_controller = this.gameObject.GetComponentInParent<TopDownController>();
			_character = this.gameObject.GetComponentInParent<Character>();
			_orientation2D = _character?.FindAbility<CharacterOrientation2D>();
			_characterMovement = _character?.FindAbility<CharacterMovement>();
			_health = _character?.CharacterHealth;
			_mmPath = this.gameObject.GetComponentInParent<MMPath>();
			// initialize the start position
			_startPosition = transform.position;
			// initialize the direction
			_direction = _orientation2D.IsFacingRight ? Vector2.right : Vector2.left;
			_initialScale = transform.localScale;
			_currentIndex = 0;
			_indexLastFrame = -1;
			_waitingDelay = 0;
			_initialized = true;
		}


		/// <summary>
		/// On PerformAction we patrol
		/// </summary>
		public override void PerformAction()
		{
			Patrol();
		}

		/// <summary>
		/// This method initiates all the required checks and moves the character
		/// </summary>
		protected virtual void Patrol()
		{
			_waitingDelay -= Time.deltaTime;

			if (_character == null)
			{
				return;
			}
			if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
			    || (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
			{
				return;
			}

			if (_waitingDelay > 0)
			{
				_characterMovement.SetHorizontalMovement(0f);
				_characterMovement.SetVerticalMovement(0f);
				return;
			}

			// moves the agent in its current direction
			CheckForObstacles();

			_currentIndex = _mmPath.CurrentIndex();
			if (_currentIndex != _indexLastFrame)
			{
				LastReachedPatrolPoint = _mmPath.CurrentPoint();
				_waitingDelay = _mmPath.PathElements[_currentIndex].Delay;
			}

			_direction = _mmPath.CurrentPoint() - this.transform.position;
			_direction = _direction.normalized;

			_characterMovement.SetHorizontalMovement(_direction.x);
			_characterMovement.SetVerticalMovement(_direction.y);

			_indexLastFrame = _currentIndex;
		}

		/// <summary>
		/// Draws bounds gizmos
		/// </summary>
		protected virtual void OnDrawGizmosSelected()
		{
			if (_mmPath == null)
			{
				return;
			}
			Gizmos.color = MMColors.IndianRed;
			Gizmos.DrawLine(this.transform.position, _mmPath.CurrentPoint());
		}

		/// <summary>
		/// When exiting the state we reset our movement
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();
			_characterMovement?.SetHorizontalMovement(0f);
			_characterMovement?.SetVerticalMovement(0f);
		}

		/// <summary>
		/// Checks for a wall and changes direction if it meets one
		/// </summary>
		protected virtual void CheckForObstacles()
		{
			if (!ChangeDirectionOnObstacle)
			{
				return;
			}

			if (Time.time - _lastObstacleDetectionTimestamp < ObstaclesCheckFrequency)
			{
				return;
			}

			RaycastHit2D raycast = MMDebug.RayCast(_controller.ColliderCenter, _direction, 1f, ObstaclesLayerMask, MMColors.Gold, true);

			// if the agent is colliding with something, make it turn around
			if (raycast)
			{
				ChangeDirection();
			}

			_lastObstacleDetectionTimestamp = Time.time;
		}
        
		/// <summary>
		/// Changes the current movement direction
		/// </summary>
		public virtual void ChangeDirection()
		{
			_direction = -_direction;
			_mmPath.ChangeDirection();
		}
        
		/// <summary>
		/// When reviving we make sure our directions are properly setup
		/// </summary>
		protected virtual void OnRevive()
		{
			if (!_initialized)
			{
				return;
			}
            
			if (_orientation2D != null)
			{
				_direction = _orientation2D.IsFacingRight ? Vector2.right : Vector2.left;
			}
            
			InitializePatrol();
		}

		/// <summary>
		/// On enable we start listening for OnRevive events
		/// </summary>
		protected virtual void OnEnable()
		{
			if (_health == null)
			{
				_health = this.gameObject.GetComponent<Health>();
			}

			if (_health != null)
			{
				_health.OnRevive += OnRevive;
			}
		}

		/// <summary>
		/// On disable we stop listening for OnRevive events
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				_health.OnRevive -= OnRevive;
			}
		}
	}
}