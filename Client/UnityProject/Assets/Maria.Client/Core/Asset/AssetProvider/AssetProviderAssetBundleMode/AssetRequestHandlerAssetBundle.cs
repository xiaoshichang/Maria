using System.Collections;
using UnityEngine;

namespace Maria.Client.Core.Asset.AssetProviderAssetBundleMode
{
	public class AssetRequestHandlerAssetBundle<T> : AssetRequestHandler<T> where T : Object
	{
		public AssetRequestHandlerAssetBundle(string assetPath) : base(assetPath)
		{
		}

		public override void OnReleaseAsset()
		{
			throw new System.NotImplementedException();
		}

		public override T SyncLoad()
		{
			throw new System.NotImplementedException();
		}

		public override IEnumerator AsyncLoad(OnAssetAsyncLoadCallback<T> callback)
		{
			throw new System.NotImplementedException();
		}

	}
}