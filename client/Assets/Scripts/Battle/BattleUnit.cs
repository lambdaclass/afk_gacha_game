using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField]
    Image unitImage;

	[SerializeField]
	UIProgressBar healthBar;

	[SerializeField]
	GameObject indicatorPrefab;

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
			Debug.LogWarning("set current health");
            if (value != currentHealth)
            {
				if(currentHealth > 0) {
					GameObject indicator = Instantiate(indicatorPrefab, gameObject.transform);
					indicator.GetComponent<BattleIndicator>().SetText((currentHealth - value).ToString());
				}
                currentHealth = value;
                healthBar.fillAmount = currentHealth / (float)maxHealth;
				// Debug.LogWarning("inside if");
            }
        }
    }

    public void SetUnit(Unit unit, bool isPlayer) {
        selectedUnit = unit;
        GetComponent<Button>().interactable = isPlayer;
        unitImage.sprite = selectedUnit.character.inGameSprite;
        unitImage.gameObject.SetActive(true);
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }
}
