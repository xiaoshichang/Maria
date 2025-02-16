using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Maria.Client.Foundation.Log;
using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	
	public partial class TcpNetworkSession : INetworkSession
	{
		public void Init(OnSessionReceiveMessageCallback onReceive, OnSessionDisconnectedCallback onDisconnect)
		{
			_OnReceiveCallback = onReceive;
			_OnDisconnectCallback = onDisconnect;
			_SessionState = SessionState.Ready;
		}

		public void UnInit()
		{
			_OnReceiveCallback = null;
			_OnDisconnectCallback = null;
		}
		
		public void Tick()
		{
			ProcessEvents();
		}

		/// <summary>
		/// process single network event in main thread.
		/// </summary>
		private void _ProcessSingleEvent(NetworkSessionEvent evt)
		{
			if (evt is NetworkSessionEventOnReceive onReceive)
			{
				_ProcessEventOnReceive(onReceive);
			}
			else if (evt is NetworkSessionEventOnConnected onConnected)
			{
				_ProcessEventOnConnected(onConnected);
			}
			else if (evt is NetworkSessionEventOnReceiveError onReceiveError)
			{
				_ProcessEventOnReceiveError(onReceiveError);
			}
		}
		
		public void ProcessEvents()
		{
			lock (_SessionEventQueue)
			{
				while (true)
				{
					NetworkSessionEvent evt;
					{
						if (_SessionEventQueue.Count <= 0)
						{
							return;
						}
						evt = _SessionEventQueue.Dequeue();
					}
					_ProcessSingleEvent(evt);
				}
			}
		}

		private SessionState _SessionState;
		private Socket _Socket;
		private readonly Queue<NetworkSessionEvent> _SessionEventQueue = new();
		
		private OnSessionDisconnectedCallback _OnDisconnectCallback;

	}
}