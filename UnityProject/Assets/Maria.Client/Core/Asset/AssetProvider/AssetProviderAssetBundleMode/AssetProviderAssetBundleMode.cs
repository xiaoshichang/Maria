using Maria.Client.Core.Log;

namespace Maria.Client.Core.Asset.AssetProviderAssetBundleMode
{
	public class AssetProviderAssetBundleMode : IAssetProvider
	{
		public override void Init()
		{
			throw new System.NotImplementedException();
		}

		public override void UnInit()
		{
			throw new System.NotImplementedException();
		}

		public override AssetRequestHandler<T> LoadAsset<T>(string assetPath)
		{
			throw new System.NotImplementedException();
		}

		public override void ReleaseAsset(AssetRequestHandler handler)
		{
			throw new System.NotImplementedException();
		}
	}
}