using Maria.Server.Core.Entity;
using Maria.Server.Core.Timer;

namespace DemoGame;

public class MatchStub : ServerStubEntity
{
	public MatchStub()
	{
		TimerManager.AddTimer(1000, _SomeWork);
	}

	private void _SomeWork(object? param)
	{
		_OnStubReady();
	}
}