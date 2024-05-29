using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters")]
public class Character : ScriptableObject
{
    public new string name;
    public Faction faction;
    public Sprite defaultSprite;
    public Sprite inGameSprite;
    public GameObject prefab;
    public Rarity rarity;
}

public enum Rarity
{
    Common,
    Rare,
    Elite
}

public enum Faction
{
    Araban,
    Otobi,
    Kaline,
    Merliot
}
