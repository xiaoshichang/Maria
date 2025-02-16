using System;
using System.Threading;
using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	public partial class TcpNetworkSession : INetworkSession
	{
		public void Send(NetworkSessionMessage message)
		{
		}

		private void _SendingLoop()
		{
			try
			{
				while (_SendThreadQuitFlag)
				{
				
				}
			}
			catch (Exception e)
			{
				var evt = new NetworkSessionEventOnSendError()
				{
					InternalException = e
				};
				lock (_SessionEventQueue)
				{
					_SessionEventQueue.Enqueue(evt);
				}
			}
		}
		
		private Thread _SendThread;
		private bool _SendThreadQuitFlag;
	}
}