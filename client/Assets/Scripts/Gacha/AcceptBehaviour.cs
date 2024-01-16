using TMPro;
using UnityEngine;

public class AcceptBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemName;

    private Box box;

    [SerializeField]
    private GachaManager gachaManager;

    public void SetBox(Box newBox){
        box = newBox;
        itemName.text = box.name;
    }

    public void Accept(){
        PullBox();
        GlobalUserData.Instance.User.SubstractCurrency(box.GetCost());
    }

    private void PullBox(){
        gachaManager.RollCharacter(box);
        PlayBoxSummonSFX(box.summonSFX);
    }

    private void PlayBoxSummonSFX(AudioClip summonSFX){
        AudioSource audioSource = transform.parent.Find("SummonSFX").GetComponent<AudioSource>();
        audioSource.clip = summonSFX;
        audioSource.Play();
    }
}
