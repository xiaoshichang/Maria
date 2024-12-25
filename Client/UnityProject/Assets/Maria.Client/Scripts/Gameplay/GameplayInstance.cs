namespace Maria.Client.Gameplay
{
	public abstract class GameplayInstance
	{
		public abstract void Init();
		public abstract void UnInit();
		public abstract void Enter();
		public abstract void Exit();

	}
}