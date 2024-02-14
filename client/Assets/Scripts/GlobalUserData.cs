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

    // Public property to access the user
    public User User
    {
        get { return user; }
        set { user = value; }
    }

    // Public property to access the user's units
    public List<Unit> Units
    {
        get { return user.units; }
    }

    // Public property to access the user's selected units
    public List<Unit> SelectedUnits
    {
        get { return user.units.FindAll(unit => unit.selected); }
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
                    new Unit { id = "1", tier = 0, level = 1, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = 0, selected = true, rank = Rank.Star1 },
                    new Unit { id = "2", tier = 0, level = 5, character = characters.Find(character => "uma" == character.name.ToLower()), slot = 1, selected = true, rank = Rank.Star2 },
                    new Unit { id = "3", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = 2, selected = true, rank = Rank.Star3 },
                    new Unit { id = "4", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = 3, selected = true, rank = Rank.Star4 },
                    new Unit { id = "5", tier = 0, level = 4, character = characters.Find(character => "otix" == character.name.ToLower()), slot = 4, selected = true, rank = Rank.Star5 },
                    new Unit { id = "6", tier = 0, level = 1, character = characters.Find(character => "muflus" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Illumination1 },
                    new Unit { id = "7", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Illumination2 },
                    new Unit { id = "8", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Illumination3 },
                    new Unit { id = "9", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Awakened },
                    new Unit { id = "10", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "11", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "12", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "13", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "14", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "15", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "16", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "17", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "18", tier = 0, level = 3, character = characters.Find(character => "kenzu" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "19", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                    new Unit { id = "20", tier = 0, level = 2, character = characters.Find(character => "valtimer" == character.name.ToLower()), slot = null, selected = false, rank = Rank.Star4 },
                },
                next_unit_id = 22,
            };
        } else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }
}
