using System.Collections.Generic;
using UnityEngine;


public class GlobalUserData : MonoBehaviour
{
    [SerializeField]
    List<Character> characters;

    // Singleton instance
    private static GlobalUserData instance;

    // User
    private User user;

    // Public property to access the user's units
    public User User
    {
        get { return user; }
        set { user = value; }
    }

    // Method to get the singleton instance
    public static GlobalUserData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GlobalUserData").AddComponent<GlobalUserData>();
            }
            return instance;
        }
    }

    // Initialize the user's units here
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // For testing purposes, initialize with sample data
            user = new User
            {
                id = "1",
                username = "SampleUser",
                units = new List<Unit>
                {
                    new Unit { id = "101", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 1, selected = true },
                    new Unit { id = "102", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 2, selected = true },
                    new Unit { id = "103", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 3, selected = true },
                    new Unit { id = "104", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 4, selected = true },
                    new Unit { id = "105", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 5, selected = true }
                }
            };
        } else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }
}
