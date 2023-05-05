using System;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Use this picker to create a new resistance on the character that picks it, or to enable/disable an existing one
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/Pickable Damage Resistance")]
	public class PickableDamageResistance : PickableItem
	{
		public enum Modes { Create, ActivateByLabel, DisableByLabel }
		
		[Header("Damage Resistance")]
		/// The chosen mode to interact with resistance, either creating one, activating one or disabling one
		[Tooltip("The chosen mode to interact with resistance, either creating one, activating one or disabling one")]
		public Modes Mode = Modes.ActivateByLabel;
		
		/// If activating or disabling by label, the exact label of the target resistance
		[Tooltip("If activating or disabling by label, the exact label of the target resistance")]
		[MMEnumCondition("Mode", (int)Modes.ActivateByLabel, (int)Modes.DisableByLabel)]
		public string TargetLabel = "SomeResistance";
		
		/// in create mode, the name of the new game object to create to host the new resistance
		[Tooltip("in create mode, the name of the new game object to create to host the new resistance")]
		[MMEnumCondition("Mode", (int)Modes.Create)]
		public string NewResistanceNodeName = "NewResistance";
		/// in create mode, a DamageResistance to copy and give to the new node. Usually you'll want to create a new DamageResistance component on your picker, and drag it in this slot
		[Tooltip("in create mode, a DamageResistance to copy and give to the new node. Usually you'll want to create a new DamageResistance component on your picker, and drag it in this slot")]
		[MMEnumCondition("Mode", (int)Modes.Create)]
		public DamageResistance DamageResistanceToGive;
		
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
			if (characterHealth == null)
			{
				return;
			}          
			DamageResistanceProcessor processor = characterHealth.TargetDamageResistanceProcessor;
			if (processor == null)
			{
				return;
			}

			switch (Mode)
			{
				case Modes.ActivateByLabel:
					processor.SetResistanceByLabel(TargetLabel, true);
					break;
				case Modes.DisableByLabel:
					processor.SetResistanceByLabel(TargetLabel, false);
					break;
				case Modes.Create:
					if (DamageResistanceToGive == null) { return; }
					GameObject newResistance = new GameObject();
					newResistance.transform.SetParent(processor.transform);
					newResistance.name = NewResistanceNodeName;
					DamageResistance newResistanceComponent = MMHelpers.CopyComponent<DamageResistance>(DamageResistanceToGive, newResistance);
					processor.DamageResistanceList.Add(newResistanceComponent);
					break;
			}
		}
	}
}