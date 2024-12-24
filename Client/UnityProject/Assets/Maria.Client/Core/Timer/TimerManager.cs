using System;
using System.Collections.Generic;
using Maria.Client.Foundation.Log;
using Maria.Client.Foundation.Utils;

#pragma warning disable

namespace Maria.Client.Core.Timer
{

	public static class TimerManager
	{
		public static void Init()
		{
			MLogger.Info("Init TimerManager Ok!");
		}

		public static void UnInit()
		{
			_Timers.Clear();
			_Tid2Timer.Clear();
			MLogger.Info("UnInit TimerManager Ok!");
		}

		public static void Update()
		{
			var now = TimeUtils.GetTimeStampInMilliseconds();
			while (true)
			{
				var timer = _Timers.Min;
				if (timer == null)
				{
					return;
				}
				
				if (!timer.IsTimeout(now))
				{
					break;
				}
				_Timers.Remove(timer);
				timer.OnTimeout();
				if (timer.IsRepeat)
				{
					timer.Repeat();
					_Timers.Add(timer);
				}
			}
		}

		public static void UnregisterTimer(ulong tid)
		{
			if (_Tid2Timer.Remove(tid, out var timer))
			{
				_Timers.Remove(timer);
			}
			else
			{
				MLogger.Warning($"TimerID:{tid} does not exist.");
			}
		}

		public static ulong RegisterTimer(bool repeat, ulong delayInMilliSeconds, TimeoutCallback callback, object? arg)
		{
			var timer = new TimerInstance(repeat, delayInMilliSeconds, callback, arg);
			_Timers.Add(timer);
			_Tid2Timer.Add(timer.TimerID, timer);
			return timer.TimerID;
		}

		public static ulong RegisterTimer(ulong delayInMilliSeconds, TimeoutCallback callback)
		{
			return RegisterTimer(false, delayInMilliSeconds, callback, null);
		}

		public static ulong RegisterTimer(ulong delayInMilliSeconds, TimeoutCallback callback, object? arg)
		{
			return RegisterTimer(false, delayInMilliSeconds, callback, arg);
		}
		
		private static readonly SortedSet<TimerInstance> _Timers = new SortedSet<TimerInstance>(new TimerCompare());
		private static readonly Dictionary<ulong, TimerInstance> _Tid2Timer = new Dictionary<ulong, TimerInstance>();
	}
}