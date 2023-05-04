using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.AI;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a 3D character and it'll be able to navigate a navmesh (if there's one in the scene of course)
	/// </summary>
	[MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Pathfinder 3D")]
	public class CharacterPathfinder3D : CharacterAbility
	{
		public enum PathRefreshModes { None, TimeBased, SpeedThresholdBased }
		
		[Header("PathfindingTarget")]

		/// the target the character should pathfind to
		[Tooltip("the target the character should pathfind to")]
		public Transform Target;
		/// the distance to waypoint at which the movement is considered complete
		[Tooltip("the distance to waypoint at which the movement is considered complete")]
		public float DistanceToWaypointThreshold = 1f;
		/// if the target point can't be reached, the distance threshold around that point in which to look for an alternative end point
		[Tooltip("if the target point can't be reached, the distance threshold around that point in which to look for an alternative end point")]
		public float ClosestPointThreshold = 3f;
		/// a minimum delay (in seconds) between two navmesh requests - longer delay means better performance but less accuracy
		[Tooltip("a minimum delay (in seconds) between two navmesh requests - longer delay means better performance but less accuracy")]
		public float MinimumDelayBeforePollingNavmesh = 0.1f;

		[Header("Path Refresh")]
		/// the chosen mode in which to refresh the path (none : nothing will happen and path will only refresh on set new destination,
		/// time based : path will refresh every x seconds, speed threshold based : path will refresh every x seconds if the character's speed is below a certain threshold
		[Tooltip("the chosen mode in which to refresh the path (none : nothing will happen and path will only refresh on set new destination, " +
		         "time based : path will refresh every x seconds, speed threshold based : path will refresh every x seconds if the character's speed is below a certain threshold")]
		public PathRefreshModes PathRefreshMode = PathRefreshModes.None;
		/// the speed under which the path should be recomputed, usually if the character blocks against an obstacle
		[Tooltip("the speed under which the path should be recomputed, usually if the character blocks against an obstacle")]
		[MMEnumCondition("PathRefreshMode", (int)PathRefreshModes.SpeedThresholdBased)]
		public float RefreshSpeedThreshold = 1f;
		/// the interval at which to refresh the path, in seconds
		[Tooltip("the interval at which to refresh the path, in seconds")]
		[MMEnumCondition("PathRefreshMode", (int)PathRefreshModes.TimeBased, (int)PathRefreshModes.SpeedThresholdBased)]
		public float RefreshInterval = 2f;

		[Header("Debug")]
		/// whether or not we should draw a debug line to show the current path of the character
		[Tooltip("whether or not we should draw a debug line to show the current path of the character")]
		public bool DebugDrawPath;

		/// the current path
		[MMReadOnly]
		[Tooltip("the current path")]
		public NavMeshPath AgentPath;
		/// a list of waypoints the character will go through
		[MMReadOnly]
		[Tooltip("a list of waypoints the character will go through")]
		public Vector3[] Waypoints;
		/// the index of the next waypoint
		[MMReadOnly]
		[Tooltip("the index of the next waypoint")]
		public int NextWaypointIndex;
		/// the direction of the next waypoint
		[MMReadOnly]
		[Tooltip("the direction of the next waypoint")]
		public Vector3 NextWaypointDirection;
		/// the distance to the next waypoint
		[MMReadOnly]
		[Tooltip("the distance to the next waypoint")]
		public float DistanceToNextWaypoint;

		public event System.Action<int, int, float> OnPathProgress;

		public virtual void InvokeOnPathProgress(int waypointIndex, int waypointsLength, float distance)
		{
			OnPathProgress?.Invoke(waypointIndex, waypointsLength, distance);
		}

		protected int _waypoints;
		protected Vector3 _direction;
		protected Vector2 _newMovement;
		protected Vector3 _lastValidTargetPosition;
		protected Vector3 _closestStartNavmeshPosition;
		protected Vector3 _closestTargetNavmeshPosition;
		protected NavMeshHit _navMeshHit;
		protected bool _pathFound;
		protected float _lastRequestAt = -Single.MaxValue;

		protected override void Initialization()
		{
			base.Initialization();
			AgentPath = new NavMeshPath();
			_lastValidTargetPosition = this.transform.position;
			Array.Resize(ref Waypoints, 5);
		}

		/// <summary>
		/// Sets a new destination the character will pathfind to
		/// </summary>
		/// <param name="destinationTransform"></param>
		public virtual void SetNewDestination(Transform destinationTransform)
		{
			if (destinationTransform == null)
			{
				Target = null;
				return;
			}
			Target = destinationTransform;
			DeterminePath(this.transform.position, Target.position);
		}

		/// <summary>
		/// On Update, we draw the path if needed, determine the next waypoint, and move to it if needed
		/// </summary>
		public override void ProcessAbility()
		{
			if (Target == null)
			{
				return;
			}

			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}

			PerformRefresh();
			DrawDebugPath();
			DetermineNextWaypoint();
			DetermineDistanceToNextWaypoint();
			MoveController();
		}
        
		/// <summary>
		/// Moves the controller towards the next point
		/// </summary>
		protected virtual void MoveController()
		{
			if ((Target == null) || (NextWaypointIndex <= 0))
			{
				_characterMovement.SetMovement(Vector2.zero);
				return;
			}
			else
			{
				_direction = (Waypoints[NextWaypointIndex] - this.transform.position).normalized;
				_newMovement.x = _direction.x;
				_newMovement.y = _direction.z;
				_characterMovement.SetMovement(_newMovement);
			}
		}

		protected virtual void PerformRefresh()
		{
			if (PathRefreshMode == PathRefreshModes.None)
			{
				return;
			}
			
			if (NextWaypointIndex <= 0)
			{
				return;
			}

			bool refreshNeeded = false;

			if (Time.time - _lastRequestAt > RefreshInterval)
			{
				refreshNeeded = true;
				_lastRequestAt = Time.time;
			}

			if (PathRefreshMode == PathRefreshModes.SpeedThresholdBased)
			{
				if (_controller.Speed.magnitude > RefreshSpeedThreshold)
				{
					refreshNeeded = false;
				}
			}

			if (refreshNeeded)
			{
				DeterminePath(this.transform.position, Target.position, true);
			}
		}
        
		/// <summary>
		/// Determines the next path position for the agent. NextPosition will be zero if a path couldn't be found
		/// </summary>
		/// <param name="startingPos"></param>
		/// <param name="targetPos"></param>
		/// <returns></returns>        
		protected virtual void DeterminePath(Vector3 startingPosition, Vector3 targetPosition, bool ignoreDelay = false)
		{
			if (!ignoreDelay && (Time.time - _lastRequestAt < MinimumDelayBeforePollingNavmesh))
			{
				return;
			}
			
			_lastRequestAt = Time.time;
			
			NextWaypointIndex = 0;
			
			// we find the closest position to the starting position on the navmesh
			_closestStartNavmeshPosition = startingPosition;
			if (NavMesh.SamplePosition(startingPosition, out _navMeshHit, ClosestPointThreshold, NavMesh.AllAreas))
			{
				_closestStartNavmeshPosition = _navMeshHit.position;
			}
                
			// we find the closest position to the target position on the navmesh
			_closestTargetNavmeshPosition = targetPosition;
			if (NavMesh.SamplePosition(targetPosition, out _navMeshHit, ClosestPointThreshold, NavMesh.AllAreas))
			{
				_closestTargetNavmeshPosition = _navMeshHit.position;
			}

			_pathFound = NavMesh.CalculatePath(_closestStartNavmeshPosition, _closestTargetNavmeshPosition, NavMesh.AllAreas, AgentPath);
			if (_pathFound)
			{
				_lastValidTargetPosition = _closestTargetNavmeshPosition;
			}
			else
			{
				NavMesh.CalculatePath(startingPosition, _lastValidTargetPosition, NavMesh.AllAreas, AgentPath);
			}

			// Waypoints = AgentPath.corners;
			_waypoints = AgentPath.GetCornersNonAlloc(Waypoints);
			if (_waypoints >= Waypoints.Length)
			{
				Array.Resize(ref Waypoints, _waypoints +5);
				_waypoints = AgentPath.GetCornersNonAlloc(Waypoints);
			}
			if (_waypoints >= 2)
			{
				NextWaypointIndex = 1;
			}

			InvokeOnPathProgress(NextWaypointIndex, Waypoints.Length, Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]));
		}
        
		/// <summary>
		/// Determines the next waypoint based on the distance to it
		/// </summary>
		protected virtual void DetermineNextWaypoint()
		{
			if (_waypoints <= 0)
			{
				return;
			}
			if (NextWaypointIndex < 0)
			{
				return;
			}

			var distance = Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]);
			if (distance <= DistanceToWaypointThreshold)
			{
				if (NextWaypointIndex + 1 < _waypoints)
				{
					NextWaypointIndex++;
				}
				else
				{
					NextWaypointIndex = -1;
				}
				InvokeOnPathProgress(NextWaypointIndex, _waypoints, distance);
			}
		}

		/// <summary>
		/// Determines the distance to the next waypoint
		/// </summary>
		protected virtual void DetermineDistanceToNextWaypoint()
		{
			if (NextWaypointIndex <= 0)
			{
				DistanceToNextWaypoint = 0;
			}
			else
			{
				DistanceToNextWaypoint = Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]);
			}
		}

		/// <summary>
		/// Draws a debug line to show the current path
		/// </summary>
		protected virtual void DrawDebugPath()
		{
			if (DebugDrawPath)
			{
				if (_waypoints <= 0)
				{
					if (Target != null)
					{
						DeterminePath(transform.position, Target.position);
					}
				}
				for (int i = 0; i < _waypoints - 1; i++)
				{
					Debug.DrawLine(Waypoints[i], Waypoints[i + 1], Color.red);
				}
			}
		}
	}
}