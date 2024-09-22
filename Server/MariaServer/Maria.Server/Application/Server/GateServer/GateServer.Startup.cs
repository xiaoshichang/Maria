using System;
using Maria.Server.Core.Network;
using Maria.Server.Core.Timer;
using Maria.Server.Log;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.GateServer;

public partial class GateServer
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

	private void _OnSystemMsgOpenGateNtf(NetworkSession session, NetworkSessionMessage message)
	{
		Logger.Info("open gate.");
		_OpenClientNetwork();
	}
}