using System.Collections.Generic;
using UnityEngine;

public class LevelItem : MonoBehaviour
{
    [SerializeField]
    List<Character> characters;

    // Singleton instance
    private static LevelItem instance;

    private Level level;

    public Level Level
    {
        get { return level; }
        set { level = value; }
    }

    // Public property to access the opponent's units
    public List<Unit> Units
    {
        get { return level.units; }
    }

    // Public property to access the level's rewards
    public Dictionary<Currency, int> Rewards
    {
        get { return level.rewards; }
    }

    // Public property to access the level's experience reward
    public int Experience
    {
        get { return level.experienceReward; }
    }

    public Dictionary<Currency, int> AfkCurrencyRate {
        get { return level.afkCurrencyRate; }
    }

    public int AfkExperienceRate
    {
        get { return level.afkExperienceRate; }
    }

    public string CampaignToComplete
    {
        get { return level.campaignToComplete; }
    }

    public string CampaignToUnlock
    {
        get { return level.campaignToUnlock; }
    }

    public string NextLevelName
    {
        get { return level.nextLevelName; }
    }

    // Method to destroy the instance after battle has been ran
    public void Destroy()
    {
        if (instance != null) { Destroy(gameObject); }
    }

    // Method to get the singleton instance
    public static LevelItem Instance
    {
        get
        {
            Debug.Log("get");
            if (instance == null)
            {
                Debug.Log("instance == null");
                instance = new GameObject("LevelData").AddComponent<LevelItem>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }
}
