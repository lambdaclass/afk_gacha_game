using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxUI : MonoBehaviour
{
    Box box;

    [SerializeField]
    Image icon;

    [SerializeField]
    TMP_Text title;

    [SerializeField]
    TMP_Text cost;

    [SerializeField]
    Button button;

    public void SetBox(Box box, Sprite boxSprite, Action<string, string> onClick)
    {
        this.box = box;
        title.text = this.box.name;
        icon.sprite = boxSprite;

        // Only shows the first cost, hardcoded
        KeyValuePair<string, int> firstCost = this.box.costs.First();
        cost.text = $"{firstCost.Key.ToString()}: {firstCost.Value}";

        button.onClick.AddListener(() => onClick.Invoke(GlobalUserData.Instance.User.id, box.id));
    }
}
