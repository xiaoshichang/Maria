using Maria.Server.Application.Server.ServerBase;
using Maria.Server.Core.Network;
using Maria.Shared.Network;

namespace Maria.Server.Application.Server.GameServer
{
	public partial class GameServer : ServerBase.ServerBase
	{
		public override void Init()
		{
			base.Init();
			_TryConnectToGMServer();
		}

		public override void UnInit()
		{
			base.UnInit();
		}

		protected override void _RegisterNetworkSessionMessageHandlers()
		{
			base._RegisterNetworkSessionMessageHandlers();
			NetworkMessageHandlers.RegisterNetworkMessageHandler<SystemMsgGameConnectToGateNtf>(_OnSystemMsgGameConnectToGate);
			NetworkMessageHandlers.RegisterNetworkMessageHandler<SystemMsgStubInitReq>(_OnSystemMsgStubInitReq);
		}


	}
}

