using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsUIContainer : MonoBehaviour
{
    [SerializeField] protected GameObject rewardItemUIPrefab;

    protected Dictionary<string, GameObject> rewardUIItemDictionary = new Dictionary<string, GameObject>();

    public void Populate(List<UIReward> rewards)
    {
        gameObject.SetActive(false);
        Clear();
        rewards.ForEach(reward =>
        {
            GameObject rewardUIItem = Instantiate(rewardItemUIPrefab, gameObject.transform);
            rewardUIItem.GetComponent<Image>().sprite = reward.Sprite();
            rewardUIItem.GetComponentInChildren<TMP_Text>().text = reward.Amount().ToString();
            rewardUIItemDictionary.Add(reward.GetName(), rewardUIItem);
        });
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        rewardUIItemDictionary.Clear();
    }
}
