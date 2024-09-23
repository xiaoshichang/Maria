using System.Collections;
using UnityEngine;

namespace Maria.Client.Core.Asset
{

	public partial class AssetManager
	{
		public AssetRequestHandler<T> LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			var handler = _Provider.LoadAsset<T>(assetPath);
			return handler;
		}

		public void ReleaseAsset(AssetRequestHandler handler)
		{
			_Provider.ReleaseAsset(handler);
		}
	}
}