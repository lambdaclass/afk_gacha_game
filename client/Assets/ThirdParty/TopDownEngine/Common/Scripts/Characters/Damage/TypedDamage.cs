using System;
using MoreMountains.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used to store and define typed damage impact : damage caused, condition or movement speed changes, etc
	/// </summary>
	[Serializable]
	public class TypedDamage 
	{
		/// the type of damage associated to this definition
		[Tooltip("the type of damage associated to this definition")]
		public DamageType AssociatedDamageType;
		/// The min amount of health to remove from the player's health
		[Tooltip("The min amount of health to remove from the player's health")]
		public float MinDamageCaused = 10f;
		/// The max amount of health to remove from the player's health
		[Tooltip("The max amount of health to remove from the player's health")]
		public float MaxDamageCaused = 10f;
		
		/// whether or not this damage, when applied, should force the character into a specified condition
		[Tooltip("whether or not this damage, when applied, should force the character into a specified condition")] 
		public bool ForceCharacterCondition = false;
		/// when in forced character condition mode, the condition to which to swap
		[Tooltip("when in forced character condition mode, the condition to which to swap")]
		[MMCondition("ForceCharacterCondition", true)]
		public CharacterStates.CharacterConditions ForcedCondition;
		/// when in forced character condition mode, whether or not to disable gravity
		[Tooltip("when in forced character condition mode, whether or not to disable gravity")]
		[MMCondition("ForceCharacterCondition", true)]
		public bool DisableGravity = false;
		/// when in forced character condition mode, whether or not to reset controller forces
		[Tooltip("when in forced character condition mode, whether or not to reset controller forces")]
		[MMCondition("ForceCharacterCondition", true)]
		public bool ResetControllerForces = false;
		/// when in forced character condition mode, the duration of the effect, after which condition will be reverted 
		[Tooltip("when in forced character condition mode, the duration of the effect, after which condition will be reverted")]
		[MMCondition("ForceCharacterCondition", true)]
		public float ForcedConditionDuration = 3f;
		
		/// whether or not to apply a movement multiplier to the damaged character
		[Tooltip("whether or not to apply a movement multiplier to the damaged character")] 
		public bool ApplyMovementMultiplier = false;
		/// the movement multiplier to apply when ApplyMovementMultiplier is true 
		[Tooltip("the movement multiplier to apply when ApplyMovementMultiplier is true")]
		[MMCondition("ApplyMovementMultiplier", true)]
		public float MovementMultiplier = 0.5f;
		/// the duration of the movement multiplier, if ApplyMovementMultiplier is true
		[Tooltip("the duration of the movement multiplier, if ApplyMovementMultiplier is true")]
		[MMCondition("ApplyMovementMultiplier", true)]
		public float MovementMultiplierDuration = 2f;
		
		

		protected int _lastRandomFrame = -1000;
		protected float _lastRandomValue = 0f;

		public virtual float DamageCaused
		{
			get
			{
				if (Time.frameCount != _lastRandomFrame)
				{
					_lastRandomValue = Random.Range(MinDamageCaused, MaxDamageCaused);
					_lastRandomFrame = Time.frameCount;
				}
				return _lastRandomValue;
			}
		} 
	}	
}

