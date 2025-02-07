using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

namespace Maria.Server.Core.Network
{
	public delegate void NetworkSessionReceiveMessageCallback(NetworkSession session, NetworkSessionMessage message);
	
	public class NetworkSession 
	{
		public NetworkSession(NativeAPI.SessionMessageEncoderType encoderType, IntPtr nativeSession, NetworkSessionReceiveMessageCallback onReceive)
		{
			_EncoderType = encoderType;
			_NativeSession = nativeSession;
			_OnReceiveMessage = onReceive;
			NativeAPI.NetworkSession_Bind(nativeSession, _OnReceive, _OnSend);
		}

		private void _SendHeaderMessage(NetworkSessionMessage message)
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

		private void _SendDelimMessage(NetworkSessionMessage message)
		{
			
			var telnetMessage = message as TelnetNetworkSessionMessage;
			if (telnetMessage == null || telnetMessage.Data == null)
			{
				Logger.Error("unknown error.");
				return;
			}
			unsafe
			{
				// if message contains delim, replace to newline.
				var processed = telnetMessage.Data.Replace(NativeAPI.Delimiter, "\n");
				// finally, append delim to the end of message and send out.
				var dataToSend = Encoding.ASCII.GetBytes(processed + NativeAPI.Delimiter);
				fixed (byte* ptr = dataToSend)
				{
					// hold dataToSend to avoid gc before async send operation finish.
					_SendQueue.Add(dataToSend);
					// do send operation
					NativeAPI.NetworkSession_Send(_NativeSession, new IntPtr(ptr), dataToSend.Length);
				}
			}
			
		}
		
		public void Send(NetworkSessionMessage message)
		{
			try
			{
				if (_EncoderType == NativeAPI.SessionMessageEncoderType.Header)
				{
					_SendHeaderMessage(message);
				}
				else if (_EncoderType == NativeAPI.SessionMessageEncoderType.Delim)
				{
					_SendDelimMessage(message);
				}
				else
				{
					throw new NotImplementedException();
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

		private void _OnReceiveHeaderMessage(IntPtr data, int length)
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

		private void _ReceiveDelimMessage(IntPtr data, int length)
		{
			unsafe
			{
				var nativePtr = (byte*)data.ToPointer();
				var stream = new UnmanagedMemoryStream(nativePtr, length, length, FileAccess.ReadWrite);
				var streamReader = new StreamReader(stream, Encoding.ASCII);
				var contents = streamReader.ReadToEnd();
				if (string.IsNullOrEmpty(contents))
				{
					return;
				}

				var commands = contents.Split(NativeAPI.Delimiter);
				if (commands.Length <= 1)
				{
					return;
				}

				var consumeBytes = 0;
				for (int i = 0; i < commands.Length - 1; i++)	// contents split into n parts means n-1 commands
				{
					var command = commands[i];
					consumeBytes += command.Length + NativeAPI.Delimiter.Length;
					var message = new TelnetNetworkSessionMessage()
					{
						Data = command
					};
					_OnReceiveMessage?.Invoke(this, message);
				}
				
				NativeAPI.NetworkSession_ConsumeReceiveBuffer(_NativeSession, consumeBytes);
				
				
			}
		}

		private void _OnReceive(IntPtr data, int length)
		{
			try
			{
				if (_EncoderType == NativeAPI.SessionMessageEncoderType.Header)
				{
					_OnReceiveHeaderMessage(data, length);
				}
				else if (_EncoderType == NativeAPI.SessionMessageEncoderType.Delim)
				{
					_ReceiveDelimMessage(data, length);
				}
				else
				{
					Logger.Error($"unknown _EncoderType {_EncoderType}");
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
		private readonly NativeAPI.SessionMessageEncoderType _EncoderType;
		private readonly List<object> _SendQueue = new List<object>();
		private readonly NetworkSessionReceiveMessageCallback? _OnReceiveMessage;

	}
}

