using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A Stimpack / health bonus, that gives health back when picked
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/Stimpack")]
	public class Stimpack : PickableItem
	{
		[Header("Stimpack")]
		/// The amount of points to add when collected
		[Tooltip("The amount of points to add when collected")]
		public float HealthToGive = 10f;
		/// if this is true, only player characters can pick this up
		[Tooltip("if this is true, only player characters can pick this up")]
		public bool OnlyForPlayerCharacter = true;

		/// <summary>
		/// Triggered when something collides with the stimpack
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker)
		{
			Character character = picker.gameObject.MMGetComponentNoAlloc<Character>();
			if (OnlyForPlayerCharacter && (character != null) && (_character.CharacterType != Character.CharacterTypes.Player))
			{
				return;
			}

			Health characterHealth = picker.gameObject.MMGetComponentNoAlloc<Health>();
			// else, we give health to the player
			if (characterHealth != null)
			{
				characterHealth.ReceiveHealth(HealthToGive, gameObject);
			}            
		}
	}
}