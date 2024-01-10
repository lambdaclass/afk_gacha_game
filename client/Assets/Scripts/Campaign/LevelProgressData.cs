using UnityEngine;
using System.Collections.Generic;

public class LevelProgressData : MonoBehaviour
{
    private static LevelProgressData instance;

    // Dictionary to store the locked/unlocked state of each level
    private readonly Dictionary<string, Status> levelStates = new Dictionary<string, Status>();

    public enum Status
    {
        Locked,
        Unlocked,
        Completed
    }
    private string levelToCompleteName;
    private string levelToUnlockName;

    // Public property to set the current level being played
    public string LevelToCompleteName
    {
        set { levelToCompleteName = value; }
    }

    // Public property to set the next level
    public string LevelToUnlockName
    {
        set { levelToUnlockName = value; }
    }

    // Singleton instance
    public static LevelProgressData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("LevelProgressData").AddComponent<LevelProgressData>();
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

    // Method to check the status of a campaign
    public Status LevelStatus(string levelName)
    {
        return levelStates.ContainsKey(levelName) ? levelStates[levelName] : Status.Locked;
    }

    // Called on battles won.
    public void ProcessLevelCompleted()
    {
        // Key should exist, this is just to be sure
        if (levelStates.ContainsKey(levelToCompleteName))
        {
            levelStates[levelToCompleteName] = Status.Completed;
        }
        else
        {
            levelStates.Add(levelToCompleteName, Status.Completed);
        }

        if (levelToUnlockName != null) {
            // Key should not exist, this is just to be sure
            if (levelStates.ContainsKey(levelToUnlockName))
            {
                levelStates[levelToUnlockName] = Status.Unlocked;
            }
            else
            {
                levelStates.Add(levelToUnlockName, Status.Unlocked);
            }
        }
    }

    // To be called with first levels of campaigns. Doesn't modify data if it already exists.
    public void SetUnlocked(string levelName) {
        if(!levelStates.ContainsKey(levelName)) { levelStates.Add(levelName, Status.Unlocked); }
    }
}
