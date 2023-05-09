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
	/// - if all players (but one) are dead, the game stops and whoever got the most coins wins
	/// - at the end of the game, a Winner screen is displayed and tapping Jump anywhere restarts the game
	/// </summary>
	public class ExplodudesMultiplayerLevelManager : MultiplayerLevelManager
	{
		[Header("Explodudes Settings")]
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

			foreach (Character character in LevelManager.Instance.Players)
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
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.GameOver, null);
		}

		/// <summary>
		/// On update, we update our countdowns and check for input if we're in game over state
		/// </summary>
		public virtual void Update()
		{
			CheckForGameOver();
		}

		/// <summary>
		/// If we're in game over state, checks for input and restarts the game if needed
		/// </summary>
		protected virtual void CheckForGameOver()
		{
			if (_gameOver)
			{
				if ((Input.GetButton("Player1_Jump"))
				    || (Input.GetButton("Player2_Jump"))
				    || (Input.GetButton("Player3_Jump"))
				    || (Input.GetButton("Player4_Jump")))
				{
					MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, 1f, 0f, false, 0f, true);
					MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name);
				}
			}
		}
        
		/// <summary>
		/// Starts listening for pickable item events
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		/// <summary>
		/// Stops listening for pickable item events
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
		}
	}
}