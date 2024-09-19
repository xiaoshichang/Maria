using System;
using System.Collections.Generic;
using Maria.Shared.Network;

namespace Maria.Server.Core.Network;


public delegate void NetworkMessageHandler(NetworkSession session, NetworkSessionMessage message);

public static class NetworkMessageHandlers
{
	public static void RegisterNetworkMessageHandler<T>(NetworkMessageHandler handler) where T : NetworkSessionMessage
	{
		var type = typeof(T);
		if (_Handlers.ContainsKey(type))
		{
			_Handlers[type] += handler;
		}
		else
		{
			_Handlers[type] = handler;
		}
	}

	public static NetworkMessageHandler? GetHandlers(Type type)
	{
		return _Handlers.GetValueOrDefault(type);
	}
	
	public static Dictionary<Type, NetworkMessageHandler> _Handlers = new();
}