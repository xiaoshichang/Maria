using System.Collections;
using UnityEngine;

namespace Maria.Client.Core.Asset
{

	public static partial class AssetManager
	{
		public static AssetRequestHandler<T> LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			var handler = _Provider.LoadAsset<T>(assetPath);
			return handler;
		}

		public static void ReleaseAsset(AssetRequestHandler handler)
		{
			_Provider.ReleaseAsset(handler);
		}
	}
}