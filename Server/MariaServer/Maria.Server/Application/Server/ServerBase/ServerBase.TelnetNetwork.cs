using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Server.NativeInterface;

namespace Maria.Server.Application.Server.ServerBase;

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
		_TelnetNetwork.Init(initInfo, null, null, null, null);
		_TelnetNetwork.StartListen("127.0.0.1", Program.ServerConfig.TelnetPort);
		
	}

	private void _UnInitTelnetNetwork()
	{
		_TelnetNetwork.UnInit();
	}

	protected readonly NetworkInstance _TelnetNetwork = new NetworkInstance();
}