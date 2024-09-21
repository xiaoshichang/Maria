using System;
using System.Collections.Generic;
using Maria.Server.Core.Entity;
using Maria.Server.Log;
using Maria.Shared.Entity;
using Maria.Shared.Foundation;

namespace Maria.Server.Application.Server.ServerBase;

public abstract partial class ServerBase
{
	
	private void _InitEntityManager()
	{
		_EntityManager.Init();
	}
	
	private void _UnInitEntityManager()
	{
		_EntityManager.UnInit();
	}
	
	
	
	protected readonly EntityManager _EntityManager = new();

}