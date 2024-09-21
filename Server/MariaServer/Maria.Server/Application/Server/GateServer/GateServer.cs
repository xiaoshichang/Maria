namespace Maria.Server.Application.Server.GateServer
{
	public partial class GateServer : ServerBase.ServerBase
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
			
		}
	}
}

