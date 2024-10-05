using System.Collections;
using UnityEngine;

namespace Maria.Client.Core.Asset
{
	public abstract class AssetRequestHandler
	{
		protected AssetRequestHandler()
		{
		}

		public abstract void OnReleaseAsset();
	}

	public delegate void AssetAsyncLoadCallback<T>(AssetRequestHandler<T> handler) where T : Object;

	public abstract class AssetRequestHandler<T> : AssetRequestHandler where T : Object
	{
		public abstract T SyncLoad();
		public abstract void AsyncLoad(AssetAsyncLoadCallback<T> callback);
		public T Asset { get; protected set; }
		protected AssetAsyncLoadCallback<T> _Callback;
	}
}