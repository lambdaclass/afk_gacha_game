using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitItemUI : MonoBehaviour {
    [SerializeField]
    private Image border;
    [SerializeField]
    private Image champion;
    [SerializeField]
    private Image faction;
    [SerializeField]
    private Image selectedChampionMark;
    [SerializeField]
    private Image lockedOverlay;
    [SerializeField]
    private TextMeshProUGUI level;

    public void SetUpUnitItemUI(Unit unit) {
        champion.sprite = unit.character.defaultSprite;
        faction.sprite = Resources.Load<Sprite>("Factions/" + unit.character.faction.ToString().ToLower());
        border.sprite = Resources.Load<Sprite>("Ranks/" + unit.rank.ToString().ToLower());
        level.text = "LVL " + unit.level.ToString();
    }

    public void SetSelectedChampionMark(bool selected) {
        selectedChampionMark.gameObject.SetActive(selected);
        GetComponent<Button>().interactable = !selected;
    }

    public bool IsSelected() {
        return selectedChampionMark.gameObject.activeSelf;
    }

    public void SetLocked(bool locked) {
        lockedOverlay.gameObject.SetActive(locked);
        if (locked || IsSelected()) {
            GetComponent<Button>().interactable = false;
        } else {
            GetComponent<Button>().interactable = true;
        }
    }

    public string GetUnitFaction() {
        return faction.sprite.name;
    }
}
