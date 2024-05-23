using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource")]
public class Resource : ScriptableObject
{
	public string currency;
	public Sprite image;
}
