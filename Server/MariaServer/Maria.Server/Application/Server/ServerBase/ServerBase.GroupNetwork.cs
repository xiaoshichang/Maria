using Maria.Server.Core.Network;
using Maria.Server.NativeInterface;

namespace Maria.Server.Application.Server.ServerBase;

public abstract partial class ServerBase
{
	private void InitGroupNetwork()
	{
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Header
		};
		_GroupNetwork.Init(initInfo);
		_GroupNetwork.StartListen(Program.ServerConfig.InnerIp, Program.ServerConfig.InnerPort);
	}

	private void UnInitGroupNetwork()
	{
		_GroupNetwork.UnInit();
	}
	
	protected readonly NetworkInstance _GroupNetwork = new NetworkInstance();
}