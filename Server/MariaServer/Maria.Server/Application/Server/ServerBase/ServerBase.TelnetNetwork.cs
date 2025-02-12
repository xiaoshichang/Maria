using System;
using System.Collections.Generic;
using System.Reflection;
using Maria.Server.Core.Network;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.ServerBase;

public class TelnetNetworkSessionMessage : NetworkSessionMessage
{
	public string? Data;
}

public static class TelnetCommand
{
	public const string STAT = "stat";
	public const string SHUTDOWN = "shutdown";
}

public abstract partial class ServerBase
{
	private void _InitTelnetNetwork()
	{
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Delim
		};
		_TelnetNetwork.Init(initInfo, _OnTelnetSessionAccepted, null, _OnTelnetSessionDisconnected, _OnTelnetSessionReceiveMessage);
		_TelnetNetwork.Start("127.0.0.1", Program.ServerConfig.TelnetPort);
		Logger.Info($"InitTelnetNetwork {initInfo.ConnectionType} {initInfo.SessionEncoderType} ...");
	}

	protected void _SendTelnetMessage(NetworkSession session, string data)
	{
		var message = new TelnetNetworkSessionMessage()
		{
			Data = data
		};
		session.Send(message);
	}
	
	private void _OnTelnetSessionAccepted(NetworkSession session)
	{
		_LazyInitInteractiveCore();
		var welcome =
			"========================================\n" +
			"Welcome to maria server debug back door!\n" +
			"========================================\n";
		_SendTelnetMessage(session, welcome);
	}

	private void _OnTelnetSessionDisconnected(NetworkSession session)
	{
	}

	private string _CollectProcessPerformanceStat()
	{
		var info = $"cpu:{GetProcessCpuUsage01()}\n";
		info += $"memory:{GetProcessMemoryUsageMB()}\n";
		return info;
	}
	
	protected virtual void _OnTelnetSessionReceiveSpecialCommand(NetworkSession session, string code)
	{
		if (code == TelnetCommand.STAT)
		{
			var stat = _CollectProcessPerformanceStat();
			_SendTelnetMessage(session, stat);
		}
	}
	
	private void _OnTelnetSessionReceiveDebugCode(NetworkSession session, string code)
	{
		try
		{
			var ret = InteractiveCore.InteractiveCore.Interpret(code);
			_SendTelnetMessage(session, ret);
		}
		catch (Exception e)
		{
			_SendTelnetMessage(session, e.ToString());
		}
	}
	
	
	private void _OnTelnetSessionReceiveMessage(NetworkSession session, NetworkSessionMessage req)
	{
		var tnsm = req as TelnetNetworkSessionMessage;
		if (tnsm == null || tnsm.Data == null)
		{
			_SendTelnetMessage(session, "unknown error.");
			return;
		}
		
		var data = tnsm.Data.Trim();
		var prefix = "$";
		if (data.StartsWith(prefix))
		{
			var cmd = data.Replace(prefix, "");
			_OnTelnetSessionReceiveSpecialCommand(session, cmd);
		}
		else
		{
			_OnTelnetSessionReceiveDebugCode(session, data);
		}
	}

	private void _UnInitTelnetNetwork()
	{
		_UnInitInteractiveCore();
		_TelnetNetwork.UnInit();
	}

	private void _LazyInitInteractiveCore()
	{
		if (_InteractiveCoreInitialized) return;
		
		_InteractiveCoreInitialized = true;
		var relativeAssemblies = new List<Assembly>();
		relativeAssemblies.AddRange(Program.GameplayAssemblies);
		relativeAssemblies.Add(Assembly.GetExecutingAssembly());
		
		var customNamespaces = new List<string>();
		customNamespaces.Add("Maria.Server.Application");
		InteractiveCore.InteractiveCore.Init(relativeAssemblies, customNamespaces);
	}

	private void _UnInitInteractiveCore()
	{
		if (!_InteractiveCoreInitialized) return;
		
		InteractiveCore.InteractiveCore.UnInit();
		_InteractiveCoreInitialized = false;
	}

	protected bool _InteractiveCoreInitialized = false;
	protected readonly NetworkInstance _TelnetNetwork = new NetworkInstance();
	
}