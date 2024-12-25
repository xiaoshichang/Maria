using System.Collections.Generic;
using System.Linq;
using Maria.Client.Core.Asset;
using Maria.Client.Foundation.Log;
using UnityEngine;

namespace Maria.Client.Core.UI
{
	
	public static partial class UIManager
	{
		public static UIPage ShowPage(string pageID, object param=null)
		{
			var registerItem = _GetRegisterItem(pageID);
			if (registerItem == null)
			{
				MLogger.Error($"{pageID} is not registered.");
				return null;
			}

			var page = _CreatePage(registerItem);
			
			_TryHideTop();
			_PageStack.Add(page);
			page.OnShow();
			return page;
		}

		public static void HidePage(UIPage page)
		{
			page.OnHide();
			_PageStack.Remove(page);
			_DestroyPage(page);
			_TryShowTop();
		}
		
		private static void _TryHideTop()
		{
			if (_PageStack.Count == 0)
			{
				return;
			}

			var page = _PageStack.Last();
			page.gameObject.SetActive(false);
		}
		
		private static void _TryShowTop()
		{
			if (_PageStack.Count == 0)
			{
				return;
			}
			var page = _PageStack.Last();
			page.gameObject.SetActive(true);
		}
		
		private static UIPage _CreatePage(UIPageRegisterItem item)
		{
			var handler = AssetManager.LoadAsset<GameObject>(item.AssetPath);
			handler.SyncLoad(); 
			MLogger.Assert(handler.Asset != null);
			
			var root = UnityEngine.Object.Instantiate(handler.Asset, _PageRoot.transform);
			var page = root.GetComponent<UIPage>();
			MLogger.Assert(page != null, "missing UIPage component in UIPage prefab.");
			
			page.SetupUIWidgetController();
			
			_PageToHandlers[page] = handler;
			return page;
		}

		private static void _DestroyPage(UIPage page)
		{
			if (_PageToHandlers.Remove(page, out var handler))
			{
				Object.Destroy(page);
				AssetManager.ReleaseAsset(handler);
			}
			else
			{
				MLogger.Error("unknown error.");
			}
		}
		
		private static readonly Dictionary<UIPage, AssetRequestHandler> _PageToHandlers = new();
		private static List<UIPage> _PageStack = new List<UIPage>();
	}
	
}