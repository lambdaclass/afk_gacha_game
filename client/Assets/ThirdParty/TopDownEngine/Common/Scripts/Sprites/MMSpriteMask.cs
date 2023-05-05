using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools
{
	/// <summary>
	/// An event type used to set a new size for the mask from any class
	/// </summary>
	public struct MMSpriteMaskEvent
	{
		public enum MMSpriteMaskEventTypes { MoveToNewPosition, ExpandAndMoveToNewPosition, DoubleMask }

		public MMSpriteMaskEventTypes EventType;
		public Vector2 NewPosition;
		public Vector2 NewSize;
		public float Duration;
		public MMTween.MMTweenCurve Curve;

		public MMSpriteMaskEvent(MMSpriteMaskEventTypes eventType, Vector2 newPosition, Vector2 newSize, float duration, MMTween.MMTweenCurve curve)
		{
			EventType = eventType;
			NewPosition = newPosition;
			NewSize = newSize;
			Duration = duration;
			Curve = curve;
		}

		static MMSpriteMaskEvent e;
		public static void Trigger(MMSpriteMaskEventTypes eventType, Vector2 newPosition, Vector2 newSize, float duration, MMTween.MMTweenCurve curve)
		{
			e.EventType = eventType;
			e.NewPosition = newPosition;
			e.NewSize = newSize;
			e.Duration = duration;
			e.Curve = curve;
			MMEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// This class will automatically look for sprite renderers, particle systems, tilemaps in the scene, and change their SpriteMaskInteraction settings according to the one set in the inspector
	/// Use the NoMask tag on objects you don't want automatically setup
	/// </summary>
	public class MMSpriteMask : MonoBehaviour, MMEventListener<MMSpriteMaskEvent>
	{
		/// the possible timescales this mask can operate on
		public enum Timescales { Scaled, Unscaled }

		[Header("Scale")]
		/// the scale multiplier to apply to the sprite mask
		[Tooltip("the scale multiplier to apply to the sprite mask")]
		public float ScaleMultiplier = 100f;

		[Header("Auto setup")]
		/// whether or not all sprite renderers should be converted
		[Tooltip("whether or not all sprite renderers should be converted")]
		public bool AutomaticallySetupSpriteRenderers = true;
		/// whether or not all particle systems should be converted
		[Tooltip("whether or not all particle systems should be converted")]
		public bool AutomaticallySetupParticleSystems = true;
		/// whether or not all tilemaps should be converted
		[Tooltip("whether or not all tilemaps should be converted")]
		public bool AutomaticallySetupTilemaps = true;

		[Header("Behaviour")]

		/// if this is true, this mask will move when catching a sprite mask event
		[Tooltip("if this is true, this mask will move when catching a sprite mask event")]
		public bool CatchEvents = true;
		/// the timescale this mask operates on
		[Tooltip("the timescale this mask operates on")]
		public Timescales Timescale = Timescales.Unscaled;
		/// the type of interaction to apply to all renderers
		[Tooltip("the type of interaction to apply to all renderers")]
		public SpriteMaskInteraction MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;

		public float MaskTime { get { float time = (Timescale == Timescales.Unscaled) ? Time.unscaledTime : Time.time; return time; } }

		/// <summary>
		/// On Awake we setup our objects
		/// </summary>
		protected virtual void Start()
		{
			SetupMaskSettingsAutomatically();
		}

		/// <summary>
		/// Looks for mask settings and updates them
		/// </summary>
		protected virtual void SetupMaskSettingsAutomatically()
		{
			if (AutomaticallySetupSpriteRenderers)
			{
				var foundSpriteRenderers = FindObjectsOfType<SpriteRenderer>();
				if (foundSpriteRenderers.Length > 0)
				{
					foreach (SpriteRenderer renderer in foundSpriteRenderers)
					{
						if (!renderer.gameObject.CompareTag("NoMask"))
						{
							renderer.maskInteraction = MaskInteraction;
						}                        
					}
				}                
			}

			if (AutomaticallySetupTilemaps)
			{
				var foundTilemapRenderers = FindObjectsOfType<TilemapRenderer>();
				if (foundTilemapRenderers.Length > 0)
				{
					foreach (TilemapRenderer renderer in foundTilemapRenderers)
					{
						if (!renderer.gameObject.CompareTag("NoMask"))
						{
							renderer.maskInteraction = MaskInteraction;
						}
					}
				}                
			}

			if (AutomaticallySetupParticleSystems)
			{
				var foundParticleSystems = FindObjectsOfType<ParticleSystem>();
				if (foundParticleSystems.Length > 0)
				{
					foreach (ParticleSystem system in foundParticleSystems)
					{
						if (!system.gameObject.CompareTag("NoMask"))
						{
							ParticleSystemRenderer pr = system.GetComponent<ParticleSystemRenderer>();
							pr.maskInteraction = MaskInteraction;
						}                        
					}
				}
			}
		}

		/// <summary>
		/// Moves the mask to a new size and position for a certain duration and along a certain curve
		/// </summary>
		/// <param name="newPosition"></param>
		/// <param name="newSize"></param>
		/// <param name="duration"></param>
		/// <param name="curve"></param>
		public virtual void MoveMaskTo(Vector2 newPosition, Vector2 newSize, float duration, MMTween.MMTweenCurve curve)
		{
			StartCoroutine(MoveMaskToCoroutine(newPosition, newSize, duration, curve));            
		}

		/// <summary>
		/// Moves the mask to a new size and position after having expanded to encompass its origin size/position and
		/// the destination's size/position
		/// </summary>
		/// <param name="newPosition"></param>
		/// <param name="newSize"></param>
		/// <param name="duration"></param>
		/// <param name="curve"></param>
		public virtual void ExpandAndMoveMaskTo(Vector2 newPosition, Vector2 newSize, float duration, MMTween.MMTweenCurve curve)
		{
			StartCoroutine(ExpandAndMoveMaskToCoroutine(newPosition, newSize, duration, curve));
		}

		protected Vector3 _initialPosition;
		protected Vector3 _initialScale;
		protected Vector3 _newPosition;
		protected Vector3 _newScale;
		protected Vector3 _targetPosition;
		protected Vector3 _targetScale;

		/// <summary>
		/// Coroutine that moves the mask 
		/// </summary>
		/// <param name="newPosition"></param>
		/// <param name="newSize"></param>
		/// <param name="duration"></param>
		/// <param name="curve"></param>
		/// <returns></returns>
		protected virtual IEnumerator MoveMaskToCoroutine(Vector2 newPosition, Vector2 newSize, float duration, MMTween.MMTweenCurve curve)
		{
			if (duration > 0)
			{
				_initialPosition = this.transform.position;
				_initialScale = this.transform.localScale;
				_targetPosition = ComputeTargetPosition(newPosition);
				_targetScale = ComputeTargetScale(newSize);
				float startedAt = MaskTime;

				while (MaskTime - startedAt <= duration)
				{
					float currentTime = MaskTime - startedAt;

					_newPosition = MMTween.Tween(currentTime, 0f, duration, _initialPosition, _targetPosition, curve);
					_newScale = MMTween.Tween(currentTime, 0f, duration, _initialScale, _targetScale, curve);

					this.transform.position = _newPosition;
					this.transform.localScale = _newScale;

					yield return null;
				}
			}

			this.transform.position = ComputeTargetPosition(newPosition);
			this.transform.localScale = ComputeTargetScale(newSize);
		}

		/// <summary>
		/// A coroutine that expands the mask to cover both its current position and its destination area, then resizes itself to match the destination size
		/// </summary>
		/// <param name="newPosition"></param>
		/// <param name="newSize"></param>
		/// <param name="duration"></param>
		/// <param name="curve"></param>
		/// <returns></returns>
		protected virtual IEnumerator ExpandAndMoveMaskToCoroutine(Vector2 newPosition, Vector2 newSize, float duration, MMTween.MMTweenCurve curve)
		{
			if (duration > 0)
			{
				_initialPosition = this.transform.position;
				_initialScale = this.transform.localScale;

				float startedAt = MaskTime;

				// first we move to the total size and position
				_targetScale.x = this.transform.localScale.x / 2f + Mathf.Abs((this.transform.position - (Vector3)newPosition).x) * ScaleMultiplier + ComputeTargetScale(newSize).x / 2f;
				_targetScale.y = this.transform.localScale.y / 2f + Mathf.Abs((this.transform.position - (Vector3)newPosition).y) * ScaleMultiplier + ComputeTargetScale(newSize).y / 2f;
				_targetScale.z = 1f;

				_targetPosition = (
					(this.transform.position + (Vector3.up * this.transform.localScale.y/ ScaleMultiplier / 2f) + (Vector3.left * this.transform.localScale.x/ ScaleMultiplier / 2f))
					+
					((Vector3)newPosition + (Vector3.down * newSize.y / 2f) + (Vector3.right * newSize.x / 2f))
				) / 2f;


				while (MaskTime - startedAt <= (duration / 2f))
				{
					float currentTime = MaskTime - startedAt;

					_newPosition = MMTween.Tween(currentTime, 0f, (duration / 2f), _initialPosition, _targetPosition, curve);
					_newScale = MMTween.Tween(currentTime, 0f, (duration / 2f), _initialScale, _targetScale, curve);

					this.transform.position = _newPosition;
					this.transform.localScale = _newScale;
                    
					yield return null;
				}
                
				// then we move to the final position
				startedAt = MaskTime;
				_initialPosition = this.transform.position;
				_initialScale = this.transform.localScale;
				_targetPosition = ComputeTargetPosition(newPosition);
				_targetScale = ComputeTargetScale(newSize);
                
				while (MaskTime - startedAt <= duration / 2f)
				{
					float currentTime = MaskTime - startedAt;

					_newPosition = MMTween.Tween(currentTime, 0f, (duration / 2f), _initialPosition, _targetPosition, curve);
					_newScale = MMTween.Tween(currentTime, 0f, (duration / 2f), _initialScale, _targetScale, curve);

					this.transform.position = _newPosition;
					this.transform.localScale = _newScale;
                    
					yield return null;
				}
			}

			this.transform.position = ComputeTargetPosition(newPosition);
			this.transform.localScale = ComputeTargetScale(newSize);
		}

		/// <summary>
		/// Determines the new position of the mask
		/// </summary>
		/// <param name="newPosition"></param>
		/// <returns></returns>
		protected virtual Vector3 ComputeTargetPosition(Vector3 newPosition)
		{
			return newPosition;
		}

		/// <summary>
		/// Determines the scale of the mask
		/// </summary>
		/// <param name="newScale"></param>
		/// <returns></returns>
		protected virtual Vector3 ComputeTargetScale(Vector3 newScale)
		{
			return ScaleMultiplier * newScale;
		}

		/// <summary>
		/// Catches sprite mask events
		/// </summary>
		/// <param name="spriteMaskEvent"></param>
		public virtual void OnMMEvent(MMSpriteMaskEvent spriteMaskEvent)
		{
			if (!CatchEvents)
			{
				return;
			}

			switch(spriteMaskEvent.EventType)
			{
				case MMSpriteMaskEvent.MMSpriteMaskEventTypes.MoveToNewPosition:
					MoveMaskTo(spriteMaskEvent.NewPosition, spriteMaskEvent.NewSize, spriteMaskEvent.Duration, spriteMaskEvent.Curve);
					break;
				case MMSpriteMaskEvent.MMSpriteMaskEventTypes.ExpandAndMoveToNewPosition:
					ExpandAndMoveMaskTo(spriteMaskEvent.NewPosition, spriteMaskEvent.NewSize, spriteMaskEvent.Duration, spriteMaskEvent.Curve);
					break;
			}
		}

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMSpriteMaskEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMSpriteMaskEvent>();
		}
	}
}