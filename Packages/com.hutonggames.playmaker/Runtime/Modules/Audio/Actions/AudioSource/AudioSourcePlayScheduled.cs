
using JetBrains.Annotations;
using System;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions.Audio
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.AudioSource)]
	[ConvertibleGroup("AudioPlay")]
	[ActionDescription("Plays the clip at a specific time on the absolute time-line that AudioSettings.ds" +
		"pTime reads from.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html")]
	public sealed class AudioSourcePlayScheduled : BaseAction
	{
		
		[Tooltip("The AudioSource.")]
		[SerializeField]
		private AudioSourceVar _audioSource;
		
		[Tooltip("Time in seconds on the absolute time-line that AudioSettings.dspTime refers to fo" +
			"r when the sound should start playing.")]
		[SerializeField]
		private DoubleVar _time;

		[Tooltip("Event to send when playback stops, including when the AudioSource is paused or stopped.")]
		[SerializeField, OptionalField]
		private EventRef _finishedEvent;

		[NonSerialized] private AudioSource _playingSource;
		[NonSerialized] private double _scheduledTime;
		[NonSerialized] private bool _hasPlayed;

		public override bool CanFinish => true;

		private bool WaitForPlayback => _finishedEvent is { IsSet: true } || IsBlocking;

		protected override bool IsFinished() => !WaitForPlayback;
		
		public override bool CanExecute()
		{
			return CheckParameters(_audioSource, _time);
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
				_scheduledTime = _time.Value;
				_playingSource.PlayScheduled(_scheduledTime);
				_hasPlayed = true;
				return;
			}

			if (AudioSettings.dspTime < _scheduledTime) return;

			if (!_playingSource || !_playingSource.isPlaying)
			{
				SendEvent(_finishedEvent);
				Finish();
			}
		}
		
		public override string GetSummary()
		{
			return "Play {_audioSource} at scheduled time {_time} {_finishedEvent}";
		}
	}
}
