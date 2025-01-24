using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.ServerBase;

public class TelnetNetworkSessionMessage : NetworkSessionMessage
{
	public string? Command;
}

public abstract partial class ServerBase
{
	private void _InitTelnetNetwork()
	{
		Logger.Info("InitTelnetNetwork...");
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Delim
		};
		_TelnetNetwork.Init(initInfo, _OnTelnetSessionAccepted, null, _OnTelnetSessionDisconnected, _OnTelnetSessionReceiveMessage);
		_TelnetNetwork.StartListen("127.0.0.1", Program.ServerConfig.TelnetPort);
		
	}
	
	private void _OnTelnetSessionAccepted(NetworkSession session)
	{
	}

	private void _OnTelnetSessionDisconnected(NetworkSession session)
	{
	}

	private void _OnTelnetSessionReceiveMessage(NetworkSession session, NetworkSessionMessage message)
	{
	}

	private void _UnInitTelnetNetwork()
	{
		_TelnetNetwork.UnInit();
	}

	protected readonly NetworkInstance _TelnetNetwork = new NetworkInstance();
	
}