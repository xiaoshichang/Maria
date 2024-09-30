
using Maria.Client.Core.Asset;
using UnityEditor;
using UnityEngine;

namespace Maria.Client.Editor.Asset
{
	public static class AssetProviderSelectMenu
	{
		private const string _EditorModePath = "Maria/Asset/AssetProviderMode/EditorMode";
		private const string _AssetBundlePath = "Maria/Asset/AssetProviderMode/AssetBundleMode";
			
		[InitializeOnLoadMethod]
		private static void _OnLoad()
		{
			var mode = EditorPrefs.GetString(IAssetProvider.ProviderMode_Key);
			if (string.IsNullOrEmpty(mode) || mode == IAssetProvider.ProviderMode_Value_Editor)
			{
				_EnableEditorMode();
			}
			else if (mode == IAssetProvider.ProviderMode_Value_AB)
			{
				_EnableAssetBundleMode();
			}
		}
			
		[MenuItem(_EditorModePath)]
		private static void _EnableEditorMode()
		{
			if (UnityEngine.Application.isPlaying)
			{
				return;
			}
			Menu.SetChecked(_EditorModePath, true);
			Menu.SetChecked(_AssetBundlePath, false);
			EditorPrefs.SetString(IAssetProvider.ProviderMode_Key, IAssetProvider.ProviderMode_Value_Editor);
		}
			
		[MenuItem(_AssetBundlePath)]
		private static void _EnableAssetBundleMode()
		{
			if (UnityEngine.Application.isPlaying)
			{
				return;
			}
			Menu.SetChecked(_EditorModePath, false);
			Menu.SetChecked(_AssetBundlePath, true);
			EditorPrefs.SetString(IAssetProvider.ProviderMode_Key, IAssetProvider.ProviderMode_Value_AB);
		}
	}
}

