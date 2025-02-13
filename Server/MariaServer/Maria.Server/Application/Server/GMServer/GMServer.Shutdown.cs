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
		_ExitProcessSessionCount = 0;
		
		_SendTelnetMessage(_ShutdownRequestSession, "start shutdown server ...");
		Logger.Info("start shutdown server ...");

		var req = new SystemMsgCloseGateReq();
		foreach (var (_, gateSession) in _AllGateSessions)
		{
			gateSession.Send(req);
		}
	}

	private void _OnSystemMsgCloseGateRsp(NetworkSession session, NetworkSessionMessage message)
	{
		if (_ShutdownRequestSession == null)
		{
			Logger.Error("_OnSystemMsgCloseGateRsp, _ShutdownRequestSession is null.");
			return;
		}
		
		_ShutdownCloseGateCount += 1;
		if (_ShutdownCloseGateCount == _AllGateSessions.Count)
		{
			Logger.Info("all gates closed ...");
			_SendTelnetMessage(_ShutdownRequestSession, "all gates closed ...");
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
		if (_ShutdownRequestSession == null)
		{
			Logger.Error("_OnSystemMsgSaveEntitiesRsp, _ShutdownRequestSession is null.");
			return;
		}
		
		_SaveEntitiesSessionCount += 1;
		if (_SaveEntitiesSessionCount == _AllGameSessions.Count + _AllGateSessions.Count)
		{
			Logger.Info("save all server entities ok ...");
			_SendTelnetMessage(_ShutdownRequestSession, "save all server entities ok ...");
			_ExitAllProcess();
		}
	}

	private void _ExitAllProcess()
	{
		var req = new SystemMsgExitProcessReq();
		foreach (var (_, gameSession) in _AllGameSessions)
		{
			gameSession.Send(req);
		}
		foreach (var (_, gateSession) in _AllGateSessions)
		{
			gateSession.Send(req);
		}
	}

	private void _OnSystemMsgExitProcessRsp(NetworkSession session, NetworkSessionMessage message)
	{
		if (_ShutdownRequestSession == null)
		{
			Logger.Error("_OnSystemMsgExitProcess, _ShutdownRequestSession is null.");
			return;
		}

		_ExitProcessSessionCount += 1;
		if (_ExitProcessSessionCount == _AllGameSessions.Count + _AllGateSessions.Count)
		{
			Logger.Info("exit all game and gate processes ok ...");
			_SendTelnetMessage(_ShutdownRequestSession, "exit all game and gate processes ok ...");
			Stop();
		}
	}

	private NetworkSession? _ShutdownRequestSession;
	private int _ShutdownCloseGateCount;
	private int _SaveEntitiesSessionCount;
	private int _ExitProcessSessionCount;
}