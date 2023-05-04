using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used in the Explodudes demo scene to handle the exploding bombs
	/// </summary>
	public class ExplodudesBomb : TopDownMonoBehaviour
	{
		[Header("Bindings")]
		/// the model of the bomb
		[Tooltip("the model of the bomb")]
		public Transform BombModel;
		/// the particle system used for the northbound explosion
		[Tooltip("the particle system used for the northbound explosion")]
		public ParticleSystem DirectedExplosionN;
		/// the particle system used for the southbound explosion
		[Tooltip("the particle system used for the southbound explosion")]
		public ParticleSystem DirectedExplosionS;
		/// the particle system used for the eastbound explosion
		[Tooltip("the particle system used for the eastbound explosion")]
		public ParticleSystem DirectedExplosionE;
		/// the particle system used for the westbound explosion
		[Tooltip("the particle system used for the westbound explosion")]
		public ParticleSystem DirectedExplosionW;

		[Header("Raycasts")]
		/// the offset to apply to the base of the obstacle detecting raycast
		[Tooltip("the offset to apply to the base of the obstacle detecting raycast")]
		public Vector3 RaycastOffset = Vector3.zero;
		/// the max distance of the raycast (should be bigger than the grid)
		[Tooltip("the max distance of the raycast (should be bigger than the grid)")]
		public float MaximumRaycastDistance = 50f;
		/// the layers to consider as obstacles to the bomb's fire
		[Tooltip("the layers to consider as obstacles to the bomb's fire")]
		public LayerMask ObstaclesMask = LayerManager.ObstaclesLayerMask;
		/// the layers to apply damage to
		[Tooltip("the layers to apply damage to")]
		public LayerMask DamageLayerMask;
		/// a small offset to apply to the raycasts
		[Tooltip("a small offset to apply to the raycasts")]
		public float SkinWidth = 0.01f;

		[Header("Bomb")]
		/// the delay (in seconds) before the bomb's explosion
		[Tooltip("the delay (in seconds) before the bomb's explosion")]
		public float BombDelayBeforeExplosion = 3f;
		/// the duration (in seconds) for which the bomb is active
		[Tooltip("the duration (in seconds) for which the bomb is active")]
		public float BombExplosionActiveDuration = 0.5f;
		/// a delay after the bomb has exploded and before it gets destroyed(in seconds)
		[Tooltip("a delay after the bomb has exploded and before it gets destroyed(in seconds)")]
		public float BombAdditionalDelayBeforeDestruction = 1.5f;
		/// the damage applied by the bomb to anything with a Health component
		[Tooltip("the damage applied by the bomb to anything with a Health component")]
		public int BombDamage = 10;
		/// the distance the bomb affects
		[Tooltip("the distance the bomb affects")]
		public int BombDistanceInGridUnits = 3;

		[Header("Feedbacks")]
		/// the feedbacks to play when the bomb explodes
		[Tooltip("the feedbacks to play when the bomb explodes")]
		public MMFeedbacks ExplosionFeedbacks;

		[Header("Owner")]
		/// the owner of the bomb
		[MMReadOnly]
		[Tooltip("the owner of the bomb")]
		public GameObject Owner;
        
		protected BoxCollider _boxCollider;
		protected WaitForSeconds _bombDuration;
		protected WaitForSeconds _explosionDuration;
		protected WaitForSeconds _additionalDelayBeforeDestruction;

		protected RaycastHit _raycastNorth;
		protected RaycastHit _raycastSouth;
		protected RaycastHit _raycastEast;
		protected RaycastHit _raycastWest;

		protected float _obstacleNorthDistance = 0f;
		protected float _obstacleEastDistance = 0f;
		protected float _obstacleWestDistance = 0f;
		protected float _obstacleSouthDistance = 0f;

		protected DamageOnTouch _damageAreaEast;
		protected DamageOnTouch _damageAreaWest;
		protected DamageOnTouch _damageAreaNorth;
		protected DamageOnTouch _damageAreaSouth;
		protected DamageOnTouch _damageAreaCenter;

		protected Vector3 _damageAreaPosition;
		protected Vector3 _damageAreaSize;
        
		protected Coroutine _delayBeforeExplosionCoroutine;
		protected ExplodudesBomb _otherBomb;
		protected bool _exploded = false;


		/// <summary>
		/// On Start we initialize our bomb
		/// </summary>
		protected virtual void Awake()
		{
			Initialization();
		}

		/// <summary>
		/// Initializes the bomb
		/// </summary>
		protected virtual void Initialization()
		{
			_bombDuration = new WaitForSeconds(BombDelayBeforeExplosion);
			_explosionDuration = new WaitForSeconds(BombExplosionActiveDuration);
			_additionalDelayBeforeDestruction = new WaitForSeconds(BombAdditionalDelayBeforeDestruction);

			// randomize model rotation
			BombModel.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

			// we grab our collider and make it a trigger
			_boxCollider = this.gameObject.MMGetComponentNoAlloc<BoxCollider>();
			_boxCollider.isTrigger = true;

			// we create our damage zones
			_damageAreaEast = CreateDamageArea("East");
			_damageAreaWest = CreateDamageArea("West");
			_damageAreaSouth = CreateDamageArea("South");
			_damageAreaNorth = CreateDamageArea("North");
			_damageAreaCenter = CreateDamageArea("Center");

			// center damage area setup
			_damageAreaSize.x = GridManager.Instance.GridUnitSize / 2f;
			_damageAreaSize.y = GridManager.Instance.GridUnitSize / 2f;
			_damageAreaSize.z = GridManager.Instance.GridUnitSize / 2f;

			_damageAreaPosition = this.transform.position                                
			                      + Vector3.up * GridManager.Instance.GridUnitSize / 2f;

			_damageAreaCenter.gameObject.transform.position = _damageAreaPosition;
			_damageAreaCenter.gameObject.MMFGetComponentNoAlloc<BoxCollider>().size = _damageAreaSize;

		}

		/// <summary>
		/// Resets the bomb (usually after having been respawned from the pool)
		/// </summary>
		protected virtual void ResetBomb()
		{
			_exploded = false;
			_boxCollider.enabled = true;
			_boxCollider.isTrigger = true;
			BombModel.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			BombModel.gameObject.SetActive(true);

			// we start our bomb coroutine
			_delayBeforeExplosionCoroutine = StartCoroutine(DelayBeforeExplosionCoroutine());
		}

		/// <summary>
		/// Creates a directed damage area
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected virtual DamageOnTouch CreateDamageArea(string name)
		{
			GameObject damageAreaGameObject = new GameObject();
			damageAreaGameObject.SetActive(false);
			damageAreaGameObject.transform.SetParent(this.transform);
			damageAreaGameObject.name = "ExplodudesBombDamageArea" + name;
			damageAreaGameObject.layer = LayerMask.NameToLayer("Enemies");

			DamageOnTouch damageOnTouch = damageAreaGameObject.AddComponent<DamageOnTouch>();
			damageOnTouch.MinDamageCaused = BombDamage;
			damageOnTouch.TargetLayerMask = DamageLayerMask;
			damageOnTouch.DamageTakenEveryTime = 0;
			damageOnTouch.InvincibilityDuration = 0f;
			damageOnTouch.DamageTakenEveryTime = 10;

			BoxCollider colllider = damageAreaGameObject.AddComponent<BoxCollider>();
			colllider.isTrigger = true;
            
			return damageOnTouch;
		}

		/// <summary>
		/// Applies a delay before exploding the bomb
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator DelayBeforeExplosionCoroutine()
		{
			yield return _bombDuration;

			Detonate();
		}

		/// <summary>
		/// Detonates the bomb
		/// </summary>
		public virtual void Detonate()
		{
			if (_exploded)
			{
				return;
			}
			StartCoroutine(DetonateCoroutine());
		}

		/// <summary>
		/// Detonates the bomb and instantiates damage areas
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator DetonateCoroutine()
		{
			_exploded = true;
			_boxCollider.enabled = false;
			StopCoroutine(_delayBeforeExplosionCoroutine);

			// we cast rays in all directions
			CastRays();

			// we add our damage areas
			DirectedExplosion(_raycastEast, _damageAreaEast, DirectedExplosionE, 90f);
			DirectedExplosion(_raycastWest, _damageAreaWest, DirectedExplosionW, 90f);
			DirectedExplosion(_raycastNorth, _damageAreaNorth, DirectedExplosionN, 0f);
			DirectedExplosion(_raycastSouth, _damageAreaSouth, DirectedExplosionS, 0f);
			_damageAreaCenter.gameObject.SetActive(true);
			ExplosionFeedbacks?.PlayFeedbacks();
			BombModel.gameObject.SetActive(false);

			yield return _explosionDuration;
			_damageAreaEast.gameObject.SetActive(false);
			_damageAreaWest.gameObject.SetActive(false);
			_damageAreaNorth.gameObject.SetActive(false);
			_damageAreaSouth.gameObject.SetActive(false);
			_damageAreaCenter.gameObject.SetActive(false);

			yield return _additionalDelayBeforeDestruction;
			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// Explodes in one of the 4 directions
		/// </summary>
		/// <param name="hit"></param>
		/// <param name="damageArea"></param>
		/// <param name="explosion"></param>
		/// <param name="angle"></param>
		protected virtual void DirectedExplosion(RaycastHit hit, DamageOnTouch damageArea, ParticleSystem explosion, float angle)
		{
			float hitDistance = hit.distance;

			// if what we find has a Health component, it's gonna be destroyed and needs to be covered by the explosion
			if (hit.collider.gameObject.MMFGetComponentNoAlloc<Health>() != null)
			{
				hitDistance += GridManager.Instance.GridUnitSize;
			}

			// if what we find has a Bomb component, it's gonna be destroyed and needs to be covered by the explosion
			_otherBomb = hit.collider.gameObject.MMFGetComponentNoAlloc<ExplodudesBomb>();
			if ((_otherBomb != null) && (hitDistance <= BombDistanceInGridUnits))
			{
				hitDistance += GridManager.Instance.GridUnitSize;
				// we detonate the other bomb
				_otherBomb.Detonate();
			}

			// if we're colliding with an obstacle, we stop this explosion
			if (hitDistance <= GridManager.Instance.GridUnitSize / 2f)
			{
				return;   
			}

			// otherwise we explode

			// we compute the size of the explosion
			float explosionLength;            
			float adjustedDistance = hitDistance - GridManager.Instance.GridUnitSize / 2f;
			float maxExplosionLength = BombDistanceInGridUnits * GridManager.Instance.GridUnitSize;
			explosionLength = Mathf.Min(adjustedDistance, maxExplosionLength);
			explosionLength -= GridManager.Instance.GridUnitSize / 2f;

			// we set our damage size and position
			_damageAreaSize.x = GridManager.Instance.GridUnitSize / 2f;
			_damageAreaSize.y = GridManager.Instance.GridUnitSize / 2f;
			_damageAreaSize.z = explosionLength;

			_damageAreaPosition = this.transform.position 
			                      + (hit.point - (this.transform.position + RaycastOffset)).normalized * (explosionLength / 2f + GridManager.Instance.GridUnitSize/2f)
			                      + Vector3.up * GridManager.Instance.GridUnitSize / 2f;

			damageArea.gameObject.transform.position = _damageAreaPosition;
			damageArea.gameObject.transform.LookAt(this.transform.position + Vector3.up * (GridManager.Instance.GridUnitSize / 2f));

			// we activate our damage area
			damageArea.gameObject.SetActive(true);            
			damageArea.gameObject.MMFGetComponentNoAlloc<BoxCollider>().size = _damageAreaSize;

			// we activate our VFX explosion
			explosion.gameObject.SetActive(true);
			explosion.transform.position = _damageAreaPosition;
			ParticleSystem.ShapeModule shape = explosion.shape;
			shape.scale = new Vector3(0.1f, 0.1f, explosionLength);
			shape.rotation = new Vector3(0f, angle, 0f);

			// we trigger a bomb event
			MMGameEvent.Trigger("Bomb");

		}

		/// <summary>
		/// Casts 4 cardinal rays to look for obstacles and victims
		/// </summary>
		protected virtual void CastRays()
		{
			float boxWidth = (_boxCollider.bounds.size.x / 2f) + SkinWidth;
			boxWidth = 0f;

			_raycastEast = MMDebug.Raycast3D(this.transform.position + Vector3.right * boxWidth +  RaycastOffset, Vector3.right, MaximumRaycastDistance, ObstaclesMask, Color.red, true);
			if (_raycastEast.collider != null) { _obstacleEastDistance = _raycastEast.distance; } else { _obstacleEastDistance = 0f; }
            
			_raycastNorth = MMDebug.Raycast3D(this.transform.position + Vector3.forward * boxWidth + RaycastOffset, Vector3.forward, MaximumRaycastDistance, ObstaclesMask, Color.red, true);
			if (_raycastNorth.collider != null) { _obstacleNorthDistance = _raycastNorth.distance; } else { _obstacleNorthDistance = 0f; }
            
			_raycastSouth = MMDebug.Raycast3D(this.transform.position + Vector3.back * boxWidth + RaycastOffset, Vector3.back, MaximumRaycastDistance, ObstaclesMask, Color.red, true);
			if (_raycastSouth.collider != null) { _obstacleSouthDistance = _raycastSouth.distance; } else { _obstacleSouthDistance = 0f; }

			_raycastWest = MMDebug.Raycast3D(this.transform.position + Vector3.left * boxWidth + RaycastOffset, Vector3.left, MaximumRaycastDistance, ObstaclesMask, Color.red, true);
			if (_raycastWest.collider != null) { _obstacleWestDistance = _raycastWest.distance; } else { _obstacleWestDistance = 0f; }

		}

		/// <summary>
		/// When the object gets spawned, we initialize it
		/// </summary>
		public virtual void OnEnable()
		{
			ResetBomb();
		}

		/// <summary>
		/// When the owner of the bomb exits it, we make it an obstacle
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject == Owner)
			{
				_boxCollider.isTrigger = false;
			}
		}
	}
}