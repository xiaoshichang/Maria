using Maria.Server.Core.Network;
using Maria.Server.NativeInterface;

namespace Maria.Server.Application.Server.ServerBase;

public abstract partial class ServerBase
{
	private void InitTelnetNetwork()
	{
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Delim
		};
		_TelnetNetwork.Init(initInfo);
		_TelnetNetwork.StartListen("127.0.0.1", Program.ServerConfig.TelnetPort);
		
	}

	private void UnInitTelnetNetwork()
	{
		_TelnetNetwork.UnInit();
	}

	protected readonly NetworkInstance _TelnetNetwork = new NetworkInstance();
}