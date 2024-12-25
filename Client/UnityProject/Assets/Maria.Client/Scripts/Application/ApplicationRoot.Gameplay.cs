using System;
using System.Collections.Generic;
using System.Reflection;
using Maria.Client.Gameplay;

namespace Maria.Client.Application
{
	public partial class ApplicationRoot
	{
		
		private void _InitGameplay(List<string> assemblies)
		{
			Type t = null;
			foreach (var assemblyName in assemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				t = _SearchGameplayType(assembly);
				if (t != null)
				{
					break;
				}
			}

			if (t == null)
			{
				throw new Exception("Gameplay not found.");
			}
			else
			{
				_GameplayInstance = Activator.CreateInstance(t, null) as GameplayInstance;
				_GameplayInstance.Init();
			}
		}

		private Type _SearchGameplayType(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
			{
				if (type.IsSubclassOf(typeof(GameplayInstance)))
				{
					return type;
				}
			}
			return null;
		}
		

		private void _UnInitGameplay()
		{
			_GameplayInstance.UnInit();
			_GameplayInstance = null;
		}

		private void _EnterGameplay()
		{
			_GameplayInstance.Enter();
		}

		private void _ExitGameplay()
		{
			_GameplayInstance.Exit();
		}
		
		private GameplayInstance _GameplayInstance;

	}
}