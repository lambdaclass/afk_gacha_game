using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    [SerializeField]
    Image unitImage;

	[SerializeField]
	UIProgressBar healthBar;

	[SerializeField]
	GameObject indicatorPrefab;

	[SerializeField]
	CanvasGroup canvasGroup;

	[SerializeField]
	LineRenderer lineRenderer;

    private Unit selectedUnit;
	public Unit SelectedUnit
    {
        get { return selectedUnit; }
        private set { selectedUnit = value; }
    }

	private int maxHealth;
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            if (value != maxHealth)
            {
                maxHealth = value;
                healthBar.fillAmount = currentHealth / (float)maxHealth;
            }
        }
    }

	private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (value != currentHealth)
            {
				if(currentHealth > 0) {
					GameObject indicatorGO = Instantiate(indicatorPrefab, gameObject.transform);
					BattleIndicator indicator = indicatorGO.GetComponent<BattleIndicator>();
					indicator.SetText((currentHealth - value).ToString());
					indicator.FadeAnimation();
				}
                currentHealth = value;
                healthBar.fillAmount = currentHealth / (float)maxHealth;
            }
        }
    }

	Dictionary<string, int> statusCount = new Dictionary<string, int>();
	Dictionary<string, BattleIndicator> statusIndicators = new Dictionary<string, BattleIndicator>();

    public void SetUnit(Unit unit, bool isPlayer) {
        selectedUnit = unit;
        GetComponent<Button>().interactable = isPlayer;
        unitImage.sprite = selectedUnit.character.inGameSprite;
        unitImage.gameObject.SetActive(true);
    }

	public void DeathFeedback()	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(unitImage.transform.DORotate(new Vector3(0, 180, 90), .5f));
		sequence.Append(unitImage.DOFade(0, 2f));
		sequence.Join(canvasGroup.DOFade(0, 2f));
		sequence.Play();
	}

	public void AttackFeedback(Vector3 target) {
		StartCoroutine(DrawProjectile(target));
	}

	IEnumerator DrawProjectile(Vector3 target)
	{
		lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
		lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target);
		lineRenderer.enabled = true;
		yield return new WaitForSeconds(.3f);
		lineRenderer.enabled = false;
	}

	public void ApplyStatus(string status)
	{
		if (!statusCount.ContainsKey(status))
		{
			statusCount.Add(status, 1);
			GameObject indicatorGO = Instantiate(indicatorPrefab, gameObject.transform);
			BattleIndicator indicator = indicatorGO.GetComponent<BattleIndicator>();
			indicator.SetText(status);
			statusIndicators.Add(status, indicator);
			indicator.SetUpAnimation();
		}
		else
		{
			statusCount[status]++;
		}
	}

	public void RemoveStatus(string status)
	{
		if (statusCount.ContainsKey(status))
		{
			statusCount[status]--;
			if (statusCount[status] == 0)
			{
				BattleIndicator indicator = statusIndicators[status];
				indicator.RemoveAnimation();
				statusIndicators.Remove(status);
				statusCount.Remove(status);
			}
		}
	}
}
