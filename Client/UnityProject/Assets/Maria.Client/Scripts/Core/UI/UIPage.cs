using UnityEngine;

namespace Maria.Client.Core.UI
{
	public abstract partial class UIPage : MonoBehaviour
	{

		public abstract void OnShow();
		public abstract void OnHide();
		public abstract void SetupUIWidgetController();
		
		protected T _Seek<T>(string path) where T : class
		{
			var node = gameObject.transform.Find(path);
			if (node == null)
			{
				return null;
			}

			var cmpt = node.GetComponent<T>();
			if (cmpt == null)
			{
				return null;
			}

			return cmpt;
		}
	}
}