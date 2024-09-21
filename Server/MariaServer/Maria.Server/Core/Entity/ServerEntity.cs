using System;

namespace Maria.Server.Core.Entity;

public class ServerEntity : Shared.Entity.Entity
{
	public ServerEntity() : base()
	{
	}

	public ServerEntity(Guid guid, object document) : base(guid, document)
	{
	}
	
	
}