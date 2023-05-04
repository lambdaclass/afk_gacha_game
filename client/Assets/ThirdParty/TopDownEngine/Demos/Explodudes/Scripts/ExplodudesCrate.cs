using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class that modifies the y scale of crates in the Explodudes demo scene
	/// </summary>
	public class ExplodudesCrate : TopDownMonoBehaviour
	{
		protected const float MinHeight = 0.8f;
		protected const float MaxHeight = 1.1f;
		protected Vector3 _newScale = Vector3.one;

		/// <summary>
		/// On Start we randomize our y scale for aesthetic considerations only
		/// </summary>
		protected virtual void Start()
		{
			_newScale.y = Random.Range(MinHeight, MaxHeight);
			this.transform.localScale = _newScale;
		}
	}
}