using UnityEngine;
using System.Collections;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a boxcollider to signify the bounds of your level
	/// </summary>
	[AddComponentMenu("TopDown Engine/Camera/LevelLimits")]
	public class LevelLimits : TopDownMonoBehaviour
	{
		/// left x coordinate
		[Tooltip("Left x coordinate")]
		public float LeftLimit;
		/// right x coordinate
		[Tooltip("Right x coordinate")]
		public float RightLimit;
		/// bottom y coordinate 
		[Tooltip("Bottom y coordinate ")]
		public float BottomLimit;
		/// top y coordinate
		[Tooltip("Top y coordinate")]
		public float TopLimit;

		protected BoxCollider2D _collider;

		/// <summary>
		/// On awake, fills the public variables with the level's limits
		/// </summary>
		protected virtual void Awake()
		{
			_collider = GetComponent<BoxCollider2D>();

			LeftLimit = _collider.bounds.min.x;
			RightLimit = _collider.bounds.max.x;
			BottomLimit = _collider.bounds.min.y;
			TopLimit = _collider.bounds.max.y;
		}
	}
}