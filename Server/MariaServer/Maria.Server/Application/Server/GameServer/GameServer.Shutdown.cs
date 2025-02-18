﻿using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.GameServer;

public partial class GameServer
{
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