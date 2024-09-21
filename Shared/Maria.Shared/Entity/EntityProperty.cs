using System;

namespace Maria.Shared.Entity
{
	[Flags]
	public enum EntityPropertyFlag
	{
		Persistent,
		ServerOnly,
		ClientOnly,
		ClientServer,
		AllClient
	}
	
}