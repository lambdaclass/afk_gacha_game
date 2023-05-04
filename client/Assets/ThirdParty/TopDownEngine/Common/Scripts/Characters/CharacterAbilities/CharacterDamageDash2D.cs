using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Damage Dash 2D")]
	public class CharacterDamageDash2D : CharacterDash2D
	{
		[Header("Damage Dash")]
		/// the DamageOnTouch object to activate when dashing (usually placed under the Character's model, will require a Collider2D of some form, set to trigger
		[Tooltip("the DamageOnTouch object to activate when dashing (usually placed under the Character's model, will require a Collider2D of some form, set to trigger")]
		public DamageOnTouch TargetDamageOnTouch;
        
		/// <summary>
		/// On initialization, we disable our damage on touch object
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			TargetDamageOnTouch?.gameObject.SetActive(false);
		}

		/// <summary>
		/// When we start to dash, we activate our damage object
		/// </summary>
		public override void DashStart()
		{
			base.DashStart();
			TargetDamageOnTouch?.gameObject.SetActive(true);
		}

		/// <summary>
		/// When we stop dashing, we disable our damage object
		/// </summary>
		public override void DashStop()
		{
			base.DashStop();
			TargetDamageOnTouch?.gameObject.SetActive(false);
		}
	}
}