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
			NativeAPI.NetworkSession_Bind(nativeSession, _OnReceive, _OnSend);
		}

		public void Send(NetworkSessionMessage message)
		{
			try
			{
				var memoryStream = NetworkSessionMessageSerializer.SerializeToStream(message);
				unsafe
				{
					var data = memoryStream.GetBuffer();
					fixed (byte* ptr = &data[0])
					{
						// keep memory available until aysnc send operation finish.
						_SendQueue.Add(memoryStream);
						NativeAPI.NetworkSession_Send(_NativeSession, new IntPtr(ptr), (int)memoryStream.Length);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
			
		}

		public void Stop()
		{
			NativeAPI.NetworkSession_Stop(_NativeSession);
		}

		private void _OnReceive(IntPtr data, int length)
		{
			try
			{
				unsafe
				{
					var nativePtr = (byte*)data.ToPointer();
					var stream = new UnmanagedMemoryStream(nativePtr, length, length, FileAccess.ReadWrite);
					var consumeBytes = NetworkSessionMessageSerializer.Deserialize(stream, out var message);
					if (consumeBytes > 0)
					{
						NativeAPI.NetworkSession_ConsumeReceiveBuffer(_NativeSession, consumeBytes);
					}
					else if (consumeBytes < 0)
					{
						Logger.Error("exception while receiving message.");
					}
					if (message != null)
					{
						_OnReceiveMessage?.Invoke(this, message);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
		}

		private void _OnSend(int bufferCount)
		{
			// remove from send queue and let gc work.
			_SendQueue.RemoveRange(0, bufferCount);
		}
		
		
		private readonly IntPtr _NativeSession;
		private readonly List<MemoryStream> _SendQueue = new List<MemoryStream>();
		private readonly NetworkSessionReceiveMessageCallback? _OnReceiveMessage;

	}
}

