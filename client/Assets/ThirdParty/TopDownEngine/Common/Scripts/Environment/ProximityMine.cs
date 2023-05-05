using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a collider (2D or 3D) and it'll let you trigger things after a duration, like a mine would.
	/// It also comes with options to interrupt or reset the timer on exit. 
	/// </summary>
	public class ProximityMine : TopDownMonoBehaviour
	{
		[Header("Proximity Mine")]
		/// the layers that will trigger this mine
		[Tooltip("the layers that will trigger this mine")]
		public LayerMask TargetLayerMask;
		/// whether or not to disable the mine when it triggers/explodes
		[Tooltip("whether or not to disable the mine when it triggers/explodes")]
		public bool DisableMineOnTrigger = true;

		[Header("WarningDuration")] 
		/// the duration of the warning phase, in seconds, betfore the mine triggers
		[Tooltip("the duration of the warning phase, in seconds, betfore the mine triggers")]
		public float WarningDuration = 2f;
		/// whether or not the warning should stop when exiting the zone
		[Tooltip("whether or not the warning should stop when exiting the zone")]
		public bool WarningStopsOnExit = false;
		/// whether or not the warning duration should reset when exiting the zone
		[Tooltip("whether or not the warning duration should reset when exiting the zone")]
		public bool WarningDurationResetsOnExit = false;

		/// a read only display of the current duration before explosion
		[Tooltip("a read only display of the current duration before explosion")]
		[MMReadOnly] 
		public float TimeLeftBeforeTrigger;
        
		[Header("Feedbacks")]
		/// the feedback to play when the warning phase starts
		[Tooltip("the feedback to play when the warning phase starts")]
		public MMFeedbacks OnWarningStartsFeedbacks;
		/// a feedback to play when the warning phase stops
		[Tooltip("a feedback to play when the warning phase stops")] 
		public MMFeedbacks OnWarningStopsFeedbacks;
		/// a feedback to play when the warning phase is reset
		[Tooltip("a feedback to play when the warning phase is reset")] 
		public MMFeedbacks OnWarningResetFeedbacks;
		/// a feedback to play when the mine triggers
		[Tooltip("a feedback to play when the mine triggers")]
		public MMFeedbacks OnMineTriggerFeedbacks;
        
		protected bool _inside = false;
        
		/// <summary>
		/// On Start we initialize our mine
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// On init we initialize our feedbacks and duration
		/// </summary>
		public virtual void Initialization()
		{
			OnWarningStartsFeedbacks?.Initialization();
			OnWarningStopsFeedbacks?.Initialization();
			OnWarningResetFeedbacks?.Initialization();
			OnMineTriggerFeedbacks?.Initialization();
			TimeLeftBeforeTrigger = WarningDuration;
		}
        
		/// <summary>
		/// When colliding, we start our timer if needed
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void Colliding(GameObject collider)
		{
			if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
			{
				return;
			}

			_inside = true;
            
			OnWarningStartsFeedbacks?.PlayFeedbacks();
		}

		/// <summary>
		/// When exiting, we stop our timer if needed
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void Exiting(GameObject collider)
		{
			if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
			{
				return;
			}

			if (!WarningStopsOnExit)
			{
				return;
			}
            
			OnWarningStopsFeedbacks?.PlayFeedbacks();

			if (WarningDurationResetsOnExit)
			{
				OnWarningResetFeedbacks?.PlayFeedbacks();
				TimeLeftBeforeTrigger = WarningDuration;
			}
            
			_inside = false;
		}

		/// <summary>
		/// Describes what happens when the mine explodes
		/// </summary>
		public virtual void TriggerMine()
		{
			OnMineTriggerFeedbacks?.PlayFeedbacks();
            
			if (DisableMineOnTrigger)
			{
				this.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// On Update if a target is inside the zone, we update our timer
		/// </summary>
		protected virtual void Update()
		{
			if (_inside)
			{
				TimeLeftBeforeTrigger -= Time.deltaTime;
			}

			if (TimeLeftBeforeTrigger <= 0)
			{
				TriggerMine();
			}
		}
        
		/// <summary>
		/// When a collision with the player is triggered, we give damage to the player and knock it back
		/// </summary>
		/// <param name="collider">what's colliding with the object.</param>
		public virtual void OnTriggerStay2D(Collider2D collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// On trigger enter 2D, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>S
		public virtual void OnTriggerEnter2D(Collider2D collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// On trigger stay, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerStay(Collider collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// On trigger enter, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerEnter(Collider collider)
		{
			Colliding(collider.gameObject);
		}

		/// <summary>
		/// On trigger enter, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>
		public virtual void OnTriggerExit(Collider collider)
		{
			Exiting(collider.gameObject);
		}

		/// <summary>
		/// On trigger enter 2D, we call our colliding endpoint
		/// </summary>
		/// <param name="collider"></param>S
		public virtual void OnTriggerExit2D(Collider2D collider)
		{
			Exiting(collider.gameObject);
		}
	}    
}