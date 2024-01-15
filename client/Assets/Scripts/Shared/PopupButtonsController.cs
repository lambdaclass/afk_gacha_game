using System.Collections;
using UnityEngine;

public class PopupButtonsController : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject popup;

    public void OnPopupButtonClick()
    {
        StartCoroutine(PlaySoundAndDeactivatePopup());
    }

    IEnumerator PlaySoundAndDeactivatePopup()
    {
        audioSource.Play();
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        popup.SetActive(false);
    }
}
