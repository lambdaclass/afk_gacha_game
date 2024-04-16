using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI;
using DG.Tweening;
using System.Collections;

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
					GameObject indicator = Instantiate(indicatorPrefab, gameObject.transform);
					indicator.GetComponent<BattleIndicator>().SetText((currentHealth - value).ToString());
				}
                currentHealth = value;
                healthBar.fillAmount = currentHealth / (float)maxHealth;
            }
        }
    }

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

	public void StatusFeedback(string message)
	{
		GameObject indicator = Instantiate(indicatorPrefab, gameObject.transform);
		indicator.GetComponent<BattleIndicator>().SetText(message);
	}
}
