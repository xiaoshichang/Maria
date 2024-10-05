using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maria.Client.Core.Asset.AssetProviderAssetBundleMode
{
	public partial class AssetProviderAssetBundleMode
	{
		public bool IsAssetBundleLoaded(string assetBundleName)
        {
            return _RuntimeAssetBundles.ContainsKey(assetBundleName);
        }

        private void _LoadAllDepAssetBundleSync(string assetBundleName)
        {
            var deps = _RootManifest.GetAllDependencies(assetBundleName);
            if (deps.Length > 0)
            {
                foreach (var dep in deps)
                {
                    var depBundle = LoadAssetBundleSync(dep);
                    depBundle.IncRefCounter();
                }
            }
        }
        
        public RuntimeAssetBundle LoadAssetBundleSync(string assetBundleName)
        {
            if (_RuntimeAssetBundles.TryGetValue(assetBundleName, out var rab))
            {
                return rab;
            }
            _LoadAllDepAssetBundleSync(assetBundleName);

            var path = _GetAssetBundlePathByName(assetBundleName);
            var ab = AssetBundle.LoadFromFile(path);
            var bundle = new RuntimeAssetBundle(this, assetBundleName, ab);
            _RuntimeAssetBundles[assetBundleName] = bundle;
            return bundle;
        }

        public RuntimeAssetBundle GetRuntimeAssetBundle(string assetBundleName)
        {
            return _RuntimeAssetBundles.GetValueOrDefault(assetBundleName);
        }

        private IEnumerator _LoadAllDepAssetBundleAsync(string assetBundleName)
        {
            var deps = _RootManifest.GetAllDependencies(assetBundleName);
            if (deps.Length > 0)
            {
                foreach (var dep in deps)
                {
                    yield return LoadAssetBundleAsync(dep);
                    var rab = GetRuntimeAssetBundle(dep);
                    rab.IncRefCounter();
                }
            }
        }

        public IEnumerator LoadAssetBundleAsync(string assetBundleName)
        {
            yield return _LoadAllDepAssetBundleAsync(assetBundleName);
            var path = _GetAssetBundlePathByName(assetBundleName);
            var request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            _RuntimeAssetBundles[assetBundleName] = new RuntimeAssetBundle(this, assetBundleName, request.assetBundle);
        }

        private void _ReleaseAllDepAssetBundle(string assetBundleName)
        {
            var deps = _RootManifest.GetAllDependencies(assetBundleName);
            if (deps.Length > 0)
            {
                foreach (var dep in deps)
                {
                    var depBundle = _RuntimeAssetBundles[dep];
                    depBundle.DecRefCounter();
                }
            }
        }

        public void ReleaseAssetBundle(RuntimeAssetBundle rab)
        {
            var name = rab.GetAssetBundleName();
            rab.Unload();
            _RuntimeAssetBundles.Remove(name);
            _ReleaseAllDepAssetBundle(name);
        }
		
		
		/// <summary>
		/// 记录所有加载的 AssetBundle
		/// </summary>
		private readonly Dictionary<string, RuntimeAssetBundle> _RuntimeAssetBundles = new Dictionary<string, RuntimeAssetBundle>();
	}
}