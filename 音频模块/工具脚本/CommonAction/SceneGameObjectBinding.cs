using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Campfire.Sound {
    public class SceneGameObjectBinding : MonoBehaviour {
        public string bindingSign;
        public bool isTrigger = true;
        public List<SceneGameObjectModel> list;

        void Awake() {
            if (isTrigger) {
                Init();
            }
        }
        public void Init() {
            isTrigger = true;
            foreach (var item in list) {
                if (item.go != null && !string.IsNullOrEmpty(item.eventSign)) {
                    item.go.isStatic = item.isStatic;
                    item.go.AddComponent<GameobjectAutoEventTrigger>().eventSign = item.eventSign;
                    CreateAKComponentByType(item.go, item.soundType);
                }
            }
        }

        void CreateAKComponentByType(GameObject go, SoundType soundType) {
            if (soundType == SoundType.Ambient_Simple) {
                AkAmbient akAmbient = go.AddComponent<AkAmbient>();
                akAmbient.multiPositionTypeLabel = MultiPositionTypeLabel.Simple_Mode;
            } else if (soundType == SoundType.Ambient_Large) {

            }
        }

        void PlaySound3D(string eventSign, GameObject go) {
            AudioManager.Instance.PlaySoundToGameObject(eventSign, go);
        }

        public void StopAllSound() {
            foreach (var item in list) {
                if (item.go != null && !string.IsNullOrEmpty(item.eventSign)) {
                    item.go.GetComponent<GameobjectAutoEventTrigger>()?.StopSound();
                }
            }
            isTrigger = false;
        }

        void OnDestroy() {
            foreach (var item in list) {
                if (item.go != null && !string.IsNullOrEmpty(item.eventSign)) {
                    item.go.GetComponent<GameobjectAutoEventTrigger>()?.StopSound();
                }
            }
        }
    }

}
