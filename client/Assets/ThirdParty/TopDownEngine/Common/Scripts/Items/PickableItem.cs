using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An event typically fired when picking an item, letting listeners know what item has been picked
	/// </summary>
	public struct PickableItemEvent
	{
		public GameObject Picker;
		public PickableItem PickedItem;

		/// <summary>
		/// Initializes a new instance of the <see cref="MoreMountains.TopDownEngine.PickableItemEvent"/> struct.
		/// </summary>
		/// <param name="pickedItem">Picked item.</param>
		public PickableItemEvent(PickableItem pickedItem, GameObject picker) 
		{
			Picker = picker;
			PickedItem = pickedItem;
		}
		static PickableItemEvent e;
		public static void Trigger(PickableItem pickedItem, GameObject picker)
		{
			e.Picker = picker;
			e.PickedItem = pickedItem;
			MMEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// Coin manager
	/// </summary>
	public class PickableItem : TopDownMonoBehaviour
	{
		[Header("Pickable Item")]
		/// A feedback to play when the object gets picked
		[Tooltip("a feedback to play when the object gets picked")]
		public MMFeedbacks PickedMMFeedbacks;
		/// if this is true, the picker's collider will be disabled on pick
		[Tooltip("if this is true, the picker's collider will be disabled on pick")]
		public bool DisableColliderOnPick = false;
		/// if this is set to true, the object will be disabled when picked
		[Tooltip("if this is set to true, the object will be disabled when picked")]
		public bool DisableObjectOnPick = true;
		/// the duration (in seconds) after which to disable the object, instant if 0
		[MMCondition("DisableObjectOnPick", true)]
		[Tooltip("the duration (in seconds) after which to disable the object, instant if 0")]
		public float DisableDelay = 0f;
		/// if this is set to true, the object will be disabled when picked
		[Tooltip("if this is set to true, the object will be disabled when picked")]
		public bool DisableModelOnPick = false;
		/// if this is set to true, the target object will be disabled when picked
		[Tooltip("if this is set to true, the target object will be disabled when picked")]
		public bool DisableTargetObjectOnPick = false;
		/// the object to disable on pick if DisableTargetObjectOnPick is true 
		[Tooltip("the object to disable on pick if DisableTargetObjectOnPick is true")]
		[MMCondition("DisableTargetObjectOnPick", true)]
		public GameObject TargetObjectToDisable;
		/// the time in seconds before disabling the target if DisableTargetObjectOnPick is true 
		[Tooltip("the time in seconds before disabling the target if DisableTargetObjectOnPick is true")]
		[MMCondition("DisableTargetObjectOnPick", true)]
		public float TargetObjectDisableDelay = 1f;
		/// the visual representation of this picker
		[MMCondition("DisableModelOnPick", true)]
		[Tooltip("the visual representation of this picker")]
		public GameObject Model;

		[Header("Pick Conditions")]
		/// if this is true, this pickable item will only be pickable by objects with a Character component 
		[Tooltip("if this is true, this pickable item will only be pickable by objects with a Character component")]
		public bool RequireCharacterComponent = true;
		/// if this is true, this pickable item will only be pickable by objects with a Character component of type player
		[Tooltip("if this is true, this pickable item will only be pickable by objects with a Character component of type player")]
		public bool RequirePlayerType = true;

		protected Collider _collider;
		protected Collider2D _collider2D;
		protected GameObject _collidingObject;
		protected Character _character = null;
		protected bool _pickable = false;
		protected ItemPicker _itemPicker = null;
		protected WaitForSeconds _disableDelay;

		protected virtual void Start()
		{
			_disableDelay = new WaitForSeconds(DisableDelay);
			_collider = gameObject.GetComponent<Collider>();
			_collider2D = gameObject.GetComponent<Collider2D>();
			_itemPicker = gameObject.GetComponent<ItemPicker> ();
			PickedMMFeedbacks?.Initialization(this.gameObject);
		}

		/// <summary>
		/// Triggered when something collides with the coin
		/// </summary>
		/// <param name="collider">Other.</param>
		public virtual void OnTriggerEnter (Collider collider) 
		{
			_collidingObject = collider.gameObject;
			PickItem (collider.gameObject);
		}

		/// <summary>
		/// Triggered when something collides with the coin
		/// </summary>
		/// <param name="collider">Other.</param>
		public virtual void OnTriggerEnter2D (Collider2D collider) 
		{
			_collidingObject = collider.gameObject;
			PickItem (collider.gameObject);
		}

		/// <summary>
		/// Check if the item is pickable and if yes, proceeds with triggering the effects and disabling the object
		/// </summary>
		public virtual void PickItem(GameObject picker)
		{
			if (CheckIfPickable ())
			{
				Effects ();
				PickableItemEvent.Trigger(this, picker);
				Pick (picker);
				if (DisableColliderOnPick)
				{
					if (_collider != null)
					{
						_collider.enabled = false;
					}
					if (_collider2D != null)
					{
						_collider2D.enabled = false;
					}
				}
				if (DisableModelOnPick && (Model != null))
				{
					Model.gameObject.SetActive(false);
				}
				
				if (DisableObjectOnPick)
				{
					// we desactivate the gameobject
					if (DisableDelay == 0f)
					{
						this.gameObject.SetActive(false);
					}
					else
					{
						StartCoroutine(DisablePickerCoroutine());
					}
				}
				
				if (DisableTargetObjectOnPick && (TargetObjectToDisable != null))
				{
					if (TargetObjectDisableDelay == 0f)
					{
						TargetObjectToDisable.SetActive(false);
					}
					else
					{
						StartCoroutine(DisableTargetObjectCoroutine());
					}
				}			
			} 
		}

		protected virtual IEnumerator DisableTargetObjectCoroutine()
		{
			yield return MMCoroutine.WaitFor(TargetObjectDisableDelay);
			TargetObjectToDisable.SetActive(false);
		}

		protected virtual IEnumerator DisablePickerCoroutine()
		{
			yield return _disableDelay;
			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// Checks if the object is pickable.
		/// </summary>
		/// <returns><c>true</c>, if if pickable was checked, <c>false</c> otherwise.</returns>
		protected virtual bool CheckIfPickable()
		{
			// if what's colliding with the coin ain't a characterBehavior, we do nothing and exit
			_character = _collidingObject.GetComponent<Character>();
			if (RequireCharacterComponent)
			{
				if (_character == null)
				{
					return false;
				}
				
				if (RequirePlayerType && (_character.CharacterType != Character.CharacterTypes.Player))
				{
					return false;
				}
			}
			if (_itemPicker != null)
			{
				if  (!_itemPicker.Pickable())
				{
					return false;	
				}
			}

			return true;
		}

		/// <summary>
		/// Triggers the various pick effects
		/// </summary>
		protected virtual void Effects()
		{
			PickedMMFeedbacks?.PlayFeedbacks();
		}

		/// <summary>
		/// Override this to describe what happens when the object gets picked
		/// </summary>
		protected virtual void Pick(GameObject picker)
		{
			
		}
	}
}