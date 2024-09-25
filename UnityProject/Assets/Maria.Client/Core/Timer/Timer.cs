using Maria.Client.Foundation.Utils;

#pragma warning disable

namespace Maria.Core.Timer
{
	public delegate void TimeoutCallback(object? arg);
	
	public class TimerInstance
	{
		public TimerInstance(bool isRepeat, ulong delayInMilliSeconds, TimeoutCallback callback, object? arg)
		{
			TimerID = NextTimerID();
			IsRepeat = isRepeat;
			_Delay = delayInMilliSeconds;
			NextTimeout = _Delay + TimeUtils.GetTimeStampInMilliseconds();
			_Arg = arg;
			_Callback = callback;
		}

		public bool IsTimeout(ulong timestamp)
		{
			return timestamp >= NextTimeout;
		}

		public void OnTimeout()
		{
			_Callback.Invoke(_Arg);
		}

		private static ulong NextTimerID()
		{
			GlobalTimerID += 1;
			if (GlobalTimerID == ulong.MaxValue)
			{
				GlobalTimerID = 1;
			}
			return GlobalTimerID;
		}

		public void Repeat()
		{
			NextTimeout += _Delay;
		}

		public readonly ulong TimerID;
		public bool IsRepeat { get; }
		public ulong NextTimeout;
		private ulong _Delay { get; }
		private readonly TimeoutCallback _Callback;
		private readonly object? _Arg;
		private static ulong GlobalTimerID = 0;
	}
}