using System;
using System.Collections.Generic;
using Maria.Client.Core.Asset.AssetProviderAssetBundleMode;
using Maria.Client.Foundation.Log;
using Maria.Client.Foundation.Utils;
using UnityEditor;

namespace Maria.Client.Editor.Asset
{
	public static class AssetBundleBuildPipeline
	{
		private const string _MenuPath_Windows = "Maria/Asset/Build AssetBundle/Windows";
		private const string _MenuPath_IOS = "Maria/Asset/Build AssetBundle/IOS";
		private const string _MenuPath_Android = "Maria/Asset/Build AssetBundle/Android";
        

		[MenuItem(_MenuPath_Windows)]
		private static void _BuildWindows()
		{
			var allBuilds = new List<AssetBundleBuild>();

			var dirs = new List<string>()
			{
				"Assets/Demo.Client/Asb/UI"
			};
			var build = new AssetBundleBuild
			{
				assetBundleName = "Demo",
				assetNames = AssetBundleBuildHelper.CollectAllAssetFromDirs(dirs)
			};
			
			allBuilds.Add(build);
			
			var output = AssetProviderAssetBundleMode.AssetBundleRootWindows;
			FileUtils.MakeSureDirectory(output);
			
			var options = _DecideBuildOptions();
			var manifest = BuildPipeline.BuildAssetBundles(output, allBuilds.ToArray(), options, BuildTarget.StandaloneWindows64);
			
			var bundles = manifest.GetAllAssetBundles();
			MLogger.Info($"Build AssetBundle OK! output path: {output}, bundles count: {bundles.Length}!");
		}

		[MenuItem(_MenuPath_IOS)]
		private static void _BuildIOS()
		{
			throw new NotImplementedException();
		}

		[MenuItem(_MenuPath_Android)]
		private static void _BuildAndroid()
		{
			throw new NotImplementedException();
		}

		private static BuildAssetBundleOptions _DecideBuildOptions()
		{
			var options = BuildAssetBundleOptions.None;
			return options;
		}
        
	}
}