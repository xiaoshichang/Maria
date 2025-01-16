using System;
using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;
using Maria.Server.Core.Timer;
using Maria.Server.Log;
using Maria.Shared.Network;

#pragma warning disable

namespace Maria.Server.Application.Server.GameServer;

public partial class GameServer
{
	private void _TryConnectToGMServer()
	{
		var delay = (uint)Random.Shared.Next(1000, 2000);
		TimerManager.AddTimer(delay, _DoConnectToGMServer);
	}

	private void _DoConnectToGMServer(object? arg)
	{
		var gmConfig = Program.ServerGroupConfig.GetGMConfig();
		Logger.Info($"Connect to GMServer, {gmConfig.InnerIp}:{gmConfig.InnerPort}");
		_InnerNetwork.ConnectTo(gmConfig.InnerIp, gmConfig.InnerPort);
	}
	
	protected override void _OnInnerNodeSessionRegister(int serverID, NetworkSession session)
	{
		base._OnInnerNodeSessionRegister(serverID, session);
		if (_AllGateSessions.Count == Program.ServerGroupConfig.GateServers.Count)
		{
			if (_GMSession == null)
			{
				Logger.Error("_OnInnerNodeSessionRegister known error.");
				return;
			}
			Logger.Info("all gate connected to this game.");
			var msg = new SystemMsgGameReadyNtf()
			{
				ServerID = Program.ServerGroupConfig.GetIDByConfig(Program.ServerConfig)
			};
			_GMSession.Send(msg);
		}
	}
	
	private void _OnSystemMsgGameConnectToGate(NetworkSession session, NetworkSessionMessage message)
	{
		foreach (var gate in Program.ServerGroupConfig.GateServers)
		{
			_InnerNetwork.ConnectTo(gate.InnerIp, gate.InnerPort);
		}
	}

	private void _OnSystemMsgStubInitReq(NetworkSession session, NetworkSessionMessage message)
	{
		var req = message as SystemMsgStubInitReq;
		Logger.Info($"_OnSystemMsgStubInitReq {req.StubDistributeTable.Stub2Game.Count}");
		
		_StubDistributeTable = req.StubDistributeTable;
		foreach (var (tid, game) in req.StubDistributeTable.Stub2Game)
		{
			if (game != Program.ServerID)
			{
				continue;
			}

			var stubType = _EntityManager.GetStubTypeByIndex(tid);
			if (stubType == null)
			{
				Logger.Error("stubType not found.");
				return;
			}
			_EntityManager.CreateServerEntity(stubType);
		}
	}

	protected StubDistributeTable _StubDistributeTable;
}