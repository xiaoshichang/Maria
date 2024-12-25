using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Maria.Client.Foundation.Log;
using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	
	public partial class TcpSession : INetworkSession
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

		public void ConnectToAsync(string ip, int port, OnSessionConnectedCallback callback)
		{
			if (_SessionState != SessionState.Ready)
			{
				MLogger.Error("session state must be ready when ConnectToAsync called.");
				return;
			}


			_SessionState = SessionState.Connecting;
			IPAddress address = IPAddress.Parse(ip);
			IPEndPoint endPoint = new IPEndPoint(address, port);
			_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
			{
				Blocking = true
			};
			_Socket.BeginConnect(endPoint, _OnConnected, null);

		}

		private void _OnConnected(IAsyncResult ar)
		{
			try
			{
				_Socket.EndConnect(ar);
				var evt = new NetworkSessionEventOnConnected()
				{
					Result = SessionOnConnectedResult.OK
				};
				lock (_SessionEventQueue)
				{
					_SessionEventQueue.Enqueue(evt);
				}
			}
			catch (Exception e)
			{
				var evt = new NetworkSessionEventOnConnected()
				{
					Result = SessionOnConnectedResult.Fail,
					Message = e.Message
				};
				lock (_SessionEventQueue)
				{
					_SessionEventQueue.Enqueue(evt);
				}
			}
		}

		public bool IsConnected()
		{
			return _SessionState == SessionState.Connected;
		}

		public void Send(NetworkSessionMessage message)
		{
		}

		public void Tick()
		{
			ProcessEvents();
		}

		public void Disconnect()
		{
		}

		/// <summary>
		/// process single network event in main thread.
		/// </summary>
		private void _ProcessSingleEvent(NetworkSessionEvent evt)
		{
			
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
		
		private OnSessionReceiveMessageCallback _OnReceiveCallback;
		private OnSessionConnectedCallback _OnConnectedCallback;
		private OnSessionDisconnectedCallback _OnDisconnectCallback;

	}
}