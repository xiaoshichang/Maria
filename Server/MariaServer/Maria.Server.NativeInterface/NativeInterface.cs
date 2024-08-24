using System.Runtime.InteropServices;

namespace Maria.Server.NativeInterface
{
	public static partial class NativeAPI
	{
		//https://blog.magnusmontin.net/2018/11/05/platform-conditional-compilation-in-net-core/
#if Linux
	private const string _DllPath = @"MariaServerNative.so";
#elif Windows
	private const string _DllPath = @"MariaServerNative.dll";
#endif
		
		private const CallingConvention _CallingConvention = CallingConvention.Winapi;
		private const CharSet _CharSet = System.Runtime.InteropServices.CharSet.Ansi;
	}
}

