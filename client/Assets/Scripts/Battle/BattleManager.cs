using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
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

	[SerializeField]
	ProjectilesPooler projectilesPooler;

	enum UnitStatus {
		Idle,
		AnimationStart,
		Trigger,
		End
	}

	Dictionary<string, UnitStatus> unitsStatus;

    void Start()
	{
		victorySplash.SetActive(false);
		defeatSplash.SetActive(false);
		List<Unit> userUnits = GlobalUserData.Instance.Units;
		List<Unit> opponentUnits = LevelProgress.selectedLevelData.units;

		SetUpUnits(userUnits, opponentUnits);

		StartCoroutine(Battle());
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

	private IEnumerator Battle()
	{
		Protobuf.Messages.BattleResult battleResult = null;

		yield return StartCoroutine(SocketConnection.Instance.Battle(GlobalUserData.Instance.User.id, LevelProgress.selectedLevelData.id, (replay) =>
		{
			battleResult = replay;
		}));

		yield return new WaitUntil(() => battleResult != null);

		SetUpInitialState(battleResult);
		yield return StartCoroutine(PlayOutSteps(battleResult.Steps));

		yield return new WaitForSeconds(2f);
		HandleBattleResult(battleResult.Result == "team_1");
	}

	private void SetUpInitialState(Protobuf.Messages.BattleResult battleResult)
	{
		foreach (var unit in battleResult.InitialState.Units)
		{
			Debug.Log($"{unit.Id}, {unit.Health}, team: {unit.Team}");

			BattleUnit battleUnit;
			if(unit.Team == 1) {
				battleUnit = playerUnitsUI.First(unitPosition => unitPosition.SelectedUnit.id == unit.Id);
			} else {
				battleUnit = opponentUnitsUI.First(unitPosition => unitPosition.SelectedUnit.id == unit.Id);
			}

			battleUnit.gameObject.SetActive(true);
			battleUnit.MaxHealth = unit.Health;
			battleUnit.CurrentHealth = unit.Health;
		}
	}

	private IEnumerator PlayOutSteps(RepeatedField<Protobuf.Messages.Step> steps)
	{
		foreach (var step in steps)
		{
			Debug.Log($"Step: {step.StepNumber}");
			yield return new WaitForSeconds(.3f);

			foreach (var action in step.Actions.Where(action => action.ActionTypeCase == Protobuf.Messages.Action.ActionTypeOneofCase.SkillAction)
													.OrderBy(action => action.SkillAction.SkillActionType)
													.Concat(step.Actions.Where(action => action.ActionTypeCase == Protobuf.Messages.Action.ActionTypeOneofCase.Death))
													.Concat(step.Actions.Where(action => action.ActionTypeCase != Protobuf.Messages.Action.ActionTypeOneofCase.Death)))
			{
				switch (action.ActionTypeCase)
				{
					case Protobuf.Messages.Action.ActionTypeOneofCase.SkillAction:
						List<BattleUnit> targetUnits = GetSkillTargets(action.SkillAction.TargetIds.ToArray());

						BattleUnit casterUnit;
						Color porjectileColor;
						if (playerUnitsUI.Any(unit => unit.SelectedUnit.id == action.SkillAction.CasterId))
						{
							casterUnit = playerUnitsUI.First(unit => unit.SelectedUnit.id == action.SkillAction.CasterId);
							porjectileColor = Color.green;
						}
						else
						{
							casterUnit = opponentUnitsUI.First(unit => unit.SelectedUnit.id == action.SkillAction.CasterId);
							porjectileColor = Color.red;
						}

						switch (action.SkillAction.SkillActionType)
						{
							case Protobuf.Messages.SkillActionType.AnimationStart:
								Debug.Log($"{action.SkillAction.CasterId} ({casterUnit.SelectedUnit.character.name}) started animation to cast {action.SkillAction.SkillId}");
								break;
							case Protobuf.Messages.SkillActionType.EffectTrigger:
								foreach (BattleUnit targetUnit in targetUnits)
								{
									projectilesPooler.StartProjectile(casterUnit, targetUnit, porjectileColor);
								}

								Debug.Log($"{action.SkillAction.CasterId} ({casterUnit.SelectedUnit.character.name}) casted {action.SkillAction.SkillId} targeting {string.Join(", ", action.SkillAction.TargetIds)}");
								break;
							case Protobuf.Messages.SkillActionType.EffectHit:
								Debug.Log($"{action.SkillAction.CasterId} ({casterUnit.SelectedUnit.character.name}) hit {string.Join(", ", action.SkillAction.TargetIds)} with skill {action.SkillAction.SkillId}");
								break;
							case Protobuf.Messages.SkillActionType.EffectMiss:
								Debug.Log($"{action.SkillAction.SkillId} missed {string.Join(", ", action.SkillAction.TargetIds)}");

								foreach (BattleUnit targetUnit in targetUnits)
								{
									projectilesPooler.ProjectileHit(casterUnit, targetUnit);
								}
								
								break;
						}
						break;
					case Protobuf.Messages.Action.ActionTypeOneofCase.Death:
						playerUnitsUI.Concat(opponentUnitsUI).First(unit => unit.SelectedUnit.id == action.Death.UnitId).DeathFeedback();
						break;
					case Protobuf.Messages.Action.ActionTypeOneofCase.ExecutionReceived:
						BattleUnit target = playerUnitsUI.Concat(opponentUnitsUI).First(unit => unit.SelectedUnit.id == action.ExecutionReceived.TargetId);
						var statAffected = action.ExecutionReceived.StatAffected;
						switch (statAffected.Stat)
						{
							case Protobuf.Messages.Stat.Health:
								target.CurrentHealth = target.CurrentHealth + (int)(statAffected.Amount);
								// playerUnitsUI.Concat(opponentUnitsUI).First(unit => unit.SelectedUnit.id == action.SkillAction.CasterId).AttackFeedback(targetUnit.transform.position);
								Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {target.SelectedUnit.id} dealing {statAffected.Amount} damage to it's health");
								break;
							case Protobuf.Messages.Stat.Energy:
								Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {target.SelectedUnit.id} dealing {statAffected.Amount} damage to it's energy");
								break;
							case Protobuf.Messages.Stat.Attack:
								Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {target.SelectedUnit.id} dealing {statAffected.Amount} damage to it's attack");
								break;
							case Protobuf.Messages.Stat.Defense:
								Debug.Log($"{action.SkillAction.CasterId} hit {action.SkillAction.SkillId} targeting {target.SelectedUnit.id} dealing {statAffected.Amount} damage it it's defense");
								break;
							default:
								Debug.Log(statAffected.Stat);
								break;
						}
						break;
					default:
						break;
				}
			}
		}
	}

	private List<BattleUnit> GetSkillTargets(string[] targetIds)
	{
		List<BattleUnit> targetUnits = new List<BattleUnit>();
		targetUnits.AddRange(playerUnitsUI.Where(unit => targetIds.Contains(unit.SelectedUnit.id)).ToArray());
		targetUnits.AddRange(opponentUnitsUI.Where(unit => targetIds.Contains(unit.SelectedUnit.id)).ToArray());
		return targetUnits;
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

    private List<UIReward> CreateRewardsList() {
        List<UIReward> rewards = new List<UIReward>();

        if (LevelProgress.selectedLevelData.experienceReward != 0) { rewards.Add(new ExperienceUIReward(LevelProgress.selectedLevelData.experienceReward)); }

        foreach (var currencyReward in LevelProgress.selectedLevelData.rewards) {
            rewards.Add(new CurrencyUIReward(currencyReward.Key, currencyReward.Value));
        }

        return rewards;
    }
}
