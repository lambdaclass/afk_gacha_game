using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A manager required in your scenes that use CharacterGridMovement.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Managers/GridManager")]
	public class GridManager : MMSingleton<GridManager>
	{
		/// the possible types of debug modes
		public enum DebugDrawModes { TwoD, ThreeD }

		[Header("Grid")]

		/// the origin of the grid in world space
		[Tooltip("the origin of the grid in world space")]
		public Transform GridOrigin;
		/// the size of each square grid cell
		[Tooltip("the size of each square grid cell")]
		public float GridUnitSize = 1f;

		[Header("Debug")]

		/// whether or not to draw the debug grid
		[Tooltip("whether or not to draw the debug grid")]
		public bool DrawDebugGrid = true;
		/// the mode in which to draw the debug grid
		[MMCondition("DrawDebugGrid", true)]
		[Tooltip("the mode in which to draw the debug grid")]
		public DebugDrawModes DebugDrawMode = DebugDrawModes.TwoD;
		/// the size (in squares of the debug grid)
		[MMCondition("DrawDebugGrid", true)]
		[Tooltip("the size (in squares of the debug grid)")]
		public int DebugGridSize = 30;
		/// the color to use to draw the debug grid lines
		[MMCondition("DrawDebugGrid", true)]
		[Tooltip("the color to use to draw the debug grid lines")]
		public Color CellBorderColor = new Color(60f, 221f, 255f, 1f);
		/// the color to use to draw the debug grid cells backgrounds
		[MMCondition("DrawDebugGrid", true)]
		[Tooltip("the color to use to draw the debug grid cells backgrounds")]
		public Color InnerColor = new Color(60f, 221f, 255f, 0.3f);
		/// a list of all cells currently occupied
		[HideInInspector]
		public List<Vector3> OccupiedGridCells;
		/// a dictionary holding all last positions registered by objects traveling on the grid
		[HideInInspector]
		public Dictionary<GameObject, Vector3Int> LastPositions;
		/// a dictionary holding all next positions registered by objects traveling on the grid
		[HideInInspector]
		public Dictionary<GameObject, Vector3Int> NextPositions;

		protected Vector3 _newGridPosition;
		protected Vector3 _debugOrigin = Vector3.zero;
		protected Vector3 _debugDestination = Vector3.zero;
		protected Vector3Int _workCoordinate = Vector3Int.zero;

		/// <summary>
		/// On start we initialize our lists and dictionaries
		/// </summary>
		protected virtual void Start()
		{
			OccupiedGridCells = new List<Vector3>();
			LastPositions = new Dictionary<GameObject, Vector3Int>();
			NextPositions = new Dictionary<GameObject, Vector3Int>();
		}

		/// <summary>
		/// Returns true if the cell at the specified coordinates is occupied, false otherwise
		/// </summary>
		/// <param name="cellCoordinates"></param>
		/// <returns></returns>
		public virtual bool CellIsOccupied(Vector3 cellCoordinates)
		{
			return OccupiedGridCells.Contains(cellCoordinates);
		}

		/// <summary>
		/// Marks the specified cell as occupied
		/// </summary>
		/// <param name="cellCoordinates"></param>
		public virtual void OccupyCell(Vector3 cellCoordinates)
		{
			if (!OccupiedGridCells.Contains(cellCoordinates))
			{
				OccupiedGridCells.Add(cellCoordinates);
			}
		}

		/// <summary>
		/// Marks the specified cell as unoccupied
		/// </summary>
		/// <param name="cellCoordinates"></param>
		public virtual void FreeCell(Vector3 cellCoordinates)
		{
			if (OccupiedGridCells.Contains(cellCoordinates))
			{
				OccupiedGridCells.Remove(cellCoordinates);
			}
		}

		/// <summary>
		/// Sets the next position of the specified object traveling on the grid. 
		/// The next position is the position the object will be at when it reaches its destination grid cell
		/// </summary>
		/// <param name="trackedObject"></param>
		/// <param name="cellCoordinates"></param>
		public virtual void SetNextPosition(GameObject trackedObject, Vector3Int cellCoordinates)
		{
			// we add that to our dictionary
			if (NextPositions.ContainsKey(trackedObject))
			{
				NextPositions[trackedObject] = cellCoordinates;
			}
			else
			{
				NextPositions.Add(trackedObject, cellCoordinates);
			}
		}

		/// <summary>
		/// Sets the last position of the specified object traveling on the grid. 
		/// The last position is the position the object was at the last time it passed on a perfect tile
		/// </summary>
		public virtual void SetLastPosition(GameObject trackedObject, Vector3Int cellCoordinates)
		{
			// we add that to our dictionary
			if (LastPositions.ContainsKey(trackedObject))
			{
				LastPositions[trackedObject] = cellCoordinates;

			}
			else
			{
				LastPositions.Add(trackedObject, cellCoordinates);
			}
		}

		/// <summary>
		/// Returns the corresponding world coordinates of a grid position
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <returns></returns>
		public virtual Vector3Int WorldToCellCoordinates(Vector3 worldPosition)
		{
			_newGridPosition = (worldPosition - GridOrigin.position) / GridUnitSize;

			_workCoordinate.x = Mathf.FloorToInt(_newGridPosition.x);
			_workCoordinate.y = Mathf.FloorToInt(_newGridPosition.y);
			_workCoordinate.z = Mathf.FloorToInt(_newGridPosition.z);

			return _workCoordinate;
		}

		/// <summary>
		/// Converts cell coordinates into their world position
		/// </summary>
		/// <param name="coordinates"></param>
		/// <returns></returns>
		public virtual Vector3 CellToWorldCoordinates(Vector3Int coordinates)
		{
			_newGridPosition = (Vector3)coordinates * GridUnitSize + GridOrigin.position;
			_newGridPosition += Vector3.one * (GridUnitSize / 2f);
			return _newGridPosition;
		}

		/// <summary>
		/// Returns the grid position of the specified vector in world position
		/// </summary>
		/// <param name="targetPosition"></param>
		/// <returns></returns>
		[System.Obsolete("As of v1.8 of the TopDown Engine, that method is obsolete, you should use WorldToCellCoordinates instead")]
		public virtual Vector3 ComputeGridPosition(Vector3 targetPosition)
		{
			_newGridPosition = (targetPosition - GridOrigin.position) / GridUnitSize;
			_newGridPosition.x = MMMaths.RoundToNearestHalf(_newGridPosition.x);
			_newGridPosition.y = MMMaths.RoundToNearestHalf(_newGridPosition.y);
			_newGridPosition.z = MMMaths.RoundToNearestHalf(_newGridPosition.z);
            
			return _newGridPosition;
		}

		/// <summary>
		/// Computes the grid position of a vector3 specified in grid units
		/// </summary>
		/// <param name="targetPosition"></param>
		/// <returns></returns>
		[System.Obsolete("As of v1.8 of the TopDown Engine, that method is obsolete, you should use WorldToCellCoordinates instead")]
		public virtual Vector3 ComputeWorldPosition(Vector3 targetPosition)
		{
			return GridOrigin.position + (targetPosition * GridUnitSize);
		}

		/// <summary>
		/// On draw gizmos, draws a debug grid
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			if (!DrawDebugGrid)
			{
				return;
			}

			Gizmos.color = CellBorderColor;

			if (DebugDrawMode == DebugDrawModes.ThreeD)
			{
				int i = -DebugGridSize;
				// draw lines
				while (i <= DebugGridSize)
				{
					_debugOrigin.x = GridOrigin.position.x - DebugGridSize * GridUnitSize;
					_debugOrigin.y = GridOrigin.position.y;
					_debugOrigin.z = GridOrigin.position.z + i * GridUnitSize;

					_debugDestination.x = GridOrigin.position.x + DebugGridSize * GridUnitSize;
					_debugDestination.y = GridOrigin.position.y;
					_debugDestination.z = GridOrigin.position.z + i * GridUnitSize;

					Debug.DrawLine(_debugOrigin, _debugDestination, CellBorderColor);

					_debugOrigin.x = GridOrigin.position.x + i * GridUnitSize;
					_debugOrigin.y = GridOrigin.position.y;
					_debugOrigin.z = GridOrigin.position.z - DebugGridSize * GridUnitSize; ;

					_debugDestination.x = GridOrigin.position.x + i * GridUnitSize;
					_debugDestination.y = GridOrigin.position.y;
					_debugDestination.z = GridOrigin.position.z + DebugGridSize * GridUnitSize;

					Debug.DrawLine(_debugOrigin, _debugDestination, CellBorderColor);

					i++;
				}

				// draw cells
				Gizmos.color = InnerColor;
				for (int a = -DebugGridSize; a < DebugGridSize; a++)
				{
					for (int b = -DebugGridSize; b < DebugGridSize; b++)
					{
						if ((a%2 == 0) && (b%2 != 0))
						{
							DrawCell3D(a, b);
						}
						if ((a%2 != 0) && (b%2 == 0))
						{
							DrawCell3D(a, b);
						}
					}
				}
			}
			else
			{
				int i = -DebugGridSize;
				// draw lines
				while (i <= DebugGridSize)
				{
					_debugOrigin.x = GridOrigin.position.x - DebugGridSize * GridUnitSize;
					_debugOrigin.y = GridOrigin.position.y + i * GridUnitSize;
					_debugOrigin.z = GridOrigin.position.z;

					_debugDestination.x = GridOrigin.position.x + DebugGridSize * GridUnitSize;
					_debugDestination.y = GridOrigin.position.y + i * GridUnitSize;
					_debugDestination.z = GridOrigin.position.z;

					Debug.DrawLine(_debugOrigin, _debugDestination, CellBorderColor);

					_debugOrigin.x = GridOrigin.position.x + i * GridUnitSize;
					_debugOrigin.y = GridOrigin.position.y - DebugGridSize * GridUnitSize; ;
					_debugOrigin.z = GridOrigin.position.z;

					_debugDestination.x = GridOrigin.position.x + i * GridUnitSize;
					_debugDestination.y = GridOrigin.position.y + DebugGridSize * GridUnitSize;
					_debugDestination.z = GridOrigin.position.z;

					Debug.DrawLine(_debugOrigin, _debugDestination, CellBorderColor);

					i++;
				}

				// draw cells
				Gizmos.color = InnerColor;
				for (int a = -DebugGridSize; a < DebugGridSize; a++)
				{
					for (int b = -DebugGridSize; b < DebugGridSize; b++)
					{
						if ((a % 2 == 0) && (b % 2 != 0))
						{
							DrawCell2D(a, b);
						}
						if ((a % 2 != 0) && (b % 2 == 0))
						{
							DrawCell2D(a, b);
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws a 2D debug cell
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		protected virtual void DrawCell2D(int a, int b)
		{
			_debugOrigin.x = GridOrigin.position.x + a * GridUnitSize + GridUnitSize / 2f;            
			_debugOrigin.y = GridOrigin.position.y + b * GridUnitSize + GridUnitSize / 2f;
			_debugOrigin.z = GridOrigin.position.z;
			Gizmos.DrawCube(_debugOrigin, GridUnitSize * new Vector3(1f, 1f, 0f));
		}

		/// <summary>
		/// Draws a 3D debug cell
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		protected virtual void DrawCell3D(int a, int b)
		{
			_debugOrigin.x = GridOrigin.position.x + a * GridUnitSize + GridUnitSize / 2f;
			_debugOrigin.y = GridOrigin.position.y;
			_debugOrigin.z = GridOrigin.position.z + b * GridUnitSize + GridUnitSize / 2f;
			Gizmos.DrawCube(_debugOrigin, GridUnitSize * new Vector3(1f, 0f, 1f));
		}
	}
}