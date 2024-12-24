using System;
using System.Collections.Generic;

#pragma warning disable

namespace Maria.Client.Core.Timer
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