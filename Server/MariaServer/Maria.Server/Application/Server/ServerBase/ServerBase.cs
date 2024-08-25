
using System.Threading;
using Maria.Server.Log;

namespace Maria.Server.Application.Server.ServerBase
{
	public abstract partial class ServerBase
	{
		public virtual void Init()
		{
			
		}

		public void Run()
		{
			while (true)
			{
				Thread.Sleep(2000);
				Logger.Info("run...");
			}
		}

		public virtual void UnInit()
		{
			
		}
		
	}
}

