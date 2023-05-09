using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class describes how the Top Down Engine demo achievements are triggered.
	/// It extends the base class MMAchievementRules
	/// It listens for different event types
	/// </summary>
	public class AchievementRules : MMAchievementRules, 
		MMEventListener<MMGameEvent>, 
		MMEventListener<MMCharacterEvent>, 
		MMEventListener<TopDownEngineEvent>,
		MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>,
		MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
		MMEventListener<PickableItemEvent>,
		MMEventListener<CheckPointEvent>,
		MMEventListener<MMInventoryEvent>
	{
		/// <summary>
		/// When we catch an MMGameEvent, we do stuff based on its name
		/// </summary>
		/// <param name="gameEvent">Game event.</param>
		public override void OnMMEvent(MMGameEvent gameEvent)
		{
			base.OnMMEvent (gameEvent);
		}

		/// <summary>
		/// When a character event happens, and if it's a Jump event, we add progress to our JumpAround achievement
		/// </summary>
		/// <param name="characterEvent"></param>
		public virtual void OnMMEvent(MMCharacterEvent characterEvent)
		{
			if (characterEvent.TargetCharacter.CharacterType == Character.CharacterTypes.Player)
			{
				switch (characterEvent.EventType)
				{
					case MMCharacterEventTypes.Jump:
						MMAchievementManager.AddProgress ("JumpAround", 1);
						break;
				}	
			}
		}

		/// <summary>
		/// When we grab a TopDownEngineEvent, and if it's a PlayerDeath event, we unlock our achievement
		/// </summary>
		/// <param name="topDownEngineEvent"></param>
		public virtual void OnMMEvent(TopDownEngineEvent topDownEngineEvent)
		{
			switch (topDownEngineEvent.EventType)
			{
				case TopDownEngineEventTypes.PlayerDeath:
					MMAchievementManager.UnlockAchievement ("DeathIsOnlyTheBeginning");
					break;
			}
		}

		/// <summary>
		/// Grabs PickableItem events
		/// </summary>
		/// <param name="pickableItemEvent"></param>
		public virtual void OnMMEvent(PickableItemEvent pickableItemEvent)
		{
			/*if (pickableItemEvent.PickedItem.GetComponent<InventoryEngineHealth>() != null)
			{
				MMAchievementManager.UnlockAchievement ("Medic");
			}*/
		}

		/// <summary>
		/// Grabs MMStateChangeEvents
		/// </summary>
		/// <param name="movementEvent"></param>
		public virtual void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> movementEvent)
		{
			/*switch (movementEvent.NewState)
			{

			}*/
		}

		/// <summary>
		/// Grabs MMStateChangeEvents
		/// </summary>
		/// <param name="conditionEvent"></param>
		public virtual void OnMMEvent(MMStateChangeEvent<CharacterStates.CharacterConditions> conditionEvent)
		{
			/*switch (conditionEvent.NewState)
			{

			}*/
		}

		/// <summary>
		/// Grabs checkpoints events. If the checkpoint's order is > 0, we unlock our achievement
		/// </summary>
		/// <param name="checkPointEvent"></param>
		public virtual void OnMMEvent(CheckPointEvent checkPointEvent)
		{
			if (checkPointEvent.Order > 0)
			{
				MMAchievementManager.UnlockAchievement("SteppingStone");
			}
		}

		/// <summary>
		/// On Inventory events, we unlock or add progress to achievements if needed
		/// </summary>
		/// <param name="inventoryEvent"></param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			if (inventoryEvent.InventoryEventType == MMInventoryEventType.Pick)
			{
				if (inventoryEvent.EventItem.ItemID == "KoalaCoin")
				{
					MMAchievementManager.AddProgress("MoneyMoneyMoney", 1);
				}
				if (inventoryEvent.EventItem.ItemID == "KoalaHealth")
				{
					MMAchievementManager.UnlockAchievement("Medic");
				}
			}
		}

		/// <summary>
		/// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable ();
			this.MMEventStartListening<MMCharacterEvent>();
			this.MMEventStartListening<TopDownEngineEvent>();
			this.MMEventStartListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
			this.MMEventStartListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
			this.MMEventStartListening<PickableItemEvent>();
			this.MMEventStartListening<CheckPointEvent>();
			this.MMEventStartListening<MMInventoryEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();
			this.MMEventStopListening<MMCharacterEvent>();
			this.MMEventStopListening<TopDownEngineEvent>();
			this.MMEventStopListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
			this.MMEventStopListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
			this.MMEventStopListening<PickableItemEvent>();
			this.MMEventStopListening<CheckPointEvent>();
			this.MMEventStopListening<MMInventoryEvent>();
		}
	}
}