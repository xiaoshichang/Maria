using System.Runtime.InteropServices;

namespace Maria.Server.NativeInterface
{
	public static partial class NativeAPI
	{

		#region Init

		//https://blog.magnusmontin.net/2018/11/05/platform-conditional-compilation-in-net-core/
#if Linux
	private const string _DllPath = @"MariaServerNative.so";
#elif Windows
		private const string _DllPath = @"MariaServerNative.dll";
#endif
		
		private const CallingConvention _CallingConvention = CallingConvention.Winapi;
		private const CharSet _CharSet = System.Runtime.InteropServices.CharSet.Ansi;

		#endregion

		#region Delegate

		public delegate void TimerCallback();

		#endregion


		#region Interface

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_Init(string path, string fileName);
	
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_Debug(string message);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_Info(string message);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_Warning(string message);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_Error(string message);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern uint Timer_AddTimer(uint delayInMS, TimerCallback callback);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern uint Timer_AddRepeatTimer(uint delayInMS, uint intervalInMS, TimerCallback callback);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern bool Timer_CancelTimer(uint timerID);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern uint Timer_GetTimersCount();

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void IOContext_Init();
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void IOContext_Run();
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void IOContext_UnInit();

		#endregion
		
	}
}

