using System;
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
		public delegate void OnSessionAcceptHandler(IntPtr session);
		public delegate void OnSessionConnectedHandler(IntPtr session, int ec);
		public delegate void OnSessionDisconnectHandler(IntPtr session);
		public delegate void OnSessionReceiveHandler(IntPtr data, int length);
		public delegate void OnSessionSendHandler(int bufferCount);
		#endregion

		#region Struct
		
		public enum NetworkConnectionType : int
		{
			Tcp = 1,
			Kcp = 2,
		}

		public enum SessionMessageEncoderType : int
		{
			Header = 1,
			Delim = 2,
		}

		public const string Delimiter = "\r\n";

		[StructLayout(LayoutKind.Sequential, CharSet = _CharSet)]
		public struct NetworkInitInfo
		{
			public NetworkConnectionType ConnectionType;
			public SessionMessageEncoderType SessionEncoderType;
		}

		#endregion


		#region Interface

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_Init(string path, string fileName);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void Logger_UnInit();
	
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

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern IntPtr NetworkInstance_Init(NetworkInitInfo info, OnSessionAcceptHandler onConnected, OnSessionConnectedHandler onAccept, OnSessionDisconnectHandler onDisconnect);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkInstance_UnInit(IntPtr network);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkInstance_StartListen(IntPtr network, string ip, int port);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkInstance_StopListen(IntPtr network);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkInstance_ConnectTo(IntPtr network, string ip, int port);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkInstance_GetSessionCount(IntPtr network);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkSession_Bind(IntPtr session, OnSessionReceiveHandler onReceive, OnSessionSendHandler onSend);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkSession_Send(IntPtr session, IntPtr data, int length);
		
		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkSession_Stop(IntPtr session);

		[DllImport(_DllPath, CallingConvention = _CallingConvention, CharSet = _CharSet)]
		public static extern void NetworkSession_ConsumeReceiveBuffer(IntPtr session, int count);

		#endregion

	}
}

