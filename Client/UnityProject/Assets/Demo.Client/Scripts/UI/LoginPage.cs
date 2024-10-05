#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;
using Maria.Client.Core.UI;


namespace Demo.Client.UI
{
	public class LoginPage : UIPage
	{
		public const string AssetPath = "Assets/Demo.Client/Asb/UI/LoginPage.prefab";
		
		public override void OnShow()
		{
		}

		public override void OnHide()
		{
		}
		
		public override void SetupUIWidgetController()
		{
			_UsernameInput = _Seek<InputField>("UsernameInputField");
			_UsernameInput.text = "xiao";
			
			_PasswordInput = _Seek<InputField>("PasswordInputField");
			_PasswordInput.text = "pass";
			
			_ServerInput = _Seek<InputField>("ServerInputField");
			_ServerInput.text = "127.0.0.1:40301";
			
			_LoginButton = _Seek<Button>("LoginButton");
			_LoginButton.onClick.AddListener(_OnLoginButtonClick);
			
			_CloseButton = _Seek<Button>("CloseButton");
			_CloseButton.onClick.AddListener(_OnCloseButtonClick);
		}
		
		private void _OnLoginButtonClick()
		{
		}

		private void _OnCloseButtonClick()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit(0);
#endif
		}
		
		private InputField _UsernameInput;
		private InputField _PasswordInput;
		private InputField _ServerInput;
		private Button _LoginButton;
		private Button _CloseButton;
		private Button _SelectButton;
		private Button _DemoSceneButton;
	}
}

