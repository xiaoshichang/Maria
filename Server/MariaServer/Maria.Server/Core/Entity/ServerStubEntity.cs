using System;
using Maria.Server.Application;
using Maria.Server.Application.Server.ServerBase;
using Maria.Shared.Foundation;

namespace Maria.Server.Core.Entity;

public class ServerStubEntity : ServerEntity
{
	public ServerStubEntity() : base()
	{
	}

	public ServerStubEntity(Guid guid, object document) : base(guid, document)
	{
	}

	protected void _OnStubReady()
	{
		var rsp = new SystemMsgStubInitRsp()
		{
			ServerID = Program.ServerID,
			StubID = StableHash.TypeToHash(GetType())
		};
		Program.Server.SendToGMServer(rsp);
	}
}