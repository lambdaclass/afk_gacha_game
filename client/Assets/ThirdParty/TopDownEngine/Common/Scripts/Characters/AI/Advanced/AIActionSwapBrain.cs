using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This simple action lets you swap the brain of an AI at runtime, for a new brain, specified in the inspector
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AI Action Swap Brain")]
	public class AIActionSwapBrain : AIAction
	{
		/// the brain to replace the Character's one with
		[Tooltip("the brain to replace the Character's one with")]
		public AIBrain NewAIBrain;

		protected Character _character;

		/// <summary>
		/// On init, we grab and store our Character
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_character = this.gameObject.GetComponentInParent<Character>();
		}

		/// <summary>
		/// On PerformAction we swap our brain
		/// </summary>
		public override void PerformAction()
		{
			SwapBrain();
		}

		/// <summary>
		/// Disables the old brain, swaps it with a new one and enables it
		/// </summary>
		protected virtual void SwapBrain()
		{
			if (NewAIBrain == null) return;

			// we disable the "old" brain
			_character.CharacterBrain.gameObject.SetActive(false);
			_character.CharacterBrain.enabled = false;            
			// we swap it with the new one
			_character.CharacterBrain = NewAIBrain;
			// we enable the new one and reset it
			NewAIBrain.gameObject.SetActive(true);
			NewAIBrain.enabled = true;
			NewAIBrain.Owner = _character.gameObject;
			NewAIBrain.ResetBrain();
		}
	}
}