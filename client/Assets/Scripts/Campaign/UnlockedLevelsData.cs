using UnityEngine;
using System.Collections.Generic;

public class UnlockedLevelsData : MonoBehaviour
{
    private static UnlockedLevelsData instance;

    // Dictionary to store the locked/unlocked state of each level
    private readonly Dictionary<string, bool> levelStates = new Dictionary<string, bool>();

    private string levelToUnlockName;

    // Public property to set the next level
    public string LevelToUnlockName
    {
        set { levelToUnlockName = value; }
    }

    // Singleton instance
    public static UnlockedLevelsData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("UnlockedLevelsData").AddComponent<UnlockedLevelsData>();
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

    // Method to check if a level is locked
    public bool IsLevelUnlocked(string levelName)
    {
        return levelStates.ContainsKey(levelName) ? levelStates[levelName] : false;
    }

    // Method to unlock a level
    public void UnlockNextLevel()
    {
        if (levelStates.ContainsKey(levelToUnlockName))
        {
            levelStates[levelToUnlockName] = true;
        }
        else
        {
            levelStates.Add(levelToUnlockName, true);
        }
    }
}
