using System;

namespace Maria.Server.Core.Network
{
	public class NetworkSession
	{
		public NetworkSession(NetworkInstance networkInstance, IntPtr nativeSession)
		{
			_NetworkInstance = networkInstance;
			_NativeSession = nativeSession;
		}

		public void Send()
		{
			
		}

		private void OnReceive()
		{
			
		}
		
		private readonly NetworkInstance _NetworkInstance;
		private readonly IntPtr _NativeSession;
	}
}

