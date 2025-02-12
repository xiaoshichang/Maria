using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;

namespace Maria.Server.Application.Server.GateServer
{
	public partial class GateServer : ServerBase.ServerBase
	{
		public override void Init()
		{
			base.Init();
			_InitClientNetwork();
			_TryConnectToGMServer();
		}

		public override void UnInit()
		{
			_UnInitClientNetwork();
			base.UnInit();
		}
		
		protected override void _RegisterNetworkSessionMessageHandlers()
		{
			base._RegisterNetworkSessionMessageHandlers();
			NetworkMessageHandlers.RegisterNetworkMessageHandler<SystemMsgOpenGateNtf>(_OnSystemMsgOpenGateNtf);
			NetworkMessageHandlers.RegisterNetworkMessageHandler<SystemMsgCloseGateReq>(_OnSystemMsgCloseGateReq);
			NetworkMessageHandlers.RegisterNetworkMessageHandler<SystemMsgCloseGateReq>(_OnSystemMsgSaveEntitiesReq);
		}
	}
}

