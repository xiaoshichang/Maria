using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
	
	private void _InitGroupNetwork()
	{
		Logger.Info("InitGroupNetwork...");
		_RegisterNetworkSessionMessagesByReflection();
		
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Header
		};
		_GroupNetwork.Init(initInfo, _OnSessionAccepted, _OnSessionConnected, null, _OnSessionReceiveMessage);
		_GroupNetwork.StartListen(Program.ServerConfig.InnerIp, Program.ServerConfig.InnerPort);
	}

	private void _UnInitGroupNetwork()
	{
		_GroupNetwork.UnInit();
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

	private void _OnSessionReceiveMessage(NetworkSession session, NetworkSessionMessage message)
	{
		Logger.Info($"OnSessionReceiveMessage {message.GetType()}");
	}
	
	protected readonly NetworkInstance _GroupNetwork = new NetworkInstance();
}