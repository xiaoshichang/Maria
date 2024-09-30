using System.Collections;
using UnityEngine;

namespace Maria.Client.Core.Asset
{
	public abstract class AssetRequestHandler
	{
		protected AssetRequestHandler(string assetPath)
		{
			_AssetPath = assetPath;
		}

		protected readonly string _AssetPath;
		public abstract void OnReleaseAsset();
	}

	public delegate void OnAssetAsyncLoadCallback<in T>(T asset) where T : Object;

	public abstract class AssetRequestHandler<T> : AssetRequestHandler where T : Object
	{
		protected AssetRequestHandler(string assetPath) : base(assetPath)
		{
		}

		public abstract T SyncLoad();
		public abstract IEnumerator AsyncLoad(OnAssetAsyncLoadCallback<T> callback);
		public T Asset { get; protected set; }
	}
}