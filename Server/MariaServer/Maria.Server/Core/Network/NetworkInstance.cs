using System;
using System.Collections.Generic;
using Maria.Server;
using Maria.Server.Log;
using Maria.Server.NativeInterface;

namespace Maria.Server.Core.Network
{
	public class NetworkInstance
	{
		public void Init(NativeAPI.NetworkInitInfo initInfo)
		{
			_NativeNetworkInstance = NativeAPI.NetworkInstance_Init(initInfo, OnSessionAcceptHandler, OnSessionConnectedHandler);
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
		
		
		private void OnSessionAcceptHandler(IntPtr session)
		{
			Logger.Assert(!_Sessions.ContainsKey(session), $"OnSessionAcceptHandler Duplicated Session. {session}");
			_Sessions[session] = new NetworkSession(this, session);
		}

		private void OnSessionConnectedHandler(IntPtr session, int ec)
		{
			Logger.Assert(!_Sessions.ContainsKey(session), $"OnSessionConnectedHandler Duplicated Session. {session}");
			_Sessions[session] = new NetworkSession(this, session);
		}

		private IntPtr _NativeNetworkInstance = IntPtr.Zero;
		private readonly Dictionary<IntPtr, NetworkSession> _Sessions = new();
	}
}

