using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;
using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	public partial class TcpNetworkSession : INetworkSession
	{
		
		/// <summary>
		/// Receive 线程逻辑入口
		/// </summary>
		private void _ReceivingLoop()
		{
			try
			{
				while (_ReceiveThreadQuitFlag)
				{
					_Receive();
					var bytes = NetworkSessionMessageSerializer.Deserialize(_ReceiveStream, out var message);
					if (message == null)
					{
						continue;
					}
					
					var evt = new NetworkSessionEventOnReceive()
					{
						Message = message
					};
					lock (_SessionEventQueue)
					{
						_SessionEventQueue.Enqueue(evt);
					}
				}
			}
			catch (Exception e)
			{
				var evt = new NetworkSessionEventOnReceiveError()
				{
					InternalException = e
				};
				lock (_SessionEventQueue)
				{
					_SessionEventQueue.Enqueue(evt);
				}
			}
		}
		
		/// <summary>
		/// Receive 线程从网络读取数据
		/// </summary>
		private void _Receive()
		{
			var transferred = _Socket.Receive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None);
			if (transferred == 0)
			{
				throw new EndOfStreamException();
			}
			_ReceiveStream.Write(_ReceiveBuffer, 0, transferred);
		}
		
		/// <summary>
		/// 处理一个收到的网络消息
		/// </summary>
		private void _ProcessEventOnReceive(NetworkSessionEventOnReceive evt)
		{
			// 有上层结构根据注册的handler处理对应的消息
			_OnReceiveCallback.Invoke(evt.Message);
		}

		
		/// <summary>
		/// 处理一个收报异常
		/// </summary>
		private void _ProcessEventOnReceiveError(NetworkSessionEventOnReceiveError evt)
		{
			Disconnect();
		}
		
		private OnSessionReceiveMessageCallback _OnReceiveCallback;
		private Thread _ReceiveThread;
		private bool _ReceiveThreadQuitFlag;
		private const int RECEIVE_BUFFER_SIZE = 1024;
		private readonly byte[] _ReceiveBuffer = new byte[RECEIVE_BUFFER_SIZE];
		private readonly MemoryStream _ReceiveStream = new MemoryStream();
	}
}