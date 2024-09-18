using System;
using System.Collections.Generic;
using System.IO;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

namespace Maria.Server.Core.Network
{
	public delegate void NetworkSessionReceiveMessageCallback(NetworkSession session, NetworkSessionMessage message);
	
	public class NetworkSession 
	{
		public NetworkSession(IntPtr nativeSession, NetworkSessionReceiveMessageCallback onReceive)
		{
			_NativeSession = nativeSession;
			_OnReceiveMessage = onReceive;
			NativeAPI.NetworkSession_Bind(nativeSession, OnReceive);
		}

		public void Send(NetworkSessionMessage message)
		{
			var memoryStream = NetworkSessionMessageSerializer.SerializeToStream(message);
			if (memoryStream == null)
			{
				Logger.Error($"unable to serialize message. {message.GetType().Name}");
				return;
			}

			unsafe
			{
				var data = memoryStream.GetBuffer();
				fixed (byte* ptr = &data[0])
				{
					NativeAPI.NetworkSession_Send(_NativeSession, new IntPtr(ptr), (int)memoryStream.Length);
				}
			}
		}

		public void Stop()
		{
			NativeAPI.NetworkSession_Stop(_NativeSession);
		}

		private void OnReceive(IntPtr data, int length)
		{
			unsafe
			{
				var nativePtr = (byte*)data.ToPointer();
				var stream = new UnmanagedMemoryStream(nativePtr, length);
				var message = NetworkSessionMessageSerializer.Deserialize(stream);
				if (message == null)
				{
					throw new Exception("deserialize message fail.");
				}
				_OnReceiveMessage?.Invoke(this, message);
			}
		}
		
		
		private readonly IntPtr _NativeSession;
		private readonly NetworkSessionReceiveMessageCallback? _OnReceiveMessage;

	}
}

