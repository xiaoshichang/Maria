
using System.Threading;
using Maria.Server.Core.Timer;
using Maria.Server.Log;
using Maria.Server.NativeInterface;

namespace Maria.Server.Application.Server.ServerBase
{
	public abstract partial class ServerBase
	{
		public virtual void Init()
		{
			NativeAPI.IOContext_Init();
			TimerManager.Init();
			_InitEntityManager();
			_InitTelnetNetwork();
			_InitInnerNetwork();
		}

		public void Run()
		{
			NativeAPI.IOContext_Run();
		}

		public virtual void UnInit()
		{
			_UnInitInnerNetwork();
			_UnInitEntityManager();
			TimerManager.UnInit();
			_UnInitTelnetNetwork();
			NativeAPI.IOContext_UnInit();
		}
		
		
	}
}

