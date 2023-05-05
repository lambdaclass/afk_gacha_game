using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this to a character and it'll trigger its MMRagdoller to ragdoll on death
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Ragdoll on Death")]
	public class CharacterRagdollOnDeath : TopDownMonoBehaviour
	{
		[Header("Binding")]
		/// The MMRagdoller for this character
		[Tooltip("the MMRagdoller for this character")]
		public MMRagdoller Ragdoller;
		/// A list of optional objects to disable on death
		[Tooltip("A list of optional objects to disable on death")]
		public List<GameObject> ObjectsToDisableOnDeath;
		/// A list of optional monos to disable on death
		[Tooltip("A list of optional monos to disable on death")]
		public List<MonoBehaviour> MonosToDisableOnDeath;

		[Header("Force")]
		/// the force by which the impact will be multiplied
		[Tooltip("the force by which the impact will be multiplied")]
		public float ForceMultiplier = 10000f;

		[Header("Test")]
		/// A test button to trigger the ragdoll from the inspector
		[MMInspectorButton("Ragdoll")]
		[Tooltip("A test button to trigger the ragdoll from the inspector")]
		public bool RagdollButton;
        
		protected TopDownController _controller;
		protected Health _health;
        
		/// <summary>
		/// On Awake we initialize our component
		/// </summary>
		protected virtual void Awake()
		{
			Initialization();
		}

		/// <summary>
		/// Grabs our health and controller
		/// </summary>
		protected virtual void Initialization()
		{
			_health = this.gameObject.GetComponent<Health>();
			_controller = this.gameObject.GetComponent<TopDownController>();
		}

		/// <summary>
		/// When we get a OnDeath event, we ragdoll
		/// </summary>
		protected virtual void OnDeath()
		{
			Ragdoll();
		}

		/// <summary>
		/// Disables the specified objects and monos and triggers the ragdoll
		/// </summary>
		protected virtual void Ragdoll()
		{
			foreach (GameObject go in ObjectsToDisableOnDeath)
			{
				go.SetActive(false);
			}
			foreach (MonoBehaviour mono in MonosToDisableOnDeath)
			{
				mono.enabled = false;
			}
			Ragdoller.Ragdolling = true;
			Ragdoller.transform.SetParent(null);
			Ragdoller.MainRigidbody.AddForce(_controller.AppliedImpact.normalized * ForceMultiplier, ForceMode.Acceleration);
		}

		/// <summary>
		/// On enable we start listening to OnDeath events
		/// </summary>
		protected virtual void OnEnable()
		{
			if (_health != null)
			{
				_health.OnDeath += OnDeath;
			}
		}
        
		/// <summary>
		/// OnDisable we stop listening to OnDeath events
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				_health.OnDeath -= OnDeath;
			}
		}
	}
}