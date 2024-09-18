using System;
using System.Collections.Generic;
using Maria.Server.Log;
using Maria.Server.NativeInterface;
using Maria.Shared.Network;

#pragma warning disable

namespace Maria.Server.Core.Network
{
	public delegate void NetworkSessionAcceptedCallback(NetworkSession session);
	public delegate void NetworkSessionConnectedCallback(NetworkSession session);
	public delegate void NetworkSessionDisconnectedCallback(NetworkSession session);
	
	public class NetworkInstance
	{
		public void Init(NativeAPI.NetworkInitInfo initInfo, 
			NetworkSessionAcceptedCallback? onAccepted, 
			NetworkSessionConnectedCallback? onConnected, 
			NetworkSessionDisconnectedCallback? onDisconnected,
			NetworkSessionReceiveMessageCallback? onReceiveMessage)
		{
			_NativeNetworkInstance = NativeAPI.NetworkInstance_Init(initInfo, OnSessionAcceptedHandler, OnSessionConnectedHandler, OnSessionDisconnectedHandler);
			_OnNetworkSessionAccepted = onAccepted;
			_OnNetworkSessionConnected = onConnected;
			_OnNetworkSessionDisconnected = onDisconnected;
			_OnNetworkSessionReceiveMessage = onReceiveMessage;
		}

		public void UnInit()
		{
			Logger.Assert(_NativeNetworkInstance != IntPtr.Zero, "_NativeNetworkInstance should not be null");
			NativeAPI.NetworkInstance_UnInit(_NativeNetworkInstance);
		}

		public void StartListen(string ip, int port)
		{
			Logger.Assert(_NativeNetworkInstance != IntPtr.Zero, "_NativeNetworkInstance should not be null");
			NativeAPI.NetworkInstance_StartListen(_NativeNetworkInstance, ip, port);
		}

		public void StopListen()
		{
			Logger.Assert(_NativeNetworkInstance != IntPtr.Zero, "_NativeNetworkInstance should not be null");
			NativeAPI.NetworkInstance_StopListen(_NativeNetworkInstance);
		}

		public void ConnectTo(string ip, int port)
		{
			NativeAPI.NetworkInstance_ConnectTo(_NativeNetworkInstance, ip, port);
		}
		
		private void OnSessionAcceptedHandler(IntPtr nativeSessionPtr)
		{
			Logger.Assert(!_Sessions.ContainsKey(nativeSessionPtr), $"OnSessionAcceptHandler Duplicated Session. {nativeSessionPtr}");
			var session = new NetworkSession(nativeSessionPtr, _OnNetworkSessionReceiveMessage);
			_Sessions[nativeSessionPtr] = session;
			_OnNetworkSessionAccepted?.Invoke(session);
		}

		private void OnSessionConnectedHandler(IntPtr nativeSessionPtr, int ec)
		{
			Logger.Assert(!_Sessions.ContainsKey(nativeSessionPtr), $"OnSessionConnectedHandler Duplicated Session. {nativeSessionPtr}");
			var session = new NetworkSession(nativeSessionPtr, _OnNetworkSessionReceiveMessage);
			_Sessions[nativeSessionPtr] = session;
			_OnNetworkSessionConnected?.Invoke(session);
		}

		private void OnSessionDisconnectedHandler(IntPtr nativeSessionPtr)
		{
			if (!_Sessions.Remove(nativeSessionPtr, out var session))
			{
				Logger.Assert(_Sessions.ContainsKey(nativeSessionPtr), $"OnSessionDisconnectHandler Session does not exist. {nativeSessionPtr}");
				return;
			}
			_OnNetworkSessionDisconnected?.Invoke(session);
		}
		
		private IntPtr _NativeNetworkInstance = IntPtr.Zero;
		private readonly Dictionary<IntPtr, NetworkSession> _Sessions = new();
		
		private NetworkSessionAcceptedCallback? _OnNetworkSessionAccepted;
		private NetworkSessionConnectedCallback? _OnNetworkSessionConnected;
		private NetworkSessionDisconnectedCallback? _OnNetworkSessionDisconnected;
		private NetworkSessionReceiveMessageCallback? _OnNetworkSessionReceiveMessage;

	}
}

