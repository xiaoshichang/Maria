#if UNITY_EDITOR

using System.Collections;
using Maria.Client.Application;
using Maria.Client.Core.Coroutine;
using Maria.Client.Foundation.Log;
using UnityEditor;
using UnityEngine;

namespace Maria.Client.Core.Asset.AssetProviderEditorMode
{
	public class AssetRequestHandlerEditor<T> : AssetRequestHandler<T> where T : Object
	{
		public AssetRequestHandlerEditor(string assetPath)
		{
			_AssetPath = assetPath;
		}

		public override void OnReleaseAsset()
		{
		}

		public override T SyncLoad()
		{
			Asset = AssetDatabase.LoadAssetAtPath<T>(_AssetPath);
			return Asset;
		}

		public override void AsyncLoad(AssetAsyncLoadCallback<T> callback)
		{
			MLogger.Assert(_Callback == null, "last operation is not finished yet.");
			
			_Callback = callback;
			CoroutineManager.StartGlobalCoroutine(_AsyncLoadSim());
		}

		private IEnumerator _AsyncLoadSim()
		{
			yield return null;
			SyncLoad();
			_Callback.Invoke(this);
			_Callback = null;
		}
		
		private readonly string _AssetPath;
	}
}

#endif