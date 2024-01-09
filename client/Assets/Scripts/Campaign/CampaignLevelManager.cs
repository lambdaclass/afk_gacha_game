using UnityEngine;
using System.Collections.Generic;

public class CampaignLevelManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayButton;

    public void LevelSelected() {
        PlayButton.SetActive(true);
    }
}
