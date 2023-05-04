using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class handles the dropping of bombs by characters in the Explodudes demo scene
	/// </summary>
	public class ExplodudesWeapon : Weapon
	{
		/// <summary>
		/// The possible ways to spawn a bomb on the grid : 
		/// - no grid : at the world position of the weapon
		/// - last cell : the last cell the owner of the weapon passed by
		/// - next cell : the cell the owner of the weapon is moving to
		/// - closest : will pick the cell closest to the movement right now
		/// </summary>
		public enum GridSpawnMethods { NoGrid, LastCell, NextCell, Closest }

		[MMInspectorGroup("Explodudes Weapon", true, 23)]
		/// the spawn method for this weapon
		[Tooltip("the spawn method for this weapon")]
		public GridSpawnMethods GridSpawnMethod;
		/// the offset to apply on spawn
		[Tooltip("the offset to apply on spawn")]
		public Vector3 BombOffset;        
		/// the max amount of bombs a character can drop on screen at once
		[Tooltip("the max amount of bombs a character can drop on screen at once")]
		public int MaximumAmountOfBombsAtOnce = 3;
		/// the delay before the bomb explodes
		[Tooltip("the delay before the bomb explodes")]
		public float BombDelayBeforeExplosion = 3f;
		/// the amount of bombs remaining
		[MMReadOnly]
		[Tooltip("the amount of bombs remaining")]
		public int RemainingBombs = 0;

		protected MMSimpleObjectPooler _objectPool;
		protected Vector3 _newSpawnWorldPosition;
		protected bool _alreadyBombed = false;
		protected Vector3 _lastBombPosition;
		protected ExplodudesBomb _bomb;
		protected WaitForSeconds _addOneRemainingBomb;

		protected Vector3 _closestLast;
		protected Vector3 _closestNext;
		protected Vector3Int _cellPosition;
		protected Vector3 _positionLastFrame;
		protected bool _hasntMoved = false;

		/// <summary>
		/// On init we grab our pool and initialize our stuff
		/// </summary>
		public override void Initialization()
		{
			base.Initialization();
			_objectPool = this.gameObject.GetComponent<MMSimpleObjectPooler>();
			RemainingBombs = MaximumAmountOfBombsAtOnce;
			_addOneRemainingBomb = new WaitForSeconds(BombDelayBeforeExplosion);
			_positionLastFrame = this.transform.position;
		}

		/// <summary>
		/// When the weapon is used, we spawn a bomb
		/// </summary>
		public override void ShootRequest()
		{
			// we don't call base on purpose
			SpawnBomb();
		}

		/// <summary>
		/// On update we store our movement position
		/// </summary>
		protected override void Update()
		{
			base.Update();
			if (_positionLastFrame != this.transform.position)
			{
				_hasntMoved = false;
			}
			_positionLastFrame = this.transform.position;
		}

		/// <summary>
		/// Spawns a bomb
		/// </summary>
		protected virtual void SpawnBomb()
		{
			// we decide where to put our bomb
			DetermineBombSpawnPosition();

			// if there's already a bomb there, we exit
			if (_alreadyBombed)
			{
				if ( (_lastBombPosition == _newSpawnWorldPosition) && _hasntMoved)
				{
					return;
				}
			}

			// if we don't have bombs left, we exit
			if (RemainingBombs <= 0)
			{
				return;
			}

			// we pool a new bomb
			GameObject nextGameObject = _objectPool.GetPooledGameObject();
			if (nextGameObject == null)
			{
				return;
			}

			// we setup our bomb and activate it
			nextGameObject.transform.position = _newSpawnWorldPosition;
			_bomb = nextGameObject.MMGetComponentNoAlloc<ExplodudesBomb>();
			_bomb.Owner = Owner.gameObject;
			_bomb.BombDelayBeforeExplosion = BombDelayBeforeExplosion;
			nextGameObject.gameObject.SetActive(true);

			// we lose one bomb and prepare to add it back
			RemainingBombs--;
			StartCoroutine(AddOneRemainingBombCoroutine());

			// we change our state
			WeaponState.ChangeState(WeaponStates.WeaponUse);
			_alreadyBombed = true;
			_hasntMoved = true;
			_lastBombPosition = _newSpawnWorldPosition;
		}

		/// <summary>
		/// Determines where the bomb should be spawned based on the inspector settings
		/// </summary>
		protected virtual void DetermineBombSpawnPosition()
		{
			_newSpawnWorldPosition = this.transform.position;
			switch (GridSpawnMethod)
			{
				case GridSpawnMethods.NoGrid:
					_newSpawnWorldPosition = this.transform.position;
					break;
				case GridSpawnMethods.LastCell:
					if (GridManager.Instance.LastPositions.ContainsKey(Owner.gameObject))
					{
						_cellPosition = GridManager.Instance.LastPositions[Owner.gameObject];
						_newSpawnWorldPosition = GridManager.Instance.CellToWorldCoordinates(_cellPosition);
					}
					break;
				case GridSpawnMethods.NextCell:
					if (GridManager.Instance.NextPositions.ContainsKey(Owner.gameObject))
					{
						_cellPosition = GridManager.Instance.NextPositions[Owner.gameObject];
						_newSpawnWorldPosition = GridManager.Instance.CellToWorldCoordinates(_cellPosition);
					}
					break;
				case GridSpawnMethods.Closest:
					if (GridManager.Instance.LastPositions.ContainsKey(Owner.gameObject))
					{
						_cellPosition = GridManager.Instance.LastPositions[Owner.gameObject];
						_closestLast = GridManager.Instance.CellToWorldCoordinates(_cellPosition);
					}
					if (GridManager.Instance.NextPositions.ContainsKey(Owner.gameObject))
					{
						_cellPosition = GridManager.Instance.NextPositions[Owner.gameObject];
						_closestNext = GridManager.Instance.CellToWorldCoordinates(_cellPosition);
					}

					if (Vector3.Distance(_closestLast, this.transform.position) < Vector3.Distance(_closestNext, this.transform.position))
					{
						_newSpawnWorldPosition = _closestLast;
					}
					else
					{
						_newSpawnWorldPosition = _closestNext;
					}
					break;
			}
			_newSpawnWorldPosition += BombOffset;
		}

		/// <summary>
		/// Adds back another bomb to use after it explodes
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator AddOneRemainingBombCoroutine()
		{
			yield return _addOneRemainingBomb;
			RemainingBombs++;
			RemainingBombs = Mathf.Min(RemainingBombs, MaximumAmountOfBombsAtOnce);
		}
	}
}