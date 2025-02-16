using System;
using Maria.Shared.Network;

namespace Maria.Client.Core.Network
{
	public abstract class NetworkSessionEvent
	{
	}

	public enum SessionOnConnectedResult
	{
		Success,
		Fail
	}
	
	public class NetworkSessionEventOnConnected : NetworkSessionEvent
	{
		public SessionOnConnectedResult Result;
		public string Message;
	}
	
	public class NetworkSessionEventOnReceive : NetworkSessionEvent
	{
		public NetworkSessionMessage Message;
	}

	public class NetworkSessionEventOnReceiveError : NetworkSessionEvent
	{
		public Exception InternalException;
	}

	public class NetworkSessionEventOnSendError : NetworkSessionEvent
	{
		public Exception InternalException;
	}
}