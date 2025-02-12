using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.GMServer;

public partial class GMServer
{
	private void _ShutdownServerGroup(NetworkSession session)
	{
		if (_ShutdownRequestSession != null)
		{
			return;
		}
		_ShutdownRequestSession = session;
		_ShutdownCloseGateCount = 0;
		_SaveEntitiesSessionCount = 0;

		_SendTelnetMessage(_ShutdownRequestSession, "Start shutdown server ...");

		var req = new SystemMsgCloseGateReq();
		foreach (var (_, gateSession) in _AllGateSessions)
		{
			gateSession.Send(req);
		}
	}

	private void _OnSystemMsgCloseGateRsp(NetworkSession session, NetworkSessionMessage message)
	{
		if (_ShutdownRequestSession == null || session != _ShutdownRequestSession)
		{
			Logger.Error("unknown error.");
			return;
		}
		
		_ShutdownCloseGateCount += 1;
		if (_ShutdownCloseGateCount == _AllSessions.Count)
		{
			_SendTelnetMessage(_ShutdownRequestSession, "all gates closed & all client sessions disconnected ...");
			_SendTelnetMessage(_ShutdownRequestSession, "saving all entities to database ...");
			_SaveAllEntitiesToDataBase();
		}
	}

	private void _SaveAllEntitiesToDataBase()
	{
		var req = new SystemMsgSaveEntitiesReq();
		foreach (var (_, gameSession) in _AllGameSessions)
		{
			gameSession.Send(req);
		}
		foreach (var (_, gateSession) in _AllGateSessions)
		{
			gateSession.Send(req);
		}
	}

	private void _OnSystemMsgSaveEntitiesRsp(NetworkSession session, NetworkSessionMessage message)
	{
		if (_ShutdownRequestSession == null || session != _ShutdownRequestSession)
		{
			Logger.Error("unknown error.");
			return;
		}
		
		_SaveEntitiesSessionCount += 1;
		if (_SaveEntitiesSessionCount == _AllGameSessions.Count + _AllGateSessions.Count)
		{
			_SendTelnetMessage(_ShutdownRequestSession, "save all server entities ok ...");
		}
	}

	private NetworkSession? _ShutdownRequestSession;
	private int _ShutdownCloseGateCount;
	private int _SaveEntitiesSessionCount;
}