using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThemeSetter : MonoBehaviour
{
	[SerializeField]
	AudioClip audioClip;

    void Start()
    {
        MainThemeManager.Instance.Play(audioClip);
    }
}
