using System;

namespace Util
{
	public class Timer
	{
		private bool _didStart = false;
		private float _seconds;

		public void Start(float atSeconds = 0f)
		{
			_seconds = atSeconds;
			_didStart = true;
		}

		public float Seconds { get { return _seconds; } }
		public long TimeInMilliSeconds { get { return (long)(_seconds * 1000f); } }

		public void TakeTime(float seconds)
		{
			if (!_didStart) return;

			_seconds += seconds;
		}
	}
}

