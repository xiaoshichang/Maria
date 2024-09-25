using System;
using System.Collections.Generic;
using Maria.Core.Timer;

#pragma warning disable

namespace Maria.Core.Timer
{
	public class TimerCompare : IComparer<TimerInstance>
	{
		public int Compare(TimerInstance? x, TimerInstance? y)
		{
			if (x == null || y == null)
			{
				throw new NullReferenceException();
			}
			return x.NextTimeout.CompareTo(y.NextTimeout);
		}
	}
}