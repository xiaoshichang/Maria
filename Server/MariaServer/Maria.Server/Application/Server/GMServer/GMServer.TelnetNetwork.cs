using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;

namespace Maria.Server.Application.Server.GMServer;

public partial class GMServer
{
	protected override void _OnTelnetSessionReceiveSpecialCommand(NetworkSession session, string code)
	{
		base._OnTelnetSessionReceiveSpecialCommand(session, code);
		if (code == TelnetCommand.SHUTDOWN)
		{
			_ShutdownServerGroup(session);
		}
	}
}