#if UNITY_EDITOR

using Maria.Client.Foundation.Log;

namespace Maria.Client.Core.Asset.AssetProviderEditorMode
{
	public class AssetProviderEditorMode : IAssetProvider
	{
		public override void Init()
		{
			MLogger.Info("AssetProvider: AssetProviderEditorMode");
		}

		public override void UnInit()
		{
		}

		public override AssetRequestHandler<T> LoadAsset<T>(string assetPath)
		{
			var handler = new AssetRequestHandlerEditor<T>(assetPath);
			return handler;
		}

		public override void ReleaseAsset(AssetRequestHandler handler)
		{
		}
	}
}

#endif