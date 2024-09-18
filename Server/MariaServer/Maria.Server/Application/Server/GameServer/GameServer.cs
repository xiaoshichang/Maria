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
	}
}

