
using System.Threading;
using Maria.Server.Log;
using Maria.Server.NativeInterface;

namespace Maria.Server.Application.Server.ServerBase
{
	public abstract partial class ServerBase
	{
		public virtual void Init()
		{
			NativeAPI.IOContext_Init();
			InitTelnetNetwork();
			InitGroupNetwork();
		}

		public void Run()
		{
			NativeAPI.IOContext_Run();
		}

		public virtual void UnInit()
		{
			UnInitGroupNetwork();
			UnInitTelnetNetwork();
			NativeAPI.IOContext_UnInit();
		}
		
		
	}
}

