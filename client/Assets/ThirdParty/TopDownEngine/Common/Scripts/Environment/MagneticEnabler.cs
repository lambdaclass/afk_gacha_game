using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This object will enable Magnetic objects in your scene when they enter its associated collider2D (make sure you add one).
	/// While magnetic objects can work on their own, and handle their own range detection, you can also use a different architecture, where an enabler makes them move.
	/// A typical use case would be to add it to a character, nested under its top level :
	/// 
	/// MyCharacter (top level, with a Character, controller, abilities, etc)
	/// - MyMagneticEnabler (with this class and a CircleCollider2D for example)
	/// 
	/// Then, in your scene you'd have Magnetic objects, with StartMagnetOnEnter disabled. 
	/// Your magnetic enabler would make them follow this specific target on enter.
	/// From the enabler you can also have it override the follow speed and acceleration.
	/// </summary>
	public class MagneticEnabler : TopDownMonoBehaviour
	{
		[Header("Detection")]
		/// the layermask this magnetic enabler looks at to enable magnetic elements
		[Tooltip("the layermask this magnetic enabler looks at to enable magnetic elements")]
		public LayerMask TargetLayerMask = LayerManager.PlayerLayerMask;
		/// a list of the magnetic type ID this enabler targets
		[Tooltip("a list of the magnetic type ID this enabler targets")]
		public List<int> MagneticTypeIDs;

		[Header("Overrides")]
		/// if this is true, the follow position speed will be overridden with the one specified here
		[Tooltip("if this is true, the follow position speed will be overridden with the one specified here")]
		public bool OverrideFollowPositionSpeed = false;
		/// the value with which to override the speed
		[Tooltip("the speed with which to override the speed")]
		[MMCondition("OverrideFollowPositionSpeed", true)]
		public float FollowPositionSpeed = 5f;
		/// if this is true, the acceleration will be overridden with the one specified here
		[Tooltip("if this is true, the acceleration will be overridden with the one specified here")]
		public bool OverrideFollowAcceleration = false;
		/// the value with which to override the acceleration
		[Tooltip("the speed with which to override the acceleration")]
		[MMCondition("OverrideFollowAcceleration", true)]
		public float FollowAcceleration = 0.75f;

		protected Collider2D _collider2D;
		protected Magnetic _magnetic;

		/// <summary>
		/// On Awake we initialize our magnetic enabler
		/// </summary>
		protected virtual void Awake()
		{
			Initialization();
		}

		/// <summary>
		/// Grabs the collider2D and ensures it's set as trigger
		/// </summary>
		protected virtual void Initialization()
		{
			_collider2D = this.gameObject.GetComponent<Collider2D>();
			if (_collider2D != null)
			{
				_collider2D.isTrigger = true;
			}
		}

		/// <summary>
		/// When something enters our trigger 2D, if it's a proper target, we start following it
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter2D(Collider2D colliding)
		{
			OnTriggerEnterInternal(colliding.gameObject);
		}

		/// <summary>
		/// When something enters our trigger, if it's a proper target, we start following it
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter(Collider colliding)
		{
			OnTriggerEnterInternal(colliding.gameObject);
		}

		/// <summary>
		/// If we trigger with a Magnetic, and the ID matches, we enable it 
		/// </summary>
		/// <param name="colliding"></param>
		protected virtual void OnTriggerEnterInternal(GameObject colliding)
		{
			if (!TargetLayerMask.MMContains(colliding.layer))
			{
				return;
			}

			_magnetic = colliding.MMGetComponentNoAlloc<Magnetic>();
			if (_magnetic == null)
			{
				return;
			}

			bool idFound = false;
			if (_magnetic.MagneticTypeID == 0)
			{
				idFound = true;
			}
			else
			{
				foreach (int id in MagneticTypeIDs)
				{
					if (id == _magnetic.MagneticTypeID)
					{
						idFound = true;
					}
				}
			}            

			if (!idFound)
			{
				return;
			}

			if (OverrideFollowAcceleration)
			{
				_magnetic.FollowAcceleration = FollowAcceleration;
			}

			if (OverrideFollowPositionSpeed)
			{
				_magnetic.FollowPositionSpeed = FollowPositionSpeed;
			}

			_magnetic.SetTarget(this.transform);
			_magnetic.StartFollowing();
		}
	}
}