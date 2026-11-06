using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Audio
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.AudioSource)]
    [ActionDescription("Check if the AudioSource is playing.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/AudioSource-isPlaying.html")]
    public sealed class AudioSourceCheckIsPlaying : BaseTrueFalseAction
    {
        [Tooltip("The AudioSource to check.")]
        [SerializeField]
        private AudioSourceVar _audioSource;

        public override bool CanExecute() => CheckParameters(_audioSource);

        protected override bool Test() => _audioSource.Value.isPlaying;

        protected override string TrueSummary => "{_audioSource} is playing";
        protected override string FalseSummary => "{_audioSource} is not playing";
    }
}
