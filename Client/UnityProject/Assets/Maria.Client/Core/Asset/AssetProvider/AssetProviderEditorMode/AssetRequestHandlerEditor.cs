#if UNITY_EDITOR

using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Maria.Client.Core.Asset.AssetProviderEditorMode
{
	public class AssetRequestHandlerEditor<T> : AssetRequestHandler<T> where T : Object
	{
		public AssetRequestHandlerEditor(string assetPath) : base(assetPath)
		{
		}

		public override void OnReleaseAsset()
		{
		}

		public override T SyncLoad()
		{
			Asset = AssetDatabase.LoadAssetAtPath<T>(_AssetPath);
			return Asset;
		}

		public override IEnumerator AsyncLoad(OnAssetAsyncLoadCallback<T> callback)
		{
			SyncLoad();
			yield return null;
			callback.Invoke(Asset);
		}
	}
}

#endif