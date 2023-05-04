using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true if the Brain's current target is facing this character. Yes, it's quite specific to Ghosts. But hey now you can use it too!
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionTargetFacingAI2D")]
	public class AIDecisionTargetFacingAI2D : AIDecision
	{
		protected CharacterOrientation2D _orientation2D;
        
		/// <summary>
		/// On Decide we check whether the Target is facing us
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return EvaluateTargetFacingDirection();
		}

		/// <summary>
		/// Returns true if the Brain's Target is facing us (this will require that the Target has a Character component)
		/// </summary>
		/// <returns></returns>
		protected virtual bool EvaluateTargetFacingDirection()
		{
			if (_brain.Target == null)
			{
				return false;
			}

			_orientation2D = _brain.Target.gameObject.GetComponent<Character>()?.FindAbility<CharacterOrientation2D>();
			if (_orientation2D != null)
			{
				if (_orientation2D.IsFacingRight && (this.transform.position.x > _orientation2D.transform.position.x))
				{
					return true;
				}
				if (!_orientation2D.IsFacingRight && (this.transform.position.x < _orientation2D.transform.position.x))
				{
					return true;
				}
			}            

			return false;
		}
	}
}