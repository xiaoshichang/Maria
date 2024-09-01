using Maria.Server.Core.Network;

namespace Maria.Server.Application.Server.ServerBase;

public abstract partial class ServerBase
{
	private void InitTelnetNetwork()
	{
		_TelnetNetwork.Init();
	}

	private void UnInitTelnetNetwork()
	{
		_TelnetNetwork.UnInit();
	}

	protected readonly NetworkInstance _TelnetNetwork = new NetworkInstance();
}