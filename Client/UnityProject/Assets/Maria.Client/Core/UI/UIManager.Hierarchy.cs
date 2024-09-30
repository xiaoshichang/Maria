using UnityEngine;
using UnityEngine.UI;

namespace Maria.Client.Core.UI
{
	public static partial class UIManager
	{
		private static void _InitHierarchy()
		{
			_InitRoot();
			_InitBlockRoot();
			_InitPageRoot();
			_InitPopupRoot();
		}
		
		private static void _ReleaseHierarchy()
		{
			Object.DestroyImmediate(_Root);
		}

		private static void _InitRoot()
		{
			_Root = new GameObject("UIRoot")
			{
				layer = LayerMask.NameToLayer("UI")
			};
			Object.DontDestroyOnLoad(_Root);

			_RootCanvas = _Root.AddComponent<Canvas>();
			_RootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
				
			_RootCanvasScaler = _Root.AddComponent<CanvasScaler>();
			_RootGraphicRaycaster = _Root.AddComponent<GraphicRaycaster>();
		}

		private static void _InitPageRoot()
		{
			_PageRoot = new GameObject("PageRoot")
			{
				layer = LayerMask.NameToLayer("UI")
			};
			var rect = _PageRoot.AddComponent<RectTransform>();
			_PageRoot.transform.SetParent(_Root.transform);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchorMin = new Vector2(0, 0);
			rect.anchorMax = new Vector2(1, 1);
			rect.offsetMin = new Vector2(0, 0);
			rect.offsetMax = new Vector2(0, 0);
		}

		private static void _InitBlockRoot()
		{
			_BlockRoot = new GameObject("BlockRoot")
			{
				layer = LayerMask.NameToLayer("UI")
			};
			var rect = _BlockRoot.AddComponent<RectTransform>();
			_BlockRoot.transform.SetParent(_Root.transform);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchorMin = new Vector2(0, 0);
			rect.anchorMax = new Vector2(1, 1);
			rect.offsetMin = new Vector2(0, 0);
			rect.offsetMax = new Vector2(0, 0);
		}

		private static void _InitPopupRoot()
		{
			_PopupRoot = new GameObject("PopupRoot")
			{
				layer = LayerMask.NameToLayer("UI")
			};
			var rect = _PopupRoot.AddComponent<RectTransform>();
			_PopupRoot.transform.SetParent(_Root.transform);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchorMin = new Vector2(0, 0);
			rect.anchorMax = new Vector2(1, 1);
			rect.offsetMin = new Vector2(0, 0);
			rect.offsetMax = new Vector2(0, 0);
		}

		private static GameObject _Root;
		private static Canvas _RootCanvas;
		private static CanvasScaler _RootCanvasScaler;
		private static GraphicRaycaster _RootGraphicRaycaster;
		
		private static GameObject _PageRoot;
		private static GameObject _BlockRoot;
		private static GameObject _PopupRoot;
	}
}