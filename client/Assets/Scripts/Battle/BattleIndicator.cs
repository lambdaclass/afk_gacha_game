using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BattleIndicator : MonoBehaviour
{
	[SerializeField]
	TMP_Text indicatorText;

    void Start()
    {
		transform.localScale = new Vector3(0, 0, 0);
		transform.DOScale(new Vector3(1, 1, 1), .5f).SetEase(Ease.InOutBack);
		transform.DOMoveY(transform.position.y + .4f, .5f).SetEase(Ease.InSine).SetDelay(.5f);
		indicatorText.DOFade(0f, .5f).SetEase(Ease.InCirc).SetDelay(.5f);
    }

	public void SetText(string text) {
		indicatorText.text = text;
	}
}
