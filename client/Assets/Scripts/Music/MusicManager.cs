using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    public static MusicManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
