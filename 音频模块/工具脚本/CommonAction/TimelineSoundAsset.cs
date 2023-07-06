using UnityEngine;
using UnityEngine.Playables;

namespace Campfire.Sound {
    public class TimelineSoundAsset : PlayableAsset {
        public string eventSign;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var loginCreateSound = ScriptPlayable<TimelineCreateSound>.Create(graph);
            loginCreateSound.GetBehaviour().eventSign = eventSign;
            return loginCreateSound;
        }
    }
}