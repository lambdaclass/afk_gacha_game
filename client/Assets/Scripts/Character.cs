using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters")]
public class Character : ScriptableObject
{
    public new string name;
    public Sprite iconSprite;
    public Sprite disabledSprite;
    public Sprite backgroundSprite;
    public GameObject prefab;
}
