using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.GateServer;

public partial class GateServer
{
	private void _InitClientNetwork()
	{
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Header
		};
		_ClientNetwork.Init(initInfo, 
			_OnClientSessionAccepted, 
			_OnClientSessionConnected, 
			_OnClientSessionDisconnected, 
			_OnClientSessionReceiveMessage);
		
		Logger.Info($"_InitClientNetwork {initInfo.ConnectionType}");
	}

	private void _UnInitClientNetwork()
	{
		
	}

	private void _OpenClientNetwork()
	{
		var gateConfig = Program.ServerConfig as GateServerConfig;
		if (gateConfig == null)
		{
			Logger.Error("_OpenClientNetwork unknown error.");
			return;
		}
		_ClientNetwork.Start(gateConfig.OuterIp, gateConfig.OuterPort);
	}

	private void _CloseClientNetwork()
	{
		_ClientNetwork.Stop();
	}

	private void _OnClientSessionAccepted(NetworkSession session)
	{
		
	}

	private void _OnClientSessionConnected(NetworkSession session)
	{
		
	}

	private void _OnClientSessionReceiveMessage(NetworkSession session, NetworkSessionMessage message)
	{
		
	}

	private void _OnClientSessionDisconnected(NetworkSession session)
	{
		
	}
	
	
	
	protected readonly NetworkInstance _ClientNetwork = new();
}