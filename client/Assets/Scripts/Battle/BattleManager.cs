using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    GameObject victorySplash;

    [SerializeField]
    GameObject defeatSplash;

    [SerializeField]
    BattleUnit[] playerUnitsUI;

    [SerializeField]
    BattleUnit[] opponentUnitsUI;

	// this has to be changed when the real battle is played out and not the mock, since the units in the backend message will match with the ones already in the client
	Dictionary<string, BattleUnit> units = new Dictionary<string, BattleUnit>();

    void Start()
	{
		victorySplash.SetActive(false);
		defeatSplash.SetActive(false);
		List<Unit> userUnits = GlobalUserData.Instance.Units;
		List<Unit> opponentUnits = LevelProgress.selectedLevelData.units;

		SetUpUnits(userUnits, opponentUnits);
		StartCoroutine(BattleCoroutine());
	}

	private IEnumerator BattleCoroutine()
	{
		yield return StartCoroutine(Battle());
	}

	private IEnumerator Battle()
	{
		yield return StartCoroutine(FakeBattleCoroutine());
	}

	private IEnumerator FakeBattleCoroutine()
	{
        Protobuf.Messages.BattleReplay battleReplay = null;

		yield return StartCoroutine(SocketConnection.Instance.FakeBattle(GlobalUserData.Instance.User.id, LevelProgress.selectedLevelData.id, (replay) => {
			battleReplay = replay;
		}));

		yield return new WaitUntil(() => battleReplay != null);

		#region Set Up Initial State
			int playerUnitsIndex = 0;
			int opponentUnitsIndex = 0;
			foreach (var unit in battleReplay.InitialState.Units)
			// for(int i = 0; i < battleReplay.InitialState.Units.Count; i++)
			{
				// var unit = battleReplay.InitialState.Units[i];
				Debug.Log($"{unit.Id}, {unit.Health}, team: {unit.Team}");

				BattleUnit battleUnit;
				if (unit.Team == 1)
				{
					// battleUnit = playerUnitsUI.First(inGameUnit => inGameUnit.SelectedUnit.id == unit.Id);
					battleUnit = playerUnitsUI[playerUnitsIndex];
					playerUnitsIndex++;
				}
				else
				{
					// battleUnit = opponentUnitsUI.First(inGameUnit => inGameUnit.SelectedUnit.id == unit.Id);
					battleUnit = opponentUnitsUI[opponentUnitsIndex];
					opponentUnitsIndex++;
				}
				units.Add(unit.Id, battleUnit);
				battleUnit.gameObject.SetActive(true);
				battleUnit.MaxHealth = unit.Health;
				battleUnit.CurrentHealth = unit.Health;
			}
		#endregion

		#region Play Out Battle
			foreach (var step in battleReplay.Steps)
			{
				Debug.Log($"Step: {step.StepNumber}");
				yield return new WaitForSeconds(.3f);

				foreach(var action in step.Actions) {
					switch (action.ActionTypeCase) {
						case Protobuf.Messages.Action.ActionTypeOneofCase.SkillAction:
							switch (action.SkillAction.SkillActionType) {
								case Protobuf.Messages.SkillActionType.AnimationStart:
									Debug.Log($"{action.SkillAction.CasterId} started animation to cast {action.SkillAction.SkillId}");
									break;
								case Protobuf.Messages.SkillActionType.EffectTrigger:
									Debug.Log($"{action.SkillAction.CasterId} casted {action.SkillAction.SkillId} targeting {string.Join(", ", action.SkillAction.TargetIds)}");
									break;
								case Protobuf.Messages.SkillActionType.EffectHit:
									Debug.Log($"{action.SkillAction.SkillId} hit {string.Join(", ", action.SkillAction.TargetIds)}");
									BattleUnit[] targetUnits = units.Where(unit => action.SkillAction.TargetIds.Contains(unit.Key)).Select(unit => unit.Value).ToArray();
									foreach(BattleUnit targetUnit in targetUnits) {
										foreach(var statAffected in action.SkillAction.StatsAffected) {
											switch(statAffected.Stat) {
												case Protobuf.Messages.Stat.Health:
													targetUnit.CurrentHealth = targetUnit.CurrentHealth + (int)(statAffected.Amount);
													Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {targetUnit.SelectedUnit.id} dealing {statAffected.Amount} damage to it's health");
													break;
												case Protobuf.Messages.Stat.Energy:
													Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {targetUnit.SelectedUnit.id} dealing {statAffected.Amount} damage to it's energy");
													break;
												case Protobuf.Messages.Stat.Damage:
													Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {targetUnit.SelectedUnit.id} dealing {statAffected.Amount} damage to it's damage");
													break;
												case Protobuf.Messages.Stat.Defense:
													Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {targetUnit.SelectedUnit.id} dealing {statAffected.Amount} damage it it's defense");
													break;
												default:
													Debug.Log(statAffected.Stat);
													break;
											}
										}
									}
									break;
								case Protobuf.Messages.SkillActionType.EffectMiss:
									Debug.Log($"{action.SkillAction.SkillId} missed {string.Join(", ", action.SkillAction.TargetIds)}");
									break;
							}
							break;
						default:
							break;
					}
				}
			}
		#endregion

		#region End Battle
			yield return new WaitForSeconds(2f);
			HandleBattleResult(battleReplay.Result == "team_1");
		#endregion
	}

    private void HandleBattleResult(bool result)
    {
        if(result) {
			// Should this be here? refactor after demo?
			try {
				SocketConnection.Instance.GetUserAndContinue();
			} catch (Exception ex) {
				Debug.LogError(ex.Message);
			}
            GlobalUserData user = GlobalUserData.Instance;
            user.AddCurrency(LevelProgress.selectedLevelData.rewards);
            user.AddExperience(LevelProgress.selectedLevelData.experienceReward);
            user.AccumulateAFKRewards();
            user.User.afkMaxCurrencyReward = LevelProgress.selectedLevelData.afkCurrencyRate;
            user.User.afkMaxExperienceReward = LevelProgress.selectedLevelData.afkExperienceRate;
            victorySplash.GetComponentInChildren<RewardsUIContainer>().Populate(CreateRewardsList());
            victorySplash.SetActive(true);
            victorySplash.GetComponent<AudioSource>().Play();

            GameObject victoryText = victorySplash.transform.Find("CenterContainer").transform.Find("Sign").transform.Find("Text").gameObject;

            // Always shows the same texts when a win is achieved, but they should change based on if it was the last level of the campaign and if there are other campaigns after that
            victoryText.GetComponent<Text>().text = "Victory!";

			SetUpNextButton();
			
			// This should be handled differently
			CampaignManager.selectedCampaignData.levels.Find(level => level.id == LevelProgress.selectedLevelData.id).status = LevelProgress.Status.Completed;
			if(CampaignManager.selectedCampaignData.levels.Any(level => level.id == LevelProgress.nextLevelData.id)) {
				CampaignManager.selectedCampaignData.levels.Find(level => level.id == LevelProgress.nextLevelData.id).status = LevelProgress.Status.Unlocked;
			}
        } else {
            defeatSplash.SetActive(true);
            defeatSplash.GetComponent<AudioSource>().Play();
        }
    }

	private void SetUpNextButton()
	{
		GameObject nextButton = victorySplash.transform.Find("Next").gameObject;
		if(LevelProgress.selectedLevelData.campaignId != LevelProgress.nextLevelData.campaignId) {
			nextButton.GetComponentInChildren<TMP_Text>().text = "NEXT CAMPAIGN";
		} else {
			nextButton.GetComponentInChildren<TMP_Text>().text = "NEXT LEVEL";
			nextButton.GetComponent<Button>().onClick.AddListener(() => {
				LevelProgress.selectedLevelData = LevelProgress.nextLevelData;
				LevelProgress.nextLevelData = LevelProgress.NextLevel(LevelProgress.nextLevelData);
				gameObject.GetComponent<LevelManager>().ChangeToScene("Lineup");
			});
		}
	}

    private void SetUpUnits(List<Unit> userUnits, List<Unit> opponentUnits)
    {
        SetUpUserUnits(userUnits, true);
        SetUpUserUnits(opponentUnits, false);
    }

    private void SetUpUserUnits(List<Unit> units, bool isPlayer)
    {
        BattleUnit[] unitPositions = isPlayer ? playerUnitsUI : opponentUnitsUI;
		// The -1 are since the indexes of the slots in the database go from 1 to 6, and the indexes of the unit position game objects range from 0 to 5
        foreach(Unit unit in units.Where(unit => unit.selected)) {
            BattleUnit unitPosition = unitPositions[unit.slot.Value - 1];
            unitPosition.SetUnit(unit, isPlayer);
        }
    }

    private List<UIReward> CreateRewardsList() {
        List<UIReward> rewards = new List<UIReward>();

        if (LevelProgress.selectedLevelData.experienceReward != 0) { rewards.Add(new ExperienceUIReward(LevelProgress.selectedLevelData.experienceReward)); }

        foreach (var currencyReward in LevelProgress.selectedLevelData.rewards) {
            rewards.Add(new CurrencyUIReward(currencyReward.Key, currencyReward.Value));
        }

        return rewards;
    }
}
