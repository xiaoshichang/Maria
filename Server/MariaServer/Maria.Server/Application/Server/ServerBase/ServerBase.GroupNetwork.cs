using Maria.Server.Core.Network;

namespace Maria.Server.Application.Server.ServerBase;

public abstract partial class ServerBase
{
	private void InitGroupNetwork()
	{
		_GroupNetwork.Init();
	}

	private void UnInitGroupNetwork()
	{
		_GroupNetwork.UnInit();
	}
	
	protected readonly NetworkInstance _GroupNetwork = new NetworkInstance();
}