using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Maria.Client.Foundation.Log;

namespace Maria.Client.Core.Network
{
	public partial class TcpNetworkSession
	{
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
					Result = SessionOnConnectedResult.Success
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
		
		/// <summary>
		/// 只能在主线程调用
		/// </summary>
		private void _ProcessEventOnConnected(NetworkSessionEventOnConnected evt)
		{
			_OnConnectedCallback.Invoke(evt.Result, this);
			_OnConnectedCallback = null;

			if (evt.Result == SessionOnConnectedResult.Success)
			{
				_SessionState = SessionState.Connected;
				
				_SendThread = new Thread(_SendingLoop)
				{
					Name = "SendThread"
				};
				_SendThread.Start();
				
				_ReceiveThread = new Thread(_ReceivingLoop)
				{
					Name = "ReceiveThread"
				};
				_ReceiveThread.Start();
			}
			else
			{
				_SessionState = SessionState.Ready;
			}
		}

		public bool IsConnected()
		{
			return _SessionState == SessionState.Connected;
		}
		
		public void Disconnect()
		{
		}
		
		private OnSessionConnectedCallback _OnConnectedCallback;

	}
}