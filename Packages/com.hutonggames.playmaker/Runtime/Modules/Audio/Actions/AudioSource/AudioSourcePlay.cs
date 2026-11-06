using System;
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions.Audio
{
	
	
	[Serializable]
	[PublicAPI]
	[ActionCategory(Category.AudioSource)]
	[ConvertibleGroup("AudioPlay")]
	[ActionDescription("Plays the clip.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/AudioSource.Play.html")]
	public sealed class AudioSourcePlay : BaseAction
	{
		
		[Tooltip("The AudioSource.")]
		[SerializeField]
		private AudioSourceVar _audioSource;

		[Tooltip("Event to send when playback stops, including when the AudioSource is paused or stopped.")]
		[SerializeField, OptionalField]
		private EventRef _finishedEvent;

		[NonSerialized] private AudioSource _playingSource;
		[NonSerialized] private bool _hasPlayed;

		public override bool CanFinish => true;

		private bool WaitForPlayback => _finishedEvent is { IsSet: true } || IsBlocking;

		protected override bool IsFinished() => !WaitForPlayback;
		
		public override bool CanExecute()
		{
			return CheckParameters(_audioSource);
		}

		public override void OnStart()
		{
			_hasPlayed = false;
			_playingSource = null;
		}
		
		public override void Execute()
		{
			if (!_hasPlayed)
			{
				_playingSource = _audioSource.Value;
				_playingSource.Play();
				_hasPlayed = true;
				return;
			}

			if (!_playingSource || !_playingSource.isPlaying)
			{
				SendEvent(_finishedEvent);
				Finish();
			}
		}
		public override string GetSummary() => "Play {_audioSource} {_finishedEvent}";
	}
}
