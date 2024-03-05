using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SummonManager : MonoBehaviour
{
	[SerializeField]
	BoxUI boxPrefab;

	[SerializeField]
	GameObject boxesContainer;

	[SerializeField]
	PopupButtonsController confirmPopup;

	[SerializeField]
	GameObject newCharacterContainer;

	[SerializeField]
	TMP_Text newUnitName;

	[SerializeField]
	Image newUnitImage;

	void Start() {
		SocketConnection.Instance.GetBoxes(
			(boxes) => {
				foreach(Box box in boxes) {
					BoxUI boxUI = Instantiate(boxPrefab, boxesContainer.transform);
					Action<string, string> onClick = (userId, boxId) => ShowConfirmPopup(userId, boxId);
					boxUI.SetBox(box, onClick);
				}
			},
			(error) => {
				Debug.LogError(error);
			}
		);
	}

	void ShowConfirmPopup(string userId, string boxId) {
		UnityAction onClickAction = () => Summon(userId, boxId);
		confirmPopup.ConfirmButton.onClick.AddListener(onClickAction);
		confirmPopup.gameObject.SetActive(true);
	}

	private void Summon(string userId, string boxId)
	{
		SocketConnection.Instance.Summon(userId, boxId,
			(user, unit) => {
				Debug.Log($"new unit: {unit.character.name}, {unit.id}");
				newCharacterContainer.SetActive(true);
				newUnitName.text = unit.character.name;
				newUnitImage.sprite = unit.character.inGameSprite;
			},
			reason => {
				Debug.LogError(reason);
			}	
		);
	}
}
