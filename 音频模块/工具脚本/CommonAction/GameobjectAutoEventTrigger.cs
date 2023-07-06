using UnityEngine;
using System.Collections.Generic;
namespace Campfire.Sound {

    public class GameobjectAutoEventTrigger : MonoBehaviour {
        public string startEventSign;   // 启动音效 不能是循环
        public string eventSign;        // 可以是循环音效
        public string endEventSign;     // 结束音效 不能是循环
        public float delayTime = 0;
        public bool isStatic = true;
        float count;
        List<uint> playingIds;

        void Awake() {
            playingIds = new List<uint>();
        }

        void Start() {
            this.gameObject.isStatic = isStatic;

            if (AudioManager.Instance != null) {
                if (!string.IsNullOrEmpty(startEventSign)) {
                    playingIds.Add(AudioManager.Instance.PlaySoundToGameObject(startEventSign, this.gameObject));
                }
            }
        }

        void Update() {
            if (count >= 0) {
                count += Time.deltaTime;
                if (count >= delayTime) {
                    if (AudioManager.Instance != null) {
                        if (!string.IsNullOrEmpty(eventSign)) {
                            playingIds.Add(AudioManager.Instance.PlaySoundToGameObject(eventSign, this.gameObject));
                        }
                        count = -1;
                    }
                }
            }

        }

        void OnDestroy() {
            if (!string.IsNullOrEmpty(endEventSign)) {
                AudioManager.Instance?.PlaySoundToGameObject(endEventSign, this.gameObject);
            }

            for (int i = 0; i < playingIds.Count; i++) {
                AudioManager.Instance?.StopSoundByPlayingID(playingIds[i]);
            }
        }

        void OnDisable() {
            if (!string.IsNullOrEmpty(endEventSign)) {
                AudioManager.Instance?.PlaySoundToGameObject(endEventSign, this.gameObject);
            }

            ClearPlayingIds();
        }

        public void StopSound() {
            ClearPlayingIds();
        }

        void ClearPlayingIds() {
            for (int i = 0; i < playingIds.Count; i++) {
                AudioManager.Instance?.StopSoundByPlayingID(playingIds[i]);
            }
            playingIds.Clear();
        }
    }
}
