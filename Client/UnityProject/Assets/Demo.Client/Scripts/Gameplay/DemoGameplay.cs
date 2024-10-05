using Demo.Client.UI;
using Maria.Client.Core.UI;
using Maria.Client.Gameplay;

namespace Demo.Client.Scripts.Gameplay
{
	public class DemoGameplay : GameplayInstance
	{
		public override void Init()
		{
		}

		public override void UnInit()
		{
		}

		public override void Enter()
		{
			_LoginPage = UIManager.ShowPage(nameof(LoginPage)) as LoginPage;
		}

		public override void Exit()
		{
			UIManager.HidePage(_LoginPage);
		}

		private LoginPage _LoginPage;

	}
}