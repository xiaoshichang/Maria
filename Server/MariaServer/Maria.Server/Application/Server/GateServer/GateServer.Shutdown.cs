using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.GateServer;

public partial class GateServer 
{
	private void _OnSystemMsgCloseGateReq(NetworkSession session, NetworkSessionMessage message)
	{
		_CloseClientNetwork();
		var rsp = new SystemMsgCloseGateRsp()
		{
		};
		session.Send(rsp);
	}

	private void _OnSystemMsgSaveEntitiesReq(NetworkSession session, NetworkSessionMessage message)
	{
		var rsp = new SystemMsgSaveEntitiesRsp()
		{
		};
		session.Send(rsp);
	}

	private void _OnSystemMsgExitProcessReq(NetworkSession session, NetworkSessionMessage message)
	{
		Stop();
		var rsp = new SystemMsgExitProcessRsp()
		{
		};
		session.Send(rsp);
	}
	
}