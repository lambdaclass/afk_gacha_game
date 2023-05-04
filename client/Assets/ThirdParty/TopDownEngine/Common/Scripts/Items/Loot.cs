using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace  MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class meant to spawn objects (usually item pickers, but not necessarily)
	/// The spawn can be triggered by any script, at any time, and comes with automatic hooks
	/// to trigger loot on damage or death
	/// </summary>
	public class Loot : TopDownMonoBehaviour
	{
		/// the possible modes by which loot can be defined 
		public enum LootModes { Unique, LootTable, LootTableScriptableObject }

		[Header("Loot Mode")] 
		/// the selected loot mode :
		/// - unique : a simple object
		/// - loot table : a LootTable specific to this Loot object
		/// - loot definition : a LootTable scriptable object (created by right click > Create > MoreMountains > TopDown Engine > Loot Definition
		/// This loot definition can then be reused in other Loot objects.
		[Tooltip("the selected loot mode : - unique : a simple object  - loot table : a LootTable specific to this Loot object - loot definition : a LootTable scriptable object (created by right click > Create > MoreMountains > TopDown Engine > Loot Definition. This loot definition can then be reused in other Loot objects.")]
		public LootModes LootMode = LootModes.Unique;

		/// the object to loot, when in LootMode
		[Tooltip("the object to loot, when in LootMode")]
		[MMEnumCondition("LootMode", (int) LootModes.Unique)]
		public GameObject GameObjectToLoot;
        
		/// a loot table defining what objects to spawn
		[Tooltip("a loot table defining what objects to spawn")]
		[MMEnumCondition("LootMode", (int) LootModes.LootTable)]
		public MMLootTableGameObject LootTable;
        
		/// a loot table scriptable object defining what objects to spawn
		[Tooltip("a loot table scriptable object defining what objects to spawn")]
		[MMEnumCondition("LootMode", (int) LootModes.LootTableScriptableObject)]
		public MMLootTableGameObjectSO LootTableSO;

		[Header("Conditions")] 
		/// if this is true, loot will happen when this object dies
		[Tooltip("if this is true, loot will happen when this object dies")]
		public bool SpawnLootOnDeath = true;
		/// if this is true, loot will happen when this object takes damage
		[Tooltip("if this is true, loot will happen when this object takes damage")]
		public bool SpawnLootOnDamage = false;
        
		[Header("Pooling")] 
		/// if this is true, lootables will be pooled
		[Tooltip("if this is true, lootables will be pooled")]
		public bool PoolLoot = false;
		/// determines the size of the pool for each object in the loot table
		[Tooltip("determines the size of the pool for each object in the loot table")]
		[MMCondition("PoolLoot", true)]
		public int PoolSize = 20;
		/// a unique name for this pool, has to be common between all Loot objects sharing the same loot table if you want to mutualize their pools
		[Tooltip("a unique name for this pool, has to be common between all Loot objects sharing the same loot table if you want to mutualize their pools")]
		[MMCondition("PoolLoot", true)]
		public string MutualizedPoolName = "";
        
		[Header("Spawn")] 
		/// if this is false, spawn won't happen
		[Tooltip("if this is false, spawn won't happen")]
		public bool CanSpawn = true;
		/// a delay (in seconds) to wait for before spawning loot
		[Tooltip("a delay (in seconds) to wait for before spawning loot")]
		public float Delay = 0f; 
		/// the minimum and maximum quantity of objects to spawn 
		[Tooltip("the minimum and maximum quantity of objects to spawn")]
		[MMVector("Min","Max")]
		public Vector2 Quantity = Vector2.one;
		/// the position, rotation and scale objects should spawn at
		[Tooltip("the position, rotation and scale objects should spawn at")]
		public MMSpawnAroundProperties SpawnProperties;
		/// if this is true, loot will be limited to MaximumQuantity, any new loot attempt beyond that will have no outcome. If this is false, loot is unlimited and can happen forever.
		[Tooltip("if this is true, loot will be limited to MaximumQuantity, any new loot attempt beyond that will have no outcome. If this is false, loot is unlimited and can happen forever.")]
		public bool LimitedLootQuantity = true;
		/// The maximum quantity of objects that can be looted from this Loot object
		[Tooltip("The maximum quantity of objects that can be looted from this object")]
		[MMCondition("LimitedLootQuantity", true)]
		public int MaximumQuantity = 100;
		/// The remaining quantity of objects that can be looted from this Loot object, displayed for debug purposes 
		[Tooltip("The remaining quantity of objects that can be looted from this Loot object, displayed for debug purposes")]
		[MMReadOnly]
		public int RemainingQuantity = 100;

		[Header("Collisions")] 
		/// Whether or not spawned objects should try and avoid obstacles 
		[Tooltip("Whether or not spawned objects should try and avoid obstacles")]
		public bool AvoidObstacles = false;
		/// the possible modes collision detection can operate on
		public enum DimensionModes { TwoD, ThreeD}
		/// whether collision detection should happen in 2D or 3D
		[Tooltip("whether collision detection should happen in 2D or 3D")]
		[MMCondition("AvoidObstacles", true)]
		public DimensionModes DimensionMode = DimensionModes.TwoD;
		/// the layer mask containing layers the spawned objects shouldn't collide with 
		[Tooltip("the layer mask containing layers the spawned objects shouldn't collide with")]
		[MMCondition("AvoidObstacles", true)]
		public LayerMask AvoidObstaclesLayerMask = LayerManager.ObstaclesLayerMask;
		/// the radius around the object within which no obstacle should be found
		[Tooltip("the radius around the object within which no obstacle should be found")]
		[MMCondition("AvoidObstacles", true)]
		public float AvoidRadius = 0.25f;
		/// the amount of times the script should try finding another position for the loot if the last one was within an obstacle. More attempts : better results, higher cost
		[Tooltip("the amount of times the script should try finding another position for the loot if the last one was within an obstacle. More attempts : better results, higher cost")]
		[MMCondition("AvoidObstacles", true)]
		public int MaxAvoidAttempts = 5;
        
		[Header("Feedback")] 
		/// A MMFeedbacks to play when spawning loot. Only one feedback will play. If you want one per item, it's best to place it on the item itself, and have it play when the object gets instantiated. 
		[Tooltip("A MMFeedbacks to play when spawning loot. Only one feedback will play. If you want one per item, it's best to place it on the item itself, and have it play when the object gets instantiated.")]
		public MMFeedbacks LootFeedback;

		[Header("Debug")] 
		/// if this is true, gizmos will be drawn to show the shape within which loot will spawn
		[Tooltip("if this is true, gizmos will be drawn to show the shape within which loot will spawn")]
		public bool DrawGizmos = false;
		/// the amount of gizmos to draw
		[Tooltip("the amount of gizmos to draw")]
		public int GizmosQuantity = 1000;
		/// the color the gizmos should be drawn with
		[Tooltip("the color the gizmos should be drawn with")]
		public Color GizmosColor = MMColors.LightGray;
		/// the size at which to draw the gizmos
		[Tooltip("the size at which to draw the gizmos")]
		public float GimosSize = 1f;
		/// a debug button used to trigger a loot
		[Tooltip("a debug button used to trigger a loot")]
		[MMInspectorButton("SpawnLootDebug")] 
		public bool SpawnLootButton;
        
		public static List<MMSimpleObjectPooler> SimplePoolers = new List<MMSimpleObjectPooler>();
		public static List<MMMultipleObjectPooler> MultiplePoolers = new List<MMMultipleObjectPooler>();

		protected Health _health;
		protected GameObject _objectToSpawn;
		protected GameObject _spawnedObject;
		protected Vector3 _raycastOrigin;
		protected RaycastHit2D _raycastHit2D;
		protected Collider[] _overlapBox;
		protected MMSimpleObjectPooler _simplePooler;
		protected MMMultipleObjectPooler _multipleObjectPooler;
        
		/// <summary>
		/// On Awake we grab the health component if there's one, and initialize our loot table
		/// </summary>
		protected virtual void Awake()
		{
			_health = this.gameObject.GetComponentInParent<Health>();
			InitializeLootTable();
			InitializePools();
			ResetRemainingQuantity();
		}

		/// <summary>
		/// Resets the remaining quantity to the maximum quantity
		/// </summary>
		public virtual void ResetRemainingQuantity()
		{
			RemainingQuantity = MaximumQuantity;
		}

		/// <summary>
		/// Computes the associated loot table's weights
		/// </summary>
		public virtual void InitializeLootTable()
		{
			switch (LootMode)
			{
				case LootModes.LootTableScriptableObject:
					if (LootTableSO != null)
					{
						LootTableSO.ComputeWeights();
					}
					break;
				case LootModes.LootTable:
					LootTable.ComputeWeights();
					break;
			}
		}

		protected virtual void InitializePools()
		{
			if (!PoolLoot)
			{
				return;
			}

			switch (LootMode)
			{
				case LootModes.Unique:
					_simplePooler = FindSimplePooler();
					break;
				case LootModes.LootTable:
					_multipleObjectPooler = FindMultiplePooler();
					break;
				case LootModes.LootTableScriptableObject:
					_multipleObjectPooler = FindMultiplePooler();
					break;
			}
		}

		protected virtual MMSimpleObjectPooler FindSimplePooler()
		{
			foreach (MMSimpleObjectPooler simplePooler in SimplePoolers)
			{
				if (simplePooler.GameObjectToPool == GameObjectToLoot)
				{
					return simplePooler;
				}
			}
			// if we haven't found one, we create one
			GameObject newObject = new GameObject("[MMSimpleObjectPooler] "+GameObjectToLoot.name);
			MMSimpleObjectPooler pooler = newObject.AddComponent<MMSimpleObjectPooler>();
			pooler.GameObjectToPool = GameObjectToLoot;
			pooler.PoolSize = PoolSize;
			pooler.NestUnderThis = true;
			pooler.FillObjectPool();            
			pooler.Owner = SimplePoolers;
			SimplePoolers.Add(pooler);
			return pooler;
		}
        
		protected virtual MMMultipleObjectPooler FindMultiplePooler()
		{
			foreach (MMMultipleObjectPooler multiplePooler in MultiplePoolers)
			{
				if ((multiplePooler != null) && (multiplePooler.MutualizedPoolName == MutualizedPoolName)) 
				{
					return multiplePooler;
				}
			}
			// if we haven't found one, we create one
			GameObject newObject = new GameObject("[MMMultipleObjectPooler] "+MutualizedPoolName);
			MMMultipleObjectPooler pooler = newObject.AddComponent<MMMultipleObjectPooler>();
			pooler.MutualizeWaitingPools = true;
			pooler.MutualizedPoolName = MutualizedPoolName;
			pooler.NestUnderThis = true;
			pooler.Pool = new List<MMMultipleObjectPoolerObject>();
			if (LootMode == LootModes.LootTable)
			{
				foreach (MMLootGameObject loot in LootTable.ObjectsToLoot)
				{
					MMMultipleObjectPoolerObject objectToPool = new MMMultipleObjectPoolerObject();
					objectToPool.PoolSize = PoolSize * (int)loot.Weight;
					objectToPool.GameObjectToPool = loot.Loot;
					pooler.Pool.Add(objectToPool);
				}
			}
			else if (LootMode == LootModes.LootTableScriptableObject)
			{
				foreach (MMLootGameObject loot in LootTableSO.LootTable.ObjectsToLoot)
				{
					MMMultipleObjectPoolerObject objectToPool = new MMMultipleObjectPoolerObject
					{
						PoolSize = PoolSize * (int)loot.Weight,
						GameObjectToPool = loot.Loot
					};
					pooler.Pool.Add(objectToPool);
				}
			}
			pooler.FillObjectPool();
			pooler.Owner = MultiplePoolers;
			MultiplePoolers.Add(pooler);
			return pooler;
		}

		/// <summary>
		/// This method spawns the specified loot after applying a delay (if there's one)
		/// </summary>
		public virtual void SpawnLoot()
		{
			if (!CanSpawn)
			{
				return;
			}
			StartCoroutine(SpawnLootCo());
		}

		/// <summary>
		/// A debug method called by the inspector button
		/// </summary>
		protected virtual void SpawnLootDebug()
		{
			if (!Application.isPlaying)
			{
				Debug.LogWarning("This debug button is only meant to be used while in Play Mode.");
				return;
			}

			SpawnLoot();
		}

		/// <summary>
		/// A coroutine used to spawn loot after a delay
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator SpawnLootCo()
		{
			yield return MMCoroutine.WaitFor(Delay);
			int randomQuantity = Random.Range((int)Quantity.x, (int)Quantity.y);
			for (int i = 0; i < randomQuantity; i++)
			{
				SpawnOneLoot();
			}
			LootFeedback?.PlayFeedbacks();
		}

		protected virtual void Spawn(GameObject gameObjectToSpawn)
		{
			if (PoolLoot)
			{
				switch (LootMode)
				{
					case LootModes.Unique:
						_spawnedObject = _simplePooler.GetPooledGameObject();
						break;
					case LootModes.LootTable: case LootModes.LootTableScriptableObject:
						_spawnedObject = _multipleObjectPooler.GetPooledGameObject();
						break;
				}
			}
			else
			{
				_spawnedObject = Instantiate(_objectToSpawn);    
			}
		}

		/// <summary>
		/// Spawns a single loot object, without delay, and regardless of the defined quantities 
		/// </summary>
		public virtual void SpawnOneLoot()
		{
			_objectToSpawn = GetObject();

			if (_objectToSpawn == null)
			{
				return;
			}

			if (LimitedLootQuantity && (RemainingQuantity <= 0))
			{
				return;
			}

			Spawn(_objectToSpawn);

			if (AvoidObstacles)
			{
				bool placementOK = false;
				int amountOfAttempts = 0;
				while (!placementOK && (amountOfAttempts < MaxAvoidAttempts))
				{
					MMSpawnAround.ApplySpawnAroundProperties(_spawnedObject, SpawnProperties, this.transform.position);
                    
					if (DimensionMode == DimensionModes.TwoD)
					{
						_raycastOrigin = _spawnedObject.transform.position;
						_raycastHit2D = Physics2D.BoxCast(_raycastOrigin + Vector3.right * AvoidRadius, AvoidRadius * Vector2.one, 0f, Vector2.left, AvoidRadius, AvoidObstaclesLayerMask);
						if (_raycastHit2D.collider == null)
						{
							placementOK = true;
						}
						else
						{
							amountOfAttempts++;
						}
					}
					else
					{
						_raycastOrigin = _spawnedObject.transform.position;
						_overlapBox = Physics.OverlapBox(_raycastOrigin, Vector3.one * AvoidRadius, Quaternion.identity, AvoidObstaclesLayerMask);
                        
						if (_overlapBox.Length == 0)
						{
							placementOK = true;
						}
						else
						{
							amountOfAttempts++;
						}
					}
				}
			}
			else
			{
				MMSpawnAround.ApplySpawnAroundProperties(_spawnedObject, SpawnProperties, this.transform.position);    
			}
			if (_spawnedObject != null)
			{
				_spawnedObject.gameObject.SetActive(true);
			}
			_spawnedObject.SendMessage("OnInstantiate", SendMessageOptions.DontRequireReceiver);

			if (LimitedLootQuantity)
			{
				RemainingQuantity--;	
			}
		}

		/// <summary>
		/// Gets the object that should be spawned
		/// </summary>
		/// <returns></returns>
		protected virtual GameObject GetObject()
		{
			_objectToSpawn = null;
			switch (LootMode)
			{
				case LootModes.Unique:
					_objectToSpawn = GameObjectToLoot;
					break;
				case LootModes.LootTableScriptableObject:
					if (LootTableSO == null)
					{
						_objectToSpawn = null;
						break;
					}
					_objectToSpawn = LootTableSO.GetLoot();
					break;
				case LootModes.LootTable:
					_objectToSpawn = LootTable.GetLoot()?.Loot;
					break;
			}

			return _objectToSpawn;
		}

		/// <summary>
		/// On hit, we spawn loot if needed
		/// </summary>
		protected virtual void OnHit()
		{
			if (!SpawnLootOnDamage)
			{
				return;
			}

			SpawnLoot();
		}
        
		/// <summary>
		/// On death, we spawn loot if needed
		/// </summary>
		protected virtual void OnDeath()
		{
			if (!SpawnLootOnDeath)
			{
				return;
			}

			SpawnLoot();
		}
        
		/// <summary>
		/// OnEnable we start listening for death and hit if needed
		/// </summary>
		protected virtual void OnEnable()
		{
			if (_health != null)
			{
				_health.OnDeath += OnDeath;
				_health.OnHit += OnHit;
			}
		}

		/// <summary>
		/// OnDisable we stop listening for death and hit if needed
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				_health.OnDeath -= OnDeath;
				_health.OnHit -= OnHit;
			}
		}
        
		/// <summary>
		/// OnDrawGizmos, we display the shape at which objects will spawn when looted
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			if (DrawGizmos)
			{
				MMSpawnAround.DrawGizmos(SpawnProperties, this.transform.position, GizmosQuantity, GimosSize, GizmosColor);    
			}
		}

	}
}