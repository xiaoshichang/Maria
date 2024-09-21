using System.Collections.Generic;
using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Shared.Network;

#pragma warning disable

namespace Maria.Server.Application.Server.GMServer;

public partial class GMServer
{
	protected override void _OnInnerNodeSessionRegister(int serverID, NetworkSession session)
	{
		base._OnInnerNodeSessionRegister(serverID, session);
		if (_AllGameSessions.Count + _AllGateSessions.Count == Program.ServerGroupConfig.GetTotalGameAndGateCount())
		{
			var message = new SystemMsgGameConnectToGateNtf();
			foreach (var (_, game) in _AllGameSessions)
			{
				game.Send(message);
			}
		}
	}

	private void _OnSystemMsgGameReady(NetworkSession session, NetworkSessionMessage message)
	{
		var ntf = message as SystemMsgGameReadyNtf;
		var serverId = ntf.ServerID;
		if (!_ReadyGameServers.Add(serverId))
		{
			Logger.Error($"_OnSystemMsgGameConnectToGate duplicated server id. {serverId}");
			return;
		}
		if (_ReadyGameServers.Count == Program.ServerGroupConfig.GameServers.Count)
		{
			_OnAllGameServerReady();
		}
	}

	private void _OnAllGameServerReady()
	{
		Logger.Info("_OnAllGameServerReady");
		
		
	}

	private readonly HashSet<int> _ReadyGameServers = new HashSet<int>();
}