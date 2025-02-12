using System.Collections.Generic;
using Maria.Server.Log;
using Maria.Server.NativeInterface;

namespace Maria.Server.Core.Timer
{
	public delegate void TimeoutCallback(object? param);
	
	public class TimerManager
	{
		internal class Timer
		{
			public Timer(uint delayMS, TimeoutCallback callback, object? param)
			{
				IsRepeat = false;
				_TimeoutCallback = callback;
				_Param = param;
				TimerID = NativeAPI.Timer_AddTimer(delayMS, _OnTimeout);
			}

			public Timer(uint delayMS, uint intervalMS, TimeoutCallback callback, object? param)
			{
				IsRepeat = true;
				_TimeoutCallback = callback;
				_Param = param;
				TimerID = NativeAPI.Timer_AddRepeatTimer(delayMS, intervalMS, _OnTimeout);
			}

			private void _OnTimeout()
			{
				_TimeoutCallback.Invoke(_Param);
				TimerManager.OnTimeout(this);
			}

			public bool Cancel()
			{
				return NativeAPI.Timer_CancelTimer(TimerID);
			}
			public readonly uint TimerID;
			public readonly bool IsRepeat;
			private readonly TimeoutCallback _TimeoutCallback;
			private readonly object? _Param;
		}

		public static void Init()
		{
			NativeAPI.TimerMgr_Init();
		}

		public static void UnInit()
		{
			NativeAPI.TimerMgr_UnInit();
		}
		
		public static uint AddTimer(uint delayMs, TimeoutCallback callback, object? param)
		{
			var timer = new Timer(delayMs, callback, param);
			_Timers[timer.TimerID] = timer;
			return timer.TimerID;
		}

		public static uint AddTimer(uint delayMs, TimeoutCallback callback)
		{
			return TimerManager.AddTimer(delayMs, callback, null);
		}

		public static uint AddRepeatTimer(uint delayMs, uint intervalMs, TimeoutCallback callback, object param)
		{
			var timer = new Timer(delayMs, intervalMs, callback, param);
			_Timers[timer.TimerID] = timer;
			return timer.TimerID;
		}
		

		public static bool CancelTimer(uint tid)
		{
			if (!_Timers.TryGetValue(tid, out var timer))
			{
				Logger.Warning($"timer({tid}) not exist");
				return false;
			}

			timer?.Cancel();
			_Timers.Remove(tid);
			return true;
		}

		internal static void OnTimeout(Timer timer)
		{
			if (!timer.IsRepeat)
			{
				_Timers.Remove(timer.TimerID);
			}
		}

		public static int GetTimersCount()
		{
			var internalTimersCount = NativeAPI.Timer_GetTimersCount();
			if (internalTimersCount != _Timers.Count)
			{
				Logger.Error("timers count not match");
			}
			return _Timers.Count;
		}

		private static readonly Dictionary<uint, Timer> _Timers = new();
	}
}

