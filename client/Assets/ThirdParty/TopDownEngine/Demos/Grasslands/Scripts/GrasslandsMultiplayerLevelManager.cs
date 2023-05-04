using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// An example class of how you can extend the MultiplayerLevelManager to implement your own specific rules.
	/// This one's rules are as follows :
	/// - player can grab coins, one coin = one point
	/// - after X seconds, the game stops and whoever got the most coins wins
	/// - if all players (but one) are dead, the game stops and whoever got the most coins wins
	/// - at the end of the game, a Winner screen is displayed and tapping Jump anywhere restarts the game
	/// </summary>
	public class GrasslandsMultiplayerLevelManager : MultiplayerLevelManager, MMEventListener<PickableItemEvent>
	{
		/// <summary>
		/// A struct used to store points for each player
		/// </summary>
		public struct GrasslandPoints
		{
			public string PlayerID;
			public int Points;
		}

		[Header("Grasslands Bindings")]
		/// An array to store each player's points
		[Tooltip("an array to store each player's points")]
		public GrasslandPoints[] Points;
		/// the list of countdowns we need to update
		[Tooltip("the list of countdowns we need to update")]
		public List<MMCountdown> Countdowns;

		[Header("Grasslands Settings")]
		/// the duration of the game, in seconds
		[Tooltip("the duration of the game, in seconds")]
		public int GameDuration = 99;
		/// the ID of the winner
		public string WinnerID { get; set; }

		protected string _playerID;
		protected bool _gameOver = false;
        
		/// <summary>
		/// On init, we initialize our points and countdowns
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			WinnerID = "";
			Points = new GrasslandPoints[Players.Count];
			int i = 0;
			foreach(Character player in Players)
			{
				Points[i].PlayerID = player.PlayerID;
				Points[i].Points = 0;
				i++;
			}
			foreach(MMCountdown countdown in Countdowns)
			{
				countdown.CountdownFrom = GameDuration;
				countdown.ResetCountdown();
			}
		}
        
		/// <summary>
		/// Whenever a player dies, we check if we only have one left alive, in which case we trigger our game over routine
		/// </summary>
		/// <param name="playerCharacter"></param>
		protected override void OnPlayerDeath(Character playerCharacter)
		{
			base.OnPlayerDeath(playerCharacter);
			int aliveCharacters = 0;
			int i = 0;
            
			foreach(Character character in LevelManager.Instance.Players)
			{
				if (character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
				{
					WinnerID = character.PlayerID;
					aliveCharacters++;
				}
				i++;
			}

			if (aliveCharacters <= 1)
			{
				StartCoroutine(GameOver());
			}
		}

		/// <summary>
		/// On game over, freezes time and displays the game over screen
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator GameOver()
		{
			yield return new WaitForSeconds(2f);
			if (WinnerID == "")
			{
				WinnerID = "Player1";
			}
			MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 0f, true);
			_gameOver = true;
			MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.FreeAllLooping);
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.GameOver, null);
		}

		/// <summary>
		/// On update, we update our countdowns and check for input if we're in game over state
		/// </summary>
		public virtual void Update()
		{
			UpdateCountdown();
			CheckForGameOver();
		}

		/// <summary>
		/// For each countdown, updates the remaining time
		/// </summary>
		protected virtual void UpdateCountdown()
		{
			if (_gameOver)
			{
				return;
			}

			float remainingTime = GameDuration;
			foreach (MMCountdown countdown in Countdowns)
			{
				if (countdown.gameObject.activeInHierarchy)
				{
					remainingTime = countdown.CurrentTime;
				}
			}
			if (remainingTime <= 0f)
			{
				int maxPoints = 0;
				foreach (GrasslandPoints points in Points)
				{
					if (points.Points > maxPoints)
					{
						WinnerID = points.PlayerID;
						maxPoints = points.Points;
					}
				}
				StartCoroutine(GameOver());
			}
		}

		/// <summary>
		/// If we're in game over state, checks for input and restarts the game if needed
		/// </summary>
		protected virtual void CheckForGameOver()
		{
			if (_gameOver)
			{
				if ( (Input.GetButton("Player1_Jump"))
				     || (Input.GetButton("Player2_Jump"))
				     || (Input.GetButton("Player3_Jump"))
				     || (Input.GetButton("Player4_Jump")) )
				{
					MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, 1f, 0f, false, 0f, true);
					MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name);
				}
			}
		}

		/// <summary>
		/// When a coin gets picked, we increase the amount of points of the character who picked it
		/// </summary>
		/// <param name="pickEvent"></param>
		public virtual void OnMMEvent(PickableItemEvent pickEvent)
		{
			_playerID = pickEvent.Picker.MMGetComponentNoAlloc<Character>()?.PlayerID;
			for (int i = 0; i < Points.Length; i++)
			{
				if (Points[i].PlayerID == _playerID)
				{
					Points[i].Points++;
					TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
				}
			}
		}

		/// <summary>
		/// Starts listening for pickable item events
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			this.MMEventStartListening<PickableItemEvent>();
		}

		/// <summary>
		/// Stops listening for pickable item events
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			this.MMEventStopListening<PickableItemEvent>();
		}
	}
}