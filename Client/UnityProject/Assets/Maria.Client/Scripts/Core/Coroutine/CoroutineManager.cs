using System.Collections;
using Maria.Client.Application;
using Maria.Client.Foundation.Log;

namespace Maria.Client.Core.Coroutine
{
	public static class CoroutineManager
	{
		public static void Init()
		{
			MLogger.Info("Init CoroutineManager Ok!");
		}

		public static void UnInit()
		{
			ApplicationRoot.Instance.StopAllCoroutines();
			MLogger.Info("UnInit CoroutineManager Ok!");
		}
		
		public static UnityEngine.Coroutine StartGlobalCoroutine(IEnumerator routine)
		{
			return ApplicationRoot.Instance.StartCoroutine(routine);
		}

		public static void StopGlobalCoroutine(UnityEngine.Coroutine routine)
		{
			ApplicationRoot.Instance.StopCoroutine(routine);
		}

		public static void StopGlobalCoroutine(IEnumerator routine)
		{
			ApplicationRoot.Instance.StopCoroutine(routine);
		}
	}
}