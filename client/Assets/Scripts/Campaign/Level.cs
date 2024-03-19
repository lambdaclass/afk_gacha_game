using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    // The characters of the units the level has, in order.
    [SerializeField] List<Character> characters;
    
    // The levels of the units the level has, in order.
    [SerializeField] List<int> levels;

    public List<Unit> units = new List<Unit>();

    // Currency rewards
    [SerializeField] public CurrencyValue[] individualRewards;
    public Dictionary<Currency, int> rewards = new Dictionary<Currency, int>();

    [SerializeField] public int experienceReward;


    // Unlock this level if current level is beaten.
    // Level instead of string (like campaigns) to make it easier to set up in UI.
    [SerializeField] public Level nextLevel;
    public string nextLevelName = null;

    [SerializeField] GameObject lockObject;

    [SerializeField] GameObject completedCrossObject;

    // Wether this level is the first one of the campaign (unlocked automatically)
    [SerializeField] bool first;

    // Mark this campaign as completed if this level is beaten
    [SerializeField] public string campaignToComplete;

    // Unlock this campaign if this level is beaten
    [SerializeField] public string campaignToUnlock;

    // AFK Rewards rate granted
    // These are how many a player makes in the maximum timespan available (12h now)
    [SerializeField] public CurrencyValue[] individualAfkCurrencyRates;
    public Dictionary<Currency, int> afkCurrencyRate = new Dictionary<Currency, int>();

    [SerializeField] public int afkExperienceRate;

    private void Awake() {
        // Set up the currency dictionaries
        foreach (CurrencyValue individualReward in individualRewards) {
            rewards.Add(individualReward.currency, individualReward.value);
        }

        foreach (CurrencyValue individualAfkCurrencyRate in individualAfkCurrencyRates) {
            afkCurrencyRate.Add(individualAfkCurrencyRate.currency, individualAfkCurrencyRate.value);
        }

        // Build Units list
        for(int i = 0; i < characters.Count; i++) {
            units.Add(new Unit { id = "op-" + i.ToString(), level = levels[i], character = characters[i], slot = i, selected = true });
        }

        if(nextLevel != null) { nextLevelName = nextLevel.name; }
    }

    private void Start(){
        if(first) { LevelProgressData.Instance.SetUnlocked(name); }

        switch(LevelProgressData.Instance.LevelStatus(name)) 
        {
            case LevelProgressData.Status.Locked:
                break;
            case LevelProgressData.Status.Unlocked:
                lockObject.SetActive(false);
                break;
            case LevelProgressData.Status.Completed:
                completedCrossObject.SetActive(true);
                lockObject.SetActive(false);
                break;
        }
    }

    public void SelectLevel(){
        if(LevelProgressData.Instance.LevelStatus(name) != LevelProgressData.Status.Unlocked) {
            return;
        }

        // Get the CampaignLevelManager parent and make it pop up the Battle button
        Transform parent = transform.parent;
        if (parent.TryGetComponent<CampaignLevelManager>(out CampaignLevelManager campaignLevelManager)) { campaignLevelManager.LevelSelected(); }
        else { Debug.LogError("Level has no CampaignLevelManager parent."); }
        
        SetLevel();
        SetLevelToComplete();
        SetLevelToUnlock();
        SetCampaignToComplete();
        SetCampaignToUnlock();
    }

    private void SetLevel() {
        LevelItem.Instance.Level = this;
    }

    private void SetLevelToComplete() {
        LevelProgressData.Instance.LevelToCompleteName = name;
    }

    private void SetLevelToUnlock() {
        if(nextLevel != null) { LevelProgressData.Instance.LevelToUnlockName = nextLevel.name; }
        else { LevelProgressData.Instance.LevelToUnlockName = null; }
    }

    private void SetCampaignToComplete() {
        // CampaignProgressData.Instance.CampaignToComplete = campaignToComplete;
    }

    private void SetCampaignToUnlock() {
        // CampaignProgressData.Instance.CampaignToUnlock = campaignToUnlock;
    }
}
