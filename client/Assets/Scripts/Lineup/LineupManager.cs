using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineupManager : MonoBehaviour, IUnitPopulator
{
    List<Unit> playerAvailableUnits;

    [SerializeField]
    LineUpUnitsUIContainer unitsContainer;

    [SerializeField]
    UnitPosition[] playerUnitPositions;

    [SerializeField]
    UnitPosition[] opponentUnitPositions;

    [SerializeField]
    Button battleButton;

    [SerializeField]
    GameObject insufficientCurrenciesPopup;

    [SerializeField]
    SceneNavigator sceneNavigator;

    [SerializeField]
    GameObject levelAttemptCostsUIContainer;
    [SerializeField]
    GameObject levelAttemptCostUIPrefab;
    [SerializeField]
    GameObject suppliesHeaderResourceUI;
    [SerializeField]
    GameObject gemsHeaderResourceUI;
    public static LevelData LevelData;

    void Start()
    {
        SocketConnection.Instance.GetUserAndContinue();
        SetUpBattleButtonBehaviour();
        InstantiateLevelAttemptCostsUI();
        UpdateHeaderResources();
        StartCoroutine(SetUpUserUnits());
    }

    private IEnumerator SetUpUserUnits()
    {
        yield return new WaitUntil(() => GlobalUserData.Instance != null);

        playerAvailableUnits = GlobalUserData.Instance.Units;

        LevelData levelData = LevelProgress.selectedLevelData;
        SetUpSelectedUnits(levelData.units, false);

        this.unitsContainer.Populate(playerAvailableUnits, this);
        SetUpSelectedUnits(playerAvailableUnits, true);

        unitsContainer.OnUnitSelected.AddListener(AddUnitToLineup);

    }

    private void SetUpSelectedUnits(List<Unit> units, bool isPlayer)
    {
        UnitPosition[] unitPositions = isPlayer ? playerUnitPositions : opponentUnitPositions;
        List<Unit> selectedUnits = units.Where(unit => unit.selected).ToList();
        foreach (Unit unit in selectedUnits)
        {
            UnitPosition unitPosition = unitPositions[unit.slot.Value - 1];
            unitPosition.SetUnit(unit, isPlayer);
            unitPosition.OnUnitRemoved += RemoveUnitFromLineup;
        }
        Debug.Log(playerUnitPositions.Where(up => up.IsOccupied).Count());
        Debug.Log(isPlayer);
        Debug.Log(isPlayer && playerUnitPositions.Where(up => up.IsOccupied).Count() <= LevelData.maxUnits);
        if (isPlayer && playerUnitPositions.Where(up => up.IsOccupied).Count() <= LevelData.maxUnits)
        {
            battleButton.interactable = true;
        }
        else
        {
            Debug.Log("Disable 1");
            battleButton.interactable = false;
        }
    }

    private void AddUnitToLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.FirstOrDefault(unitPosition => !unitPosition.IsOccupied);

        if (unitPosition != null)
        {
            int slot = Array.FindIndex(playerUnitPositions, up => !up.IsOccupied) + 1;
            unit.selected = true;
            unit.slot = slot;
            unitPosition.SetUnit(unit, true);
            unitPosition.OnUnitRemoved += RemoveUnitFromLineup;
            SocketConnection.Instance.SelectUnit(unit.id, GlobalUserData.Instance.User.id, slot);
            unitsContainer.SetUnitUIActiveById(unit.id, false);
            battleButton.interactable = playerUnitPositions.Where(up => up.IsOccupied).Count() <= LevelData.maxUnits;
        }
    }

    private void RemoveUnitFromLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.First(unitPosition => CompareUnitId(unitPosition, unit));
        unitPosition.OnUnitRemoved -= RemoveUnitFromLineup;
        unit.selected = false;
        unit.slot = null;
        SocketConnection.Instance.UnselectUnit(unit.id, GlobalUserData.Instance.User.id);
        unitsContainer.SetUnitUIActiveById(unit.id, true);
        Debug.Log(playerUnitPositions.Where(up => up.IsOccupied).Count());
        if (!playerUnitPositions.Any(unit => unit.IsOccupied) || playerUnitPositions.Where(up => up.IsOccupied).Count() > LevelData.maxUnits)
        {
            Debug.Log("Disable 2");
            battleButton.interactable = false;
        }
        else if (playerUnitPositions.Where(up => up.IsOccupied).Count() <= LevelData.maxUnits)
        {
            battleButton.interactable = true;
        }
    }

    private bool CompareUnitId(UnitPosition unitPosition, Unit unit)
    {
        Unit selectedUnit = unitPosition.GetSelectedUnit();
        if (selectedUnit != null)
        {
            return selectedUnit.id == unit.id;
        }
        return false;
    }

    public void Populate(Unit unit, GameObject unitItem)
    {
        UnitItemUI unitItemUI = unitItem.GetComponent<UnitItemUI>();
        unitItemUI.SetUpUnitItemUI(unit);
        Button unitItemButton = unitItem.GetComponent<Button>();
        if (unit.selected)
        {
            unitItemUI.SetSelectedChampionMark(true);
            unitItemButton.interactable = false;
        }
        else if (playerAvailableUnits.Where(unit => unit.selected).Count() >= LineUpUnitsUIContainer.numberOfPositions)
        {
            unitItemUI.SetLocked(true);
            unitItemButton.interactable = false;
        }
    }

    private void SetUpBattleButtonBehaviour()
    {
        battleButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Dictionary<string, int> userCurrencies = GlobalUserData.Instance.User.currencies;
            bool userCanAffordAttempt = LevelData.attempt_costs.All(cost => userCurrencies[cost.Key] >= cost.Value);
            if (userCanAffordAttempt)
            {
                DecrementAttemptCostsInHeader(userCurrencies);
                sceneNavigator.ChangeToScene("Battle");
            }
            else
            {
                insufficientCurrenciesPopup.SetActive(true);
            }
        });
    }

    private void InstantiateLevelAttemptCostsUI()
    {
        foreach (KeyValuePair<string, int> cost in LevelData.attempt_costs)
        {
            GameObject costUI = Instantiate(levelAttemptCostUIPrefab, levelAttemptCostsUIContainer.transform.parent);
            costUI.transform.SetParent(levelAttemptCostsUIContainer.transform, false);
            costUI.transform.SetSiblingIndex(0);

            costUI.GetComponent<Image>().sprite = GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == cost.Key).image;
            costUI.GetComponentInChildren<TextMeshProUGUI>().text = cost.Value.ToString();
        }
    }

    private void UpdateHeaderResources()
    {
        if (IsDungeonMode())
        {
            GlobalUserData.Instance.SetCurrencyAmount("Supplies", GlobalUserData.Instance.User.currencies["Supplies"]);
            gemsHeaderResourceUI.SetActive(false);
            suppliesHeaderResourceUI.SetActive(true);
        }
    }

    private void DecrementAttemptCostsInHeader(Dictionary<string, int> userCurrencies)
    {
        if (IsDungeonMode())
        {
            GlobalUserData.Instance.SetCurrencyAmount("Supplies", userCurrencies["Supplies"] - LevelData.attempt_costs["Supplies"]);
        }
    }

    private bool IsDungeonMode()
    {
        return LevelData.attempt_costs.ContainsKey("Supplies");
    }
}
