using System.Collections.Generic;
using UnityEngine;

public class GlobalUserData : MonoBehaviour
{
    // This should be in it's own manager like of Curse of Mirra, not a list in the user singleton (should there be a user singleton?)
    [SerializeField]
    List<Character> characters;

    public List<Character> AvailableCharacters {
        get {
            return this.characters;
        }
    }

    [SerializeField]
    List<ItemTemplate> itemtemplates;
    public List<ItemTemplate> AvailableItemTemplates {
        get {
            return this.itemtemplates;
        }
    }

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

    private void Awake()
    {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }
}
