using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Bomb")]
	public class Bomb : TopDownMonoBehaviour 
	{
		/// the shape of the bomb's damage area
		public enum DamageAreaShapes { Rectangle, Circle }

		[Header("Explosion")]
		/// the delay before the bomb explodes
		[Tooltip("the delay before the bomb explodes")]
		public float TimeBeforeExplosion = 2f;
		/// a vfx to instantiate when the bomb explodes
		[Tooltip("a vfx to instantiate when the bomb explodes")]
		public GameObject ExplosionEffect;
		/// a sound to play when the bomb explodes
		[Tooltip("a sound to play when the bomb explodes")]
		public AudioClip ExplosionSfx;

		[Header("Flicker")]
		/// whether or not the sprite should flicker before explosion
		[Tooltip("whether or not the sprite should flicker before explosion")]
		public bool FlickerSprite = true;
		/// the duration before the flicker starts
		[Tooltip("the duration before the flicker starts")]
		public float TimeBeforeFlicker = 1f;
		/// the name of the property that should flicker
		[Tooltip("the name of the property that should flicker")]
		public string MaterialPropertyName = "_Color";

		[Header("Damage Area")]
		/// the collider of the damage area
		[Tooltip("the collider of the damage area")]
		public Collider2D DamageAreaCollider;
		/// the duration of the damage area
		[Tooltip("the duration of the damage area")]
		public float DamageAreaActiveDuration = 1f;

		protected float _timeSinceStart;
		protected Renderer _renderer;
		protected MMPoolableObject _poolableObject;
		protected bool _flickering;
		protected bool _damageAreaActive;
		protected Color _initialColor;
		protected Color _flickerColor = new Color32(255, 20, 20, 255);
		protected MaterialPropertyBlock _propertyBlock;
		
		/// <summary>
		/// On enable, we initialize our bomb
		/// </summary>
		protected virtual void OnEnable()
		{
			Initialization ();
		}

		/// <summary>
		/// Initializes the bomb
		/// </summary>
		protected virtual void Initialization()
		{
			if (DamageAreaCollider == null)
			{
				Debug.LogWarning ("There's no damage area associated to this bomb : " + this.name + ". You should set one via its inspector.");
				return;
			}
			DamageAreaCollider.isTrigger = true;
			DisableDamageArea ();

			_propertyBlock = new MaterialPropertyBlock();
			_renderer = gameObject.MMGetComponentNoAlloc<Renderer> ();
			if (_renderer != null)
			{
				if (_renderer.sharedMaterial.HasProperty(MaterialPropertyName))
				{
					_initialColor = _renderer.sharedMaterial.GetColor(MaterialPropertyName);    
				}
			}

			_poolableObject = gameObject.MMGetComponentNoAlloc<MMPoolableObject> ();
			if (_poolableObject != null)
			{
				_poolableObject.LifeTime = 0;
			}

			_timeSinceStart = 0;
			_flickering = false;
			_damageAreaActive = false;
		}

		/// <summary>
		/// On update, makes our bomb flicker, activates the damage area and destroys the bomb if needed
		/// </summary>
		protected virtual void Update()
		{
			_timeSinceStart += Time.deltaTime;
			// flickering
			if (_timeSinceStart >= TimeBeforeFlicker)
			{
				if (!_flickering && FlickerSprite)
				{
					// We make the bomb's sprite flicker
					if (_renderer != null)
					{
						StartCoroutine(MMImage.Flicker(_renderer,_initialColor,_flickerColor,0.05f,(TimeBeforeExplosion - TimeBeforeFlicker)));	
					}
				}
			}

			// activate damage area
			if (_timeSinceStart >= TimeBeforeExplosion && !_damageAreaActive)
			{
				EnableDamageArea ();
				_renderer.enabled = false;
				InstantiateExplosionEffect ();
				PlayExplosionSound ();
				_damageAreaActive = true;
			}

			if (_timeSinceStart >= TimeBeforeExplosion + DamageAreaActiveDuration)
			{
				DestroyBomb ();
			}
		}

		/// <summary>
		/// Destroys the bomb
		/// </summary>
		protected virtual void DestroyBomb()
		{
			_renderer.enabled = true;
			_renderer.GetPropertyBlock(_propertyBlock);
			_propertyBlock.SetColor(MaterialPropertyName, _initialColor);
			_renderer.SetPropertyBlock(_propertyBlock);
			if (_poolableObject != null)
			{
				_poolableObject.Destroy ();	
			}
			else
			{
				Destroy (gameObject);
			}

		}

		/// <summary>
		/// Instantiates a VFX at the bomb's position
		/// </summary>
		protected virtual void InstantiateExplosionEffect()
		{
			// instantiates the destroy effect
			if (ExplosionEffect!=null)
			{
				GameObject instantiatedEffect=(GameObject)Instantiate(ExplosionEffect,transform.position,transform.rotation);
				instantiatedEffect.transform.localScale = transform.localScale;
			}
		}

		/// <summary>
		/// Plays a sound on explosion
		/// </summary>
		protected virtual void PlayExplosionSound()
		{
			if (ExplosionSfx!=null)
			{
				MMSoundManagerSoundPlayEvent.Trigger(ExplosionSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);
			}
		}

		/// <summary>
		/// Enables the damage area.
		/// </summary>
		protected virtual void EnableDamageArea()
		{
			DamageAreaCollider.enabled = true;
		}

		/// <summary>
		/// Disables the damage area.
		/// </summary>
		protected virtual void DisableDamageArea()
		{
			DamageAreaCollider.enabled = false;
		}
	}
}