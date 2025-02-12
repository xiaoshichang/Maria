
using System.Threading;
using Maria.Server.Log;
using Maria.Server.NativeInterface;

namespace Maria.Server.Application.Server.ServerBase
{
	public abstract partial class ServerBase
	{
		public virtual void Init()
		{
			_InitEntityManager();
			NativeAPI.IOContext_Init();
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
			_UnInitTelnetNetwork();
			NativeAPI.IOContext_UnInit();
			_UnInitEntityManager();
		}
		
		
	}
}

