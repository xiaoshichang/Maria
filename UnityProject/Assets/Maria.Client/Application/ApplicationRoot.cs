using System;
using Maria.Client.Foundation.Log;
using Maria.Core.Timer;
using UnityEngine;

namespace Maria.Client.Application
{
	public partial class ApplicationRoot : MonoBehaviour
	{
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			MLogger.Init();
			TimerManager.Init();
			_InitAssetManager();
		}

		private void Update()
		{
			TimerManager.Update();
		}

		private void OnApplicationQuit()
		{
			_UnInitAssetManager();
			TimerManager.UnInit();
		}
	}
}