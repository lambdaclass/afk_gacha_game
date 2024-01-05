using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    public Button audioToggleButton;
    public Image audioToggle;
    public Sprite audioOn;
    public Sprite audioOff;
    public static MusicManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.gameObject);
            DontDestroyOnLoad(audioToggleButton.gameObject);
            _audioSource = GetComponent<AudioSource>();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMusic()
    {
        if (_audioSource.isPlaying) 
        {
            _audioSource.Pause();
            audioToggle.sprite = audioOff;
            return;
        }
        _audioSource.UnPause();
        audioToggle.sprite = audioOn;
    }
}
