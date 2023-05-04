using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.TopDownEngine
{
	
	/// <summary>
	/// Add this component to a character and it'll let you define a number of surfaces and associate walk and run sounds to them
	/// It will also let you trigger events when entering or exiting these surfaces
	/// Important : Surfaces are evaluated from top to bottom. The first surface definition that matches the current detected
	/// ground will be considered the current surface. So make sure your order them accordingly.
	/// </summary>
	[MMHiddenProperties("AbilityStopFeedbacks", "AbilityStartFeedbacks")]
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Surface Sounds")] 
	public class CharacterSurfaceSounds : CharacterAbility
	{	
		[Serializable]
		public class CharacterSurfaceSoundsItems
		{
			/// an ID to identify this surface in the list. Not used by anything but makes the list more readable
			[Tooltip("an ID to identify this surface in the list. Not used by anything but makes the list more readable")]
			public string ID;
			/// the list of layers that identify this surface
			[Tooltip("the list of layers that identify this surface")]
			public LayerMask Layers;
			/// whether or not to use a tag to identify this surface or just rely only on the layer
			[Tooltip("whether or not to use a tag to identify this surface or just rely only on the layer")]
			public bool UseTag;
			/// if using tags, the Tag that should be on this surface to identify it (on top of the layer)
			[Tooltip("if using tags, the Tag that should be on this surface to identify it (on top of the layer)")]
			[MMCondition("UseTag", true)]
			public string Tag;
			/// the sound to use for walking when on this surface
			[Tooltip("the sound to use for walking when on this surface")]
			public AudioClip WalkSound;
			/// the sound to use for running when on this surface
			[Tooltip("the sound to use for running when on this surface")]
			public AudioClip RunSound;
			/// a UnityEvent that will trigger when entering this surface
			[Tooltip("a UnityEvent that will trigger when entering this surface")]
			public UnityEvent OnEnterSurfaceFeedbacks;
			/// a UnityEvent that will trigger when exiting this surface
			[Tooltip("a UnityEvent that will trigger when exiting this surface")]
			public UnityEvent OnExitSurfaceFeedbacks;
		}
		
		/// the different dimensions detection can operate on (either 2D or 3D physics)
		public enum DimensionModes { TwoD, ThreeD }
		/// whether detection should rely on periodical raycasts or be driven by an external script (via the SetCurrentSurfaceIndex(int index) method)
		public enum SurfaceDetectionModes { Raycast, Script }
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows a character and it'll let you define a number of surfaces and associate walk and run sounds to them. " +
		                                              "It will also let you trigger events when entering or exiting these surfaces." +
		                                              "Important : Surfaces are evaluated from top to bottom. The first surface definition that matches the current detected ground will " +
		                                              "be considered the current surface. So make sure your order them accordingly."; }

		[Header("List of Surfaces")] 
		/// a list of surface definitions, defined by a layer, an optional tag, and a walk and run sound. These will be evaluated from top to bottom, first match found becomes the current surface.
		[Tooltip("a list of surface definitions, defined by a layer, an optional tag, and a walk and run sound. These will be evaluated from top to bottom, first match found becomes the current surface.")]
		public List<CharacterSurfaceSoundsItems> Surfaces;
		
		[Header("Detection")]
		/// the different dimensions detection can operate on (either 2D or 3D physics)
		[Tooltip("the different dimensions detection can operate on (either 2D or 3D physics)")]
		public DimensionModes DimensionMode = DimensionModes.ThreeD;
		/// whether detection should rely on periodical raycasts or be driven by an external script (via the SetCurrentSurfaceIndex(int index) method)
		[Tooltip("whether detection should rely on periodical raycasts or be driven by an external script (via the SetCurrentSurfaceIndex(int index) method)")]
		public SurfaceDetectionModes SurfaceDetectionMode = SurfaceDetectionModes.Raycast;
		/// the length of the raycast to cast to detect surfaces
		[Tooltip("the length of the raycast to cast to detect surfaces")]
		[MMEnumCondition("SurfaceDetectionMode", (int)SurfaceDetectionModes.Raycast)]
		public float RaycastLength = 2f;
		/// the direction of the raycast to cast to detect surfaces
		[Tooltip("the direction of the raycast to cast to detect surfaces")] 
		[MMEnumCondition("SurfaceDetectionMode", (int)SurfaceDetectionModes.Raycast)]
		public Vector3 RaycastDirection = Vector3.down;
		/// the frequency (in seconds) at which to cast the raycast to detect surfaces, usually you'll want to space them a bit to save on performance
		[Tooltip("the frequency (in seconds) at which to cast the raycast to detect surfaces, usually you'll want to space them a bit to save on performance")]
		[MMEnumCondition("SurfaceDetectionMode", (int)SurfaceDetectionModes.Raycast)]
		public float RaycastFrequency = 1f;
		
		[Header("Debug")]
		/// The current index of the surface we're on in the Surfaces list
		[Tooltip("The current index of the surface we're on in the Surfaces list")]
		[MMReadOnly]
		public int CurrentSurfaceIndex = -1;
		
		protected RaycastHit _raycastDownHit;
		protected LayerMask _raycastLayerMask;
		protected float _timeSinceLastCheck = -float.PositiveInfinity;
		protected int _surfaceIndexLastFrame;
		protected CharacterRun _characterRun;
		protected Collider2D _testSurface2D;
		
		/// <summary>
		/// A method you can use to force the surface index, when in ScriptDriven mode
		/// </summary>
		/// <param name="index"></param>
		public virtual void SetCurrentSurfaceIndex(int index)
		{
			CurrentSurfaceIndex = index;
		}

		/// <summary>
		/// On init we grab our run ability and init our layermasks
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_characterRun = _character.FindAbility<CharacterRun>();
			_surfaceIndexLastFrame = -1;
			foreach (CharacterSurfaceSoundsItems item in Surfaces)
			{
				_raycastLayerMask |= item.Layers;
			}
		}
		
		/// <summary>
		/// Every frame we detect surfaces if needed, and handle a potential surface change
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			DetectSurface();
			HandleSurfaceChange();
		}

		/// <summary>
		/// If we're on a new surface, we swap sounds and invoke our events
		/// </summary>
		protected virtual void HandleSurfaceChange()
		{
			if (_surfaceIndexLastFrame != CurrentSurfaceIndex)
			{
				if (_surfaceIndexLastFrame >= 0 && _surfaceIndexLastFrame < Surfaces.Count)
				{
					Surfaces[_surfaceIndexLastFrame].OnExitSurfaceFeedbacks?.Invoke();
				}
				Surfaces[CurrentSurfaceIndex].OnEnterSurfaceFeedbacks?.Invoke();
				_characterMovement.AbilityInProgressSfx = Surfaces[CurrentSurfaceIndex].WalkSound;
				_characterMovement.StopAbilityUsedSfx();
				_characterRun.AbilityInProgressSfx = Surfaces[CurrentSurfaceIndex].RunSound;
				_characterRun.StopAbilityUsedSfx();
			}
			_surfaceIndexLastFrame = CurrentSurfaceIndex;
		}

		/// <summary>
		/// Casts rays to detect surfaces that may match our surface list's layers and tags
		/// </summary>
		protected virtual void DetectSurfaces3D()
		{
			Physics.Raycast(this.transform.position, RaycastDirection, out _raycastDownHit, RaycastLength, _raycastLayerMask);
			if (_raycastDownHit.collider == null)
			{
				return;
			}
			foreach (CharacterSurfaceSoundsItems item in Surfaces)
			{
				if (item.Layers.MMContains(_raycastDownHit.collider.gameObject.layer) && TagsMatch(item.UseTag, item.Tag, _raycastDownHit.collider.gameObject.tag))
				{
					CurrentSurfaceIndex = Surfaces.IndexOf(item);
					return;
				}
			}
		}

		/// <summary>
		/// Tests a point under the character to try and find a surface, then compares it to the list of surfaces to find a match
		/// </summary>
		protected virtual void DetectSurfaces2D()
		{
			_testSurface2D = Physics2D.OverlapPoint((Vector2)_controller2D.ColliderBounds.center, _raycastLayerMask);
			if (_testSurface2D == null)
			{
				return;
			}
			foreach (CharacterSurfaceSoundsItems item in Surfaces)
			{
				if (item.Layers.MMContains(_testSurface2D.gameObject.layer) && TagsMatch(item.UseTag, item.Tag, _testSurface2D.gameObject.tag))
				{
					CurrentSurfaceIndex = Surfaces.IndexOf(item);
					return;
				}
			}
		}

		/// <summary>
		/// Returns true if the tags match or if we're not using tags
		/// </summary>
		/// <param name="useTag"></param>
		/// <param name="contactTag"></param>
		/// <param name="surfaceTag"></param>
		/// <returns></returns>
		protected virtual bool TagsMatch(bool useTag, string contactTag, string surfaceTag)
		{
			if (!useTag)
			{
				return true;
			}
			return contactTag == surfaceTag;
		}

		/// <summary>
		/// Checks if a surface detection is needed and performs it
		/// </summary>
		protected virtual void DetectSurface()
		{
			if (SurfaceDetectionMode == SurfaceDetectionModes.Script)
			{
				return;
			}
			
			if (Time.time - _timeSinceLastCheck < RaycastFrequency)
			{
				return;
			}
			_timeSinceLastCheck = Time.time;

			if (DimensionMode == DimensionModes.ThreeD)
			{
				DetectSurfaces3D();
			}
			else
			{
				DetectSurfaces2D();
			}
		}
	}
}