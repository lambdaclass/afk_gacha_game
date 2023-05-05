using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace  MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class is an example of how you can create pickable abilities, items that, when picked, enable an ability
	/// on the picking character. This example will permit the CharacterDash2D ability.
	/// You can of course create more items like this for any other ability.
	///
	/// To give it a try, in the KoalaDungeon demo scene for example, create a new empty game object, add a box collider 2D to it, make sure it's trigger.
	/// then add this component to it. Select the Koala prefab, uncheck "permit ability" on its Dash 2D ability. Now play the scene,
	/// try dashing (you can't), grab this object, and you can now dash.
	/// </summary>
	public class PickableDash2D : PickableItem
	{
		/// <summary>
		/// To make sure this can be picked, we make sure we're dealing with a Player Character
		/// </summary>
		/// <returns></returns>
		protected override bool CheckIfPickable()
		{
			_character = _collidingObject.GetComponent<Character>();

			if (_character == null)
			{
				return false;
			}
			if (_character.CharacterType != Character.CharacterTypes.Player)
			{
				return false;
			}
			return true;
		}
        
		/// <summary>
		/// On pick, we permit the ability, if found
		/// </summary>
		/// <param name="picker"></param>
		protected override void Pick(GameObject picker)
		{
			_character.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterDash2D>()?.PermitAbility(true);
		}
	}
}