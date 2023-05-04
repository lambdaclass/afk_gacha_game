using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	[AddComponentMenu("TopDown Engine/GUI/SfxSwitch")]
	public class SfxSwitch : TopDownMonoBehaviour
	{
		public virtual void On()
		{
			MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.UnmuteTrack, MMSoundManager.MMSoundManagerTracks.Sfx);
		}

		public virtual void Off()
		{
			MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.MuteTrack, MMSoundManager.MMSoundManagerTracks.Sfx);
		}
	}
}