using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;

namespace Maria.Server.Application.Server.GMServer
{
	public partial class GMServer : ServerBase.ServerBase
	{
		public override void Init()
		{
			base.Init();
		}

		public override void UnInit()
		{
			base.UnInit();
		}
		
		protected override void _RegisterNetworkSessionMessageHandlers()
		{
			base._RegisterNetworkSessionMessageHandlers();
			NetworkMessageHandlers.RegisterNetworkMessageHandler<SystemMsgGameReadyNtf>(_OnSystemMsgGameReady);
		}
	}
}

