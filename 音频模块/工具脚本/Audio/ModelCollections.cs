using UnityEngine;

namespace Campfire.Sound {
    [System.Serializable]
    public class SceneGameObjectModel {
        public GameObject go;
        public string eventSign;
        public SoundType soundType;
        public bool isStatic = true;
    }

    public struct EventSignToPlayingId {
        public string eventSign;
        public uint playingId;
    }

    public class AudioListenerModel {
        public int instanceId;
        public AudioListenerType listenerType;
        public AkAudioListener listener;
        public short priority;
    }

}
