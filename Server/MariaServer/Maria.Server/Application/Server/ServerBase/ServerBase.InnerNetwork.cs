using System.Collections.Generic;
using System.Reflection;
using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

#pragma warning disable

namespace Maria.Server.Application.Server.ServerBase;


public abstract partial class ServerBase
{
	private void _RegisterNetworkSessionMessagesByReflection()
	{
		var assemblies = new List<Assembly>();
		assemblies.AddRange(Program.GameplayAssemblies);
		assemblies.Add(Assembly.GetExecutingAssembly());
		foreach (var assembly in assemblies)
		{
			NetworkSessionMessage.RegisterNetworkSessionMessageType(assembly);
		}
	}
	
	protected virtual void _RegisterNetworkSessionMessageHandlers()
	{
		NetworkMessageHandlers.RegisterNetworkMessageHandler<InnerSessionHandShakeReq>(_OnInnerSessionHandShakeReq);
		NetworkMessageHandlers.RegisterNetworkMessageHandler<InnerNodeHandShakeRsp>(_OnInnerSessionHandShakeRsp);
	}
	
	private void _InitInnerNetwork()
	{
		_RegisterNetworkSessionMessagesByReflection();
		_RegisterNetworkSessionMessageHandlers();
		
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Header
		};
		_InnerNetwork.Init(initInfo, _OnInnerSessionAccepted, _OnInnerSessionConnected, _OnInnerSessionDisconnected, _OnInnerSessionReceiveMessage);
		_InnerNetwork.Start(Program.ServerConfig.InnerIp, Program.ServerConfig.InnerPort);
		Logger.Info($"_InitInnerNetwork {initInfo.ConnectionType}...");
	}

	private void _UnInitInnerNetwork()
	{
		_InnerNetwork.UnInit();
	}

	private void _OnInnerSessionAccepted(NetworkSession session)
	{
	}

	private void _OnInnerSessionConnected(NetworkSession session)
	{
		var req = new InnerSessionHandShakeReq
		{
			ServerName = Program.ServerConfig.Name,
			ServerID = Program.ServerGroupConfig.GetIDByConfig(Program.ServerConfig)
		};
		session.Send(req);
	}

	private void _OnInnerSessionDisconnected(NetworkSession session)
	{
		Logger.Info($"_OnSessionDisconnected");
	}

	private void _OnInnerSessionReceiveMessage(NetworkSession session, NetworkSessionMessage message)
	{
		var handlers = NetworkMessageHandlers.GetHandlers(message.GetType());
		if (handlers == null)
		{
			Logger.Warning($"no handler found. {message}");
			return;
		}
		handlers.Invoke(session, message);
	}

	private void _OnInnerSessionHandShakeReq(NetworkSession session, NetworkSessionMessage message)
	{
		var req = message as InnerSessionHandShakeReq;
		_RegisterInnerSession(req.ServerID, session);
		var rsp = new InnerNodeHandShakeRsp()
		{
			ServerName = Program.ServerConfig.Name,
			ServerID = Program.ServerGroupConfig.GetIDByConfig(Program.ServerConfig)
		};
		session.Send(rsp);
	}

	private void _OnInnerSessionHandShakeRsp(NetworkSession session, NetworkSessionMessage message)
	{
		var rsp = message as InnerNodeHandShakeRsp;
		_RegisterInnerSession(rsp.ServerID, session);
	}

	private void _RegisterInnerSession(int serverID, NetworkSession session)
	{
		_AllSessions[serverID] = session;
		var config = Program.ServerGroupConfig.GetConfigByID(serverID);
		if (config is GMServerConfig)
		{
			_GMSession = session;
		}
		else if (config is GameServerConfig)
		{
			_AllGameSessions[serverID] = session;
		}
		else if (config is GateServerConfig)
		{
			_AllGateSessions[serverID] = session;
		}
		else
		{
			Logger.Error("unknown server type.");
		}
		_OnInnerNodeSessionRegister(serverID, session);
	}

	protected virtual void _OnInnerNodeSessionRegister(int serverID, NetworkSession session)
	{
	}

	public void SendToGMServer(NetworkSessionMessage message)
	{
		if (_GMSession == null)
		{
			Logger.Error("can not send message to gm.");
			return;
		}
		_GMSession.Send(message);
	}
	
	protected readonly NetworkInstance _InnerNetwork = new NetworkInstance();

	protected readonly Dictionary<int, NetworkSession> _AllSessions = new Dictionary<int, NetworkSession>();
	protected NetworkSession? _GMSession;
	protected readonly Dictionary<int, NetworkSession> _AllGameSessions = new Dictionary<int, NetworkSession>();
	protected readonly Dictionary<int, NetworkSession> _AllGateSessions = new Dictionary<int, NetworkSession>();
}