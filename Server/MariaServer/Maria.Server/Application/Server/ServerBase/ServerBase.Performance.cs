
using System.Diagnostics;

namespace Maria.Server.Application.Server.ServerBase;

public abstract partial class ServerBase
{
	protected float GetProcessCpuUsage01()
	{
		//todo: get performance data using platform api.
		return 0.5f;
	}

	protected float GetProcessMemoryUsageMB()
	{
		//todo: get performance data using platform api.
		return 100;
	}
}