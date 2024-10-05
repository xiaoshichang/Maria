using System;
using System.Collections.Generic;
using Maria.Client.Foundation.Log;
using UnityEngine;

namespace Maria.Client.Core.UI
{
	public static partial class UIManager
	{
		public static void Init(List<string> assemblies)
		{
			_InitHierarchy();
			_InitRegisterItems(assemblies);
		}

		public static void UnInit()
		{
			MLogger.Assert(_PageToHandlers.Count <= 0, "_AssetRequestHandlers should be empty.");
			_ReleaseHierarchy();
		}

		public static void ScreeFadeTo()
		{
			
		}

		public static void ClearFade()
		{
			
		}
		
	}
}