using System;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	public class DeadlineDoor : TopDownMonoBehaviour
	{

		public float DoorMovementDuration = 0.3f;
		public MMTween.MMTweenCurve MovementCurve;
		public float OpenAngle = 100f;

		protected BoxCollider2D _boxCollider2D;
		protected Vector3 _vectorA;
		protected Vector3 _vectorB;
		protected Vector3 _newAngles;
		protected bool _rotating = false;

		protected virtual void Start()
		{
			Initialization();
		}
		
		protected virtual void Initialization()
		{
			_boxCollider2D = this.gameObject.GetComponent<BoxCollider2D>();
		}
		
		protected void OnTriggerEnter2D(Collider2D other)
		{
			if (_rotating)
			{
				return;
			}
			
			_newAngles = this.transform.localEulerAngles;
			float targetAngle = 0f;
			float localAngle = this.transform.localEulerAngles.z;
			localAngle = (localAngle > 180) ? localAngle - 360 : localAngle;
			
			
			if (localAngle == 0f)
			{
				targetAngle = IsLeft(other.transform.position) ? -OpenAngle : OpenAngle;
			}
			else
			{
				
				if (localAngle < 0f)
				{
					targetAngle = IsLeft(other.transform.position) ? localAngle : 0f;	 
				}
				else
				{
					targetAngle = IsLeft(other.transform.position) ? 0f : localAngle;
				}
			}

			StartCoroutine(RotateDoor(localAngle, targetAngle));
		}

		protected virtual IEnumerator RotateDoor(float currentAngle, float targetAngle)
		{
			_rotating = true;
			float startTime = Time.time;

			while (Time.time - startTime < DoorMovementDuration)
			{
				float elapsed = Time.time - startTime;
				float newAngle = MMTween.Tween(elapsed, 0f, DoorMovementDuration, currentAngle, targetAngle, MovementCurve);
				_newAngles.z = newAngle;
				this.transform.localEulerAngles = _newAngles;
				yield return null;
			}
			_newAngles.z = targetAngle;
			this.transform.localEulerAngles = _newAngles;
			_rotating = false;
		}
		
		protected virtual bool IsLeft(Vector3 otherPosition)
		{
			_vectorA = (this.transform.position + this.transform.right * _boxCollider2D.size.x) - this.transform.position;
			_vectorB = otherPosition - this.transform.position;
			return (-_vectorA.x * _vectorB.y + _vectorA.y * _vectorB.x < 0);
		}
	}
}
