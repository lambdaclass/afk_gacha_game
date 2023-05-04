using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	public enum MMCameraEventTypes { SetTargetCharacter, SetConfiner, StartFollowing, StopFollowing, RefreshPosition, ResetPriorities, RefreshAutoFocus }

	public struct MMCameraEvent
	{
		public MMCameraEventTypes EventType;
		public Character TargetCharacter;
		public Collider Bounds;

		public MMCameraEvent(MMCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
		{
			EventType = eventType;
			TargetCharacter = targetCharacter;
			Bounds = bounds;
		}

		static MMCameraEvent e;
		public static void Trigger(MMCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
		{
			e.EventType = eventType;
			e.Bounds = bounds;
			e.TargetCharacter = targetCharacter;
			MMEventManager.TriggerEvent(e);
		}
	}
}