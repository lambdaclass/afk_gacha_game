using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static GameObject campaingReference;

    void Start(){
        Instantiate(campaingReference,transform);
    }
}
