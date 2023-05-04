using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a GameObject to have it play a background music when instanciated.
	/// Careful : only one background music will be played at a time.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Sound/PersistentBackgroundMusic")]
	public class PersistentBackgroundMusic : MMPersistentSingleton<PersistentBackgroundMusic>
	{
		/// the background music clip to use as persistent background music
		[Tooltip("the background music clip to use as persistent background music")]
		public AudioClip SoundClip;
		/// whether or not the music should loop
		[Tooltip("whether or not the music should loop")]
		public bool Loop = true;
        
		protected AudioSource _source;
		protected PersistentBackgroundMusic _otherBackgroundMusic;

		protected virtual void OnEnable()
		{
			_otherBackgroundMusic = (PersistentBackgroundMusic)FindObjectOfType(typeof(PersistentBackgroundMusic));
			if ((_otherBackgroundMusic != null) && (_otherBackgroundMusic != this) )
			{
				_otherBackgroundMusic.enabled = false;
			}
		}

		/// <summary>
		/// Gets the AudioSource associated to that GameObject, and asks the GameManager to play it.
		/// </summary>
		protected virtual void Start()
		{
			MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
			options.Loop = Loop;
			options.Location = Vector3.zero;
			options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Music;
			options.Persistent = true;
            
			MMSoundManagerSoundPlayEvent.Trigger(SoundClip, options);
		}
	}
}