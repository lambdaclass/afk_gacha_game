using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitDetail : MonoBehaviour
{
    private static Unit selectedUnit;

    [SerializeField]
    TMP_Text goldCostText;

    [SerializeField]
    TMP_Text gemCostText;
    
    [SerializeField]
    Text actionButtonText;

    [SerializeField]
    Image backgroundImage;

    [SerializeField]
    GameObject modelContainer;

    [SerializeField]
    GameObject characterNameContainer;

    [SerializeField]
    GameObject levelStatUI;

    [SerializeField]
    GameObject tierStatUI;

    [SerializeField]
    GameObject rankStatUI;

    [SerializeField]
    GameObject headItemSprite;

    [SerializeField]
    GameObject chestItemSprite;

    [SerializeField]
    GameObject bootsItemSprite;

    [SerializeField]
    GameObject weaponItemSprite;

    private Dictionary<Currency, int> cost;

    // true if we're leveling up, false if we're tiering up
    private bool actionLevelUp;

    void Start() {
        SetActionAndCosts();
        UpdateTexts();
        UpdateStats();
        SetBackgroundImage();
        DisplayUnit();
        DisplayUnitItems();
    }

    public void ActionButton() {
        User user = GlobalUserData.Instance.User;
        if (user.CanAfford(cost)) {
            bool result;
            if (actionLevelUp) { result = LevelUp(); } else { result = TierUp(); }

            if (result) { 
                user.SubstractCurrency(cost);
                SetActionAndCosts();
                UpdateTexts();
            }
        } else {
            // Blocked by currency cost.
        }
    }

    private bool LevelUp() {
        if (selectedUnit.LevelUp()) {
            return true;
        }
        Debug.LogError("[UnitDetail.cs] Could not level up unit. Likely cause: a disparity between User.CanLevelUp() calls in UnitDetail.SetActionAndCosts() and Unit.LevelUp().");
        return false;
    }

    private bool TierUp() {
        if (selectedUnit.TierUp()) {
            return true;
        }
        // Tell user they need to improve the ranking of their unit via fusion.
        return false;
    }

    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        SceneManager.LoadScene("UnitDetail");
    }

    private void UpdateTexts() {
        if (actionLevelUp) actionButtonText.text = "Level Up";
        else actionButtonText.text = "Tier up";

        goldCostText.text = cost.ContainsKey(Currency.Gold) ? cost[Currency.Gold].ToString() : "0";
        gemCostText.text = cost.ContainsKey(Currency.Gems) ? cost[Currency.Gems].ToString() : "0";
    }

    private void UpdateStats()
    {
        tierStatUI.GetComponentInChildren<TextMeshProUGUI>().text = "Tier\n" + selectedUnit.tier.ToString();
        levelStatUI.GetComponentInChildren<TextMeshProUGUI>().text = "Level\n" + selectedUnit.level.ToString();
        UpdateRank();    

    }

    private void UpdateRank()
    {
        GameObject rankPrefab;
        int rankNumber;

        switch (selectedUnit.rank)
        {
            case Rank.Star1:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Star");
                rankNumber = 1;
                break;
            case Rank.Star2:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Star");
                rankNumber = 2;
                break;
            case Rank.Star3:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Star");
                rankNumber = 3;
                break;
            case Rank.Star4:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Star");
                rankNumber = 4;
                break;
            case Rank.Star5:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Star");
                rankNumber = 5;
                break;
            case Rank.Illumination1:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Illumination");
                rankNumber = 1;
                break;
            case Rank.Illumination2:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Illumination");
                rankNumber = 2;
                break;
            case Rank.Illumination3:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Illumination");
                rankNumber = 3;
                break;
            case Rank.Awakened:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Awakened");
                rankNumber = 1;
                break;
            default:
                rankPrefab = Resources.Load<GameObject>("UI/Ranks/Star");
                rankNumber = 1;
                break;
        }

        Transform rankContainer = rankStatUI.transform.Find("Value");
        for (int i = 0; i < rankNumber; i++)
        {
            GameObject rank = Instantiate(rankPrefab, rankContainer);
            // The instantiated rankPrefabs are placed equidistantly from each other inside the rankContainer, without exceeding the rankContainer's width.
            // Also, they should be placed in the middle of the rankContainer's height.
            rank.transform.localPosition = new Vector3(
                (i - (rankNumber - 1) / 2f) * rankContainer.GetComponent<RectTransform>().rect.width / rankNumber,                 
                -rankStatUI.transform.localPosition.y/2f, 
                0);
        }

    }

    private void SetActionAndCosts() {
        if (selectedUnit.CanLevelUp()) {
            actionLevelUp = true;
            cost = selectedUnit.LevelUpCost;
        } else {
            actionLevelUp = false;
            cost = selectedUnit.TierUpCost;
        }
    }

    private void SetBackgroundImage() 
    {
        switch (selectedUnit.character.faction) 
        {
            case Faction.Araban:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/ArabanBackground");
                break;
            case Faction.Kaline:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/KalineBackground");
                break;
            case Faction.Merliot:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/MerliotBackground");
                break;
            case Faction.Otobi:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/OtobiBackground");
                break;
            default:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/ArabanBackground");
                break;
        }
    }

    private void DisplayUnit()
    {
        if (modelContainer.transform.childCount > 0)
        {
            RemoveUnitFromContainer();
        }
        Instantiate(selectedUnit.character.prefab, modelContainer.transform);
        characterNameContainer.GetComponentInChildren<TextMeshProUGUI>().text = selectedUnit.character.name;
        characterNameContainer.SetActive(true);
    }

    private void RemoveUnitFromContainer()
    {
        Destroy(modelContainer.transform.GetChild(0).gameObject);
        characterNameContainer.SetActive(false);
    }

    private void DisplayUnitItems()
    {
        if (selectedUnit.head != null)
        {
            headItemSprite.GetComponent<Image>().sprite = selectedUnit.head.concreteItem.Sprite;
        }
        if (selectedUnit.chest != null)
        {
            chestItemSprite.GetComponent<Image>().sprite = selectedUnit.chest.concreteItem.Sprite;
        }
        if (selectedUnit.boots != null)
        {
            bootsItemSprite.GetComponent<Image>().sprite = selectedUnit.boots.concreteItem.Sprite;
        }
        if (selectedUnit.weapon != null)
        {
            weaponItemSprite.GetComponent<Image>().sprite = selectedUnit.weapon.concreteItem.Sprite;
        }
    }
}
