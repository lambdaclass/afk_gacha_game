using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this component to a platform and define its new friction or force which will be applied to any TopDownController that walks on it
	/// TODO, still work in progress
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Surface Modifier")]
	public class SurfaceModifier : TopDownMonoBehaviour 
	{
		[Header("Friction")]
		[MMInformation("Set a friction between 0.01 and 0.99 to get a slippery surface (close to 0 is very slippery, close to 1 is less slippery).\nOr set it above 1 to get a sticky surface. The higher the value, the stickier the surface.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// the amount of friction to apply to a TopDownController walking over this surface		
		[Tooltip("the amount of friction to apply to a TopDownController walking over this surface")]
		public float Friction;

		[Header("Force")]
		[MMInformation("Use these to add X or Y (or both) forces to any TopDownController that gets grounded on this surface. Adding a X force will create a treadmill (negative value > treadmill to the left, positive value > treadmill to the right). A positive y value will create a trampoline, a bouncy surface, or a jumper for example.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// the amount of force to add to a TopDownController walking over this surface
		[Tooltip("the amount of force to add to a TopDownController walking over this surface")]
		public Vector3 AddedForce=Vector3.zero;

		/// <summary>
		/// Triggered when a TopDownController collides with the surface
		/// </summary>
		/// <param name="collider">Collider.</param>
		/*public virtual void OnTriggerStay2D(Collider2D collider)
		{
			TODO
		}*/
	}
}