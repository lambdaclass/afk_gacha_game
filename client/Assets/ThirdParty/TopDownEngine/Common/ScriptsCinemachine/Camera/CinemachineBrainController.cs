using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
#if MM_CINEMACHINE
using Cinemachine;
#endif

namespace MoreMountains.TopDownEngine
{
	public enum MMCinemachineBrainEventTypes { ChangeBlendDuration }

	/// <summary>
	/// An event used to interact with camera brains
	/// </summary>
	public struct MMCinemachineBrainEvent
	{
		public MMCinemachineBrainEventTypes EventType;
		public float Duration;

		public MMCinemachineBrainEvent(MMCinemachineBrainEventTypes eventType, float duration)
		{
			EventType = eventType;
			Duration = duration;
		}

		static MMCinemachineBrainEvent e;
		public static void Trigger(MMCinemachineBrainEventTypes eventType, float duration)
		{
			e.EventType = eventType;
			e.Duration = duration;
			MMEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// This class is designed to control CinemachineBrains, letting you control their default blend values via events from any class
	/// </summary>
	#if MM_CINEMACHINE
	[RequireComponent(typeof(CinemachineBrain))]
	#endif
	public class CinemachineBrainController : TopDownMonoBehaviour, MMEventListener<MMCinemachineBrainEvent>
	{
		#if MM_CINEMACHINE
		protected CinemachineBrain _brain;
		#endif

		/// <summary>
		/// On Awake we store our brain reference
		/// </summary>
		protected virtual void Awake()
		{
			#if MM_CINEMACHINE
			_brain = this.gameObject.GetComponent<CinemachineBrain>();
			#endif
		}

		/// <summary>
		/// Changes the default blend duration for this brain to the one set in parameters
		/// </summary>
		/// <param name="newDuration"></param>
		public virtual void SetDefaultBlendDuration(float newDuration)
		{
			#if MM_CINEMACHINE
			_brain.m_DefaultBlend.m_Time = newDuration;
			#endif
		}

		/// <summary>
		/// When we get a brain event, we treat it
		/// </summary>
		/// <param name="cinemachineBrainEvent"></param>
		public virtual void OnMMEvent(MMCinemachineBrainEvent cinemachineBrainEvent)
		{
			switch (cinemachineBrainEvent.EventType)
			{
				case MMCinemachineBrainEventTypes.ChangeBlendDuration:
					SetDefaultBlendDuration(cinemachineBrainEvent.Duration);
					break;
			}
		}

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMCinemachineBrainEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMCinemachineBrainEvent>();
		}
	}
}