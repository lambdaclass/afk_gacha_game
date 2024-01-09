using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Level : MonoBehaviour
{
    [SerializeField]
    List<Character> characters;
    
    [SerializeField]
    List<int> levels;

    [SerializeField]
    Level nextLevel;

    [SerializeField]
    GameObject lockObject;

    private void Start(){
        if (UnlockedLevelsData.Instance.IsLevelUnlocked(name)) {
            lockObject.SetActive(false);
        }
    }

    public void SelectLevel(){
        if(lockObject.activeSelf) {
            return;
        }

        // Get the CampaignLevelManager parent and make it pop up the Battle button
        Transform parent = transform.parent;
        if (parent.TryGetComponent<CampaignLevelManager>(out CampaignLevelManager campaignLevelManager)) { campaignLevelManager.LevelSelected(); }
        else { Debug.LogError("Level has no CampaignLevelManager parent."); }
        
        SetUnits();
        SetLevelToUnlock();
    }

    private void SetUnits() {
        OpponentData opponentData = OpponentData.Instance;
        List<Unit> units = new List<Unit>();
        for(int i = 0; i < characters.Count; i++) {
            units.Add(new Unit { id = "op-" + i.ToString(), level = levels[i], character = characters[i], slot = i, selected = true });
        }
        opponentData.Units = units;
    }

    private void SetLevelToUnlock() {
        if(nextLevel != null) { UnlockedLevelsData.Instance.LevelToUnlockName = nextLevel.name; }
    }
}
