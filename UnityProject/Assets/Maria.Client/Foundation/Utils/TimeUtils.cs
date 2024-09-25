using System;

namespace Maria.Client.Foundation.Utils
{
	public class TimeUtils
	{
		public static ulong GetTimeStampInSeconds()
		{
			var ts = DateTime.UtcNow - BaseTime;
			return (ulong)ts.TotalSeconds;
		}

		public static ulong GetTimeStampInMilliseconds()
		{
			var ts = DateTime.UtcNow - BaseTime;
			return (ulong)ts.TotalMilliseconds;
		}

		private static readonly DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
	}
}