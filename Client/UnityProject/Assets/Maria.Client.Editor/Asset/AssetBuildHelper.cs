﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Maria.Client.Editor.Asset
{
	public static class AssetBundleBuildHelper
	{
        
		public static string[] CollectAllAssetFromDirs(List<string> dirs)
		{
			var assets = new List<string>();

			foreach (var dir in dirs)
			{
				if (!Directory.Exists(dir))
				{
					throw new IOException();
				}

				var guids = AssetDatabase.FindAssets("*", new[]{dir});
				foreach (var guid in guids)
				{
					var asset = AssetDatabase.GUIDToAssetPath(guid);
					assets.Add(asset);
				}
			}
			return assets.ToArray();
		}
	}
}