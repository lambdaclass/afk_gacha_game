using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesListItem : MonoBehaviour
{
    [SerializeField]
    GameObject idContainer;

    public void setId(string id)
    {
        idContainer.GetComponent<Text>().text = id;
    }
}
