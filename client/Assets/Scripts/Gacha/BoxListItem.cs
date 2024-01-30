using System.Collections.Generic;
using UnityEngine;

public class BoxListItem : MonoBehaviour
{
    [SerializeField] Box box;
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] GameObject rejectPopUp;

    public void SelectBox()
    {
        if (GlobalUserData.Instance.User.CanAfford(box.GetCost())) {
            confirmPopUp.GetComponent<GachaAcceptBehaviour>().SetBox(box);
            confirmPopUp.SetActive(true);
        }
        else {
            rejectPopUp.SetActive(true);
        }
    }
}
