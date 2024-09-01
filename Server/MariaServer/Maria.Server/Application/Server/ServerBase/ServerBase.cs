
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

			var tid = NativeAPI.Timer_AddTimer(10000, () =>
			{
				Logger.Info("timeout");
			});
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

