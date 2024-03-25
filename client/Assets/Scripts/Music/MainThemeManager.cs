using UnityEngine;
using UnityEngine.UI;

public class MainThemeManager : MonoBehaviour
{
    public static MainThemeManager Instance { get; private set; }

	[SerializeField]
	AudioSource audioSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);            
        }
        else
        {
            Destroy(gameObject);
        }
    }

	public void Play(AudioClip audioClip) {
		if(audioSource.clip != audioClip) {
			audioSource.clip = audioClip;
			audioSource.Play();
		}
	}
}
