using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Requires a CharacterGridMovement ability. 
	/// Makes the character move randomly in the grid, until it finds an obstacle in its path, in which case it'll pick a new direction at random
	/// Supports both 2D and 3D grids
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveRandomlyGrid")]
	//[RequireComponent(typeof(CharacterGridMovement))]
	public class AIActionMoveRandomlyGrid : AIAction
	{
		public enum Modes { TwoD, ThreeD }

		[Header("Dimension")]
		public Modes Mode = Modes.ThreeD;

		[Header("Duration")]
		/// the maximum time a character can spend going in a direction without changing
		[Tooltip("the maximum time a character can spend going in a direction without changing")]
		public float MaximumDurationInADirection = 3f;

		[Header("Obstacles")]
		/// the layers the character will try to avoid
		[Tooltip("the layers the character will try to avoid")]
		public LayerMask ObstacleLayerMask = LayerManager.ObstaclesLayerMask;
		/// the minimum distance from the target this Character can reach.
		[Tooltip("the minimum distance from the target this Character can reach.")]
		public float ObstaclesDetectionDistance = 1f;
		/// the frequency (in seconds) at which to check for obstacles
		[Tooltip("the frequency (in seconds) at which to check for obstacles")]
		public float ObstaclesCheckFrequency = 1f;
		/// the minimal random direction to randomize from
		[Tooltip("the minimal random direction to randomize from")]
		public Vector2 MinimumRandomDirection = new Vector2(-1f, -1f);
		/// the maximum random direction to randomize from
		[Tooltip("the maximum random direction to randomize from")]
		public Vector2 MaximumRandomDirection = new Vector2(1f, 1f);
		/// if this is true, the AI will avoid 180° turns if possible
		[Tooltip("if this is true, the AI will avoid 180° turns if possible")]
		public bool Avoid180 = true;

		protected CharacterGridMovement _characterGridMovement;
		protected TopDownController _topDownController;
		protected Vector2 _direction;
		protected Collider _collider;
		protected Collider2D _collider2D;
		protected float _lastObstacleDetectionTimestamp = 0f;
		protected float _lastDirectionChangeTimestamp = 0f;
		protected Vector3 _rayDirection;
		protected Vector2 _temp2DVector;
		protected Vector3 _temp3DVector;

		protected Vector2[] _raycastDirections2D;
		protected Vector3[] _raycastDirections3D;
		protected RaycastHit _hit;
		protected RaycastHit2D _hit2D;

		/// <summary>
		/// On start we grab our character movement component and pick a random direction
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_characterGridMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterGridMovement>();
			_topDownController = this.gameObject.GetComponentInParent<TopDownController>();
			_collider = this.gameObject.GetComponentInParent<Collider>();
			_collider2D = this.gameObject.GetComponentInParent<Collider2D>();

			_raycastDirections2D = new[] { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
			_raycastDirections3D = new[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

			PickNewDirection();
		}

		/// <summary>
		/// On PerformAction we move
		/// </summary>
		public override void PerformAction()
		{
			CheckForObstacles();
			CheckForDuration();
			Move();
		}

		/// <summary>
		/// Moves the character
		/// </summary>
		protected virtual void Move()
		{
			_characterGridMovement.SetMovement(_direction);
		}

		/// <summary>
		/// Checks for obstacles by casting a ray
		/// </summary>
		protected virtual void CheckForObstacles()
		{
			if (Time.time - _lastObstacleDetectionTimestamp < ObstaclesCheckFrequency)
			{
				return;
			}

			_lastObstacleDetectionTimestamp = Time.time;

			if (Mode == Modes.ThreeD)
			{
				_temp3DVector = _direction;
				_temp3DVector.z = _direction.y;
				_temp3DVector.y = 0;
				_hit = MMDebug.Raycast3D(_collider.bounds.center, _temp3DVector, ObstaclesDetectionDistance, ObstacleLayerMask, Color.gray);
				if (_topDownController.CollidingWithCardinalObstacle)
				{
					PickNewDirection();
				}
			}
			else
			{
				_temp2DVector = _direction;
				_hit2D = MMDebug.RayCast(_collider2D.bounds.center, _temp2DVector, ObstaclesDetectionDistance, ObstacleLayerMask, Color.gray);
				if (_topDownController.CollidingWithCardinalObstacle)
				{
					PickNewDirection();
				}

			}
		}

		/// <summary>
		/// Tests and picks a new direction to move towards
		/// </summary>
		protected virtual void PickNewDirection()
		{
			int retries = 0;
			switch (Mode)
			{

				case Modes.ThreeD:  
					while (retries < 10)
					{
						retries++;
						int random = MMMaths.RollADice(4) - 1;
						_temp3DVector = _raycastDirections3D[random];
                        
						if (Avoid180)
						{
							if ((_temp3DVector.x == -_direction.x) && (Mathf.Abs(_temp3DVector.x) > 0))
							{
								continue;
							}
							if ((_temp3DVector.y == -_direction.y) && (Mathf.Abs(_temp3DVector.y) > 0))
							{
								continue;
							}
						}

						_hit = MMDebug.Raycast3D(_collider.bounds.center, _temp3DVector, ObstaclesDetectionDistance, ObstacleLayerMask, Color.gray);
						if (_hit.collider == null)
						{
							_direction = _temp3DVector;
							_direction.y = _temp3DVector.z;

							return;
						}
					}
					break;

				case Modes.TwoD:
					while (retries < 10)
					{
						retries++;
						int random = MMMaths.RollADice(4) - 1;
						_temp2DVector = _raycastDirections2D[random];

						if (Avoid180)
						{
							if ((_temp2DVector.x == -_direction.x) && (Mathf.Abs(_temp2DVector.x) > 0))
							{
								continue;
							}
							if ((_temp2DVector.y == -_direction.y) && (Mathf.Abs(_temp2DVector.y) > 0))
							{
								continue;
							}
						}

						_hit2D = MMDebug.RayCast(_collider2D.bounds.center, _temp2DVector, ObstaclesDetectionDistance, ObstacleLayerMask, Color.gray);
						if (_hit2D.collider == null)
						{
							_direction = _temp2DVector;

							return;
						}
					}
					break;
			}
		}

		/// <summary>
		/// Checks whether or not we should pick a new direction at random
		/// </summary>
		protected virtual void CheckForDuration()
		{
			if (Time.time - _lastDirectionChangeTimestamp > MaximumDurationInADirection)
			{
				PickNewDirection();
				_lastDirectionChangeTimestamp = Time.time;
			}
		}

		/// <summary>
		/// On exit state we stop our movement
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();

			_characterGridMovement?.StopMovement();
		}
	}
}