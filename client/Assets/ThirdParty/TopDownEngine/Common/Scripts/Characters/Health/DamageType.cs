using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	public enum DamageTypeModes { BaseDamage, TypedDamage }
	/// <summary>
	/// A scriptable object you can create assets from, to identify damage types
	/// </summary>
	[CreateAssetMenu(menuName = "MoreMountains/TopDownEngine/DamageType", fileName = "DamageType")]
	public class DamageType : ScriptableObject
	{
	}    
}