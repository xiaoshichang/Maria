using System.Collections;
using Maria.Client.Application;
using Maria.Client.Core.Coroutine;
using Maria.Client.Foundation.Log;
using UnityEngine;

namespace Maria.Client.Core.Asset.AssetProviderAssetBundleMode
{
	public class AssetRequestHandlerAssetBundle<T> : AssetRequestHandler<T> where T : Object
	{
		public AssetRequestHandlerAssetBundle(string assetPath, AssetProviderAssetBundleMode provider)
		{
			_Provider = provider;
			_AssetID = provider.AssetPathToAssetID(assetPath);
			_AssetBundleName = provider.GetAssetBundleByAssetID(_AssetID);
		}

		public override T SyncLoad()
		{
			MLogger.Assert(Asset == null, "Asset should be null.");
			
			if (!_Provider.IsAssetBundleLoaded(_AssetBundleName))
			{
				_RuntimeAssetBundle = _Provider.LoadAssetBundleSync(_AssetBundleName);
			}
			else
			{
				_RuntimeAssetBundle = _Provider.GetRuntimeAssetBundle(_AssetBundleName);
			}
			_RuntimeAssetBundle.IncRefCounter();
			Asset = _RuntimeAssetBundle.LoadAssetSync<T>(_AssetID);
			return Asset;
		}

		public override void AsyncLoad(AssetAsyncLoadCallback<T> callback)
		{
			_Callback = callback;
			CoroutineManager.StartGlobalCoroutine(_LoadAssetAsync());
		}
		
		private IEnumerator _LoadAssetAsync()
		{
			MLogger.Assert(Asset == null, "Asset should be null.");

			if (!_Provider.IsAssetBundleLoaded(_AssetBundleName))
			{
				yield return _Provider.LoadAssetBundleAsync(_AssetBundleName);
			}
			_RuntimeAssetBundle = _Provider.GetRuntimeAssetBundle(_AssetBundleName);
			
			MLogger.Assert(_RuntimeAssetBundle == null, "_RuntimeAssetBundle should be null.");
			
			_NativeRequest = _RuntimeAssetBundle.LoadAssetAsync<T>(_AssetID);
			yield return _NativeRequest;
			if (_NativeRequest.asset == null)
			{
				MLogger.Error($"Load {_AssetID} fail!");
				yield break;
			}
			_RuntimeAssetBundle.IncRefCounter();
			Asset = _NativeRequest.asset as T;
			
			_Callback.Invoke(this);
			_Callback = null;
		}
		
		public override void OnReleaseAsset()
		{
			if (_RuntimeAssetBundle == null || Asset is null)
			{
				MLogger.Error("release before load");
				return;
			}
			_RuntimeAssetBundle.DecRefCounter();
		}
		
		private RuntimeAssetBundle _RuntimeAssetBundle;
		private readonly string _AssetID;
		private readonly string _AssetBundleName;
		private AssetBundleRequest _NativeRequest;
		private readonly AssetProviderAssetBundleMode _Provider;

	}
}