using System.Collections.Generic;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.ServerBase;


public class InnerSessionHandShakeReq : NetworkSessionMessage
{
	public string? ServerName { get; set; }
	public int ServerID { get; set; }
}

public class InnerNodeHandShakeRsp : NetworkSessionMessage
{
	public string? ServerName { get; set; }
	public int ServerID { get; set; }
}

public class SystemMsgGameConnectToGateNtf : NetworkSessionMessage
{
}

public class SystemMsgGameReadyNtf : NetworkSessionMessage
{
	public int ServerID { get; set; }
}

public class StubDistributeTable
{
	public Dictionary<int, int>? Stub2Game { get; set; } = new Dictionary<int, int>();
}

public class SystemMsgStubInitReq : NetworkSessionMessage
{
	public StubDistributeTable? StubDistributeTable { get; set; }
}

public class SystemMsgStubInitRsp : NetworkSessionMessage
{
	public int ServerID { get; set; }
	public int StubID { get; set; }
}

public class SystemMsgOpenGateNtf : NetworkSessionMessage
{
	
}
