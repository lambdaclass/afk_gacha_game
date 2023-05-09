using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An AIACtion used to have an AI stop running
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionRunStop")]
	public class AIActionRunStop : AIAction
	{
		/// if this is true, this action will only run once in a state (this flag will reset on state exit) 
		public bool OnlyRunOnce = true;
        
		protected Character _character;
		protected CharacterRun _characterRun;
		protected bool _alreadyRan = false;
        
		/// <summary>
		/// On init we grab our Run ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_character = this.gameObject.GetComponentInParent<Character>();
			_characterRun = _character?.FindAbility<CharacterRun>();
		}

		/// <summary>
		/// Requests running to stop
		/// </summary>
		public override void PerformAction()
		{
			if (OnlyRunOnce && _alreadyRan)
			{
				return;
			}
			_characterRun.RunStop();
			_alreadyRan = true;
		}

		/// <summary>
		/// On enter state we reset our flag
		/// </summary>
		public override void OnEnterState()
		{
			base.OnEnterState();
			_alreadyRan = false;
		}
	}
}