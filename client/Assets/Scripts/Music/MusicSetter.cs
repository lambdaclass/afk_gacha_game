using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSetter : MonoBehaviour
{
	[SerializeField]
	AudioClip audioClip;

    void Start()
    {
        MusicManager.Instance.Play(audioClip);
    }
}
