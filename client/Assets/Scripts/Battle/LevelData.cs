using System.Collections.Generic;
using UnityEngine;


public class LevelData : MonoBehaviour
{
    [SerializeField]
    List<Character> characters;

    // Singleton instance
    private static LevelData instance;

    // User
    private User user = new User {
        username = "Opponent",
        units = new List<Unit>()
    };

    // Rewards
    private Dictionary<Currency, int> rewards = new Dictionary<Currency, int>();

    private int experience;

    // AFK Rewards
    private Dictionary<Currency, int> afkCurrencyRate = new Dictionary<Currency, int>();

    private int afkExperienceRate;

    // Public property to access the opponent
    public User User
    {
        get { return user; }
        set { user = value; }
    }


    // Public property to access the opponent's units
    public List<Unit> Units
    {
        get { return user.units; }
        set { user.units = value; }
    }

    // Public property to access the level's rewards
    public Dictionary<Currency, int> Rewards
    {
        get { return rewards; }
        set { rewards = value; }
    }

    // Public property to access the level's experience reward
    public int Experience
    {
        get { return experience; }
        set { experience = value; }
    }

    public Dictionary<Currency, int> AfkCurrencyRate {
        get { return afkCurrencyRate; }
        set { afkCurrencyRate = value; }
    }

    public int AfkExperienceRate
    {
        get { return afkExperienceRate; }
        set { afkExperienceRate = value; }
    }

    // Method to destroy the instance after battle has been ran
    public void Destroy()
    {
        if (instance != null) { Destroy(gameObject); }
    }

    // Method to get the singleton instance
    public static LevelData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("LevelData").AddComponent<LevelData>();
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
