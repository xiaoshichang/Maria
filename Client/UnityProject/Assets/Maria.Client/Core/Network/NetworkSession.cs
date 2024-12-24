using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	public enum SessionState
	{
		/// <summary>
		/// the state when
		/// 1. a socket connect can be call.
		/// </summary>
		Ready,
		
		/// <summary>
		/// the state when
		/// 1. socket is connecting.
		/// </summary>
		Connecting,
		
		/// <summary>
		/// the state when
		/// 1. socket is connected.
		/// 2. read/write thread is working.
		/// </summary>
		Connected,
		
		/// <summary>
		/// the state when
		/// 1. socket is cleaning up.
		/// 2. read/write thread is cleaning up.
		/// </summary>
		Disconnecting,
		
		/// <summary>
		/// the state when
		/// 1. application disconnect callback is calling.
		/// </summary>
		Disconnected,
	}
	
	
	public delegate void OnSessionConnectedCallback(SessionOnConnectedResult ret, INetworkSession session);
	public delegate void OnSessionReceiveMessageCallback(NetworkSessionMessage message);
	public delegate void OnSessionDisconnectedCallback();
	

	public interface INetworkSession
	{
		public void Init(OnSessionReceiveMessageCallback onReceive, OnSessionDisconnectedCallback onDisconnect);
		public void UnInit();
		public void ConnectToAsync(string ip, int port, OnSessionConnectedCallback callback);
		public bool IsConnected();
		public void Send(NetworkSessionMessage message);
		public void Tick();
		public void Disconnect();
		public void ProcessEvents();

	}
}