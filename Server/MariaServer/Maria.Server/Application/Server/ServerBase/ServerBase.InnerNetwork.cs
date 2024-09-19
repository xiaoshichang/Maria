using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.ServerBase;


public class GroupNodeHandShakeReq : NetworkSessionMessage
{
	public string? ServerName { get; set; }
	public int ServerID { get; set; }
}

public class GroupNodeHandShakeRsp : NetworkSessionMessage
{
	public string? ServerName { get; set; }
	public int ServerID { get; set; }
}

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
	
	private void _RegisterNetworkSessionMessageHandlers()
	{
		NetworkMessageHandlers.RegisterNetworkMessageHandler<GroupNodeHandShakeReq>(_OnGroupNodeHandShakeReq);
		NetworkMessageHandlers.RegisterNetworkMessageHandler<GroupNodeHandShakeRsp>(_OnGroupNodeHandShakeRsp);
	}
	
	private void _InitGroupNetwork()
	{
		Logger.Info("InitGroupNetwork...");
		_RegisterNetworkSessionMessagesByReflection();
		_RegisterNetworkSessionMessageHandlers();
		
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Header
		};
		_InnerNetwork.Init(initInfo, _OnSessionAccepted, _OnSessionConnected, _OnSessionDisconnected, _OnSessionReceiveMessage);
		_InnerNetwork.StartListen(Program.ServerConfig.InnerIp, Program.ServerConfig.InnerPort);
	}

	private void _UnInitGroupNetwork()
	{
		_InnerNetwork.UnInit();
	}

	private void _OnSessionAccepted(NetworkSession session)
	{
	}

	private void _OnSessionConnected(NetworkSession session)
	{
		var req = new GroupNodeHandShakeReq
		{
			ServerName = Program.ServerConfig.Name,
			ServerID = Program.ServerGroupConfig.GetIDByConfig(Program.ServerConfig)
		};
		session.Send(req);
	}

	private void _OnSessionDisconnected(NetworkSession session)
	{
		Logger.Info($"_OnSessionDisconnected");
	}

	private void _OnSessionReceiveMessage(NetworkSession session, NetworkSessionMessage message)
	{
		var handlers = NetworkMessageHandlers.GetHandlers(message.GetType());
		handlers?.Invoke(session, message);
	}

	private void _OnGroupNodeHandShakeReq(NetworkSession session, NetworkSessionMessage message)
	{
		var req = message as GroupNodeHandShakeReq;
		_RegisterGroupNodeSession(req.ServerID, session);
		var rsp = new GroupNodeHandShakeRsp()
		{
			ServerName = Program.ServerConfig.Name,
			ServerID = Program.ServerGroupConfig.GetIDByConfig(Program.ServerConfig)
		};
		session.Send(rsp);
	}

	private void _OnGroupNodeHandShakeRsp(NetworkSession session, NetworkSessionMessage message)
	{
		var rsp = message as GroupNodeHandShakeRsp;
		_RegisterGroupNodeSession(rsp.ServerID, session);
	}

	private void _RegisterGroupNodeSession(int serverID, NetworkSession session)
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
	}
	
	private 
	
	protected readonly NetworkInstance _InnerNetwork = new NetworkInstance();

	protected readonly Dictionary<int, NetworkSession> _AllSessions = new Dictionary<int, NetworkSession>();
	protected NetworkSession? _GMSession;
	protected readonly Dictionary<int, NetworkSession> _AllGameSessions = new Dictionary<int, NetworkSession>();
	protected readonly Dictionary<int, NetworkSession> _AllGateSessions = new Dictionary<int, NetworkSession>();
}