using UnityEditor;

namespace Maria.Client.Core.Asset
{
	public partial class AssetManager
	{
		public void Init()
		{
			_InitProvider();
		}


		public void UnInit()
		{
			_UnInitProvider();
		}
		

		private void _InitProvider()
		{
#if UNITY_EDITOR
			var mode = EditorPrefs.GetString(IAssetProvider.ProviderMode_Key);
			if (string.IsNullOrEmpty(mode) || mode == IAssetProvider.ProviderMode_Value_Editor)
			{
				_Provider = new AssetProviderEditorMode.AssetProviderEditorMode();
			}
			else
			{
				_Provider = new AssetProviderAssetBundleMode.AssetProviderAssetBundleMode();
			}
#else
			_Provider = new AssetProviderAssetBundleMode.AssetProviderAssetBundleMode();
#endif
			_Provider.Init();
		}

		private void _UnInitProvider()
		{
			_Provider.UnInit();
			_Provider = null;
		}


		private IAssetProvider _Provider;
	}
}