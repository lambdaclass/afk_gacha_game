using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to an object with a trigger box collider 2D, and it'll become a pickable object, able to permit or forbid an ability on a Character
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/Pickable Ability")]
	public class PickableAbility : PickableItem
	{
		public enum Methods
		{
			Permit,
			Forbid
		}

		[Header("Pickable Ability")] 
		/// whether this object should permit or forbid an ability when picked
		[Tooltip("whether this object should permit or forbid an ability when picked")]
		public Methods Method = Methods.Permit;
		/// whether or not only characters of Player type should be able to pick this 
		[Tooltip("whether or not only characters of Player type should be able to pick this")]
		public bool OnlyPickableByPlayerCharacters = true;

		[HideInInspector] public string AbilityTypeAsString;

		/// <summary>
		/// Checks if the object is pickable 
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		protected override bool CheckIfPickable()
		{
			_character = _collidingObject.GetComponent<Character>();

			// if what's colliding with the coin ain't a characterBehavior, we do nothing and exit
			if (_character == null)
			{
				return false;
			}

			if (OnlyPickableByPlayerCharacters && (_character.CharacterType != Character.CharacterTypes.Player))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// on pick, we permit or forbid our target ability
		/// </summary>
		protected override void Pick(GameObject picker)
		{
			if (_character == null)
			{
				return;
			}
			bool newState = (Method == Methods.Permit);
			CharacterAbility ability = _character.FindAbilityByString(AbilityTypeAsString);
			if (ability != null)
			{
				ability.PermitAbility(newState);
			}
		}
	}
}