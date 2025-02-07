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

public abstract partial class ServerBase
{
	private void _InitTelnetNetwork()
	{
		Logger.Info("InitTelnetNetwork...");
		var initInfo = new NativeAPI.NetworkInitInfo()
		{
			ConnectionType = NativeAPI.NetworkConnectionType.Tcp,
			SessionEncoderType = NativeAPI.SessionMessageEncoderType.Delim
		};
		_TelnetNetwork.Init(initInfo, _OnTelnetSessionAccepted, null, _OnTelnetSessionDisconnected, _OnTelnetSessionReceiveMessage);
		_TelnetNetwork.StartListen("127.0.0.1", Program.ServerConfig.TelnetPort);
		
	}

	private void _SendTelnetMessage(NetworkSession session, string data)
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

	private void _OnTelnetSessionReceiveMessage(NetworkSession session, NetworkSessionMessage req)
	{
		var tnsm = req as TelnetNetworkSessionMessage;
		if (tnsm == null || tnsm.Data == null)
		{
			_SendTelnetMessage(session, "unknown error.");
			return;
		}
		
		var code = tnsm.Data.Trim();
		try
		{
			var ret = InteractiveCore.InteractiveCore.Interpret(code);
			Logger.Info(ret);
			_SendTelnetMessage(session, ret);
		}
		catch (Exception e)
		{
			Logger.Error(e.ToString());
			_SendTelnetMessage(session, e.ToString());
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