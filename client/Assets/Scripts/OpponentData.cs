using System.Collections.Generic;
using UnityEngine;


public class OpponentData : MonoBehaviour
{
    [SerializeField]
    List<Character> characters;

    // Singleton instance
    private static OpponentData instance;

    // User
    private User user;

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

    // Method to destroy the instance after battle has been ran
    public void Destroy()
    {
        if (instance != null) { Destroy(gameObject); }
    }

    // Method to get the singleton instance
    public static OpponentData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("OpponentData").AddComponent<OpponentData>();
            }
            return instance;
        }
    }

    // Initialize the user's units here
    private void Awake()
    {
        print("Awake!");
        if (instance == null)
        {
            print("Instance is null! Creating new.");
            instance = this;
            DontDestroyOnLoad(gameObject);

            // For testing purposes, initialize with sample data
            user = new User
            {
                username = "SampleUser",
                units = new List<Unit>
                {
                    new Unit { id = "101", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 0, selected = true },
                    new Unit { id = "102", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 1, selected = true },
                    new Unit { id = "103", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 2, selected = true },
                    new Unit { id = "104", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 3, selected = true },
                    new Unit { id = "105", level = 5, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 4, selected = true }
                }
            };
        } else {
            print("Instance exists! Destroying.");
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }
}
