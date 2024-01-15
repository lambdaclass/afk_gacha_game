using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcceptBehaviour : MonoBehaviour
{
    public List<Box> boxes;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private GachaManager gachaManager;

    public Box box;

    public void SetBoxByName(string boxName){
        box = boxes.Find(box => boxName == box.name);
        itemName.text = boxName;
    }

    public void PullBox(){
        gachaManager.RollCharacter(box);
        PlayBoxSummonSFX(box.summonSFX);
    }

    private void PlayBoxSummonSFX(AudioClip summonSFX){
        AudioSource audioSource = transform.parent.Find("SummonSFX").GetComponent<AudioSource>();
        audioSource.clip = summonSFX;
        audioSource.Play();
    }
}
