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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize with sample data. Overwritten by Level selection.
            user = new User
            {
                username = "Opponent",
                units = new List<Unit>()
            };
        } else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }
}
