using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonManager : MonoBehaviour
{
	[SerializeField]
	BoxUI boxPrefab;

	[SerializeField]
	GameObject boxesContainer;

	void Start() {
		SocketConnection.Instance.GetBoxes(
			(boxes) => {
				foreach(Box box in boxes) {
					Debug.Log($"{box.name}, {box.description}");

					Instantiate(boxPrefab, boxesContainer.transform);
				}
			},
			(error) => {
				Debug.LogError(error);
			}
		);
	}
}
