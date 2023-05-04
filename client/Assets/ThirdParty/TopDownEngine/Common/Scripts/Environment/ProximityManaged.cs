using System;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class to add to any object in your scene to mark it as managed by a proximity manager.
	/// </summary>
	public class ProximityManaged : TopDownMonoBehaviour
	{
		[Header("Thresholds")]
		/// the distance from the proximity center (the player) under which the object should be enabled
		[Tooltip("the distance from the proximity center (the player) under which the object should be enabled")]
		public float EnableDistance = 35f;
		/// the distance from the proximity center (the player) after which the object should be disabled
		[Tooltip("the distance from the proximity center (the player) after which the object should be disabled")]
		public float DisableDistance = 45f;

		/// whether or not this object was disabled by the ProximityManager
		[MMReadOnly]
		[Tooltip("whether or not this object was disabled by the ProximityManager")]
		public bool DisabledByManager;

		[Header("Debug")] 
		/// a debug manager to add this object to, only used for debug
		[Tooltip("a debug manager to add this object to, only used for debug")]
		public ProximityManager DebugProximityManager;
		/// a debug button to add this object to the debug manager
		[MMInspectorButton("DebugAddObject")]
		public bool AddButton;

		public ProximityManager Manager { get; set; }
		
		/// <summary>
		/// A debug method used to add this object to a proximity manager
		/// </summary>
		public virtual void DebugAddObject()
		{
			DebugProximityManager.AddControlledObject(this);
		}

		/// <summary>
		/// On enable, we register to our manager
		/// </summary>
		private void OnEnable()
		{
			if (Manager != null)
			{
				return;
			}
			
			ProximityManager proximityManager = ProximityManager.Current;
			if (proximityManager != null) 
			{
				var targets = proximityManager.ControlledObjects;
				if (targets != null && targets.Count > 0) 
				{
					if (Manager == null)
					{
						Manager = proximityManager;
						proximityManager.ControlledObjects.Add(this);
					}
				}
			}
		}

		/// <summary>
		/// On Destroy we let our manager know we're gone
		/// </summary>
		private void OnDestroy()
		{
			if (Manager != null && Manager.ControlledObjects != null)
			{
				Manager.ControlledObjects.Remove(this);
			}
		}
	}
}