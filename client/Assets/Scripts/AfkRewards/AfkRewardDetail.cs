using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AfkRewardDetail : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text quantity;

    public void SetData(Sprite image, string quantity)
    {
        this.image.sprite = image;
        this.quantity.text = quantity;
    }
}
