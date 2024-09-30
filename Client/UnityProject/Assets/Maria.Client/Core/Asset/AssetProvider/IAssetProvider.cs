using System.Collections;
using UnityEngine;

namespace Maria.Client.Core.Asset
{
	
	public abstract class IAssetProvider
	{
		public const string ProviderMode_Key = "_Key_ProviderMode";
		public const string ProviderMode_Value_Editor = "editor";
		public const string ProviderMode_Value_AB = "ab";


		public abstract void Init();
		public abstract void UnInit();
		
		/// <summary>
		/// 加载资源的统一接口
		/// </summary>
		/// <param name="assetPath"> 资源路径 </param>
		/// <typeparam name="T"> 资源类型 </typeparam>
		/// <returns> 资源请求句柄，用于决定异步或者同步加载，以及获取相应资源 </returns>
		public abstract AssetRequestHandler<T> LoadAsset<T>(string assetPath) where T : Object;
        
		/// <summary>
		/// 释放资源
		/// </summary>
		/// <param name="handler"> 资源请求句柄 </param>
		public abstract void ReleaseAsset(AssetRequestHandler handler);
	}
}