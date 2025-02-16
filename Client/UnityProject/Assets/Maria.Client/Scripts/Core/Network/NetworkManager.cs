using System.Collections.Generic;
using System.Reflection;
using Maria.Client.Foundation.Log;
using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	public static class NetworkManager
	{
		public static void Init(List<string> assemblies)
		{
			foreach (var assemblyName in assemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				NetworkSessionMessage.RegisterNetworkSessionMessageType(assembly);
			}
			NetworkSessionMessage.RegisterNetworkSessionMessageType(Assembly.GetExecutingAssembly());
		}

		public static void UnInit()
		{
			if (_Session != null)
			{
				_Session.UnInit();
				_Session = null;
			}
		}

		public static void Tick()
		{
			_Session?.Tick();
			_Session?.ProcessEvents();
		}

		public static void ConnectToAsync(string ip, int port)
		{
			MLogger.Assert(_Session == null);
			_Session = new TcpNetworkSession();
			_Session.Init(OnReceive, OnDisconnect);
			_Session.ConnectToAsync(ip, port, OnConnected);
		}

		private static void OnConnected(SessionOnConnectedResult ret, INetworkSession session)
		{
			OnSessionConnected?.Invoke(ret, session);
		}

		private static void OnDisconnect()
		{
			OnSessionDisconnected?.Invoke();
		}

		public static void Disconnect()
		{
			_Session.Disconnect();
		}

		public static void Send(NetworkSessionMessage message)
		{
			_Session.Send(message);
		}

		private static void OnReceive(NetworkSessionMessage message)
		{
			OnSessionReceiveMessage?.Invoke(message);
		}

		private static INetworkSession _Session;
		public static OnSessionReceiveMessageCallback OnSessionReceiveMessage;
		public static OnSessionDisconnectedCallback OnSessionDisconnected;
		public static OnSessionConnectedCallback OnSessionConnected;



	}
}