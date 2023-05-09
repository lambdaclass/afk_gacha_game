using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used to handle aim markers, (usually circular) visual elements 
	/// </summary>
	public class AimMarker : TopDownMonoBehaviour
	{
		/// the possible movement modes for aim markers
		public enum MovementModes { Instant, Interpolate }

		[Header("Movement")]
		/// The selected movement mode for this aim marker. Instant will move the marker instantly to its target, Interpolate will animate its position over time
		[Tooltip("The selected movement mode for this aim marker. Instant will move the marker instantly to its target, Interpolate will animate its position over time")]
		public MovementModes MovementMode;
		/// an offset to apply to the position of the target (useful if you want, for example, the marker to appear above the target)
		[Tooltip("an offset to apply to the position of the target (useful if you want, for example, the marker to appear above the target)")]
		public Vector3 Offset;
		/// When in Interpolate mode, the duration of the movement animation
		[Tooltip("When in Interpolate mode, the duration of the movement animation")]
		[MMEnumCondition("MovementMode", (int)MovementModes.Interpolate)]
		public float MovementDuration = 0.2f;
		/// When in Interpolate mode, the curve to animate the movement on
		[Tooltip("When in Interpolate mode, the curve to animate the movement on")]
		[MMEnumCondition("MovementMode", (int)MovementModes.Interpolate)]
		public MMTween.MMTweenCurve MovementCurve = MMTween.MMTweenCurve.EaseInCubic;
		/// When in Interpolate mode, the delay before the marker moves when changing target
		[Tooltip("When in Interpolate mode, the delay before the marker moves when changing target")]
		[MMEnumCondition("MovementMode", (int)MovementModes.Interpolate)]
		public float MovementDelay = 0f;

		[Header("Feedbacks")]
		/// A feedback to play when a target is found and we didn't have one already
		[Tooltip("A feedback to play when a target is found and we didn't have one already")]
		public MMFeedbacks FirstTargetFeedback;
		/// a feedback to play when we already had a target and just found a new one
		[Tooltip("a feedback to play when we already had a target and just found a new one")]
		public MMFeedbacks NewTargetAssignedFeedback;
		/// a feedback to play when no more targets are found, and we just lost our last target
		[Tooltip("a feedback to play when no more targets are found, and we just lost our last target")]
		public MMFeedbacks NoMoreTargetFeedback;

		protected Transform _target;
		protected Transform _targetLastFrame = null;
		protected WaitForSeconds _movementDelayWFS;
		protected float _lastTargetChangeAt = 0f;

		/// <summary>
		/// On Awake we initialize our feedbacks and delay
		/// </summary>
		protected virtual void Awake()
		{
			FirstTargetFeedback?.Initialization(this.gameObject);
			NewTargetAssignedFeedback?.Initialization(this.gameObject);
			NoMoreTargetFeedback?.Initialization(this.gameObject);
			if (MovementDelay > 0f)
			{
				_movementDelayWFS = new WaitForSeconds(MovementDelay);
			}
		}

		/// <summary>
		/// On Update we check if we've changed target, and follow it if needed
		/// </summary>
		protected virtual void Update()
		{
			HandleTargetChange();
			FollowTarget();
			_targetLastFrame = _target;
		}

		/// <summary>
		/// Makes this object follow the target's position
		/// </summary>
		protected virtual void FollowTarget()
		{
			if (MovementMode == MovementModes.Instant)
			{
				this.transform.position = _target.transform.position + Offset;
			}
			else
			{
				if ((_target != null) && (Time.time - _lastTargetChangeAt > MovementDuration))
				{
					this.transform.position = _target.transform.position + Offset;
				}
			}
		}

		/// <summary>
		/// Sets a new target for this aim marker
		/// </summary>
		/// <param name="newTarget"></param>
		public virtual void SetTarget(Transform newTarget)
		{
			_target = newTarget;

			if (newTarget == null)
			{
				return;
			}

			this.gameObject.SetActive(true);

			if (_targetLastFrame == null)
			{
				this.transform.position = _target.transform.position + Offset;
			}

			if (MovementMode == MovementModes.Instant)
			{
				this.transform.position = _target.transform.position + Offset;
			}
			else
			{
				MMTween.MoveTransform(this, this.transform, this.transform.position, _target.transform.position + Offset, _movementDelayWFS, MovementDelay, MovementDuration, MovementCurve);
			}
		}

		/// <summary>
		/// Checks for target changes and triggers the appropriate methods if needed
		/// </summary>
		protected virtual void HandleTargetChange()
		{
			if (_target == _targetLastFrame)
			{
				return;
			}

			_lastTargetChangeAt = Time.time;

			if (_target == null)
			{
				NoMoreTargets();
				return;
			}

			if (_targetLastFrame == null)
			{
				FirstTargetFound();
				return;
			}

			if ((_targetLastFrame != null) && (_target != null))
			{
				NewTargetFound();
			}
		}

		/// <summary>
		/// When no more targets are found, and we just lost one, we play a dedicated feedback
		/// </summary>
		protected virtual void NoMoreTargets()
		{
			NoMoreTargetFeedback?.PlayFeedbacks();
		}

		/// <summary>
		/// When a new target is found and we didn't have one already, we play a dedicated feedback
		/// </summary>
		protected virtual void FirstTargetFound()
		{
			FirstTargetFeedback?.PlayFeedbacks();
		}

		/// <summary>
		/// When a new target is found, and we previously had another, we play a dedicated feedback
		/// </summary>
		protected virtual void NewTargetFound()
		{
			NewTargetAssignedFeedback?.PlayFeedbacks();
		}

		/// <summary>
		/// Hides this object
		/// </summary>
		public virtual void Disable()
		{
			this.gameObject.SetActive(false);
		}
	}
}