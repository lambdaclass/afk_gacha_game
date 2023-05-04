using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  MoreMountains.Tools;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This component, added on an empty object in your level will handle the generation of a unique and randomized tilemap
	/// </summary>
	public class TilemapLevelGenerator : MMTilemapGenerator
	{
		[FormerlySerializedAs("GenerateOnStart")]
		[Header("TopDown Engine Settings")]
		/// Whether or not this level should be generated automatically on Awake
		[Tooltip("Whether or not this level should be generated automatically on Awake")]
		public bool GenerateOnAwake = false;

		[Header("Bindings")] 
		/// the Grid on which to work
		[Tooltip("the Grid on which to work")]
		public Grid TargetGrid;
		/// the tilemap containing the walls
		[Tooltip("the tilemap containing the walls")]
		public Tilemap ObstaclesTilemap; 
		/// the tilemap containing the walls' shadows
		[Tooltip("the tilemap containing the walls' shadows")]
		public MMTilemapShadow WallsShadowTilemap;
		/// the level manager
		[Tooltip("the level manager")]
		public LevelManager TargetLevelManager;

		[Header("Spawn")] 
		/// the object at which the player will spawn
		[Tooltip("the object at which the player will spawn")]
		public Transform InitialSpawn;
		/// the exit of the level
		[Tooltip("the exit of the level")]
		public Transform Exit;
		/// the minimum distance that should separate spawn and exit.
		[Tooltip("the minimum distance that should separate spawn and exit.")]
		public float MinDistanceFromSpawnToExit = 2f;

		protected const int _maxIterationsCount = 100;
        
		/// <summary>
		/// On awake we generate our level if needed
		/// </summary>
		protected virtual void Awake()
		{
			if (GenerateOnAwake)
			{
				Generate();
			}
		}

		/// <summary>
		/// Generates a new level
		/// </summary>
		public override void Generate()
		{
			base.Generate();
			HandleWallsShadow();
			PlaceEntryAndExit();
			ResizeLevelManager();
		}

		/// <summary>
		/// Resizes the level manager's bounds to match the new level
		/// </summary>
		protected virtual void ResizeLevelManager()
		{
			BoxCollider boxCollider = TargetLevelManager.GetComponent<BoxCollider>();
            
			Bounds bounds = ObstaclesTilemap.localBounds;
			boxCollider.center = bounds.center;
			boxCollider.size = new Vector3(bounds.size.x, bounds.size.y, boxCollider.size.z);
		}

		/// <summary>
		/// Moves the spawn and exit to empty places
		/// </summary>
		protected virtual void PlaceEntryAndExit()
		{
			UnityEngine.Random.InitState(GlobalSeed);
			int width = UnityEngine.Random.Range(GridWidth.x, GridWidth.y);
			int height = UnityEngine.Random.Range(GridHeight.x, GridHeight.y);
            
			Vector3 spawnPosition = MMTilemap.GetRandomPosition(ObstaclesTilemap, TargetGrid, width, height, false, width * height * 2);
			InitialSpawn.transform.position = spawnPosition;

			Vector3 exitPosition = spawnPosition;
			int iterationsCount = 0;
            
			while ((Vector3.Distance(exitPosition, spawnPosition) < MinDistanceFromSpawnToExit) && (iterationsCount < _maxIterationsCount))
			{
				exitPosition = MMTilemap.GetRandomPosition(ObstaclesTilemap, TargetGrid, width, height, false, width * height * 2);
				Exit.transform.position = exitPosition;
				iterationsCount++;
			}
		}
        
		/// <summary>
		/// Copies the contents of the Walls layer to the WallsShadows layer to get nice shadows automatically
		/// </summary>
		protected virtual void HandleWallsShadow()
		{
			if (WallsShadowTilemap != null)
			{
				WallsShadowTilemap.UpdateShadows();
			}
		}
	}    
}