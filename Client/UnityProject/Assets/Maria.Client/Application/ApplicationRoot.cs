using System;
using System.Collections.Generic;
using Maria.Client.Core.Asset;
using Maria.Client.Core.Coroutine;
using Maria.Client.Core.UI;
using Maria.Client.Foundation.Log;
using Maria.Client.Gameplay;
using Maria.Core.GM;
using Maria.Core.Timer;
using UnityEngine;

namespace Maria.Client.Application
{
	public partial class ApplicationRoot : MonoBehaviour
	{
		private void Start()
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			
			MLogger.Init();
			CoroutineManager.Init();
			AssetManager.Init();
			TimerManager.Init();
			UIManager.Init(Assemblies);
			GMManager.Init(Assemblies);
			
			_InitGameplay(Assemblies);
			_EnterGameplay();
		}

		private void Update()
		{
			TimerManager.Update();
		}

		private void OnApplicationQuit()
		{
			_ExitGameplay();
			_UnInitGameplay();
			
			GMManager.UnInit();
			UIManager.UnInit();
			TimerManager.UnInit();
			AssetManager.UnInit();
			CoroutineManager.UnInit();
		}

		public List<string> Assemblies;
		public static ApplicationRoot Instance;
	}
}