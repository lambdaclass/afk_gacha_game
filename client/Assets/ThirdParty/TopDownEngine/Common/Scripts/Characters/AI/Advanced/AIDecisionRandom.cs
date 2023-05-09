using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will roll a dice and return true if the result is below or equal the Odds value
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionRandom")]
	public class AIDecisionRandom : AIDecision
	{
		[Header("Random")]
		/// the total number to consider (in "5 out of 10", this would be 10)
		[Tooltip("the total number to consider (in '5 out of 10', this would be 10)")]
		public int TotalChance = 10;
		/// when rolling our dice, if the result is below the Odds, this decision will be true. In "5 out of 10", this would be 5.
		[Tooltip("when rolling our dice, if the result is below the Odds, this decision will be true. In '5 out of 10', this would be 5.")]
		public int Odds = 4;

		protected Character _targetCharacter;

		/// <summary>
		/// On Decide we check if the odds are in our favour
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return EvaluateOdds();
		}

		/// <summary>
		/// Returns true if the Brain's Target is facing us (this will require that the Target has a Character component)
		/// </summary>
		/// <returns></returns>
		protected virtual bool EvaluateOdds()
		{
			int dice = MMMaths.RollADice(TotalChance);
			bool result = (dice <= Odds);
			return result;
		}
	}
}