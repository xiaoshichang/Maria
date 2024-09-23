using System;
using Maria.Client.Core.Log;
using UnityEngine;

namespace Maria.Client.Application
{
	public partial class ApplicationRoot : MonoBehaviour
	{
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			MLogger.Init();
			_InitAssetManager();
		}

		private void Update()
		{
			
		}

		private void OnApplicationQuit()
		{
			_UnInitAssetManager();
		}
	}
}