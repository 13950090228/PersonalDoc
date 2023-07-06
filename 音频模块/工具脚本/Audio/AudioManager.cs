using System.Collections.Generic;
using UnityEngine;
using System;

namespace Campfire.Sound {
    /// <summary>
    /// 音效管理类
    /// </summary>
    public class AudioManager {
        static AudioManager instance;

        public static AudioManager Instance => instance;

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SFXVolume { get; private set; }
        public float AMBVolume { get; private set; }

        public Action<uint> SoundEndOfEvent; // 音效播放结束事件


        // 音效播放载体
        private GameObject sound_2D;
        private GameObject soundPoolRoot;
        private List<SoundGameObject> soundGameObjectPool;   // 声音实体列表
        private List<AudioListenerModel> audioListenerModels;  // 声音监听者列表
        private bool isOpenMusic = false;
        private List<string> ambSoundLocks = new List<string>();

        public AudioManager() {
            if (instance == null) {
                instance = this;
            }
        }

        /// <summary>
        /// 初始化音频管理器(把入口的 this.transform 丢进去)
        /// </summary>
        public void AwakeInitAudioManager(Transform soundPoolRoot, Transform sound_2D) {
            soundGameObjectPool = new List<SoundGameObject>();
            audioListenerModels = new List<AudioListenerModel>();
            this.soundPoolRoot = soundPoolRoot.gameObject;
            this.sound_2D = sound_2D.gameObject;
            this.sound_2D.AddComponent<AkGameObj>();

            // AkSoundEngine.AddBasePath(System.IO.Path.Combine(UnityEngine.Application.dataPath, "Sound/Audio/GeneratedSoundBanks/HotUpdate"));
        }

        /// <summary>
        /// 设置热更路径
        /// </summary>
        public void AddBasePath(string path) {
            AkSoundEngine.AddBasePath(path);
        }

        /// <summary>
        /// 加载 InitBank (热更完成后需要把 Init Bank 卸载再重新加载)
        /// </summary>
        public void LoadInitBank() {
            AkBankManager.LoadInitBank();
        }

        /// <summary>
        /// 卸载 InitBank (热更完成后需要把 Init Bank 卸载再重新加载)
        /// </summary>
        public void UnLoadInitBank() {
            AkBankManager.UnloadInitBank();
        }

        /// <summary>
        /// 加载 SoundBank(同步)
        /// </summary>
        public void LoadBank(string bankSign) {
            AkBankManager.LoadBank(bankSign, false, false);
        }

        /// <summary>
        /// 加载 SoundBank(异步)
        /// </summary>
        public void LoadBankAsync(string bankSign) {
            AkBankManager.LoadBankAsync(bankSign);
        }

        /// <summary>
        /// 卸载 SoundBank
        /// </summary>
        public void UnLoadBank(string bankSign) {
            AkBankManager.UnloadBank(bankSign);
        }

        /// <summary>
        /// 设置主音量
        /// </summary>
        public void SetMasterVolume(float volume) {
            if (volume > 1) {
                MasterVolume = 1;
            } else if (volume < 0) {
                MasterVolume = 0;
            } else {
                MasterVolume = volume;
            }

            AkSoundEngine.SetRTPCValue("Volume_Bus", volume);
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public void SetMusicVolume(float volume) {
            if (volume > 1) {
                MusicVolume = 1;
            } else if (volume < 0) {
                MusicVolume = 0;
            } else {
                MusicVolume = volume;
            }

            AKRESULT aKRESULT = AkSoundEngine.SetRTPCValue("Volume_Music", volume);
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume) {
            if (volume > 1) {
                SFXVolume = 1;
            } else if (volume < 0) {
                SFXVolume = 0;
            } else {
                SFXVolume = volume;
            }

            AkSoundEngine.SetRTPCValue("Volume_SFX", volume);
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetAmbientVolume(float volume) {
            if (volume > 1) {
                SFXVolume = 1;
            } else if (volume < 0) {
                SFXVolume = 0;
            } else {
                SFXVolume = volume;
            }

            AkSoundEngine.SetRTPCValue("Volume_AMB", volume);
        }

        /// <summary>
        /// 初始化音乐
        /// </summary>
        public void InitMusic() {
            AudioManager.Instance.Play2DSound("Music_Play");
        }

        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void StopMusic() {
            AudioManager.Instance.Play2DSound("Music_Stop");
            isOpenMusic = false;
        }

        /// <summary>
        /// 切换音乐
        /// </summary>
        public void PlayMusic(string eventSign) {
            if (!isOpenMusic) {
                AudioManager.Instance.Play2DSound("Music_Play");
                isOpenMusic = true;
            }
            AkSoundEngine.PostEvent(eventSign, sound_2D);
        }

        /// <summary>
        /// 通过字符串播放 2D 音效
        /// </summary>
        public uint Play2DSound(string eventSign) {
            return AkSoundEngine.PostEvent(eventSign, sound_2D);
        }

        /// <summary>
        /// 通过 Id 播放 2D 音效
        /// </summary>
        public uint Play2DSound(uint eventId) {
            return AkSoundEngine.PostEvent(eventId, sound_2D);
        }

        /// <summary>
        /// 通过字符串播放音效
        /// </summary>
        public uint PlaySound(string eventSign) {
            SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == 0);

            if (soundGameObject == null) {
                soundGameObject = new SoundGameObject();
                soundGameObject.akGameObj = CreateAkGameObj();
                soundGameObjectPool.Add(soundGameObject);
            }

            soundGameObject.akGameObj.enabled = true;
            soundGameObject.playingID = AkSoundEngine.PostEvent(eventSign, soundGameObject.akGameObj.gameObject, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            soundGameObject.eventSign = eventSign;
            return soundGameObject.playingID;
        }

        /// <summary>
        /// 通过 Id 播放音效
        /// </summary>
        public uint PlaySound(uint eventId) {
            SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == 0);

            if (soundGameObject == null) {
                soundGameObject = new SoundGameObject();
                soundGameObject.akGameObj = CreateAkGameObj();
                soundGameObjectPool.Add(soundGameObject);
            }

            soundGameObject.akGameObj.enabled = true;
            soundGameObject.playingID = AkSoundEngine.PostEvent(eventId, soundGameObject.akGameObj.gameObject, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            return soundGameObject.playingID;
        }

        /// <summary>
        /// 播放音频，直接挂载在物体身上，不加入对象池
        /// </summary>
        public uint PlaySoundToGameObject(string eventSign, GameObject go) {
            uint playingID = AkSoundEngine.PostEvent(eventSign, go, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            return playingID;
        }

        /// <summary>
        /// 播放音频，直接挂载在物体身上，不加入对象池
        /// </summary>
        public uint PlaySoundToGameObject(uint eventId, GameObject go) {
            uint playingID = AkSoundEngine.PostEvent(eventId, go, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            return playingID;
        }

        /// <summary>
        /// 在指定位置播放音效
        /// </summary>
        public uint PlaySoundToPosition(uint eventId, Vector3 position) {
            SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == 0);

            if (soundGameObject == null) {
                soundGameObject = new SoundGameObject();
                soundGameObject.akGameObj = CreateAkGameObj();
                soundGameObjectPool.Add(soundGameObject);
            }

            soundGameObject.akGameObj.enabled = true;
            var obj = soundGameObject.akGameObj.gameObject;
            obj.transform.position = position;
            AkSoundEngine.SetObjectPosition(obj, obj.transform);
            soundGameObject.playingID = AkSoundEngine.PostEvent(eventId, soundGameObject.akGameObj.gameObject, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            return soundGameObject.playingID;
        }

        /// <summary>
        /// 在指定位置播放音效
        /// </summary>
        public uint PlaySoundToPosition(string eventSign, Vector3 position) {
            SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == 0);

            if (soundGameObject == null) {
                soundGameObject = new SoundGameObject();
                soundGameObject.akGameObj = CreateAkGameObj();
                soundGameObjectPool.Add(soundGameObject);
            }

            soundGameObject.akGameObj.enabled = true;
            var obj = soundGameObject.akGameObj.gameObject;
            obj.transform.position = position;
            AkSoundEngine.SetObjectPosition(obj, obj.transform);
            soundGameObject.playingID = AkSoundEngine.PostEvent(eventSign, soundGameObject.akGameObj.gameObject, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            soundGameObject.eventSign = eventSign;
            return soundGameObject.playingID;
        }

        /// <summary>
        /// 在指定位置播放音效
        /// </summary>
        public uint PlaySoundToPosition(string eventSign, Vector3 position, Quaternion rotation) {
            SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == 0);

            if (soundGameObject == null) {
                soundGameObject = new SoundGameObject();
                soundGameObject.akGameObj = CreateAkGameObj();
                soundGameObjectPool.Add(soundGameObject);
            }

            var obj = soundGameObject.akGameObj.gameObject;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            AkSoundEngine.SetObjectPosition(obj, obj.transform);
            soundGameObject.akGameObj.enabled = true;
            soundGameObject.playingID = AkSoundEngine.PostEvent(eventSign, soundGameObject.akGameObj.gameObject, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            soundGameObject.eventSign = eventSign;
            return soundGameObject.playingID;
        }

        /// <summary>
        /// 通过 playingID 停止音效播放
        /// </summary>
        public void StopSoundByPlayingID(uint playingID) {
            AkSoundEngine.StopPlayingID(playingID);
        }

        /// <summary>
        /// 通过 playingID 获取发声对象 transform 
        /// </summary>
        /// <param name="playingID"></param>
        public Transform GetSoungByPlayingID(uint playingID) {
            SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == playingID);
            return soundGameObject.akGameObj.transform;
        }

        /// <summary>
        /// 通过 playingID 获取 EventID(找不到返回0)
        /// </summary>
        /// <returns></returns>
        public uint GetEventIDFromPlayingID(uint playingID) {
            return AkSoundEngine.GetEventIDFromPlayingID(playingID);
        }

        /// <summary>
        /// 通过传入实体和事件标识发送音效事件
        /// </summary>
        /// <param name="eventSign"></param>
        public void SendAudioByGameObject(string eventSign, GameObject go) {
            AkGameObj akGameObj = go.GetComponent<AkGameObj>();
            if (akGameObj != null && eventSign != "") {
                AkSoundEngine.PostEvent(eventSign, go, (uint)AkCallbackType.AK_EndOfEvent, EventCallback, null);
            }
        }

        /// <summary>
        /// 设置监听者
        /// </summary>
        /// <param name="go"></param>
        /// <param name="listenerType"></param>
        public void AddAudioListener(GameObject go, AudioListenerType listenerType, short priority) {
            if (audioListenerModels.Find(model => model.instanceId == go.GetInstanceID()) == null) {
                AkAudioListener listener = go.AddComponent<AkAudioListener>();
                audioListenerModels.Add(new AudioListenerModel() {
                    instanceId = go.GetInstanceID(),
                    listenerType = listenerType,
                    listener = listener,
                    priority = priority
                });
                AutoAudioListenerSwitch();
            } else {
                Debug.LogError("重复添加相同对象");
            }
        }

        /// <summary>
        /// 主动切换声音监听者
        /// </summary>
        /// <param name="go"></param>
        public void AudioListenerSwitch(GameObject go) {
            AudioListenerModel model = audioListenerModels.Find(model => model.instanceId == go.GetInstanceID());
            if (model != null) {
                foreach (var item in audioListenerModels) {
                    if (item.instanceId == model.instanceId) {
                        item.listener.enabled = true;
                    } else {
                        item.listener.enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新切换声音监听者
        /// </summary>
        public void AutoAudioListenerSwitch() {
            AudioListenerModel model;
            model = audioListenerModels.Count != 0 ? audioListenerModels[0] : null;

            foreach (var item in audioListenerModels) {
                if (((int)item.listenerType > (int)model.listenerType)
                || ((int)item.listenerType == (int)model.listenerType && item.priority > model.priority)) {

                    model = item;
                }
            }

            foreach (var item in audioListenerModels) {
                if (item.instanceId == model.instanceId) {
                    item.listener.enabled = true;
                } else {
                    item.listener.enabled = false;
                }
            }
        }

        /// <summary>
        /// 添加通用动画事件音效触发组件
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public CommonSoundEventTrigger AddComponentCommonSoundEventTrigger(GameObject go) {
            return go.AddComponent<CommonSoundEventTrigger>();
        }

        /// <summary>
        /// 状态切换
        /// </summary>
        public void SetSwitch(string switchGroup, string switchState) {
            AKRESULT aKRESULT = AkSoundEngine.SetSwitch(switchGroup, switchState, sound_2D);
            if (aKRESULT != AKRESULT.AK_Success) {
                Debug.LogError("状态切换异常 异常代码:" + aKRESULT);
            }
        }

        /// <summary>
        /// 删除声音监听者
        /// </summary>
        public void RemoveAudioListener(GameObject go) {
            for (int i = 0; i < audioListenerModels.Count; i++) {
                if (audioListenerModels[i].instanceId == go.GetInstanceID()) {
                    audioListenerModels.RemoveAt(i);
                    AutoAudioListenerSwitch();
                    return;
                }
            }
        }

        /// <summary>
        /// 中止所有环境音
        /// </summary>
        public void StopAmbientSound() {
            Play2DSound("amb_Stop");
        }

        /// <summary>
        /// 停止所有声音
        /// </summary>
        public void StopAllSound() {
            AkSoundEngine.StopAll();
        }

        /// <summary>
        /// 中止所有音效
        /// </summary>
        public void StopAllSFX() {
            Play2DSound("SFX_Stop");
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void ClearSoundGameObjecttPool() {
            for (int i = 0; i < soundGameObjectPool.Count; i++) {
                GameObject.Destroy(soundGameObjectPool[i].akGameObj.gameObject);
            }
            soundGameObjectPool.Clear();
        }

        /// <summary>
        /// 添加环境音限制锁
        /// </summary>
        /// <param name="sign"></param>
        public void AddAmbSoundLock(string sign) {
            if (!ambSoundLocks.Contains(sign)) {
                ambSoundLocks.Add(sign);
                SetAmbientVolume(0);
            }
        }

        /// <summary>
        /// 移除环境音限制锁
        /// </summary>
        /// <param name="sign"></param>
        public void RemoveAmbSoundLock(string sign) {
            ambSoundLocks.Remove(sign);
            if (ambSoundLocks.Count == 0) {
                SetAmbientVolume(1);
            }
        }

        void ResetAmbSound() {
            ambSoundLocks.Clear();
            SetAmbientVolume(1);
        }

        /// <summary>
        /// 销毁整个音频管理器
        /// </summary>
        public void TearDown() {
            StopAllSound();
            ClearSoundGameObjecttPool();
            GameObject.Destroy(soundPoolRoot);
            audioListenerModels.Clear();
            ResetAmbSound();
            instance = null;
        }

        /// <summary>
        /// 音效播放回调
        /// </summary>
        private void EventCallback(object cookie, AkCallbackType type, AkCallbackInfo callbackInfo) {
            if (type == AkCallbackType.AK_EndOfEvent) {
                var info = callbackInfo as AkEventCallbackInfo;
                if (info != null) {
                    SoundGameObject soundGameObject = soundGameObjectPool.Find(obj => obj.playingID == info.playingID);
                    if (soundGameObject != null) {
                        SoundEndOfEvent?.Invoke(soundGameObject.playingID);
                        soundGameObject.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// 创建 AkGameObj 实体
        /// </summary>
        private AkGameObj CreateAkGameObj() {
            var go = new GameObject();
            go.name = "AkGameObj";
            go.transform.SetParent(soundPoolRoot.transform);
            return go.AddComponent<AkGameObj>();
        }

        internal class SoundGameObject {
            public uint playingID;
            public string eventSign;
            public AkGameObj akGameObj;

            public void Reset() {
                akGameObj.enabled = false;
                playingID = 0;
                eventSign = "";
            }

        }
    }

}
