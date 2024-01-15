using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters")]
public class Character : ScriptableObject
{
    public new string name;
    public string faction;
    public Sprite iconSprite;
    public Sprite disabledSprite;
    public Sprite backgroundSprite;
    public GameObject prefab;
    public Rarity rarity;
}

public enum Rarity{
    Common,
    Rare,
    Elite
}
