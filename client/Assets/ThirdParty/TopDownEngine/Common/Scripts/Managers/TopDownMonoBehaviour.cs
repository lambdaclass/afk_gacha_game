using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// The TopDownMonoBehaviour class is a base class for all TopDownEngine classes.
	/// It doesn't do anything, but ensures you have a single point of change should you want your classes to inherit from something else than a plain MonoBehaviour
	/// A frequent use case for this would be adapting scripts 
	/// </summary>
	public class TopDownMonoBehaviour : MonoBehaviour { }	
}

