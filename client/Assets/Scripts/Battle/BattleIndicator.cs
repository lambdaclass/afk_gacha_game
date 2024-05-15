using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BattleIndicator : MonoBehaviour
{
	[SerializeField]
	TMP_Text indicatorText;
	const float animationMultiplier = 1f;

	void Start()
	{
		transform.localScale = new Vector3(0, 0, 0);
		transform.DOScale(new Vector3(1, 1, 1), .5f).SetEase(Ease.InOutBack);
		transform.DOMoveY(transform.position.y + .4f, .5f).SetEase(Ease.InSine).SetDelay(.5f);
		indicatorText.DOFade(0f, .5f).SetEase(Ease.InCirc).SetDelay(.5f).onComplete = DestroyGameObject;
	}

	public void SetText(string text)
	{
		indicatorText.text = text;
	}

	public void FadeAnimation()
	{
		transform.localScale = new Vector3(0, 0, 0);
		transform.DOScale(new Vector3(1, 1, 1), animationMultiplier / 2).SetEase(Ease.InOutBack);
		transform.DOMoveY(transform.position.y + .4f, animationMultiplier / 2).SetEase(Ease.InSine).SetDelay(.5f);
		indicatorText.DOFade(0f, animationMultiplier / 2).SetEase(Ease.InCirc).SetDelay(animationMultiplier / 2).onComplete = DestroyGameObject;
	}

	public void SetUpAnimation()
	{
		transform.localScale = new Vector3(0, 0, 0);
		transform.position += new Vector3(0, .5f, 0);
		transform.DOScale(new Vector3(1, 1, 1), animationMultiplier / 2).SetEase(Ease.InOutBack);
	}

	public void RemoveAnimation()
	{
		transform.DOScale(new Vector3(0, 0, 0), animationMultiplier / 2).SetEase(Ease.InOutBack).SetDelay(animationMultiplier / 2).onComplete = DestroyGameObject;
	}

	private void DestroyGameObject()
	{
		Destroy(gameObject);
	}
}
