using System;
using System.Collections.Generic;
using System.Linq;
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

	private StubDistributeTable _BuildStubDistributeTable()
	{
		var table = new StubDistributeTable();
		var keyList = _AllGameSessions.Keys.ToList();
		foreach (var (index, type) in _EntityManager.GetAllStubTypes())
		{
			table.Stub2Game[index] = keyList[Random.Shared.Next(keyList.Count)];
		}
		return table;
	}

	private void _OnAllGameServerReady()
	{
		Logger.Info("_OnAllGameServerReady");
		_StubDistributeTable = _BuildStubDistributeTable();
		var req = new SystemMsgStubInitReq
		{
			StubDistributeTable = _StubDistributeTable
		};
		foreach (var (_, game) in _AllGameSessions)
		{
			game.Send(req);
		}
	}

	private void _OnSystemMsgStubInitRsp(NetworkSession session, NetworkSessionMessage message)
	{
		var rsp = message as SystemMsgStubInitRsp;
		if (_ReadyStub.Contains(rsp.StubID))
		{
			Logger.Error($"_OnSystemMsgStubInitRsp. duplicated stub. {rsp.StubID}");
			return;
		}
		if (!_StubDistributeTable.Stub2Game.TryGetValue(rsp.StubID, out var serverID))
		{
			Logger.Error($"_OnSystemMsgStubInitRsp. unknown stub. {rsp.StubID}");
			return;
		}
		if (rsp.ServerID != serverID)
		{
			Logger.Error($"_OnSystemMsgStubInitRsp. server miss match. {rsp.ServerID} != {serverID}");
			return;
		}
		_ReadyStub.Add(rsp.StubID);
		if (_ReadyStub.Count == _EntityManager.GetAllStubTypes().Count)
		{
			Logger.Info($"_OnAllStubReady. {_ReadyStub.Count} stubs.");
			_OnAllStubReady();
		}
	}

	private void _OnAllStubReady()
	{
	}

	protected StubDistributeTable _StubDistributeTable;
	private readonly HashSet<int> _ReadyGameServers = new();
	private readonly HashSet<int> _ReadyStub = new();
}