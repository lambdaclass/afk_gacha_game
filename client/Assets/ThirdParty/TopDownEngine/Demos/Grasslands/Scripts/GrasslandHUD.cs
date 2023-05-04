using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
	public class GrasslandHUD : TopDownMonoBehaviour, MMEventListener<TopDownEngineEvent>
	{
		/// The playerID associated to this HUD
		[Tooltip("The playerID associated to this HUD")]
		public string PlayerID = "Player1";
		/// the progress bar to use to show the healthbar
		[Tooltip("the progress bar to use to show the healthbar")]
		public MMProgressBar HealthBar;
		/// the Text comp to use to display the player name
		[Tooltip("the Text comp to use to display the player name")]
		public Text PlayerName;
		/// the radial progress bar to put around the avatar
		[Tooltip("the radial progress bar to put around the avatar")]
		public MMRadialProgressBar AvatarBar;
		/// the counter used to display coin amounts
		[Tooltip("the counter used to display coin amounts")]
		public Text CoinCounter;
		/// the mask to use when the target player dies
		[Tooltip("the mask to use when the target player dies")]
		public CanvasGroup DeadMask;
		/// the screen to display if the target player wins
		[Tooltip("the screen to display if the target player wins")]
		public CanvasGroup WinnerScreen;

		protected virtual void Start()
		{
			CoinCounter.text = "0";
			DeadMask.gameObject.SetActive(false);
			WinnerScreen.gameObject.SetActive(false);
		}

		public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
		{
			switch (tdEvent.EventType)
			{
				case TopDownEngineEventTypes.PlayerDeath:
					if (tdEvent.OriginCharacter.PlayerID == PlayerID)
					{
						DeadMask.gameObject.SetActive(true);
						DeadMask.alpha = 0f;
						StartCoroutine(MMFade.FadeCanvasGroup(DeadMask, 0.5f, 0.8f, true));
					}
					break;
				case TopDownEngineEventTypes.Repaint:
					foreach (GrasslandsMultiplayerLevelManager.GrasslandPoints points in (LevelManager.Instance as GrasslandsMultiplayerLevelManager).Points)
					{
						if (points.PlayerID == PlayerID)
						{
							CoinCounter.text = points.Points.ToString();
						}
					}
					break;
				case TopDownEngineEventTypes.GameOver:
					if (PlayerID == (LevelManager.Instance as GrasslandsMultiplayerLevelManager).WinnerID)
					{
						WinnerScreen.gameObject.SetActive(true);
						WinnerScreen.alpha = 0f;
						StartCoroutine(MMFade.FadeCanvasGroup(WinnerScreen, 0.5f, 0.8f, true));
					}
					break;
			}
            
		}

		/// <summary>
		/// OnDisable, we start listening to events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<TopDownEngineEvent>();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<TopDownEngineEvent>();
		}
	}
}