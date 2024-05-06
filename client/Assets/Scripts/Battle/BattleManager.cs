using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    BattleUnit[] battleUnitsUI;

	[SerializeField]
	ProjectilesPooler projectilesPooler;

	private bool continuePlayback = true;
	
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
		BattleUnit[] unitPositions = battleUnitsUI.Where(battleUnit => battleUnit.IsPlayerTeam == isPlayer).ToArray();
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

		projectilesPooler.ClearProjectiles();
		
		if(continuePlayback) {
			yield return new WaitForSeconds(2f);
		}
		
		HandleBattleResult(battleResult.Result == "team_1");
	}

	private void SetUpInitialState(Protobuf.Messages.BattleResult battleResult)
	{
		foreach (var unit in battleResult.InitialState.Units)
		{
			BattleUnit battleUnit = battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Single(battleUnit => battleUnit.SelectedUnit.id == unit.Id);
			battleUnit.gameObject.SetActive(true);
			battleUnit.MaxHealth = unit.Health;
			battleUnit.CurrentHealth = unit.Health;
		}
	}

	private IEnumerator PlayOutSteps(RepeatedField<Protobuf.Messages.Step> steps)
	{
		foreach (var step in steps)
		{
			if(!continuePlayback) {
				yield break;
			}

			yield return new WaitForSeconds(.05f);

			ProcessEffectTriggers(step.Actions);
			ProcessHitsAndMisses(step.Actions);
			ProcessExecutionsReceived(step.Actions);

			var actionsExcludingSkills = step.Actions
				.Where(action => 	action.ActionTypeCase != Protobuf.Messages.Action.ActionTypeOneofCase.SkillAction &&
									action.ActionTypeCase != Protobuf.Messages.Action.ActionTypeOneofCase.ExecutionReceived);

			foreach (var action in actionsExcludingSkills) {
				switch (action.ActionTypeCase)
				{
					case Protobuf.Messages.Action.ActionTypeOneofCase.Death:
						battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Single(unit => unit.SelectedUnit.id == action.Death.UnitId).DeathFeedback();
						break;
					default:
						break;
				}
			}
		}
	}

	public void CancelBattlePlayback() {
		continuePlayback = false;
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
            user.AddCurrencies(GetLevelRewards());
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

	private void ProcessEffectTriggers(IEnumerable<Protobuf.Messages.Action> actions)
	{
		IEnumerable<Protobuf.Messages.Action> effectTriggerActions = actions
			.Where(action => action.ActionTypeCase == Protobuf.Messages.Action.ActionTypeOneofCase.SkillAction)
			.Where(action => action.SkillAction.SkillActionType == Protobuf.Messages.SkillActionType.EffectTrigger);

		foreach (Protobuf.Messages.Action action in effectTriggerActions)
		{
			BattleUnit casterUnit = battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Single(battleUnit => battleUnit.SelectedUnit.id == action.SkillAction.CasterId);
			List<BattleUnit> targetUnits = battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Where(unit => action.SkillAction.TargetIds.Contains(unit.SelectedUnit.id)).ToList();

			foreach (BattleUnit targetUnit in targetUnits)
			{
				Color projectileColor = (casterUnit.IsPlayerTeam && targetUnit.IsPlayerTeam) ? Color.green : Color.red;
				projectilesPooler.TriggerProjectile(casterUnit, targetUnit, projectileColor);
			}
		}
	}

	private void ProcessHitsAndMisses(IEnumerable<Protobuf.Messages.Action> actions)
	{
		IEnumerable<Protobuf.Messages.Action> hitAndMissActions = actions
				.Where(action => action.ActionTypeCase == Protobuf.Messages.Action.ActionTypeOneofCase.SkillAction)
				.Where(action => action.SkillAction.SkillActionType == Protobuf.Messages.SkillActionType.EffectHit || action.SkillAction.SkillActionType == Protobuf.Messages.SkillActionType.EffectMiss);

		foreach (Protobuf.Messages.Action action in hitAndMissActions)
		{
			BattleUnit casterUnit = battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Single(battleUnit => battleUnit.SelectedUnit.id == action.SkillAction.CasterId);
			List<BattleUnit> targetUnits = battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Where(unit => action.SkillAction.TargetIds.Contains(unit.SelectedUnit.id)).ToList();

			foreach (BattleUnit targetUnit in targetUnits)
			{
				Color projectileColor = (casterUnit.IsPlayerTeam && targetUnit.IsPlayerTeam) ? Color.green : Color.red;
				casterUnit.AttackTrigger();
				projectilesPooler.ProjectileHit(casterUnit, targetUnit, projectileColor);
			}
		}
	}

	private void ProcessExecutionsReceived(IEnumerable<Protobuf.Messages.Action> actions)
	{
		var executionReceivedActions = actions
			.Where(action => action.ActionTypeCase == Protobuf.Messages.Action.ActionTypeOneofCase.ExecutionReceived);

		foreach (Protobuf.Messages.Action action in executionReceivedActions)
		{
			BattleUnit target = battleUnitsUI.Where(battleUnit => battleUnit.SelectedUnit != null).Single(unit => unit.SelectedUnit.id == action.ExecutionReceived.TargetId);
			var statAffected = action.ExecutionReceived.StatAffected;
			switch (statAffected.Stat)
			{
				case Protobuf.Messages.Stat.Health:
					target.CurrentHealth = target.CurrentHealth + (int)(statAffected.Amount);
					break;
				case Protobuf.Messages.Stat.Energy:
					break;
				case Protobuf.Messages.Stat.Attack:
					break;
				case Protobuf.Messages.Stat.Defense:
					break;
				default:
					Debug.Log(statAffected.Stat);
					break;
			}
		}
	}

    private Dictionary<Currency, int> GetLevelRewards()
    {
        Dictionary<Currency, int> rewards = LevelProgress.selectedLevelData.rewards;
        if (LevelProgress.selectedLevelData.experienceReward > 0)
        {
            rewards.Add(Currency.Experience, LevelProgress.selectedLevelData.experienceReward);
        }
        return rewards;
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

        foreach (var currencyReward in LevelProgress.selectedLevelData.rewards) {
            if (currencyReward.Value > 0)
            {
                rewards.Add(new CurrencyUIReward(currencyReward.Key, currencyReward.Value));            
            }
        }
        return rewards;
    }
}
