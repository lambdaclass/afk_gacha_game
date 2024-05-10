using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public void ChangeToScene(string sceneName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // This is a workaround to avoid killing the sound effect when changing scenes.
    public void ChangeToSceneWithDelay(string sceneName) {
        StartCoroutine(ChangeToSceneAfterSeconds(sceneName, 0.1f));
    }

    public IEnumerator ChangeToSceneAfterSeconds(string sceneName, float seconds) {
        yield return new WaitForSeconds(seconds);
        ChangeToScene(sceneName);
    }

    // public IEnumerator ChangeToSupercampaignMap(string supercampaignName)
    // {
    //     SupercampaignsMapManager.selectedSuperCampaignData = GlobalUserData.Instance.User.supercampaignsProgresses.Find(s => s.superCampaignId == supercampaignName).superCampaign;
    //     StartCoroutine(ChangeToSceneAfterSeconds("SupercampaignsMap", 0.1f));
    // }
}
