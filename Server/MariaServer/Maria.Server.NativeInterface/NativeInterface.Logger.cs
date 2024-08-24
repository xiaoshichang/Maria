using System.Runtime.InteropServices;

namespace Maria.Server.NativeInterface
{
	public static partial class NativeAPI
	{
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
	}
}