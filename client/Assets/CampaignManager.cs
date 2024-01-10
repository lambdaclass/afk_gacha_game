using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static GameObject campaingReference;

    void Start(){
        GameObject instantiatedObject = Instantiate(campaingReference,transform);
        instantiatedObject.transform.SetSiblingIndex(0);
    }
}
