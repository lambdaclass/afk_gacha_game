using UnityEngine;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private Button audioToggleButton;
    [SerializeField]
    private Image audioToggle;
    [SerializeField]
    private Sprite audioOn;
    [SerializeField]
    private Sprite audioOff;
    public static ToggleMusic Instance { get; private set; }
    private const string TAG_MUSIC = "Music";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GameObject.FindGameObjectWithTag(TAG_MUSIC).GetComponent<AudioSource>();
            DontDestroyOnLoad(audioSource);
            audioToggle.sprite = audioSource.isPlaying ? audioOn : audioOff;           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Toggle()
    {
        if (audioSource.isPlaying) 
        {
            audioSource.Pause();
            audioToggle.sprite = audioOff;
            return;
        }
        audioSource.UnPause();
        audioToggle.sprite = audioOn;
    }
}
