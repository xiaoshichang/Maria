namespace Maria.Client.Core.Network
{
	public abstract class NetworkSessionEvent
	{
	}

	public enum SessionOnConnectedResult
	{
		OK,
		Fail
	}
	
	public class NetworkSessionEventOnConnected : NetworkSessionEvent
	{
		public SessionOnConnectedResult Result;
		public string Message;
	}
	
	public class NetworkSessionEventOnReceive : NetworkSessionEvent
	{
		
	}

	public class NetworkSessionEventOnError : NetworkSessionEvent
	{
		
	}
}