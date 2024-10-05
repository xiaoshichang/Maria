using System;
using System.Collections.Generic;
using System.Reflection;
using Maria.Client.Foundation.Log;

namespace Maria.Client.Core.UI
{
	public class UIPageRegisterItem
	{
		public string PageID;
		public string AssetPath;
		public Type PageType;

		public bool IsValid()
		{
			if (string.IsNullOrEmpty(PageID))
			{
				return false;
			}

			if (string.IsNullOrEmpty(AssetPath))
			{
				return false;
			}

			return true;
		}
	}
	
	
	public static partial class UIManager
	{
		private static void _InitRegisterItems(List<string> assemblies)
		{
			// collect from gameplay
			foreach (var assemblyName in assemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				var items = _CollectUIPageByReflection(assembly);
				foreach (var item in items)
				{
					_AllRegisteredItems.Add(item.PageID, item);
				}
			}
		}

		private static List<UIPageRegisterItem> _CollectUIPageByReflection(Assembly assembly)
		{
			List<UIPageRegisterItem> items = new List<UIPageRegisterItem>();
			foreach (var type in assembly.GetTypes())
			{
				if (!type.IsSubclassOf(typeof(UIPage)))
				{
					continue;
				}

				var field = type.GetField("AssetPath");
				if (field == null)
				{
					MLogger.Error("UIPage should contains AssetPath field");
					continue;
				}

				var assetPath = field.GetValue(null) as string;
				var item = new UIPageRegisterItem()
				{
					PageID = type.Name,
					PageType = type,
					AssetPath = assetPath
				};

				if (!item.IsValid())
				{
					MLogger.Error($"UIPageRegisterItem is not valid. {item.PageType}");
					continue;
				}
				items.Add(item);
			}
			return items;
		}
		
		private static UIPageRegisterItem _GetRegisterItem(string pageID)
		{
			return _AllRegisteredItems.GetValueOrDefault(pageID);
		}
		
		private static readonly Dictionary<string, UIPageRegisterItem> _AllRegisteredItems = new();
	}
	
}