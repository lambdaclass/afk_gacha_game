using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A decorative class used to make background elements jump
	/// </summary>
	public class ExplodudesDecoration : TopDownMonoBehaviour, MMEventListener<MMGameEvent>
	{
		/// the minimum force to apply to the background elements
		[Tooltip("the minimum force to apply to the background elements")]
		public Vector3 MinForce;
		/// the maximum force to apply to the background elements
		[Tooltip("the maximum force to apply to the background elements")]
		public Vector3 MaxForce;

		/// a test button
		[MMInspectorButton("Jump")]
		public bool JumpButton;

		protected Rigidbody _rigidbody;
		protected const string eventName = "Bomb";
		protected Vector3 _force;

		/// <summary>
		/// On start we grab our rigidbody
		/// </summary>
		protected virtual void Start()
		{
			_rigidbody = this.gameObject.GetComponent<Rigidbody>();
		}
        
		/// <summary>
		/// When we get a bomb event we make our rb jump
		/// </summary>
		/// <param name="gameEvent"></param>
		public virtual void OnMMEvent(MMGameEvent gameEvent)
		{
			if (gameEvent.EventName == eventName)
			{
				Jump();
			}
		}

		/// <summary>
		/// Adds a random force to the rb
		/// </summary>
		public virtual void Jump()
		{
			_force.x = Random.Range(MinForce.x, MaxForce.x);
			_force.y = Random.Range(MinForce.y, MaxForce.y);
			_force.z = Random.Range(MinForce.z, MaxForce.z);
			_rigidbody.AddForce(_force, ForceMode.Impulse);
		}

		/// <summary>
		/// On enable we start listening for bomb events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMGameEvent>();
		}

		/// <summary>
		/// On disable we stop listening for bomb events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMGameEvent>();
		}
	}
}