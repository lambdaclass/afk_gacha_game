using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DungeonUpgradesManager : MonoBehaviour
{
    [SerializeField]
    GameObject upgradePrefab;
    [SerializeField]
    GameObject upgradesContainer;
    [SerializeField]
    ItemDetailPopup upgradeDetailPopup;


    void Start()
    {
        User user = GlobalUserData.Instance.User;

        SocketConnection.Instance.GetDungeonUpgrades(user.id, (upgrades) =>
        {
            foreach (var group in upgrades.GroupBy(upgrade => upgrade.template.name))
            {
                GameObject upgradeUIObject = Instantiate(upgradePrefab, upgradesContainer.transform);
                upgradeUIObject.GetComponent<DungeonUpgradeUI>().SetUpUpgrade(group.First(), group.Count());
                Button unitUpgradeButton = upgradeUIObject.GetComponent<Button>();
                unitUpgradeButton.onClick.AddListener(() => ShowUpgradeDetailPopup(group.First()));
            }
        },
        (error) =>
        {
            Debug.LogError(error);
        });
    }

}
