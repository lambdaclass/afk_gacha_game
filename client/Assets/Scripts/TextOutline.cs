using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextOutline : MonoBehaviour
{
    void Awake()
    {
        TMP_Text textmeshPro = GetComponent<TMP_Text>();
        textmeshPro.outlineWidth = 0.1f;
        textmeshPro.outlineColor = new Color32(0, 0, 0, 255);
    }
}
