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
            // DontDestroyOnLoad(audioSource);
            audioToggle.sprite = AudioListener.volume > 0 ? audioOn : audioOff;           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Toggle()
    {
        if (AudioListener.volume == 1) 
        {
            AudioListener.volume = 0;
            audioToggle.sprite = audioOff;
            return;
        }
        AudioListener.volume = 1;
        audioToggle.sprite = audioOn;
    }
}
