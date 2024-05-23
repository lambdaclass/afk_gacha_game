using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Currency")]
public class Currency : ScriptableObject
{
	public new string name;
	public Sprite image;
}
