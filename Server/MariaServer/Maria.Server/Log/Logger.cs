using System.Diagnostics;
using Maria.Server.NativeInterface;

namespace Maria.Server.Log
{
	public static class Logger
	{
		public static void Init(string target, string fileName)
		{
			NativeAPI.Logger_Init(target, fileName);
		}

		public static void Debug(string format, params object[] args)
		{
			NativeAPI.Logger_Debug(string.Format(format, args));
		}
	
		public static void Info(string format, params object[] args)
		{
			NativeAPI.Logger_Info(string.Format(format, args));
		}

		public static void Warning(string format, params object[] args)
		{
			NativeAPI.Logger_Warning(string.Format(format, args));
		}

		public static void Error(string format, params object[] args)
		{
			NativeAPI.Logger_Error(string.Format(format, args));
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				NativeAPI.Logger_Error(message);
			}
		}
	}
}

