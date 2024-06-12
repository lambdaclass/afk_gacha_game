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
            Debug.Log("Got dungeon upgrades: " + upgrades.Count);
        },
        (error) =>
        {
            Debug.LogError(error);
        });
    }

}
