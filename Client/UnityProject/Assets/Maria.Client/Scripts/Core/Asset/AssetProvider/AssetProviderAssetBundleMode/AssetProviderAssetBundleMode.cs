
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Maria.Client.Core.Asset.AssetProviderAssetBundleMode
{
	public partial class AssetProviderAssetBundleMode : IAssetProvider
	{
		private static readonly string AssetBundleRoot = Path.Join(UnityEngine.Application.streamingAssetsPath, "AssetBundles");
		public static readonly string AssetBundleRootWindows = Path.Join(AssetBundleRoot, "windows");
		public static readonly string AssetBundleRootIOS = Path.Join(AssetBundleRoot, "ios");
		public static readonly string AssetBundleRootAndroid = Path.Join(AssetBundleRoot, "android");
		
		
		public override void Init()
		{
			_InitAssetBundleManifest();
			_InitAssetMap();
		}
		
		public override void UnInit()
		{
			_ReleaseAssetMap();
			_ReleaseAssetBundleManifest();
		}
		
		public override AssetRequestHandler<T> LoadAsset<T>(string assetPath)
		{
			var handler = new AssetRequestHandlerAssetBundle<T>(assetPath, this);
			return handler;
		}

		public override void ReleaseAsset(AssetRequestHandler handler)
		{
			handler.OnReleaseAsset();
		}
		
		private string _GetAssetBundlePathByName(string name)
		{
			string root;
			if (UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer || 
			    UnityEngine.Application.platform == RuntimePlatform.WindowsEditor)
			{
				root = AssetBundleRootWindows;
			}
			else if (UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer)
			{
				throw new NotImplementedException();
			}
			else if (UnityEngine.Application.platform == RuntimePlatform.Android)
			{
				throw new NotImplementedException();
			}
			else
			{
				throw new NotImplementedException();
			}
			return Path.Join(root, name);
		}
		
		private string _GetRootManifestName()
		{
			if (UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer ||
			    UnityEngine.Application.platform == RuntimePlatform.WindowsEditor)
			{
				return "windows";
			}
			else if (UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer)
			{
				throw new NotImplementedException();
			}
			else if (UnityEngine.Application.platform == RuntimePlatform.Android)
			{
				throw new NotImplementedException();
			}
			else
			{
				throw new NotImplementedException();
			}
		}
		
		private void _InitAssetBundleManifest()
		{
			var rootManifestAssetBundlePath = _GetAssetBundlePathByName(_GetRootManifestName());
			_RootManifestAssetBundle = AssetBundle.LoadFromFile(rootManifestAssetBundlePath);
			_RootManifest = _RootManifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		}
		
		private void _ReleaseAssetBundleManifest()
		{
			_RootManifest = null;
			_RootManifestAssetBundle.Unload(true);
			_RootManifestAssetBundle = null;
		}
		
		private void _InitAssetMap()
		{
			var allAssetBundles = _RootManifest.GetAllAssetBundles();
			foreach (var assetBundleName in allAssetBundles)
			{
				var assetBundlePath = _GetAssetBundlePathByName(assetBundleName);
				var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
				foreach (var assetName in assetBundle.GetAllAssetNames())
				{
					_AssetMap[assetName] = assetBundleName;
				}
				assetBundle.Unload(true);
			}
		}
		
		private void _ReleaseAssetMap()
		{
			_AssetMap.Clear();
		}
		
		/// <summary>
		/// AssetPath - 上层业务对资源ID的描述，一般为资源路径
		/// AssetID - AssetBundle对资源ID的描述，全小写
		/// </summary>
		public string AssetPathToAssetID(string assetPath)
		{
			return assetPath.ToLower();
		}

		/// <summary>
		/// 根据 AssetID 获取所属的 AssetBundle
		/// </summary>
		public string GetAssetBundleByAssetID(string assetID)
		{
			if (!_AssetMap.TryGetValue(assetID, out var assetBundleName))
			{
				return "";
			}
			return assetBundleName;
		}

		/// <summary>
		/// RootManifest AssetBundle
		/// </summary>
		private AssetBundle _RootManifestAssetBundle;

		/// <summary>
		/// RootManifest
		/// </summary>
		private AssetBundleManifest _RootManifest;
		
		/// <summary>
		/// 资源字典 - 记录每一个资源对应的AssetBundle
		/// </summary>
		private readonly Dictionary<string, string> _AssetMap = new Dictionary<string, string>();
	}
}