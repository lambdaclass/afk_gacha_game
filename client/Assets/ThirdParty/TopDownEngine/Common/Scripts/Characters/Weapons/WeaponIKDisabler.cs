using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// You can add this class next to a WeaponIK component (so usually on a Character's Animator), and it'll let you
	/// disable IK and optionally reparent the WeaponAttachment during certain animations 
	/// </summary>
	public class WeaponIKDisabler : TopDownMonoBehaviour
	{
		[Header("Animation Parameter Names")] 
		/// a list of animation parameter names which, if true, should cause IK to be disabled 
		public List<string> AnimationParametersPreventingIK;
        
		[Header("Attachments")]
		/// the WeaponAttachment transform to reparent
		public Transform WeaponAttachment;
		/// the transform the WeaponAttachment will be reparented to when certain animation parameters are true
		public Transform WeaponAttachmentParentNoIK;

		protected Transform _initialParent;
		protected Vector3 _initialLocalPosition;
		protected Vector3 _initialLocalScale;
		protected Quaternion _initialRotation;
		protected WeaponIK _weaponIK;
		protected Animator _animator;
		protected List<int> _animationParametersHashes;
		protected bool _shouldSetIKLast = true;

		/// <summary>
		/// On Start we initialize our component
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// Grabs animator, weaponIK, hashes the animation parameter names, and stores initial positions
		/// </summary>
		protected virtual void Initialization()
		{
			_weaponIK = this.gameObject.GetComponent<WeaponIK>();
			_animator = this.gameObject.GetComponent<Animator>();
			_animationParametersHashes = new List<int>();
            
			foreach (string _animationParameterName in AnimationParametersPreventingIK)
			{
				int newHash = Animator.StringToHash(_animationParameterName);
				_animationParametersHashes.Add(newHash);
			}

			if (WeaponAttachment != null)
			{
				_initialParent = WeaponAttachment.parent;
				_initialLocalPosition = WeaponAttachment.transform.localPosition;
				_initialLocalScale = WeaponAttachment.transform.localScale;
				_initialRotation = WeaponAttachment.transform.localRotation;
			}
		}
        
		/// <summary>
		/// On animator IK, we turn IK on or off if needed
		/// </summary>
		/// <param name="layerIndex"></param>
		protected virtual void OnAnimatorIK(int layerIndex)
		{
			if ((_animator == null) || (_weaponIK == null) || (WeaponAttachment == null))
			{
				return;
			}

			if (_animationParametersHashes.Count <= 0)
			{
				return;
			}

			bool shouldPreventIK = false;
			foreach (int hash in _animationParametersHashes)
			{
				if (_animator.GetBool(hash))
				{
					shouldPreventIK = true;
				}
			}

			if (shouldPreventIK != _shouldSetIKLast)
			{
				PreventIK(shouldPreventIK);
			}

			_shouldSetIKLast = shouldPreventIK;
		}

		/// <summary>
		/// Enables or disables IK
		/// </summary>
		/// <param name="status"></param>
		protected virtual void PreventIK(bool status)
		{
			if (status)
			{
				_weaponIK.AttachLeftHand = false;
				_weaponIK.AttachRightHand = false;
				WeaponAttachment.transform.SetParent(WeaponAttachmentParentNoIK);
			}
			else
			{
				_weaponIK.AttachLeftHand = true;
				_weaponIK.AttachRightHand = true;
				WeaponAttachment.transform.SetParent(_initialParent);
                
				WeaponAttachment.transform.localPosition = _initialLocalPosition;
				WeaponAttachment.transform.localScale = _initialLocalScale;
				WeaponAttachment.transform.localRotation = _initialRotation;
			}
		}
	}
}