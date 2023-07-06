using System;
using UnityEngine;

namespace Campfire.Sound {
    /// <summary>
    /// 通用动画事件音效触发
    /// </summary>
    public class CommonSoundEventTrigger : MonoBehaviour {
        public Action<EventSignToPlayingId> PlayingIdReturnEvent;
        public bool isTrigger = true;
        public void PlaySound2D(string eventSign) {
            if (isTrigger) {
                uint playingId = AudioManager.Instance.Play2DSound(eventSign);
                PlayingIdReturnEvent?.Invoke(new EventSignToPlayingId() { eventSign = eventSign, playingId = playingId });
            }
        }

        public void PlaySound3D(string eventSign) {
            if (isTrigger) {
                uint playingId = AudioManager.Instance.PlaySoundToGameObject(eventSign, this.gameObject);
                PlayingIdReturnEvent?.Invoke(new EventSignToPlayingId() { eventSign = eventSign, playingId = playingId });
            }
        }
    }
}