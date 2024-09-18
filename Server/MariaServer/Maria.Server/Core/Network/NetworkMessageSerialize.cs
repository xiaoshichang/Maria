using System;
using System.IO;
using System.Text.Json;
using Maria.Server.Log;
using Maria.Shared.Network;

namespace Maria.Server.Core.Network;


public class NetworkSessionMessageSerializer
{
	public static MemoryStream? SerializeToStream(NetworkSessionMessage message)
	{
		var stream = new MemoryStream();
		
		try
		{
			var tid = NetworkSessionMessage.GetTypeIDByType(message.GetType());
			stream.Write(BitConverter.GetBytes(tid));
			JsonSerializer.Serialize(stream, message);
			return stream;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public static NetworkSessionMessage? Deserialize(Stream stream)
	{
		try
		{
			var bytes = new byte[4];
			var read = stream.Read(bytes, 0, 4);
			Logger.Assert(read == 4, "read != 4");
			var tid = BitConverter.ToInt32(bytes);
			var typeInfo = NetworkSessionMessage.GetTypeByTypeID(tid);
			return JsonSerializer.Deserialize(stream, typeInfo) as NetworkSessionMessage;
		}
		catch
		{
			return null;
		}
	}
	
}