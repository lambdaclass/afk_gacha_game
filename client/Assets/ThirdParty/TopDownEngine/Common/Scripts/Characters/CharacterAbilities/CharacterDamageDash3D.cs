using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Damage Dash 3D")]
	public class CharacterDamageDash3D : CharacterDash3D
	{
		[Header("Damage Dash")]
		/// the DamageOnTouch object to activate when dashing (usually placed under the Character's model, will require a Collider2D of some form, set to trigger
		[Tooltip("the DamageOnTouch object to activate when dashing (usually placed under the Character's model, will require a Collider2D of some form, set to trigger")]
		public DamageOnTouch TargetDamageOnTouch;
        
		protected const string _damageDashingAnimationParameterName = "DamageDashing";
		protected int _damageDashingAnimationParameter;
        
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
        
		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			base.InitializeAnimatorParameters();
			RegisterAnimatorParameter(_damageDashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _damageDashingAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			base.UpdateAnimator();
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _damageDashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters, _character.RunAnimatorSanityChecks);
			_dashStartedThisFrame = false;
		}
	}
}