using UnityEngine;
using UnityEngine.Playables;
namespace Campfire.Sound {
    public class TimelineCreateSound : PlayableBehaviour {
        public string eventSign;

        public override void OnBehaviourPlay(Playable playable, FrameData info) {
            PlaySound();
        }

        public void PlaySound() {
            Campfire.Sound.AudioManager.Instance?.Play2DSound(eventSign);
        }
    }

}
