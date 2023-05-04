using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this ability to a Character to have it move on a grid.
	/// This will require a GridManager be present in your scene
	/// DO NOT use that component and a CharacterMovement component on the same character.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Grid Movement")] 
	public class CharacterGridMovement : CharacterAbility 
	{
		/// the possible directions on the grid
		public enum GridDirections { None, Up, Down, Left, Right }
		/// the possible input modes
		public enum InputModes { InputManager, Script }
		/// the possible modes
		public enum DimensionModes { TwoD, ThreeD }

		[Header("Movement")]

		/// the maximum speed of the character
		[Tooltip("the maximum speed of the character")]
		public float MaximumSpeed = 8;
		/// the acceleration of the character
		[Tooltip("the acceleration of the character")]
		public float Acceleration = 5;
		/// the current speed at which the character is going
		[MMReadOnly]
		[Tooltip("the current speed at which the character is going")]
		public float CurrentSpeed;
		/// a multiplier to apply to the maximum speed
		[MMReadOnly]
		[Tooltip("a multiplier to apply to the maximum speed")]
		public float MaximumSpeedMultiplier = 1f;
		/// a multiplier to apply to the acceleration, letting you modify it safely from outside
		[MMReadOnly]
		[Tooltip("a multiplier to apply to the acceleration, letting you modify it safely from outside")]
		public float AccelerationMultiplier = 1f;

		[Header("Input Settings")]

		/// whether to use the input manager or a script to feed the inputs
		[Tooltip("whether to use the input manager or a script to feed the inputs")]
		public InputModes InputMode = InputModes.InputManager;
		/// whether or not input should be buffered (to anticipate the next turn)
		[Tooltip("whether or not input should be buffered (to anticipate the next turn)")]
		public bool UseInputBuffer = true;
		/// the size of the input buffer (in grid units)
		[Tooltip("the size of the input buffer (in grid units)")]
		public int BufferSize = 2;
		/// whether or not the agent can perform fast direction changes such as U turns
		[Tooltip("whether or not the agent can perform fast direction changes such as U turns")]
		public bool FastDirectionChanges = true;
		/// the speed threshold after which the character is not considered idle anymore
		[Tooltip("the speed threshold after which the character is not considered idle anymore")]
		public float IdleThreshold = 0.05f;
		/// if this is true, movement values will be normalized - prefer checking this when using mobile controls
		[Tooltip("if this is true, movement values will be normalized - prefer checking this when using mobile controls")]
		public bool NormalizedInput = false;

		[Header("Grid")]

		/// the offset to apply when detecting obstacles
		[Tooltip("the offset to apply when detecting obstacles")]
		public Vector3 ObstacleDetectionOffset = new Vector3(0f, 0.5f, 0f);
		/// the position of the object on the grid
		public Vector3Int CurrentGridPosition { get; protected set; }
		/// the position the object will be at when it reaches its next perfect tile
		public Vector3Int TargetGridPosition { get; protected set; }
		/// this is true everytime a character is at the exact position of a tile
		[MMReadOnly]
		[Tooltip("this is true everytime a character is at the exact position of a tile")]
		public bool PerfectTile;
		/// the coordinates of the cell this character currently occupies
		[MMReadOnly]
		[Tooltip("the coordinates of the cell this character currently occupies")]
		public Vector3Int CurrentCellCoordinates;
		/// whether this character is in 2D or 3D. This gets automatically computed at start
		[MMReadOnly]
		[Tooltip("whether this character is in 2D or 3D. This gets automatically computed at start")]
		public DimensionModes DimensionMode = DimensionModes.TwoD;

		[Header("Test")]
		[MMInspectorButton("Left")]
		public bool LeftButton;
		[MMInspectorButton("Right")]
		public bool RightButton;
		[MMInspectorButton("Up")]
		public bool UpButton;
		[MMInspectorButton("Down")]
		public bool DownButton;
		[MMInspectorButton("StopMovement")]
		public bool StopButton;
		[MMInspectorButton("LeftOneCell")]
		public bool LeftOneCellButton;
		[MMInspectorButton("RightOneCell")]
		public bool RightOneCellButton;
		[MMInspectorButton("UpOneCell")]
		public bool UpOneCellButton;
		[MMInspectorButton("DownOneCell")]
		public bool DownOneCellButton;

		protected GridDirections _inputDirection;
		protected GridDirections _currentDirection = GridDirections.Up;
		protected GridDirections _bufferedDirection;
		protected bool _movementInterruptionBuffered = false;
		protected bool _perfectTile = false;                
		protected Vector3 _inputMovement;
		protected Vector3 _endWorldPosition;
		protected bool _movingToNextGridUnit = false;
		protected bool _stopBuffered = false;        
		protected int _lastBufferInGridUnits;
		protected bool _agentMoving;
		protected GridDirections _newDirection;
		protected float _horizontalMovement;
		protected float _verticalMovement;
		protected Vector3Int _lastOccupiedCellCoordinates;
		protected const string _speedAnimationParameterName = "Speed";
		protected const string _walkingAnimationParameterName = "Walking";
		protected const string _idleAnimationParameterName = "Idle";
		protected int _speedAnimationParameter;
		protected int _walkingAnimationParameter;
		protected int _idleAnimationParameter;
		protected bool _firstPositionRegistered = false;
		protected Vector3Int _newCellCoordinates = Vector3Int.zero;
		protected Vector3 _lastCurrentDirection;

		protected bool _leftPressedLastFrame = false;
		protected bool _rightPressedLastFrame = false;
		protected bool _downPressedLastFrame = false;
		protected bool _upPressedLastFrame = false;

		/// <summary>
		/// A public method used to set the character's direction (only in Script InputMode)
		/// </summary>
		/// <param name="newMovement"></param>
		public virtual void SetMovement(Vector2 newMovement)
		{
			_horizontalMovement = newMovement.x;
			_verticalMovement = newMovement.y;
		}

		/// <summary>
		/// Moves the character one cell to the left
		/// </summary>
		public virtual void LeftOneCell()
		{
			StartCoroutine(OneCell(Vector2.left));
		}

		/// <summary>
		/// Moves the character one cell to the right
		/// </summary>
		public virtual void RightOneCell()
		{
			StartCoroutine(OneCell(Vector2.right));
		}
        
		/// <summary>
		/// Moves the character one cell up
		/// </summary>
		public virtual void UpOneCell()
		{
			StartCoroutine(OneCell(Vector2.up));
		}

		/// <summary>
		/// Moves the character one cell down
		/// </summary>
		public virtual void DownOneCell()
		{
			StartCoroutine(OneCell(Vector2.down));
		}

		/// <summary>
		/// An internal coroutine used to move the character one cell in the specified direction
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		protected virtual IEnumerator OneCell(Vector2 direction)
		{
			SetMovement(direction);
			yield return null;
			SetMovement(Vector2.zero);
		}

		/// <summary>
		/// Sets script movement to left
		/// </summary>
		public virtual void Left() { SetMovement(Vector2.left); }
		/// <summary>
		/// Sets script movement to right
		/// </summary>
		public virtual void Right() { SetMovement(Vector2.right); }
		/// <summary>
		/// Sets script movement to up
		/// </summary>
		public virtual void Up() { SetMovement(Vector2.up); }
		/// <summary>
		/// Sets script movement to down
		/// </summary>
		public virtual void Down() { SetMovement(Vector2.down); }
		/// <summary>
		/// Stops script movement
		/// </summary>
		public virtual void StopMovement() { SetMovement(Vector2.zero); }

		/// <summary>
		/// On Initialization, we set our movement speed to WalkSpeed.
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization ();
			DimensionMode = DimensionModes.ThreeD;
			if (_controller.gameObject.MMGetComponentNoAlloc<TopDownController2D>() != null)
			{
				DimensionMode = DimensionModes.TwoD;
				_controller.FreeMovement = false;
			}
			_controller.PerformCardinalObstacleRaycastDetection = true;
			_bufferedDirection = GridDirections.None;
		} 

		/// <summary>
		/// Registers the first position to the GridManager
		/// </summary>
		protected virtual void RegisterFirstPosition()
		{
			if (!_firstPositionRegistered)
			{
				_endWorldPosition = this.transform.position;
				_lastOccupiedCellCoordinates = GridManager.Instance.WorldToCellCoordinates(_endWorldPosition);
				CurrentCellCoordinates = _lastOccupiedCellCoordinates;
				GridManager.Instance.OccupyCell(_lastOccupiedCellCoordinates);
				GridManager.Instance.SetLastPosition(this.gameObject, _lastOccupiedCellCoordinates);
				GridManager.Instance.SetNextPosition(this.gameObject, _lastOccupiedCellCoordinates);
				_firstPositionRegistered = true;
			}
		}
        
		public virtual void SetCurrentWorldPositionAsNewPosition()
		{
			_endWorldPosition = this.transform.position;
			GridManager.Instance.FreeCell(_lastOccupiedCellCoordinates);
			_lastOccupiedCellCoordinates = GridManager.Instance.WorldToCellCoordinates(_endWorldPosition);
			CurrentCellCoordinates = _lastOccupiedCellCoordinates;
			GridManager.Instance.OccupyCell(_lastOccupiedCellCoordinates);
			GridManager.Instance.SetLastPosition(this.gameObject, _lastOccupiedCellCoordinates);
			GridManager.Instance.SetNextPosition(this.gameObject, _lastOccupiedCellCoordinates);
			_firstPositionRegistered = true;
		}
        
		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
				|| (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Stunned))
			{
				return;
			}

			RegisterFirstPosition();

			_controller.DetectObstacles(GridManager.Instance.GridUnitSize, ObstacleDetectionOffset);
			DetermineInputDirection();
			ApplyAcceleration();
			HandleMovement();
			HandleState();
		}
        
		/// <summary>
		/// Grabs horizontal and vertical input and stores them
		/// </summary>
		protected override void HandleInput()
		{
			if (!AbilityAuthorized)
			{
				return;
			}
			if (InputMode == InputModes.InputManager)
			{
				_horizontalMovement = NormalizedInput ? Mathf.Round(_horizontalInput) : _horizontalInput;
				_verticalMovement = NormalizedInput ? Mathf.Round(_verticalInput) : _verticalInput;
			}            
		}

		/// <summary>
		/// Based on press times, determines the input direction
		/// </summary>
		protected virtual void DetermineInputDirection()
		{
			// if we're not pressing any direction, we stop
			if ((Mathf.Abs(_horizontalMovement) <= IdleThreshold) && (Mathf.Abs(_verticalMovement) <= IdleThreshold))
			{
				Stop(_newDirection);
				_newDirection = GridDirections.None;
				_inputMovement = Vector3.zero;
			}
            
			// if we're pressing a direction for the first time, it becomes our new direction
			if ((_horizontalMovement < 0f) && !_leftPressedLastFrame) { _newDirection = GridDirections.Left; _inputMovement = Vector3.left; }
			if ((_horizontalMovement > 0f) && !_rightPressedLastFrame) { _newDirection = GridDirections.Right; _inputMovement = Vector3.right; }
			if ((_verticalMovement < 0f) && !_downPressedLastFrame) { _newDirection = GridDirections.Down; _inputMovement = Vector3.down; }
			if ((_verticalMovement > 0f) && !_upPressedLastFrame) { _newDirection = GridDirections.Up; _inputMovement = Vector3.up; }
            
			// if we were pressing a direction, and have just released it, we'll look for an other direction
			if ((_horizontalMovement == 0f) && (_leftPressedLastFrame || _rightPressedLastFrame)) { _newDirection = GridDirections.None; }
			if ((_verticalMovement == 0f) && (_downPressedLastFrame || _upPressedLastFrame)) { _newDirection = GridDirections.None; }

			// if at this point we have no direction, we take any pressed one
			if (_newDirection == GridDirections.None)
			{
				if (_horizontalMovement < 0f) { _newDirection = GridDirections.Left; _inputMovement = Vector3.left; }
				if (_horizontalMovement > 0f) { _newDirection = GridDirections.Right; _inputMovement = Vector3.right;  }
				if (_verticalMovement < 0f) { _newDirection = GridDirections.Down; _inputMovement = Vector3.down; }
				if (_verticalMovement > 0f) { _newDirection = GridDirections.Up; _inputMovement = Vector3.up; }
			}
            
			_inputDirection = _newDirection;
            
			// we store our presses for next frame
			_leftPressedLastFrame = (_horizontalMovement < 0f);
			_rightPressedLastFrame = (_horizontalMovement > 0f);
			_downPressedLastFrame = (_verticalMovement < 0f);
			_upPressedLastFrame = (_verticalMovement > 0f);
		}

		/// <summary>
		/// Stops the character and has it face the specified direction
		/// </summary>
		/// <param name="direction"></param>
		public virtual void Stop(GridDirections direction)
		{
			if (direction == GridDirections.None)
			{
				return;
			}
			_bufferedDirection = direction;
			_stopBuffered = true;
		}

		/// <summary>
		/// Modifies the current speed based on the acceleration
		/// </summary>
		protected virtual void ApplyAcceleration()
		{
			if ((_currentDirection != GridDirections.None) && (CurrentSpeed < MaximumSpeed * MaximumSpeedMultiplier))
			{
				CurrentSpeed = CurrentSpeed + Acceleration * AccelerationMultiplier * Time.deltaTime;
				CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, MaximumSpeed * MaximumSpeedMultiplier);
			}
		}

		/// <summary>
		/// Moves the character on the grid
		/// </summary>
		protected virtual void HandleMovement()
		{
			_perfectTile = false;
			PerfectTile = false;
			ProcessBuffer();
            
			// if we're performing a U turn, we change direction if allowed
			/*if (FastDirectionChanges && _agentMoving && !_stopBuffered && _movingToNextGridUnit)
			{
			    if ((_bufferedDirection != GridDirections.None) 
			        && (_bufferedDirection == GetInverseDirection(_currentDirection)))
			    {
			        _newCellCoordinates = CurrentCellCoordinates + ConvertDirectionToVector3Int(_currentDirection);
			        _endWorldPosition = GridManager.Instance.CellToWorldCoordinates(_newCellCoordinates);
			        _endWorldPosition = DimensionClamp(_endWorldPosition);
			        _currentDirection = _bufferedDirection;
			    }
			}*/

			// if we're not in between grid cells
			if (!_movingToNextGridUnit)
			{
				PerfectTile = true;

				// if we have a stop buffered
				if (_movementInterruptionBuffered)
				{
					_perfectTile = true;
					_movementInterruptionBuffered = false;
					return;
				}

				// if we don't have a direction anymore
				if (_bufferedDirection == GridDirections.None)
				{
					_currentDirection = GridDirections.None;
					_bufferedDirection = GridDirections.None;
					_agentMoving = false;
					CurrentSpeed = 0;

					GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));
					GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));

					return;
				}

				// we check if we can move in the selected direction
				if (((_currentDirection == GridDirections.Left) && (_controller.DetectedObstacleLeft != null))
				    || ((_currentDirection == GridDirections.Right) && (_controller.DetectedObstacleRight != null))
				    || ((_currentDirection == GridDirections.Up) && (_controller.DetectedObstacleUp != null))
				    || ((_currentDirection == GridDirections.Down) && (_controller.DetectedObstacleDown != null)))
				{
					_currentDirection = _bufferedDirection;
                    
					GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));
					GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));

					return;
				}

				// we check if we can move in the selected direction
				if (((_bufferedDirection == GridDirections.Left) && !(_controller.DetectedObstacleLeft != null))
				    || ((_bufferedDirection == GridDirections.Right) && !(_controller.DetectedObstacleRight != null))
				    || ((_bufferedDirection == GridDirections.Up) && !(_controller.DetectedObstacleUp != null))
				    || ((_bufferedDirection == GridDirections.Down) && !(_controller.DetectedObstacleDown != null)))
				{
					_currentDirection = _bufferedDirection;
				}

				// we compute and move towards our new destination
				_movingToNextGridUnit = true;
				DetermineEndPosition();

				// we make sure the target cell is free
				if (GridManager.Instance.CellIsOccupied(TargetGridPosition))
				{
					_movingToNextGridUnit = false;
					_currentDirection = GridDirections.None;
					_bufferedDirection = GridDirections.None;
					_agentMoving = false;
					CurrentSpeed = 0;
				}
				else
				{
					GridManager.Instance.FreeCell(_lastOccupiedCellCoordinates);
					GridManager.Instance.SetLastPosition(this.gameObject, _lastOccupiedCellCoordinates);
					GridManager.Instance.SetNextPosition(this.gameObject, TargetGridPosition);
					GridManager.Instance.OccupyCell(TargetGridPosition);
					CurrentCellCoordinates = TargetGridPosition;
					_lastOccupiedCellCoordinates = TargetGridPosition;
				}
			}

			// computes our new grid position
			TargetGridPosition = GridManager.Instance.WorldToCellCoordinates(_endWorldPosition);
            
			// moves the controller to the next position
			Vector3 newPosition = Vector3.MoveTowards(transform.position, _endWorldPosition, Time.deltaTime * CurrentSpeed);

			_lastCurrentDirection = _endWorldPosition - this.transform.position;
			_lastCurrentDirection = _lastCurrentDirection.MMRound();
			if (_lastCurrentDirection != Vector3.zero)
			{
				_controller.CurrentDirection = _lastCurrentDirection;
			}
			_controller.MovePosition(newPosition);
		}

		/// <summary>
		/// Processes buffered input
		/// </summary>
		protected virtual void ProcessBuffer()
		{
			// if we have a direction in input, it becomes our new buffered direction
			if ((_inputDirection != GridDirections.None) && !_stopBuffered)
			{
				_bufferedDirection = _inputDirection;
				_lastBufferInGridUnits = BufferSize;
			}

			// if we're not moving and get an input, we start moving
			if (!_agentMoving && _inputDirection != GridDirections.None)
			{
				_currentDirection = _inputDirection;
				_agentMoving = true;
			}

			// if we've reached our next tile, we're not moving anymore
			if (_movingToNextGridUnit && (transform.position == _endWorldPosition))
			{
				_movingToNextGridUnit = false;
				CurrentGridPosition = GridManager.Instance.WorldToCellCoordinates(_endWorldPosition);
			}

			// we handle the buffer. If we have a buffered direction, are on a perfect tile, and don't have an input
			if ((_bufferedDirection != GridDirections.None) && !_movingToNextGridUnit && (_inputDirection == GridDirections.None) && UseInputBuffer)
			{
				// we reduce the buffer counter
				_lastBufferInGridUnits--;
				// if our buffer is expired, we revert to our current direction
				if ((_lastBufferInGridUnits < 0) && (_bufferedDirection != _currentDirection))
				{
					_bufferedDirection = _currentDirection;
				}
			}

			// if we have a stop planned and are not moving, we stop
			if ((_stopBuffered) && !_movingToNextGridUnit)
			{
				_bufferedDirection = GridDirections.None;
				_stopBuffered = false;
			}
		}

		/// <summary>
		/// Determines the end position based on the current direction
		/// </summary>
		protected virtual void DetermineEndPosition()
		{
			TargetGridPosition = CurrentCellCoordinates + ConvertDirectionToVector3Int(_currentDirection);
			_endWorldPosition = GridManager.Instance.CellToWorldCoordinates(TargetGridPosition);
			// we maintain our z(2D) or y (3D)
			_endWorldPosition = DimensionClamp(_endWorldPosition);
		}

		protected Vector3 DimensionClamp(Vector3 newPosition)
		{
			if (DimensionMode == DimensionModes.TwoD)
			{
				newPosition.z = this.transform.position.z;
			}
			else
			{
				newPosition.y = this.transform.position.y;
			}

			return newPosition;
		}

		/// <summary>
		/// Converts a GridDirection to a Vector3 based on the current D
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		protected virtual Vector3Int ConvertDirectionToVector3Int(GridDirections direction)
		{
			if (direction != GridDirections.None)
			{
				if (direction == GridDirections.Left) return Vector3Int.left;
				if (direction == GridDirections.Right) return Vector3Int.right;
                
				if (DimensionMode == DimensionModes.TwoD)
				{
					if (direction == GridDirections.Up) return Vector3Int.up;
					if (direction == GridDirections.Down) return Vector3Int.down;
				}
				else
				{
					if (direction == GridDirections.Up) return Vector3Int.RoundToInt(Vector3.forward);
					if (direction == GridDirections.Down) return Vector3Int.RoundToInt(Vector3.back);
				}                
			}
			return Vector3Int.zero;
		}

		/// <summary>
		/// Returns the opposite direction of a GridDirection
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		protected virtual GridDirections GetInverseDirection(GridDirections direction)
		{
			if (direction != GridDirections.None)
			{
				if (direction == GridDirections.Left) return GridDirections.Right;
				if (direction == GridDirections.Right) return GridDirections.Left;
				if (direction == GridDirections.Up) return GridDirections.Down;
				if (direction == GridDirections.Down) return GridDirections.Up;
			}
			return GridDirections.None;
		}

		protected virtual void HandleState()
		{
			if (_movingToNextGridUnit)
			{
				if (_movement.CurrentState != CharacterStates.MovementStates.Walking)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Walking);
					PlayAbilityStartFeedbacks();
				}
			}
			else
			{
				if (_movement.CurrentState != CharacterStates.MovementStates.Idle)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
					if (_startFeedbackIsPlaying)
					{
						StopStartFeedbacks();
						PlayAbilityStopFeedbacks();
					}
				}
			}            
		}

		/// <summary>
		/// On death we free our last occupied cell
		/// </summary>
		protected override void OnDeath()
		{
			base.OnDeath();
			GridManager.Instance.FreeCell(_lastOccupiedCellCoordinates);
		}

		/// <summary>
		/// On Respawn we force a reinitialization
		/// </summary>
		protected override void OnRespawn()
		{
			base.OnRespawn();
			Initialization();
			_firstPositionRegistered = false;
		}


		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
			RegisterAnimatorParameter (_walkingAnimationParameterName, AnimatorControllerParameterType.Bool, out _walkingAnimationParameter);
			RegisterAnimatorParameter (_idleAnimationParameterName, AnimatorControllerParameterType.Bool, out _idleAnimationParameter);
		}

		/// <summary>
		/// Sends the current speed and the current value of the Walking state to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _speedAnimationParameter, CurrentSpeed, _character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Walking),_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Idle),_character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}