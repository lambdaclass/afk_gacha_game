using System.Collections;
using UnityEngine;

public class PopupButtonsController : MonoBehaviour
{
    public AudioSource audioSource;

    public void OnPopupButtonClick()
    {
        StartCoroutine(PlaySoundAndDeactivatePopup());
    }

    IEnumerator PlaySoundAndDeactivatePopup()
    {
        audioSource.Play();
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        gameObject.SetActive(false);
    }
}
