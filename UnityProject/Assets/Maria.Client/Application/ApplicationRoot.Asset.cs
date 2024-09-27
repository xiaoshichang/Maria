using Maria.Client.Core.Asset;

namespace Maria.Client.Application
{
	public partial class ApplicationRoot
	{
		private void _InitAssetManager()
		{
			_AssetManager = new AssetManager();
			_AssetManager.Init();
		}

		private void _UnInitAssetManager()
		{
			_AssetManager.UnInit();
			_AssetManager = null;
		}
		
		private AssetManager _AssetManager;
	}
}