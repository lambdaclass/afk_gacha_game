using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class to handle a platform that moves in 2D along a set of nodes
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Moving Platform 2D")]
	public class MovingPlatform2D : MMPathMovement
	{
		[Header("Safe Distance")]
		/// whether or not to use Safe Distance mode, to force the character to move onto the platform 
		[Tooltip("whether or not to use Safe Distance mode, to force the character to move onto the platform ")]
		public bool UseSafeDistance = false;
		/// the distance to move the character at in safe distance mode
		[MMCondition("UseSafeDistance", true)]
		[Tooltip("the distance to move the character at in safe distance mode")]
		public float ForcedSafeDistance = 1f;

		protected TopDownController2D _topdDownController2D;
		protected Vector3 _translationVector;
        
		protected virtual void AttachCharacterToMovingPlatform(Collider2D collider)
		{
			_topdDownController2D = collider.gameObject.MMGetComponentNoAlloc<TopDownController2D>();
			if (_topdDownController2D != null)
			{
				_topdDownController2D.SetMovingPlatform(this);
			}
			// 
            
			if (UseSafeDistance)
			{
				float distance = Vector3.Distance(collider.transform.position, this.transform.position);
				if (distance > ForcedSafeDistance)
				{
					_translationVector = (this.transform.position - collider.transform.position).normalized * Mathf.Min(distance, ForcedSafeDistance);
					collider.transform.Translate(_translationVector);
				}                    
			}
		}

		protected virtual void DetachCharacterFromPlatform(Collider2D collider)
		{
			_topdDownController2D = collider.gameObject.MMGetComponentNoAlloc<TopDownController2D>();
			if (_topdDownController2D != null)
			{
				_topdDownController2D.SetMovingPlatform(null);
			}
		}

		/// <summary>
		/// When something collides, if it's a top down controller, we assign this platform to it
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			AttachCharacterToMovingPlatform(collider);
		}

		/// <summary>
		/// When something stops colliding, if it's a top down controller, we unassign this platform to it
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			DetachCharacterFromPlatform(collider);
		}
	}
}