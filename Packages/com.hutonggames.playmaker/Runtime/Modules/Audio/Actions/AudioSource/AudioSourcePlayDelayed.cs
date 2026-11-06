
using JetBrains.Annotations;
using System;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions.Audio
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.AudioSource)]
	[ConvertibleGroup("AudioPlay")]
	[ActionDescription("Plays the clip with a delay specified in seconds. Users are advised to use this f" +
		"unction instead of the old Play(delay) function that took a delay specified in s" +
		"amples relative to a reference rate of 44.1 kHz as an argument.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/AudioSource.PlayDelayed.html")]
	public sealed class AudioSourcePlayDelayed : BaseAction
	{
		
		[Tooltip("The AudioSource.")]
		[SerializeField]
		private AudioSourceVar _audioSource;
		
		[Tooltip("Delay time specified in seconds.")]
		[SerializeField]
		private FloatVar _delay;

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
			return CheckParameters(_audioSource, _delay);
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
				_playingSource.PlayDelayed(_delay.Value);
				_hasPlayed = true;
				return;
			}

			if (!_playingSource || !_playingSource.isPlaying)
			{
				SendEvent(_finishedEvent);
				Finish();
			}
		}
		
		public override string GetSummary()
		{
			return "Play {_audioSource} after {_delay} {_finishedEvent}";
		}
	}
}
