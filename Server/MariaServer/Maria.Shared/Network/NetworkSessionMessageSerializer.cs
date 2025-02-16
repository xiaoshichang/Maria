using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Maria.Shared.Network
{
	[Serializable]
	public class SimpleMessage
	{
		public string? Data { get; set; }
	}
	
	public static class NetworkSessionMessageSerializer
	{
		public static string Test()
		{
			var message = new SimpleMessage
			{
				Data = "Hello"
			};

			var serialized = JsonConvert.SerializeObject(message);
			var deserialized = JsonConvert.DeserializeObject<SimpleMessage>(serialized);
			if (deserialized == null || deserialized.Data == null)
			{
				throw new Exception();
			}
			else
			{
				return deserialized.Data;
			}
		}
		
		public static MemoryStream SerializeToStream(NetworkSessionMessage message)
		{
			var type = message.GetType();
			var stream = new MemoryStream();
			var streamWriter = new StreamWriter(stream);
			
			// write json body
			stream.Seek(NetworkSessionMessage.HeaderLength, SeekOrigin.Begin);
			_Serializer.Serialize(streamWriter, message);
			streamWriter.Flush();
			var messageLength = (int)(stream.Length - NetworkSessionMessage.HeaderLength);
			
			stream.Seek(0, SeekOrigin.Begin);
			
			// write message length
			messageLength = IPAddress.HostToNetworkOrder(messageLength);
			stream.Write(BitConverter.GetBytes(messageLength));
			
			// write type id
			var tid = NetworkSessionMessage.GetTypeIDByType(type);
			tid = IPAddress.HostToNetworkOrder(tid);
			stream.Write(BitConverter.GetBytes(tid));
			return stream;
		}

		public static int Deserialize(Stream stream, out NetworkSessionMessage? message)
		{
			message = null;
			var streamTotalLength = stream.Length;
			if (streamTotalLength < NetworkSessionMessage.HeaderLength)
			{
				return 0; // not enough data to parse header
			}

			var streamReader = new StreamReader(stream, Encoding.UTF8);
			var buffer = new byte[NetworkSessionMessage.HeaderLength];
			
			// read message length
			var readBytes = stream.Read(buffer);
			if (readBytes != NetworkSessionMessage.HeaderLength)
			{
				throw new Exception($"readBytes should be {NetworkSessionMessage.HeaderLength}");
			}

			var messageLength = BitConverter.ToInt32(buffer, 0);
			messageLength = IPAddress.NetworkToHostOrder(messageLength);
			if (streamTotalLength - NetworkSessionMessage.HeaderLength < messageLength)
			{
				return 0; // not enough data to deserialize
			}
			
			// read type id
			var tid = BitConverter.ToInt32(buffer, 4);
			tid = IPAddress.NetworkToHostOrder(tid);
			var type = NetworkSessionMessage.GetTypeByTypeID(tid);

			// read json body
			stream.SetLength(messageLength + NetworkSessionMessage.HeaderLength);
			stream.Seek(NetworkSessionMessage.HeaderLength, SeekOrigin.Begin);
			var obj = _Serializer.Deserialize(streamReader, type);
			message = obj as NetworkSessionMessage;
			var consumeBytes = messageLength + NetworkSessionMessage.HeaderLength;
			return consumeBytes;
		}

		public static JsonSerializer _Serializer = new JsonSerializer();

	}
}